using Microsoft.EntityFrameworkCore;
using Sways.Data;
using Sways.Models;

namespace Sways.Services;

public class ChatService
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public ChatService(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<List<UserAccount>> GetRecentChatUsersAsync(int currentUserId)
    {
        using var context = _contextFactory.CreateDbContext();

        var messageUserIds = await context.DirectMessages
            .Where(m => m.SenderId == currentUserId || m.ReceiverId == currentUserId)
            .OrderByDescending(m => m.SentAt)
            .Select(m => m.SenderId == currentUserId ? m.ReceiverId : m.SenderId)
            .Distinct()
            .Take(10)
            .ToListAsync();

        if (!messageUserIds.Any())
        {
            var following = await context.UserFollowers
                .Where(uf => uf.FollowerId == currentUserId)
                .Select(uf => uf.FollowingId)
                .Take(5)
                .ToListAsync();

            messageUserIds.AddRange(following);
        }

        return await context.UserAccounts
            .Where(u => messageUserIds.Contains(u.Id))
            .ToListAsync();
    }

    public async Task<List<DirectMessage>> GetChatMessagesAsync(int user1Id, int user2Id)
    {
        using var context = _contextFactory.CreateDbContext();
        return await context.DirectMessages
            .Where(m => (m.SenderId == user1Id && m.ReceiverId == user2Id) ||
                        (m.SenderId == user2Id && m.ReceiverId == user1Id))
            .OrderBy(m => m.SentAt)
            .Include(m => m.Sender)
            .Include(m => m.Receiver)
            .ToListAsync();
    }

    public async Task<DirectMessage> SendMessageAsync(int senderId, int receiverId, string content)
    {
        if (string.IsNullOrWhiteSpace(content)) throw new ArgumentException("Message content cannot be empty.");

        using var context = _contextFactory.CreateDbContext();
        var msg = new DirectMessage
        {
            SenderId = senderId,
            ReceiverId = receiverId,
            Content = content.Trim(),
            SentAt = DateTime.UtcNow,
            IsRead = false
        };

        context.DirectMessages.Add(msg);
        await context.SaveChangesAsync();
        return msg;
    }
}
