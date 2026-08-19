using Microsoft.EntityFrameworkCore;
using Sways.Data;
using Sways.Models;

namespace Sways.Services;

public class ClubService
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public ClubService(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<bool> CanUserCreateClubAsync(int userId)
    {
        using var context = _contextFactory.CreateDbContext();
        var createdCount = await context.Clubs.CountAsync(c => c.CreatedByUserId == userId);
        return createdCount < 1; 
    }

    public async Task<Club> CreateClubAsync(string name, string description, string category, string? iconUrl, string? bannerUrl, int createdByUserId)
    {
        using var context = _contextFactory.CreateDbContext();
        
        var club = new Club
        {
            Name = name,
            Description = description,
            Category = string.IsNullOrWhiteSpace(category) ? "Engineering" : category,
            IconUrl = iconUrl,
            BannerUrl = bannerUrl,
            CreatedByUserId = createdByUserId,
            InviteCode = Guid.NewGuid().ToString("N")[..8].ToUpper(),
            CreatedAt = DateTime.UtcNow
        };

        context.Clubs.Add(club);
        await context.SaveChangesAsync();

        context.ClubMembers.Add(new ClubMember
        {
            ClubId = club.Id,
            UserId = createdByUserId,
            Role = "admin",
            JoinedAt = DateTime.UtcNow
        });

        context.ClubChannels.AddRange(
            new ClubChannel { ClubId = club.Id, Name = "general", Description = "General conversation for members", WritePermission = "everyone" },
            new ClubChannel { ClubId = club.Id, Name = "rules", Description = "Club rules & guidelines", WritePermission = "admin" },
            new ClubChannel { ClubId = club.Id, Name = "media", Description = "Photos, videos and projects", WritePermission = "everyone" }
        );

        await context.SaveChangesAsync();
        return club;
    }

    public async Task<List<Club>> GetUserClubsAsync(int userId)
    {
        using var context = _contextFactory.CreateDbContext();
        return await context.ClubMembers
            .Where(cm => cm.UserId == userId && !cm.IsBlocked)
            .Include(cm => cm.Club)
                .ThenInclude(c => c!.Members)
            .Select(cm => cm.Club!)
            .ToListAsync();
    }

    public async Task<Club?> GetClubByIdAsync(int clubId)
    {
        using var context = _contextFactory.CreateDbContext();
        return await context.Clubs
            .Include(c => c.CreatedByUser)
            .Include(c => c.Members)
                .ThenInclude(m => m.User)
            .Include(c => c.Channels)
            .FirstOrDefaultAsync(c => c.Id == clubId);
    }

    public async Task<(bool Success, string Message, Club? Club)> JoinClubByInviteCodeAsync(string inviteCodeOrLink, int userId)
    {
        using var context = _contextFactory.CreateDbContext();
        if (string.IsNullOrWhiteSpace(inviteCodeOrLink))
        {
            return (false, "Please enter a valid link or invite code.", null);
        }

        var code = inviteCodeOrLink.Trim();
        if (code.Contains("/"))
        {
            code = code.Split('/').Last();
        }
        if (code.StartsWith("#"))
        {
            code = code[1..];
        }

        var club = await context.Clubs.FirstOrDefaultAsync(c => c.InviteCode.ToLower() == code.ToLower());
        if (club == null)
        {
            return (false, "Club / Server not found with that link or code.", null);
        }

        var member = await context.ClubMembers.FirstOrDefaultAsync(cm => cm.ClubId == club.Id && cm.UserId == userId);
        if (member != null)
        {
            if (member.IsBlocked) return (false, "You are blocked from joining this club.", null);
            return (true, "You are already a member of this club!", club);
        }

        context.ClubMembers.Add(new ClubMember
        {
            ClubId = club.Id,
            UserId = userId,
            Role = "member",
            JoinedAt = DateTime.UtcNow
        });
        await context.SaveChangesAsync();

        return (true, $"Successfully joined {club.Name}!", club);
    }

    public async Task<List<ClubChannel>> GetClubChannelsAsync(int clubId)
    {
        using var context = _contextFactory.CreateDbContext();
        var channels = await context.ClubChannels
            .Where(cc => cc.ClubId == clubId)
            .OrderBy(cc => cc.Id)
            .ToListAsync();

        if (channels.Count == 0)
        {
            var defaultChannel = new ClubChannel { ClubId = clubId, Name = "general", Description = "General conversation" };
            context.ClubChannels.Add(defaultChannel);
            await context.SaveChangesAsync();
            channels.Add(defaultChannel);
        }
        return channels;
    }

    public async Task<ClubChannel> CreateChannelAsync(int clubId, string name, string writePermission, int actionByUserId)
    {
        using var context = _contextFactory.CreateDbContext();
        var member = await context.ClubMembers.FirstOrDefaultAsync(cm => cm.ClubId == clubId && cm.UserId == actionByUserId);
        if (member == null || (member.Role != "admin" && member.Role != "moderator"))
        {
            throw new InvalidOperationException("Only admins and moderators can add channels.");
        }

        var channel = new ClubChannel
        {
            ClubId = clubId,
            Name = name.ToLower().Replace(" ", "-"),
            WritePermission = writePermission,
            CreatedAt = DateTime.UtcNow
        };
        context.ClubChannels.Add(channel);
        await context.SaveChangesAsync();
        return channel;
    }

    public async Task DeleteChannelAsync(int clubId, int channelId, int actionByUserId)
    {
        using var context = _contextFactory.CreateDbContext();
        var adminMember = await context.ClubMembers.FirstOrDefaultAsync(cm => cm.ClubId == clubId && cm.UserId == actionByUserId);
        if (adminMember == null || (adminMember.Role != "admin" && adminMember.Role != "moderator")) return;

        var channel = await context.ClubChannels.FirstOrDefaultAsync(cc => cc.Id == channelId && cc.ClubId == clubId);
        if (channel != null && channel.Name != "general")
        {
            context.ClubChannels.Remove(channel);
            await context.SaveChangesAsync();
        }
    }

    public async Task<List<ClubMessage>> GetChannelMessagesAsync(int channelId)
    {
        using var context = _contextFactory.CreateDbContext();
        return await context.ClubMessages
            .Where(cm => cm.ChannelId == channelId)
            .Include(cm => cm.User)
            .OrderBy(cm => cm.CreatedAt)
            .ToListAsync();
    }

    public async Task<ClubMessage> SendChannelMessageAsync(int clubId, int channelId, int userId, string content, string? imageUrl = null, string? linkUrl = null)
    {
        using var context = _contextFactory.CreateDbContext();
        
        var channel = await context.ClubChannels.FindAsync(channelId);
        var member = await context.ClubMembers.FirstOrDefaultAsync(cm => cm.ClubId == clubId && cm.UserId == userId);

        if (member == null || member.IsBlocked)
        {
            throw new InvalidOperationException("You are not a member of this club.");
        }

        if (channel != null && channel.WritePermission == "admin" && member.Role != "admin")
        {
            throw new InvalidOperationException("Only admins can post messages in this channel.");
        }
        if (channel != null && channel.WritePermission == "moderator" && member.Role != "admin" && member.Role != "moderator")
        {
            throw new InvalidOperationException("Only moderators and admins can post in this channel.");
        }

        var msg = new ClubMessage
        {
            ClubId = clubId,
            ChannelId = channelId,
            UserId = userId,
            Content = content,
            ImageUrl = imageUrl,
            LinkUrl = linkUrl,
            CreatedAt = DateTime.UtcNow
        };

        context.ClubMessages.Add(msg);
        await context.SaveChangesAsync();

        msg.User = await context.UserAccounts.FindAsync(userId);
        return msg;
    }

    public async Task<List<ClubMember>> GetClubMembersAsync(int clubId)
    {
        using var context = _contextFactory.CreateDbContext();
        return await context.ClubMembers
            .Where(cm => cm.ClubId == clubId && !cm.IsBlocked)
            .Include(cm => cm.User)
            .OrderByDescending(cm => cm.Role == "admin")
            .ThenByDescending(cm => cm.Role == "moderator")
            .ToListAsync();
    }

    public async Task UpdateMemberRoleAsync(int clubId, int targetUserId, string newRole, int actionByUserId)
    {
        using var context = _contextFactory.CreateDbContext();
        var adminMember = await context.ClubMembers.FirstOrDefaultAsync(cm => cm.ClubId == clubId && cm.UserId == actionByUserId);
        if (adminMember == null || adminMember.Role != "admin") return;

        var targetMember = await context.ClubMembers.FirstOrDefaultAsync(cm => cm.ClubId == clubId && cm.UserId == targetUserId);
        if (targetMember != null && targetMember.Role != "admin")
        {
            targetMember.Role = newRole;
            await context.SaveChangesAsync();
        }
    }

    public async Task KickMemberAsync(int clubId, int targetUserId, int actionByUserId)
    {
        using var context = _contextFactory.CreateDbContext();
        var adminMember = await context.ClubMembers.FirstOrDefaultAsync(cm => cm.ClubId == clubId && cm.UserId == actionByUserId);
        if (adminMember == null || (adminMember.Role != "admin" && adminMember.Role != "moderator")) return;

        var targetMember = await context.ClubMembers.FirstOrDefaultAsync(cm => cm.ClubId == clubId && cm.UserId == targetUserId);
        if (targetMember != null && targetMember.Role != "admin")
        {
            context.ClubMembers.Remove(targetMember);
            await context.SaveChangesAsync();
        }
    }

    public async Task BlockMemberAsync(int clubId, int targetUserId, int actionByUserId)
    {
        using var context = _contextFactory.CreateDbContext();
        var adminMember = await context.ClubMembers.FirstOrDefaultAsync(cm => cm.ClubId == clubId && cm.UserId == actionByUserId);
        if (adminMember == null || adminMember.Role != "admin") return;

        var targetMember = await context.ClubMembers.FirstOrDefaultAsync(cm => cm.ClubId == clubId && cm.UserId == targetUserId);
        if (targetMember != null && targetMember.Role != "admin")
        {
            targetMember.IsBlocked = true;
            await context.SaveChangesAsync();
        }
    }

    public async Task UpdateClubDetailsAsync(int clubId, string name, string description, string category, string? iconUrl, string? bannerUrl, int actionByUserId)
    {
        using var context = _contextFactory.CreateDbContext();
        var adminMember = await context.ClubMembers.FirstOrDefaultAsync(cm => cm.ClubId == clubId && cm.UserId == actionByUserId);
        if (adminMember == null || adminMember.Role != "admin") return;

        var club = await context.Clubs.FindAsync(clubId);
        if (club != null)
        {
            club.Name = name;
            club.Description = description;
            club.Category = category;
            if (!string.IsNullOrEmpty(iconUrl)) club.IconUrl = iconUrl;
            if (!string.IsNullOrEmpty(bannerUrl)) club.BannerUrl = bannerUrl;
            await context.SaveChangesAsync();
        }
    }
}
