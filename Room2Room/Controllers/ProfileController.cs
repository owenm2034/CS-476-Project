using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Room2Room.Data;
using Room2Room.Models.DTOs;
using Room2Room.Repositories;

namespace Room2Room.Controllers;

[Authorize]
public class ProfileController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWatchlistRepository _watchlistRepository;

    public ProfileController(ApplicationDbContext context, IWatchlistRepository watchlistRepository)
    {
        _context = context;
        _watchlistRepository = watchlistRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var accountIdClaim = User.Claims.FirstOrDefault(c => c.Type == "AccountId")?.Value;
        if (!int.TryParse(accountIdClaim, out var accountId))
        {
            return RedirectToAction("LogIn", "Account");
        }

        var account = await _context.Accounts.FirstOrDefaultAsync(x => x.Id == accountId);
        if (account == null)
        {
            return RedirectToAction("LogIn", "Account");
        }

        var savedListings = (await _watchlistRepository.GetByUserIdAsync(accountId)).ToList();
        var activeChats = await GetActiveChatsAsync(accountId);

        var model = new ProfileViewModel
        {
            Username = account.Username,
            ProfilePictureUrl = account.ProfilePictureUrl ?? string.Empty,
            SavedListings = savedListings,
            ActiveChats = activeChats
        };

        return View(model);
    }

    private async Task<List<ProfileChatPreviewModel>> GetActiveChatsAsync(int currentUserId)
    {
        var chatIds = await _context.ChatMember
            .Where(cm => cm.AccountId == currentUserId)
            .Select(cm => cm.ChatId)
            .Distinct()
            .ToListAsync();

        if (chatIds.Count == 0)
        {
            return [];
        }

        var allMessages = await _context.ChatMessage
            .Where(m => chatIds.Contains(m.ChatId))
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();

        var latestMessageByChatId = allMessages
            .GroupBy(m => m.ChatId)
            .ToDictionary(g => g.Key, g => g.First());

        if (latestMessageByChatId.Count == 0)
        {
            return [];
        }

        var listingChats = await _context.ListingChat
            .Where(c => chatIds.Contains(c.ChatId))
            .Select(c => new { c.ChatId, c.ListingId })
            .ToListAsync();

        var listingIds = listingChats.Select(c => c.ListingId).Distinct().ToList();
        var listingNameById = await _context.Items
            .Where(i => listingIds.Contains(i.Id))
            .ToDictionaryAsync(i => i.Id, i => i.ItemName ?? $"Listing #{i.Id}");

        var privateChats = await _context.PrivateChat
            .Where(c => chatIds.Contains(c.ChatId))
            .Select(c => c.ChatId)
            .ToListAsync();

        var privateChatMembers = await _context.ChatMember
            .Where(cm => privateChats.Contains(cm.ChatId) && cm.AccountId != currentUserId)
            .ToListAsync();

        var otherAccountIds = privateChatMembers.Select(cm => cm.AccountId).Distinct().ToList();
        var accountNameById = await _context.Accounts
            .Where(a => otherAccountIds.Contains(a.Id))
            .ToDictionaryAsync(a => a.Id, a => a.Username);

        var privateChatNameById = privateChatMembers
            .GroupBy(cm => cm.ChatId)
            .ToDictionary(
                g => g.Key,
                g =>
                {
                    var participants = g
                        .Select(x => accountNameById.GetValueOrDefault(x.AccountId, $"User #{x.AccountId}"))
                        .Distinct();
                    return string.Join(", ", participants);
                }
            );

        var result = new List<ProfileChatPreviewModel>();

        foreach (var listingChat in listingChats)
        {
            if (!latestMessageByChatId.TryGetValue(listingChat.ChatId, out var latestMessage))
            {
                continue;
            }

            result.Add(new ProfileChatPreviewModel
            {
                ChatId = listingChat.ChatId,
                ChatName = listingNameById.GetValueOrDefault(
                    listingChat.ListingId,
                    $"Listing #{listingChat.ListingId}"
                ),
                LastMessagePreview = latestMessage.Message,
                LastMessageAt = latestMessage.CreatedAt,
                IsListingChat = true
            });
        }

        foreach (var privateChatId in privateChats)
        {
            if (!latestMessageByChatId.TryGetValue(privateChatId, out var latestMessage))
            {
                continue;
            }

            result.Add(new ProfileChatPreviewModel
            {
                ChatId = privateChatId,
                ChatName = privateChatNameById.GetValueOrDefault(privateChatId, "Private chat"),
                LastMessagePreview = latestMessage.Message,
                LastMessageAt = latestMessage.CreatedAt,
                IsListingChat = false
            });
        }

        return result
            .OrderByDescending(x => x.LastMessageAt)
            .ToList();
    }
}
