using Sways.Models;

namespace Sways.Services;

public class UserSession
{
    public UserAccount? CurrentUser { get; private set; }
    public bool IsLoggedIn => CurrentUser != null;

    public string ThemeMode { get; private set; } = "open";
    public string AccentColor { get; private set; } = "blue";

    public event Action? OnStateChanged;

    public void Login(UserAccount user)
    {
        CurrentUser = user;
        if (!string.IsNullOrEmpty(user.ThemeMode)) ThemeMode = user.ThemeMode;
        if (!string.IsNullOrEmpty(user.AccentColor)) AccentColor = user.AccentColor;
        NotifyStateChanged();
    }

    public void Logout()
    {
        CurrentUser = null;
        ThemeMode = "open";
        AccentColor = "blue";
        NotifyStateChanged();
    }

    public void UpdateCurrentUser(UserAccount user)
    {
        CurrentUser = user;
        if (!string.IsNullOrEmpty(user.ThemeMode)) ThemeMode = user.ThemeMode;
        if (!string.IsNullOrEmpty(user.AccentColor)) AccentColor = user.AccentColor;
        NotifyStateChanged();
    }

    public void SetTheme(string mode, string accent)
    {
        ThemeMode = mode;
        AccentColor = accent;
        if (CurrentUser != null)
        {
            CurrentUser.ThemeMode = mode;
            CurrentUser.AccentColor = accent;
        }
        NotifyStateChanged();
    }

    public string GetThemeStyle()
    {
        bool isLight = ThemeMode == "open";

        string bg = isLight ? "#f4f6fb" : "#0b0c10";
        string panel = isLight ? "#ffffff" : "#161b26";
        string panelSoft = isLight ? "#f8fafc" : "#202533";
        string input = isLight ? "#ffffff" : "#1e2330";
        string text = isLight ? "#0f172a" : "#f8fafc";
        string muted = isLight ? "#64748b" : "#94a3b8";
        string line = isLight ? "#e2e8f0" : "#2d3748";
        string shadow = isLight ? "rgba(0, 0, 0, 0.06)" : "rgba(0, 0, 0, 0.4)";

        string accentHex = AccentColor switch
        {
            "pink" => "#ec4899",
            "blue" => "#4169e1",
            "green" => "#10b981",
            "purple" => "#8b5cf6",
            "orange" => "#f97316",
            _ => "#4169e1"
        };
        string accentSoft = AccentColor switch
        {
            "pink" => "rgba(236, 72, 153, 0.18)",
            "blue" => "rgba(65, 105, 225, 0.18)",
            "green" => "rgba(16, 185, 129, 0.18)",
            "purple" => "rgba(139, 92, 246, 0.18)",
            "orange" => "rgba(249, 115, 22, 0.18)",
            _ => "rgba(65, 105, 225, 0.18)"
        };
        string accentDark = AccentColor switch
        {
            "pink" => "#db2777",
            "blue" => "#2563eb",
            "green" => "#059669",
            "purple" => "#7c3aed",
            "orange" => "#ea580c",
            _ => "#2563eb"
        };

        return $"--bg:{bg}; --panel:{panel}; --panel-soft:{panelSoft}; --input:{input}; --text:{text}; --muted:{muted}; --line:{line}; --shadow:{shadow}; --accent:{accentHex}; --accent-soft:{accentSoft}; --accent-dark:{accentDark}; --accent-strong:{accentDark}; background-color:{bg}; color:{text}; min-height:100vh;";
    }

    private void NotifyStateChanged() => OnStateChanged?.Invoke();
}
