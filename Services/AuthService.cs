using Microsoft.EntityFrameworkCore;
using Sways.Data;
using Sways.Models;

namespace Sways.Services;

public class AuthService
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public AuthService(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<(bool Success, string Message, UserAccount? User)> RegisterAsync(
        string firstName, string lastName, string username, string email, string password, DateTime? birthDate)
    {
        using var context = _contextFactory.CreateDbContext();

        var existingUser = await context.UserAccounts.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower() || u.Username.ToLower() == username.ToLower());
        if (existingUser != null)
        {
            return (false, "Username or Email already exists.", null);
        }

        var newUser = new UserAccount
        {
            Name = $"{firstName} {lastName}".Trim(),
            Username = username,
            Email = email,
            PasswordHash = password, // In production use BCrypt/Identity, simple string here for custom auth
            BirthDate = birthDate,
            Gender = "they/them",
            AboutMe = "Hello! I am a new Sways user.",
            CreatedAt = DateTime.UtcNow
        };

        context.UserAccounts.Add(newUser);
        await context.SaveChangesAsync();

        return (true, "Registration successful!", newUser);
    }

    public async Task<(bool Success, string Message, UserAccount? User)> LoginAsync(string emailOrUsername, string password)
    {
        using var context = _contextFactory.CreateDbContext();

        var term = emailOrUsername.Trim().ToLower();
        var user = await context.UserAccounts.FirstOrDefaultAsync(u => u.Email.ToLower() == term || u.Username.ToLower() == term);

        if (user == null || user.PasswordHash != password)
        {
            return (false, "Invalid credentials.", null);
        }

        return (true, "Login successful!", user);
    }

    public async Task<UserAccount?> GetUserByIdAsync(int userId)
    {
        using var context = _contextFactory.CreateDbContext();
        return await context.UserAccounts.FindAsync(userId);
    }

    public async Task<List<UserAccount>> SearchUsersAsync(string query, int currentUserId)
    {
        using var context = _contextFactory.CreateDbContext();
        if (string.IsNullOrWhiteSpace(query))
        {
            return await context.UserAccounts
                .Where(u => u.Id != currentUserId)
                .Take(10)
                .ToListAsync();
        }

        var q = query.ToLower();
        return await context.UserAccounts
            .Where(u => u.Id != currentUserId && (u.Username.ToLower().Contains(q) || u.Name.ToLower().Contains(q)))
            .Take(10)
            .ToListAsync();
    }
}
