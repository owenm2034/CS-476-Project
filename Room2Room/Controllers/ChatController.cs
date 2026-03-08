using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Room2Room.Data;
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
    public async Task<IActionResult> Index()
    {
        int userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "AccountId")?.Value ?? "0");

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
            var accountIds = messagesTask.Where(x => x.ChatId == c.ChatId).Select(x => x.FromAccountId).ToList().Distinct().ToList();
            var accountDict = accounts.Where(x => accountIds.Contains(x.Id)).ToDictionary(x => x.Id, x => x.Username);

            var cm = new ChatModel {
                Chat = c,
                Item = c.ListingId.HasValue ? itemIdToItemDict.GetValueOrDefault(c.ListingId.Value) : null,
                AccountIdToNameDictionary = accountDict,
                Messages = messagesTask.Where(x => x.ChatId == c.ChatId).OrderBy(x => x.CreatedAt).ToList()
            };

            chatModels.Add(cm);
        }

        return PartialView("_Chats", chatModels);
    }

    
    [HttpPost]
    public IActionResult SendMessage(ChatMessage message) {
        // todo
        var nextId = _context.ChatMessage.OrderByDescending(x => x.MessageId).First().MessageId + 1;
        
        message.FromAccountId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "AccountId")?.Value ?? "0");
        message.CreatedAt = DateTime.Now;
        message.MessageId = nextId;
        _context.Add(message);
        _context.SaveChanges();
        
        var chat = _context.Chat.Where(x => x.ChatId == message.ChatId).ToList().First();
        var messagesTask = _context.ChatMessage.Where(x => x.ChatId == message.ChatId).ToList();
        var accountIdsTask = _context.ChatMember.Where(x => x.ChatId == message.ChatId).Select(x => x.AccountId).ToList();

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
}