using Microsoft.EntityFrameworkCore;
using Sways.Data;
using Sways.Models;

namespace Sways.Services;

public class FeedItem
{
    public bool IsPoll { get; set; }
    public BlogPost? Blog { get; set; }
    public PollPost? Poll { get; set; }
    public DateTime DatePosted => IsPoll ? Poll!.DatePosted : Blog!.DatePosted;
}

public class AchievementBadge
{
    public string Id { get; set; } = string.Empty;
    public string Emoji { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Rarity { get; set; } = "common"; 
    public bool IsUnlocked { get; set; }
    public DateTime? UnlockedDate { get; set; }
    public bool IsEquipped { get; set; }
}

public class ProfileService
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public ProfileService(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<UserAccount> GetUserProfileAsync(int userId)
    {
        using var context = _contextFactory.CreateDbContext();
        var profile = await context.UserAccounts.FindAsync(userId);
        if (profile == null)
        {
            profile = await context.UserAccounts.FirstOrDefaultAsync() ?? new UserAccount { Name = "Demo User", Username = "username" };
        }
        return profile;
    }

    public async Task UpdateUserProfileAsync(UserAccount profile)
    {
        using var context = _contextFactory.CreateDbContext();
        context.UserAccounts.Update(profile);
        await context.SaveChangesAsync();
    }

    public async Task SaveUserThemeAsync(int userId, string themeMode, string accentColor)
    {
        using var context = _contextFactory.CreateDbContext();
        var user = await context.UserAccounts.FindAsync(userId);
        if (user != null)
        {
            user.ThemeMode = themeMode;
            user.AccentColor = accentColor;
            await context.SaveChangesAsync();
        }
    }

    public async Task ChangePasswordAsync(int userId, string newPassword)
    {
        using var context = _contextFactory.CreateDbContext();
        var user = await context.UserAccounts.FindAsync(userId);
        if (user != null)
        {
            user.PasswordHash = newPassword;
            await context.SaveChangesAsync();
        }
    }

    public async Task DeleteUserAccountAsync(int userId)
    {
        using var context = _contextFactory.CreateDbContext();
        var user = await context.UserAccounts.FindAsync(userId);
        if (user != null)
        {
            context.UserAccounts.Remove(user);
            await context.SaveChangesAsync();
        }
    }

    public async Task<(int FollowersCount, int FollowingCount, int PostCount)> GetUserStatsAsync(int userId)
    {
        using var context = _contextFactory.CreateDbContext();
        int followersCount = await context.UserFollowers.CountAsync(uf => uf.FollowingId == userId);
        int followingCount = await context.UserFollowers.CountAsync(uf => uf.FollowerId == userId);
        int blogCount = await context.BlogPosts.CountAsync(b => b.UserId == userId);
        int pollCount = await context.PollPosts.CountAsync(p => p.UserId == userId);

        return (followersCount, followingCount, blogCount + pollCount);
    }

    public async Task<List<UserAccount>> GetFollowersAsync(int userId)
    {
        using var context = _contextFactory.CreateDbContext();
        return await context.UserFollowers
            .Where(uf => uf.FollowingId == userId)
            .Include(uf => uf.Follower)
            .Select(uf => uf.Follower!)
            .ToListAsync();
    }

    public async Task<int> GetFollowersCountAsync(int userId)
    {
        using var context = _contextFactory.CreateDbContext();
        return await context.UserFollowers.CountAsync(uf => uf.FollowingId == userId);
    }

    public async Task<int> GetFollowingCountAsync(int userId)
    {
        using var context = _contextFactory.CreateDbContext();
        return await context.UserFollowers.CountAsync(uf => uf.FollowerId == userId);
    }

    public string GetBadgeEmojis(string equippedBadges)
    {
        if (string.IsNullOrWhiteSpace(equippedBadges)) return "";
        var ids = equippedBadges.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToHashSet();
        var dict = new Dictionary<string, string>
        {
            { "first-article", "✍️" },
            { "young-journalist", "📰" },
            { "on-fire", "🔥" },
            { "community-fav", "❤️" },
            { "conversation", "💬" },
            { "photographer", "📸" },
            { "creative-mind", "🎨" },
            { "bookworm", "📚" },
            { "rising-star", "🌟" },
            { "top-contributor", "🏆" },
            { "master-writer", "🔒" }
        };
        var emojis = ids.Where(id => dict.ContainsKey(id)).Select(id => dict[id]);
        return string.Join(" ", emojis);
    }

    public async Task<List<UserAccount>> GetFollowingAsync(int userId)
    {
        using var context = _contextFactory.CreateDbContext();
        return await context.UserFollowers
            .Where(uf => uf.FollowerId == userId)
            .Include(uf => uf.Following)
            .Select(uf => uf.Following!)
            .ToListAsync();
    }

    public async Task<bool> IsFollowingAsync(int followerId, int targetUserId)
    {
        using var context = _contextFactory.CreateDbContext();
        return await context.UserFollowers.AnyAsync(uf => uf.FollowerId == followerId && uf.FollowingId == targetUserId);
    }

    public async Task ToggleFollowAsync(int followerId, int targetUserId)
    {
        if (followerId == targetUserId) return;
        using var context = _contextFactory.CreateDbContext();
        var existing = await context.UserFollowers.FirstOrDefaultAsync(uf => uf.FollowerId == followerId && uf.FollowingId == targetUserId);
        if (existing != null)
        {
            context.UserFollowers.Remove(existing);
        }
        else
        {
            context.UserFollowers.Add(new UserFollower { FollowerId = followerId, FollowingId = targetUserId, FollowedAt = DateTime.UtcNow });
        }
        await context.SaveChangesAsync();
    }

    public async Task<List<BlogPost>> GetUserBlogPostsAsync(int userId)
    {
        using var context = _contextFactory.CreateDbContext();
        return await context.BlogPosts
            .Where(b => b.UserId == userId)
            .Include(b => b.User)
            .Include(b => b.Likes)
            .Include(b => b.Comments).ThenInclude(c => c.User)
            .OrderByDescending(p => p.DatePosted)
            .ToListAsync();
    }

    public async Task AddBlogPostAsync(BlogPost post)
    {
        using var context = _contextFactory.CreateDbContext();
        post.DatePosted = DateTime.UtcNow;
        context.BlogPosts.Add(post);
        await context.SaveChangesAsync();
    }

    public async Task UpdateBlogPostAsync(BlogPost post)
    {
        using var context = _contextFactory.CreateDbContext();
        post.LastEdited = DateTime.UtcNow;
        context.BlogPosts.Update(post);
        await context.SaveChangesAsync();
    }

    public async Task DeleteBlogPostAsync(int id)
    {
        using var context = _contextFactory.CreateDbContext();
        var post = await context.BlogPosts.FindAsync(id);
        if (post != null)
        {
            context.BlogPosts.Remove(post);
            await context.SaveChangesAsync();
        }
    }

    public async Task AddPollPostAsync(PollPost poll, List<string> optionTexts)
    {
        using var context = _contextFactory.CreateDbContext();
        poll.DatePosted = DateTime.UtcNow;
        context.PollPosts.Add(poll);
        await context.SaveChangesAsync();

        foreach (var opt in optionTexts)
        {
            if (!string.IsNullOrWhiteSpace(opt))
            {
                context.PollOptions.Add(new PollOption { PollPostId = poll.Id, Text = opt.Trim() });
            }
        }
        await context.SaveChangesAsync();
    }

    public async Task<bool> VotePollAsync(int pollId, int optionId, int userId)
    {
        using var context = _contextFactory.CreateDbContext();
        var poll = await context.PollPosts.Include(p => p.Votes).FirstOrDefaultAsync(p => p.Id == pollId);
        if (poll == null) return false;

        var existingVote = poll.Votes.FirstOrDefault(v => v.UserId == userId);
        if (poll.IsSingleChoice)
        {
            if (existingVote != null) return false;

            context.PollVotes.Add(new PollVote
            {
                PollPostId = pollId,
                PollOptionId = optionId,
                UserId = userId,
                VotedAt = DateTime.UtcNow
            });
        }
        else
        {
            if (existingVote != null)
            {
                context.PollVotes.Remove(existingVote);
            }
            context.PollVotes.Add(new PollVote
            {
                PollPostId = pollId,
                PollOptionId = optionId,
                UserId = userId,
                VotedAt = DateTime.UtcNow
            });
        }
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<List<FeedItem>> GetCombinedFeedAsync()
    {
        using var context = _contextFactory.CreateDbContext();

        var blogs = await context.BlogPosts
            .Include(b => b.User)
            .Include(b => b.Likes)
            .Include(b => b.Comments).ThenInclude(c => c.User)
            .ToListAsync();

        var polls = await context.PollPosts
            .Include(p => p.User)
            .Include(p => p.Options)
            .Include(p => p.Votes)
            .Include(p => p.Likes)
            .Include(p => p.Comments).ThenInclude(c => c.User)
            .ToListAsync();

        var feed = new List<FeedItem>();
        feed.AddRange(blogs.Select(b => new FeedItem { IsPoll = false, Blog = b }));
        feed.AddRange(polls.Select(p => new FeedItem { IsPoll = true, Poll = p }));

        return feed.OrderByDescending(f => f.DatePosted).ToList();
    }

    public async Task ToggleLikeBlogAsync(int blogId, int userId)
    {
        using var context = _contextFactory.CreateDbContext();
        var existing = await context.PostLikes.FirstOrDefaultAsync(l => l.BlogPostId == blogId && l.UserId == userId);
        if (existing != null)
        {
            context.PostLikes.Remove(existing);
        }
        else
        {
            context.PostLikes.Add(new PostLike { BlogPostId = blogId, UserId = userId, LikedAt = DateTime.UtcNow });
        }
        await context.SaveChangesAsync();
    }

    public async Task ToggleLikePollAsync(int pollId, int userId)
    {
        using var context = _contextFactory.CreateDbContext();
        var existing = await context.PostLikes.FirstOrDefaultAsync(l => l.PollPostId == pollId && l.UserId == userId);
        if (existing != null)
        {
            context.PostLikes.Remove(existing);
        }
        else
        {
            context.PostLikes.Add(new PostLike { PollPostId = pollId, UserId = userId, LikedAt = DateTime.UtcNow });
        }
        await context.SaveChangesAsync();
    }

    public async Task AddCommentToBlogAsync(int blogId, int userId, string commentText)
    {
        if (string.IsNullOrWhiteSpace(commentText)) return;
        using var context = _contextFactory.CreateDbContext();
        context.PostComments.Add(new PostComment
        {
            BlogPostId = blogId,
            UserId = userId,
            Content = commentText.Trim(),
            CreatedAt = DateTime.UtcNow
        });
        await context.SaveChangesAsync();
    }

    public async Task AddCommentToPollAsync(int pollId, int userId, string commentText)
    {
        if (string.IsNullOrWhiteSpace(commentText)) return;
        using var context = _contextFactory.CreateDbContext();
        context.PostComments.Add(new PostComment
        {
            PollPostId = pollId,
            UserId = userId,
            Content = commentText.Trim(),
            CreatedAt = DateTime.UtcNow
        });
        await context.SaveChangesAsync();
    }

    public async Task<List<AchievementBadge>> GetAchievementsAsync(int userId)
    {
        using var context = _contextFactory.CreateDbContext();
        var user = await context.UserAccounts.FindAsync(userId);
        if (user == null) return new();

        int blogCount = await context.BlogPosts.CountAsync(b => b.UserId == userId);
        int pollCount = await context.PollPosts.CountAsync(p => p.UserId == userId);
        int totalPosts = blogCount + pollCount;
        int commentCount = await context.PostComments.CountAsync(c => c.UserId == userId);
        
        int blogLikes = await context.PostLikes.CountAsync(l => l.BlogPost != null && l.BlogPost.UserId == userId);
        int pollLikes = await context.PostLikes.CountAsync(l => l.PollPost != null && l.PollPost.UserId == userId);
        int totalLikes = blogLikes + pollLikes;

        var equippedList = (user.EquippedBadges ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToHashSet();

        var badges = new List<AchievementBadge>
        {
            new AchievementBadge { Id = "first-article", Emoji = "✍️", Title = "First Steps", Description = "Publish your first article", Rarity = "common", IsUnlocked = totalPosts >= 1, UnlockedDate = DateTime.UtcNow.AddDays(-10) },
            new AchievementBadge { Id = "young-journalist", Emoji = "📰", Title = "Young Journalist", Description = "Publish 5 articles", Rarity = "common", IsUnlocked = totalPosts >= 5, UnlockedDate = DateTime.UtcNow.AddDays(-2) },
            new AchievementBadge { Id = "on-fire", Emoji = "🔥", Title = "On Fire", Description = "Publish 10 articles", Rarity = "rare", IsUnlocked = totalPosts >= 10 },
            new AchievementBadge { Id = "community-fav", Emoji = "❤️", Title = "Community Favorite", Description = "Get 50 likes", Rarity = "rare", IsUnlocked = totalLikes >= 50 },
            new AchievementBadge { Id = "conversation", Emoji = "💬", Title = "Conversation Starter", Description = "Write 10 comments", Rarity = "epic", IsUnlocked = commentCount >= 10 },
            new AchievementBadge { Id = "photographer", Emoji = "📸", Title = "Photographer", Description = "Upload your first photo", Rarity = "common", IsUnlocked = true, UnlockedDate = DateTime.UtcNow.AddDays(-15) },
            new AchievementBadge { Id = "creative-mind", Emoji = "🎨", Title = "Creative Mind", Description = "Publish a creative post / poll", Rarity = "rare", IsUnlocked = pollCount >= 1, UnlockedDate = DateTime.UtcNow.AddDays(-1) },
            new AchievementBadge { Id = "bookworm", Emoji = "📚", Title = "Bookworm", Description = "Read 10 articles", Rarity = "common", IsUnlocked = true, UnlockedDate = DateTime.UtcNow.AddDays(-5) },
            new AchievementBadge { Id = "rising-star", Emoji = "🌟", Title = "Rising Star", Description = "Get 100 total reactions", Rarity = "epic", IsUnlocked = totalLikes >= 100 },
            new AchievementBadge { Id = "top-contributor", Emoji = "🏆", Title = "Top Contributor", Description = "Most active author of the month", Rarity = "legendary", IsUnlocked = false },
            new AchievementBadge { Id = "master-writer", Emoji = "🔒", Title = "Master Writer", Description = "Publish 25 articles", Rarity = "epic", IsUnlocked = totalPosts >= 25 }
        };

        foreach (var b in badges)
        {
            b.IsEquipped = equippedList.Contains(b.Id);
        }

        return badges;
    }

    public async Task ToggleEquipBadgeAsync(int userId, string badgeId)
    {
        using var context = _contextFactory.CreateDbContext();
        var user = await context.UserAccounts.FindAsync(userId);
        if (user == null) return;

        var equippedList = (user.EquippedBadges ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList();
        if (equippedList.Contains(badgeId))
        {
            equippedList.Remove(badgeId);
        }
        else
        {
            equippedList.Add(badgeId);
        }

        user.EquippedBadges = string.Join(",", equippedList);
        await context.SaveChangesAsync();
    }

    public async Task<List<UserAccount>> GetAllUsersAsync()
    {
        using var context = _contextFactory.CreateDbContext();
        return await context.UserAccounts.Take(50).ToListAsync();
    }
}

