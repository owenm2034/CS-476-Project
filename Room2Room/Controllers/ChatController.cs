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
        var listingChats = _context.ListingChat.Where(x => chatIds.Contains(x.ChatId)).ToList();
        // var privateChats = _context.Chat.Where TODO
        var messagesTask = _context.ChatMessage.Where(x => chatIds.Contains(x.ChatId) && x.CreatedAt > since).ToList();
        var accountIdsTask = _context.ChatMember.Where(x => chatIds.Contains(x.ChatId)).Select(x => x.AccountId).ToList();


        Dictionary<int, Item> itemIdToItemDict =
            _context.Items.Where(x => listingChats.Select(x => x.ListingId).ToList().Contains(x.Id))
                .ToList().Distinct().ToDictionary(x => x.Id);

        List<Account> accounts = _context.Accounts.Where(x => accountIdsTask.Distinct().ToList().Contains(x.Id)).ToList();

        List<ChatModel> chatModels = [];
        foreach (ListingChat c in listingChats) {
            var messages = messagesTask.Where(x => x.ChatId == c.ChatId).OrderBy(x => x.CreatedAt).ToList();

            if (messages.Count == 0) {
                continue;
            }
            var accountIds = messagesTask.Where(x => x.ChatId == c.ChatId).Select(x => x.FromAccountId).ToList().Distinct().ToList();
            var accountDict = accounts.Where(x => accountIds.Contains(x.Id)).ToDictionary(x => x.Id, x => x.Username);

            var cm = new ChatModel {
                Chat = c,
                ChatName = itemIdToItemDict.GetValueOrDefault(c.ListingId)?.ItemName,
                AccountIdToNameDictionary = accountDict,
                Messages = messages
            };

            chatModels.Add(cm);
        }

        return PartialView("_NewChats", chatModels);
    }


    [HttpPost]
    public IActionResult SendMessage(SendChatModel message) {
        if (!message.ChatId.HasValue  && !message.ListingId.HasValue && !message.ToAccountId.HasValue) {
            return BadRequest();
        }
        if (string.IsNullOrEmpty(message.Message)) {
            return BadRequest();
        }

        int userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "AccountId")?.Value ?? "0");

        Chat chat = null!;

        // check chat existence, if not exists, use factory to create
        if (message.ChatId != null) {
            chat = _context.Chat.Where(x => x.ChatId == message.ChatId).ToList().First();
        } else if (message.ListingId != null) {
            var listing = _context.Items.Where(x => x.Id == message.ListingId).FirstOrDefault();

            if (listing == null) {
                return BadRequest();
            }

            chat = _context.ListingChat
                .Join(_context.ChatMember,
                    c => c.ChatId,
                    cm => cm.ChatId,
                    (c, cm) => new { Chat = c, ChatMember = cm })
                .Where(x => x.Chat.ListingId == message.ListingId && x.ChatMember.AccountId == userId)
                .Select(x => x.Chat)
                .FirstOrDefault()!;


            if (chat == null) {
                chat = ChatFactory.CreateChat("listing");
                chat.SetTarget([message.ListingId.Value]);
                chat.ChatId = _context.Chat.OrderByDescending(x => x.ChatId).First().ChatId + 1;
                _context.Chat.Add(chat);
                _context.SaveChanges();


                var user = new ChatMember() { AccountId = userId, ChatId = chat.ChatId };
                var other = new ChatMember() { AccountId = listing.AccountId, ChatId = chat.ChatId };
                _context.ChatMember.AddRange([user, other]);
                _context.SaveChanges();
            }
        } else if (message.ToAccountId != null) {
            // check if chat exists
            // see if chat with chat member other and accountid
            // if chat exists, do nothing other than assign private chat to
            chat = _context.PrivateChat
                .Join(_context.ChatMember,
                    c => c.ChatId,
                    cm => cm.ChatId,
                    (c, cm) => new { Chat = c, ChatMember = cm })
                .Where(x => x.ChatMember.AccountId == message.ToAccountId || x.ChatMember.AccountId == userId)
                .GroupBy(x => x.Chat.ChatId)
                .Where(g => g.Select(x => x.ChatMember.AccountId).Distinct().Count() == 2)
                .Select(g => g.First().Chat)
                .FirstOrDefault()!;

            if (chat == null) {
                chat = ChatFactory.CreateChat("private");
                chat.SetTarget([message.ListingId.Value]);
                chat.ChatId = _context.Chat.OrderByDescending(x => x.ChatId).First().ChatId + 1;
                _context.Chat.Add(chat);
                _context.SaveChanges();


                var user = new ChatMember() { AccountId = userId, ChatId = chat.ChatId };
                var other = new ChatMember() { AccountId = message.ToAccountId.Value, ChatId = chat.ChatId };
                _context.ChatMember.AddRange([user, other]);
                _context.SaveChanges();
            }
        }


        var nextId = _context.ChatMessage.OrderByDescending(x => x.MessageId).First().MessageId + 1;

        var newMessage = new ChatMessage(message);
        newMessage.FromAccountId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "AccountId")?.Value ?? "0");
        newMessage.CreatedAt = DateTime.Now;
        newMessage.MessageId = nextId;
        newMessage.ChatId = chat.ChatId;
        _context.Add(newMessage);
        _context.SaveChanges();


        var messagesTask = _context.ChatMessage.Where(x => x.ChatId == newMessage.ChatId).ToList();
        var accountIdsTask = _context.ChatMember.Where(x => x.ChatId == newMessage.ChatId).Select(x => x.AccountId).ToList();
        List<Account> accounts = _context.Accounts.Where(x => accountIdsTask.Distinct().ToList().Contains(x.Id)).ToList();
        ChatModel cm = null;

        if (chat is ListingChat lc) {
            Dictionary<int, Item> itemIdToItemDict =
                _context.Items.Where(x => x.Id == lc.ListingId)
                    .ToList().Distinct().ToDictionary(x => x.Id);


            var accountIds = messagesTask.Where(x => x.ChatId == lc.ChatId).Select(x => x.FromAccountId).ToList().Distinct().ToList();
            var accountDict = accounts.Where(x => accountIds.Contains(x.Id)).ToDictionary(x => x.Id, x => x.Username);


            cm = new ChatModel {
                Chat = lc,
                ChatName = itemIdToItemDict.GetValueOrDefault(lc.ListingId)?.ItemName,
                AccountIdToNameDictionary = accountDict,
                Messages = messagesTask.Where(x => x.ChatId == lc.ChatId).OrderBy(x => x.CreatedAt).ToList()
            };
        } else if (chat is PrivateChat pc) {
            var accountIds = messagesTask.Where(x => x.ChatId == pc.ChatId).Select(x => x.FromAccountId).ToList().Distinct().ToList();
            var accountDict = accounts.Where(x => accountIds.Contains(x.Id)).ToDictionary(x => x.Id, x => x.Username);
            pc.AccountIds = accountIds;

            cm = new ChatModel {
                Chat = pc,
                AccountIdToNameDictionary = accountDict,
                Messages = messagesTask.Where(x => x.ChatId == pc.ChatId).OrderBy(x => x.CreatedAt).ToList()
            };
        }

        return PartialView("_Messages", cm);
    }

    private AllChatsModel GetAllChats(int userId) {
        // todo, make work for private chats
        var chatIds = _context.ChatMember.Where(x => x.AccountId == userId).Select(x => x.ChatId).ToList();
        var chatsTask = _context.Chat.Where(x => chatIds.Contains(x.ChatId)).ToList();
        var messagesTask = _context.ChatMessage.Where(x => chatIds.Contains(x.ChatId)).ToList();
        var accountIdsTask = _context.ChatMember.Where(x => chatIds.Contains(x.ChatId)).Select(x => x.AccountId).ToList();

        IEnumerable<ListingChat> listingChats = chatsTask.Where(x => x is ListingChat).Select(x => x as ListingChat);
        IEnumerable<PrivateChat> privateChats = chatsTask.Where(x => x is PrivateChat).Select(x => x as PrivateChat);

        Dictionary<int, Item> itemIdToItemDict =
            _context.Items.Where(x => listingChats.Select(x => x.ListingId).ToList().Contains(x.Id))
                .ToList().Distinct().ToDictionary(x => x.Id);

        List<Account> accounts = _context.Accounts.Where(x => accountIdsTask.Distinct().ToList().Contains(x.Id)).ToList();


        List<ChatModel> chatModels = [];
        foreach (ListingChat c in listingChats) {
            var messages = messagesTask.Where(x => x.ChatId == c.ChatId).OrderBy(x => x.CreatedAt).ToList();
            if (messages.Count == 0) {
                continue;
            }
            var accountIds = messagesTask.Where(x => x.ChatId == c.ChatId).Select(x => x.FromAccountId).ToList().Distinct().ToList();
            var accountDict = accounts.Where(x => accountIds.Contains(x.Id)).ToDictionary(x => x.Id, x => x.Username);

            var cm = new ChatModel {
                Chat = c,
                ChatName = itemIdToItemDict.GetValueOrDefault(c.ListingId)?.ItemName,
                AccountIdToNameDictionary = accountDict,
                Messages = messages
            };

            chatModels.Add(cm);
        }

        foreach (PrivateChat pc in privateChats) {
            var messages = messagesTask.Where(x => x.ChatId == pc.ChatId).OrderBy(x => x.CreatedAt).ToList();
            if (messages.Count == 0) {
                continue;
            }
            var accountIds = messagesTask.Where(x => x.ChatId == pc.ChatId).Select(x => x.FromAccountId).ToList().Distinct().ToList();
            var accountDict = accounts.Where(x => accountIds.Contains(x.Id)).ToDictionary(x => x.Id, x => x.Username);

            pc.AccountIds = accountIds;

            var cm = new ChatModel {
                Chat = pc,
                ChatName = string.Join(", ", accountDict.Where(kvp => kvp.Key != userId).Select(x => x.Value)),
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