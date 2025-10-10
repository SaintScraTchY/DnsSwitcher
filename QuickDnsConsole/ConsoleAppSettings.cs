using Spectre.Console;

namespace QuickDNSConsole;

public class ConsoleAppSettings
{
    public bool UseSimpleDashboard { get; set; } = false;
    public RgbColor AccentColor { get; set; } = new(255, 255, 0);
    public RgbColor HighlightBackground { get; set; } = new(0, 95, 0);
    public RgbColor HighlightForeground { get; set; } = new(255, 255, 255);
    public int HistoryLimit { get; set; } = 10;
}

public record struct RgbColor(byte R, byte G, byte B);
