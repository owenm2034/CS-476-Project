using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Room2Room.Models.Accounts;
using Room2Room.Models.Chats;

namespace Room2Room.Controllers;

[Authorize]
public class ChatController : Controller
{
    // TODO: move out of controller, move db connection instantiation into factory
    private string ConnectionString;

    private readonly ApplicationDbContext _context;

    public ChatController(ApplicationDbContext context, IConfiguration configuration)
    {
        ConnectionString = configuration.GetConnectionString("DefaultConnection")!;

        var contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer(ConnectionString)
            .Options;

        _context = new ApplicationDbContext(contextOptions);
    }

    [HttpGet]
    public async Task<IActionResult> Index(int? forUserId)
    {
        int userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "AccountId")?.Value ?? "0");

        if (User.IsInRole("Admin") && forUserId.HasValue && forUserId > 0) {
            userId = forUserId.Value;
        }

        var chatModel = GetAllChats(userId);



        return PartialView("_Chats", chatModel);
    }

    [HttpGet] 
    public IActionResult GetNewMessages(DateTime since) {
        // get new chatModels since the supllied date time. Return this object, selectively update the messages ONLY. this solves all the poling issues
        since = since.ToUniversalTime().AddHours(-6); // convert to regina time
        int userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "AccountId")?.Value ?? "0");

        var chatIds = _context.ChatMember.Where(x => x.AccountId == userId).Select(x => x.ChatId).ToList();
        var chatsTask = _context.Chat.Where(x => chatIds.Contains(x.ChatId)).ToList();
        var messagesTask = _context.ChatMessage.Where(x => chatIds.Contains(x.ChatId) && x.CreatedAt > since).ToList();
        var accountIdsTask = _context.ChatMember.Where(x => chatIds.Contains(x.ChatId)).Select(x => x.AccountId).ToList();


        Dictionary<int, Item> itemIdToItemDict =
            _context.Items.Where(x => chatsTask.Select(x => x.ListingId).ToList().Contains(x.Id))
                .ToList().Distinct().ToDictionary(x => x.Id);

        List<Account> accounts = _context.Accounts.Where(x => accountIdsTask.Distinct().ToList().Contains(x.Id)).ToList();

        List<ChatModel> chatModels = [];
        foreach (Chat c in chatsTask) {
            var messages = messagesTask.Where(x => x.ChatId == c.ChatId).OrderBy(x => x.CreatedAt).ToList();

            if (messages.Count == 0) {
                continue;
            }
            var accountIds = messagesTask.Where(x => x.ChatId == c.ChatId).Select(x => x.FromAccountId).ToList().Distinct().ToList();
            var accountDict = accounts.Where(x => accountIds.Contains(x.Id)).ToDictionary(x => x.Id, x => x.Username);

            var cm = new ChatModel {
                Chat = c,
                Item = c.ListingId.HasValue ? itemIdToItemDict.GetValueOrDefault(c.ListingId.Value) : null,
                AccountIdToNameDictionary = accountDict,
                Messages = messages
            };

            chatModels.Add(cm);
        }

        return PartialView("_NewChats", chatModels);
    }

    
    [HttpPost]
    public IActionResult SendMessage(SendChatModel message) {
        if (string.IsNullOrEmpty(message.Message)) {
            return BadRequest();
        }

        var newMessage = new ChatMessage(message);

        var nextId = _context.ChatMessage.OrderByDescending(x => x.MessageId).First().MessageId + 1;
        
        newMessage.FromAccountId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "AccountId")?.Value ?? "0");
        newMessage.CreatedAt = DateTime.Now;
        newMessage.MessageId = nextId;
        _context.Add(newMessage);
        _context.SaveChanges();
        
        var chat = _context.Chat.Where(x => x.ChatId == newMessage.ChatId).ToList().First();
        var messagesTask = _context.ChatMessage.Where(x => x.ChatId == newMessage.ChatId).ToList();
        var accountIdsTask = _context.ChatMember.Where(x => x.ChatId == newMessage.ChatId).Select(x => x.AccountId).ToList();

        Dictionary<int, Item> itemIdToItemDict =
            _context.Items.Where(x => x.Id == chat.ListingId)
                .ToList().Distinct().ToDictionary(x => x.Id);

        List<Account> accounts = _context.Accounts.Where(x => accountIdsTask.Distinct().ToList().Contains(x.Id)).ToList();

        var accountIds = messagesTask.Where(x => x.ChatId == chat.ChatId).Select(x => x.FromAccountId).ToList().Distinct().ToList();
        var accountDict = accounts.Where(x => accountIds.Contains(x.Id)).ToDictionary(x => x.Id, x => x.Username);

        var cm = new ChatModel {
            Chat = chat,
            Item = chat.ListingId.HasValue ? itemIdToItemDict.GetValueOrDefault(chat.ListingId.Value) : null,
            AccountIdToNameDictionary = accountDict,
            Messages = messagesTask.Where(x => x.ChatId == chat.ChatId).OrderBy(x => x.CreatedAt).ToList()
        };

        return PartialView("_Messages", cm);
    }

    [HttpPost]
    public IActionResult SendFirstMessage(SendChatModel message) {
        if (!message.ListingId.HasValue) {
            return BadRequest();
        }

        var listing = _context.Items.Where(x => x.Id == message.ListingId).FirstOrDefault();

        if (listing == null) {
            return BadRequest();
        }

        int userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "AccountId")?.Value ?? "0");
        var chat = _context.Chat
            .Join(_context.ChatMember,
                  c => c.ChatId,
                  cm => cm.ChatId,
                  (c, cm) => new { Chat = c, ChatMember = cm })
            .Where(x => x.Chat.ListingId == message.ListingId && x.ChatMember.AccountId == userId)
            .Select(x => x.Chat)
            .FirstOrDefault();

        // TODO: handle if chat is null (create new chat, etc.)
        if (chat == null)
        {
            chat = new Chat();
            chat.ListingId = message.ListingId;
            chat.ChatId = _context.Chat.OrderByDescending(x => x.ChatId).First().ChatId + 1;
            _context.Chat.Add(chat);
            _context.SaveChanges();

            
            var user = new ChatMember() { AccountId = userId, ChatId = chat.ChatId };
            var other = new ChatMember() { AccountId = listing.AccountId, ChatId = chat.ChatId };
            _context.ChatMember.AddRange([user, other]);
            _context.SaveChanges();
        }

        var nextId = _context.ChatMessage.OrderByDescending(x => x.MessageId).First().MessageId + 1;

        var newMessage = new ChatMessage(message);
        newMessage.FromAccountId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "AccountId")?.Value ?? "0");
        newMessage.CreatedAt = DateTime.Now;
        newMessage.MessageId = nextId;
        newMessage.ChatId = chat.ChatId;

        _context.Add(newMessage);
        _context.SaveChanges();


        var allChatModel = GetAllChats(userId);
        int selectedIndex = allChatModel.ChatModels
            .Select((x, idx) => new { Model = x, Index = idx })
            .Where(x => x.Model.Item?.Id == message.ListingId)
            .Select(x => x.Index)
            .FirstOrDefault();
        allChatModel.SelectedIndex = selectedIndex;

        return PartialView("_Chats", allChatModel);
   }

    private AllChatsModel GetAllChats(int userId) {
        var chatIds = _context.ChatMember.Where(x => x.AccountId == userId).Select(x => x.ChatId).ToList();
        var chatsTask = _context.Chat.Where(x => chatIds.Contains(x.ChatId)).ToList();
        var messagesTask = _context.ChatMessage.Where(x => chatIds.Contains(x.ChatId)).ToList();
        var accountIdsTask = _context.ChatMember.Where(x => chatIds.Contains(x.ChatId)).Select(x => x.AccountId).ToList();


        Dictionary<int, Item> itemIdToItemDict =
            _context.Items.Where(x => chatsTask.Select(x => x.ListingId).ToList().Contains(x.Id))
                .ToList().Distinct().ToDictionary(x => x.Id);

        List<Account> accounts = _context.Accounts.Where(x => accountIdsTask.Distinct().ToList().Contains(x.Id)).ToList();

        
        List<ChatModel> chatModels = [];
        foreach (Chat c in chatsTask) {
            var messages = messagesTask.Where(x => x.ChatId == c.ChatId).OrderBy(x => x.CreatedAt).ToList();
            if (messages.Count == 0) {
                continue;
            }
            var accountIds = messagesTask.Where(x => x.ChatId == c.ChatId).Select(x => x.FromAccountId).ToList().Distinct().ToList();
            var accountDict = accounts.Where(x => accountIds.Contains(x.Id)).ToDictionary(x => x.Id, x => x.Username);

            var cm = new ChatModel {
                Chat = c,
                Item = c.ListingId.HasValue ? itemIdToItemDict.GetValueOrDefault(c.ListingId.Value) : null,
                AccountIdToNameDictionary = accountDict,
                Messages = messages
            };

            chatModels.Add(cm);
        }

        chatModels = chatModels.OrderByDescending(x => x.Messages.Last().CreatedAt).ToList();
        foreach (var c in chatModels) {
            c.ViewingId = userId;
        }

        return new AllChatsModel() { ChatModels = chatModels, SelectedIndex = 0, ViewingId = userId };
    }
}