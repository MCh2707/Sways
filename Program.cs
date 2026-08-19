using Microsoft.EntityFrameworkCore;
using Sways.Components;
using Sways.Data;
using Sways.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<UserSession>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ProfileService>();
builder.Services.AddScoped<ClubService>();
builder.Services.AddScoped<ChatService>();
builder.Services.AddScoped<FileUploadService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var contextFactory = services.GetRequiredService<IDbContextFactory<AppDbContext>>();
    using var context = contextFactory.CreateDbContext();
    context.Database.EnsureCreated();

    try
    {
        var existingColumns = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        using (var cmd = context.Database.GetDbConnection().CreateCommand())
        {
            cmd.CommandText = "PRAGMA table_info(UserAccounts);";
            if (cmd.Connection!.State != System.Data.ConnectionState.Open) cmd.Connection.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                existingColumns.Add(reader.GetString(1));
            }
        }

        var requiredColumns = new Dictionary<string, string>
        {
            { "FirstName", "TEXT DEFAULT ''" },
            { "LastName", "TEXT DEFAULT ''" },
            { "Bio", "TEXT DEFAULT ''" },
            { "Country", "TEXT DEFAULT 'Georgia'" },
            { "City", "TEXT DEFAULT ''" },
            { "Languages", "TEXT DEFAULT ''" },
            { "EducationLevel", "TEXT DEFAULT ''" },
            { "SchoolName", "TEXT DEFAULT ''" },
            { "SchoolAddress", "TEXT DEFAULT ''" },
            { "SchoolClass", "TEXT DEFAULT ''" },
            { "ShowSchoolPublicly", "INTEGER DEFAULT 1" },
            { "UniversityName", "TEXT DEFAULT ''" },
            { "UniversityAddress", "TEXT DEFAULT ''" },
            { "UniversityEmail", "TEXT DEFAULT ''" },
            { "CollegeName", "TEXT DEFAULT ''" },
            { "CollegeAddress", "TEXT DEFAULT ''" },
            { "CollegeEmail", "TEXT DEFAULT ''" },
            { "WorkplaceCompany", "TEXT DEFAULT ''" },
            { "WorkplacePosition", "TEXT DEFAULT ''" },
            { "FacebookLink", "TEXT DEFAULT ''" },
            { "EquippedBadges", "TEXT DEFAULT ''" },
            { "IsPublicProfile", "INTEGER DEFAULT 1" },
            { "IsSchoolOnly", "INTEGER DEFAULT 0" },
            { "HideEmail", "INTEGER DEFAULT 0" },
            { "HideLocation", "INTEGER DEFAULT 0" },
            { "BannerUrl", "TEXT DEFAULT ''" },
            { "ThemeMode", "TEXT DEFAULT 'open'" },
            { "AccentColor", "TEXT DEFAULT 'blue'" }
        };

        foreach (var col in requiredColumns)
        {
            if (!existingColumns.Contains(col.Key))
            {
                try
                {
                    context.Database.ExecuteSqlRaw($"ALTER TABLE UserAccounts ADD COLUMN {col.Key} {col.Value};");
                }
                catch { }
            }
        }

        try
        {
            context.Database.ExecuteSqlRaw("ALTER TABLE Clubs ADD COLUMN BannerUrl TEXT DEFAULT '';");
        }
        catch { }
        try
        {
            context.Database.ExecuteSqlRaw("ALTER TABLE Clubs ADD COLUMN Category TEXT DEFAULT 'Engineering';");
        }
        catch { }

        try
        {
            context.Database.ExecuteSqlRaw("ALTER TABLE ClubMembers ADD COLUMN Role TEXT DEFAULT 'member';");
        }
        catch { }
        try
        {
            context.Database.ExecuteSqlRaw("ALTER TABLE ClubMembers ADD COLUMN IsBlocked INTEGER DEFAULT 0;");
        }
        catch { }

        try
        {
            context.Database.ExecuteSqlRaw(@"
                CREATE TABLE IF NOT EXISTS ClubChannels (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    ClubId INTEGER NOT NULL,
                    Name TEXT NOT NULL DEFAULT 'general',
                    Description TEXT NOT NULL DEFAULT '',
                    WritePermission TEXT NOT NULL DEFAULT 'everyone',
                    CreatedAt TEXT NOT NULL DEFAULT '0001-01-01 00:00:00',
                    FOREIGN KEY (ClubId) REFERENCES Clubs(Id) ON DELETE CASCADE
                );");
        }
        catch { }

        try
        {
            context.Database.ExecuteSqlRaw(@"
                CREATE TABLE IF NOT EXISTS ClubMessages (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    ClubId INTEGER NOT NULL,
                    ChannelId INTEGER NOT NULL,
                    UserId INTEGER NOT NULL,
                    Content TEXT NOT NULL DEFAULT '',
                    ImageUrl TEXT,
                    LinkUrl TEXT,
                    CreatedAt TEXT NOT NULL DEFAULT '0001-01-01 00:00:00',
                    FOREIGN KEY (ClubId) REFERENCES Clubs(Id) ON DELETE CASCADE,
                    FOREIGN KEY (ChannelId) REFERENCES ClubChannels(Id) ON DELETE CASCADE,
                    FOREIGN KEY (UserId) REFERENCES UserAccounts(Id) ON DELETE CASCADE
                );");
        }
        catch { }
    }
    catch { }
}


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
