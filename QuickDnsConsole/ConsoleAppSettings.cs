using Spectre.Console;

namespace QuickDNSConsole;

public class ConsoleAppSettings
{
    public bool UseSimpleDashboard { get; set; } = false;   // true => use simple selection mode
    public Color AccentColor { get; set; } = Color.Yellow;  // default colors
    public Color HighlightBackground { get; set; } = Color.DarkGreen;
    public Color HighlightForeground { get; set; } = Color.White;
    public int HistoryLimit { get; set; } = 10;
}