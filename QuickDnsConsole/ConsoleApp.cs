using System.Text;
using System.Text.Json;
using QD.Core.Cotracts;
using Microsoft.Extensions.Hosting;
using QD.Core.Cotracts.DnsObjContracts;
using Spectre.Console;

namespace QuickDNSConsole;

public class ConsoleApp
{
    private readonly IDnsObjApplication _dnsApp;
    private readonly CancellationToken _ct;
    private readonly Stack<string> _history = new();
    private ConsoleAppSettings _settings = new ConsoleAppSettings();
    private int _selectedIndex = 0;
    private AppState _state = AppState.Home;
    private Layout layout;
    private LiveDisplayContext _liveDisplayContext;

    public ConsoleApp(IDnsObjApplication dnsApp, IHostApplicationLifetime lifetime)
    {
        ReloadSettings();
        _dnsApp = dnsApp ?? throw new ArgumentNullException(nameof(dnsApp));
        _ct = lifetime.ApplicationStopping;
        Run();
    }

    private void AddHistory(string text)
    {
        if (_history.Count > _settings.HistoryLimit)
            _history.Pop();
        
        _history.Push(text);
    }

    private void ReloadSettings()
    {
        var setting = new ConsoleAppSettings();
        if (File.Exists("settings.json"))
        {
            var settings = JsonSerializer.Deserialize<ConsoleAppSettings>(
                File.ReadAllText("settings.json")
            );
             _settings = settings ?? new ConsoleAppSettings();
            return;
        }

        _settings = setting;
        var jsonStr = JsonSerializer.Serialize(setting);
        File.WriteAllText("settings.json", JsonSerializer.Serialize(setting));
    }

    // Call this from Program.Main synchronously (no background Task.Run)
    public void Run()
    {
        layout = new Layout("Root")
            .SplitRows(
                new Layout("Content").Ratio(10),
                new Layout("Footer").Ratio(1)
            );

        layout["Content"].SplitColumns(
            new Layout("Left").Ratio(2),
            new Layout("Right").Ratio(1)
        );

        // Start Live only once
        AnsiConsole.Live(layout)
            .Start(ctx =>
            {
                _liveDisplayContext = ctx;

                while (!_ct.IsCancellationRequested && _state != AppState.Exit)
                {
                    var objs = SafeGetAll();
                    var current = SafeGetCurrentDns()?.DnsAddresses ?? "(none)";

                    if (objs.Count == 0)
                        _selectedIndex = 0;
                    else
                        _selectedIndex = Math.Clamp(_selectedIndex, 0, objs.Count - 1);

                    // Update layout slots
                    var accentStyle = new Style(_settings.AccentColor);
                    layout["Left"].Update(BuildDnsTable(ref objs, accentStyle));
                    layout["Right"].Update(BuildInfoTable(current, accentStyle));
                    layout["Footer"].Update(BuildInstructionPanel());

                    ctx.Refresh(); // draw new state

                    var key = Console.ReadKey(true);
                    HandleKey(key, objs, current); // <-- factor key logic here
                }
            });
    }
    private void HandleKey(ConsoleKeyInfo key, IList<DnsObjViewModel> objs, string current)
    {
        switch (key.Key)
        {
            case ConsoleKey.UpArrow:
                if (objs.Count > 0) _selectedIndex = Math.Max(0, _selectedIndex - 1);
                break;
            case ConsoleKey.DownArrow:
                if (objs.Count > 0) _selectedIndex = Math.Min(objs.Count - 1, _selectedIndex + 1);
                break;
            case ConsoleKey.Enter:
                if (objs.Count > 0)
                    ApplySelection(objs[_selectedIndex], layout, _liveDisplayContext);
                break;
            case ConsoleKey.C:
                _state = AppState.Create;
                CreateFlow();
                _state = AppState.Home;
                break;
            case ConsoleKey.M:
                if (objs.Count > 0)
                {
                    _state = AppState.Edit;
                    EditFlow(objs[_selectedIndex]);
                    _state = AppState.Home;
                }
                break;
            case ConsoleKey.D:
                if (objs.Count > 0)
                {
                    _state = AppState.Delete;
                    DeleteFlow(objs[_selectedIndex]);
                    _state = AppState.Home;
                }
                break;
            case ConsoleKey.U:
                UnsetDns();
                break;
            case ConsoleKey.E:
                _state = AppState.Exit;
                break;
        }
    }



    #region Renderers & helpers

    private void RenderDashboardAsync(IList<DnsObjViewModel> objs, string currentDns)
        {
            if (_settings.UseSimpleDashboard)
            {
                RenderSimpleDashboard(objs, currentDns);
                return;
            }
            
            var accentStyle = new Style(_settings.AccentColor);

            // Root split into main content + footer (instructions)
            layout = new Layout("Root")
                .SplitRows(
                    new Layout("Content").Ratio(10),
                    new Layout("Footer").Ratio(1)
                );

            // --- Content is split left/right ---
            layout["Content"].SplitColumns(
                new Layout("Left").Ratio(2),
                new Layout("Right").Ratio(1)
            );
            AnsiConsole.Live(layout)
                .Start(ctx =>
                {
                    while (!_ct.IsCancellationRequested)
                    {
                        _liveDisplayContext = ctx;  
                        layout["Left"].Update(BuildDnsTable(ref objs,accentStyle));
                        layout["Right"].Update(BuildInfoTable(currentDns,accentStyle));
                        ctx.Refresh();
                        layout["Footer"].Update(BuildInstructionPanel());
                    }
                });
    }
    
        // --- Build DNS table ---
    Table BuildDnsTable(ref IList<DnsObjViewModel> objs,Style accentStyle)
    {
        var dnsTable = new Table()
            .Border(TableBorder.HeavyEdge)
            .Expand()
            .Title(new TableTitle($"[bold]Available DNS[/]", accentStyle));

        dnsTable.AddColumn("[u]Id[/]");
        dnsTable.AddColumn("[u]Name[/]");
        dnsTable.AddColumn("[u]Address[/]");

        if (objs.Count == 0)
        {
            dnsTable.AddRow("[grey](No DNS configured yet)[/]", string.Empty, string.Empty);
        }
        else
        {
            for (int i = 0; i < objs.Count; i++)
            {
                var o = objs[i];
                var prefix = i == _selectedIndex ? "-> " : "  ";

                if (i == _selectedIndex)
                {
                    var style = new Style(_settings.HighlightForeground, _settings.HighlightBackground);
                    dnsTable.AddRow(
                        new Markup($"{prefix}{o.Id}", style),
                        new Markup($"{o.Name ?? "(no name)"}", style),
                        new Markup($"{o.DnsAddresses ?? "(no address)"}", style)
                    );
                }
                else
                {
                    dnsTable.AddRow(
                        $"{prefix}{o.Id}",
                        $"{o.Name ?? "(no name)"}",
                        $"{o.DnsAddresses ?? "(no address)"}"
                    );
                }
            }
        }

        return dnsTable;
    }
    

    // --- Build info table ---
    Table BuildInfoTable(string currentDns, Style accentStyle)
    {
        var infoTable = new Table()
            .Border(TableBorder.Minimal)
            .Expand()
            .Title(new TableTitle($"[bold]Current & History[/]", accentStyle));

        infoTable.AddColumn("Info");
        infoTable.AddRow($"[green]Current:[/] {currentDns}");
        infoTable.AddRow($"[grey]History (last {_settings.HistoryLimit}):[/]");

        foreach (var h in _history.TakeLast(_settings.HistoryLimit))
        {
            infoTable.AddRow($" - {h}");
        }

        return infoTable;
    }

    Panel BuildInstructionPanel()
    {
        return new Panel(
            new Markup("[grey]Ready.[/] \n \n" +
                "[grey]Use ↑Up/↓Down Arrow Keys to move selection. Enter = apply selected DNS.\n" +
                "Commands: (C)reate | (M)odify | (D)elete | (U)nset | (E)xit\n" +
                "Press [underline]ESC[/] to cancel while you are in a Create/Edit/Delete page.[/]"
            )
        )
        {
            Border = BoxBorder.None,
            Expand = true
        };
    }

// ====================
// Simple Dashboard Mode
// ====================
private void RenderSimpleDashboard(IList<DnsObjViewModel> objs, string currentDns)
{
    Console.Clear();

    var prompt = new SelectionPrompt<string>()
        .Title($"[bold]{_settings.AccentColor}Select DNS to apply:[/]")
        .PageSize(10)
        .MoreChoicesText("[grey](Use arrow keys to navigate)[/]");

    if (objs.Count == 0)
    {
        prompt.AddChoice("(No DNS configured yet)");
    }
    else
    {
        prompt.AddChoices(objs.Select(o => $"{o.Id}: {o.Name ?? "(no name)"} - {o.DnsAddresses}"));
    }

    var choice = AnsiConsole.Prompt(prompt);
    AnsiConsole.MarkupLine($"[green]Current DNS:[/] {currentDns}");
}


    // safe wrappers in case your IDnsObjApplication is async — adjust if needed
    private IList<DnsObjViewModel> SafeGetAll()
    {
        try
        {
            // If GetAll is synchronous:
            var res = _dnsApp.GetAll();
            if (res != null) return res;
        }
        catch
        {
            // If GetAll is async Task<List<T>> and you're calling sync, uncomment below:
            // return _dnsApp.GetAll().GetAwaiter().GetResult();
        }
        return new List<DnsObjViewModel>();
    }

    private DnsObjViewModel SafeGetCurrentDns()
    {
        try
        {
            var d = _dnsApp.GetCurrentDns();
            if (d != null) return d;
        }
        catch
        {
            // if async, consider GetAwaiter().GetResult()
        }
        return new DnsObjViewModel { DnsAddresses = string.Empty, Name = string.Empty };
    }

    #endregion

    #region Actions / flows

    private void ApplySelection(DnsObjViewModel selected, Layout layout, LiveDisplayContext ctx)
    {
        Console.CursorVisible = false;
        try
        {
            var result = _dnsApp.SetDns(selected.Id);
            if (result.IsSucceeded)
            {
                AddHistory($"{selected.Name} ({selected.DnsAddresses})");
                layout["Footer"].Update(
                    new Panel($"[green]DNS applied: {selected.Name}[/]\n[grey]Press any key to continue...[/]")
                    {
                        Border = BoxBorder.None,
                        Expand = true
                    });
            }
            else
            {
                layout["Footer"].Update(
                    new Panel($"[red]Failed: {result.Message}[/]")
                    {
                        Border = BoxBorder.None,
                        Expand = true
                    });
            }
        }
        catch (Exception ex)
        {
            layout["Footer"].Update(
                new Panel($"[red]Error: {ex.Message}[/]")
                {
                    Border = BoxBorder.None,
                    Expand = true
                });
        }

        ctx.Refresh(); // force redraw
        Console.ReadKey(true);
        Console.CursorVisible = true;

        // restore instructions after keypress
        layout["Footer"].Update(BuildInstructionPanel());
        ctx.Refresh();
    }


    private void UnsetDns()
    {
        try
        {
            _dnsApp.UnSetDns();
            AddHistory("Unset DNS");
            AnsiConsole.MarkupLine("[green]DNS unset.[/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error unsetting DNS: {ex.Message}[/]");
        }
        AnsiConsole.MarkupLine("[grey]Press any key to continue...[/]");
        Console.ReadKey(true);
    }

    private void CreateFlow()
    {
        Console.Clear();
        AnsiConsole.MarkupLine("[yellow]-- Create DNS (ESC to cancel) --[/]");

        var title = ReadLineWithEsc("Dns Title: ");
        if (title == null) { Cancelled(); return; }

        var first = ReadLineWithEsc("First DNS: ");
        if (first == null) { Cancelled(); return; }

        var second = ReadLineWithEsc("Second DNS (optional): ");
        if (second == null) { Cancelled(); return; }

        try
        {
            var cmd = new CreateDnsObj(title,first,second);
            var r = _dnsApp.Create(cmd);
            if (r.IsSucceeded)
            {
                AddHistory($"{title} ({(string.IsNullOrEmpty(cmd.FirstDns) ? "" : (", " + cmd.FirstDns))} - {(string.IsNullOrEmpty(cmd.SecondDns) ? "" : (", " + cmd.SecondDns))})");
                AnsiConsole.MarkupLine("[green]Created successfully.[/]");
            }
            else
            {
                AnsiConsole.MarkupLine($"[red]Create failed: {r.Message}[/]");
            }
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
        }

        AnsiConsole.MarkupLine("[grey]Press any key to return to Home...[/]");
        Console.ReadKey(true);
    }

    private void EditFlow(DnsObjViewModel selected)
    {
        Console.Clear();
        AnsiConsole.MarkupLine($"[yellow]-- Edit DNS #{selected.Id} (ESC to cancel) --[/]");
        AnsiConsole.MarkupLine($"[grey]Current Title: {selected.Name}[/]");
        var newTitle = ReadLineWithEsc("New Title (leave empty to keep current): ");
        if (newTitle == null) { Cancelled(); return; }
        if (string.IsNullOrWhiteSpace(newTitle)) newTitle = selected.Name;

        var parts = (selected.DnsAddresses ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries);
        var cur1 = parts.Length > 0 ? parts[0] : "";
        var cur2 = parts.Length > 1 ? parts[1] : "";

        AnsiConsole.MarkupLine($"[grey]Current First DNS: {cur1}[/]");
        var first = ReadLineWithEsc("First DNS (leave empty to keep current): ");
        if (first == null) { Cancelled(); return; }
        if (string.IsNullOrWhiteSpace(first)) first = cur1;

        AnsiConsole.MarkupLine($"[grey]Current Second DNS: {cur2}[/]");
        var second = ReadLineWithEsc("Second DNS (leave empty to keep current): ");
        if (second == null) { Cancelled(); return; }
        if (string.IsNullOrWhiteSpace(second)) second = cur2;

        // apply edit
        try
        {
            selected.Name = newTitle;
            selected.DnsAddresses = first + (string.IsNullOrEmpty(second) ? "" : ("," + second));
            var r = _dnsApp.Edit(selected.MapEditDnsObj());
            if (r.IsSucceeded)
            {
                AddHistory($"Edited: {selected.Name} ({selected.DnsAddresses})");
                AnsiConsole.MarkupLine("[green]Edited successfully.[/]");
            }
            else
            {
                AnsiConsole.MarkupLine($"[red]Edit failed: {r.Message}[/]");
            }
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
        }

        AnsiConsole.MarkupLine("[grey]Press any key to return to Home...[/]");
        Console.ReadKey(true);
    }

    private void DeleteFlow(DnsObjViewModel selected)
    {
        Console.Clear();
        AnsiConsole.MarkupLine($"[yellow]-- Delete DNS #{selected.Id} (ESC to cancel) --[/]");
        AnsiConsole.MarkupLine($"Are you sure you want to delete: [red]{selected.Name}[/] ? (Y=confirm / ESC=cancel)");

        while (true)
        {
            var k = Console.ReadKey(true);
            if (k.Key == ConsoleKey.Escape)
            {
                Cancelled();
                return;
            }

            if (k.Key == ConsoleKey.Y)
            {
                try
                {
                    _dnsApp.Delete(selected.Id);
                    AddHistory($"Deleted: {selected.Name}");
                    AnsiConsole.MarkupLine("[green]Deleted.[/]");
                }
                catch (Exception ex)
                {
                    AnsiConsole.MarkupLine($"[red]Delete error: {ex.Message}[/]");
                }

                AnsiConsole.MarkupLine("[grey]Press any key to return to Home...[/]");
                Console.ReadKey(true);
                return;
            }
        }
    }

    #endregion

    #region Utilities (ReadLine with ESC support)

    /// <summary>
    /// Read a line from console but return null if ESC is pressed (cancel).
    /// Handles backspace editing and immediate echoing.
    /// </summary>
    private string ReadLineWithEsc(string prompt)
    {
        Console.CursorVisible = true;
        Console.Write(prompt);

        var sb = new StringBuilder();
        while (true)
        {
            var keyInfo = Console.ReadKey(intercept: true);

            if (keyInfo.Key == ConsoleKey.Escape)
            {
                Console.WriteLine();
                Console.CursorVisible = false;
                return null; // canceled
            }

            if (keyInfo.Key == ConsoleKey.Enter)
            {
                Console.WriteLine();
                Console.CursorVisible = false;
                return sb.ToString();
            }

            if (keyInfo.Key == ConsoleKey.Backspace)
            {
                if (sb.Length > 0)
                {
                    // remove last char from buffer and console
                    sb.Length--;
                    Console.Write("\b \b");
                }
                continue;
            }

            // ignore control keys
            if (char.IsControl(keyInfo.KeyChar))
                continue;

            // normal char
            sb.Append(keyInfo.KeyChar);
            Console.Write(keyInfo.KeyChar);
        }
    }

    private void Cancelled()
    {
        AnsiConsole.MarkupLine("[grey]Cancelled. Returning to Home...[/]");
        Thread.Sleep(250);
    }

    #endregion
}
