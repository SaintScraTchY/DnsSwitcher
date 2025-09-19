using System.Text;
using DC.Core.Cotracts;
using DC.Core.Cotracts.DnsObjContracts;
using Microsoft.Extensions.Hosting;
using Spectre.Console;

namespace DnsChangerConsole
{
    public enum AppState
    {
        Home,
        Create,
        Edit,
        Delete,
        Exit
    }

    public class ConsoleApp
    {
        private readonly IDnsObjApplication _dnsApp;
        private readonly CancellationToken _ct;
        private readonly List<string> _history = new();
        private int _selectedIndex = 0;
        private AppState _state = AppState.Home;

        public ConsoleApp(IDnsObjApplication dnsApp, IHostApplicationLifetime lifetime)
        {
            _dnsApp = dnsApp ?? throw new ArgumentNullException(nameof(dnsApp));
            _ct = lifetime.ApplicationStopping;
            Run();
        }

        // Call this from Program.Main synchronously (no background Task.Run)
        public void Run()
        {
            var rule = new Rule("[yellow]Welcome to Dns Changer :face_blowing_a_kiss:! [/]")
            {
                Justification = Justify.Center
            };
            Console.WriteLine(rule);
            // Main loop
            while (!_ct.IsCancellationRequested && _state != AppState.Exit)
            {
                // Grab fresh data each loop (synchronous call assumed)
                var objs = SafeGetAll();
                var current = SafeGetCurrentDns()?.DnsAddresses ?? "(none)";

                // Clamp selectedIndex
                if (objs.Count == 0)
                    _selectedIndex = 0;
                else
                    _selectedIndex = Math.Clamp(_selectedIndex, 0, objs.Count - 1);

                // Render dashboard (table + instructions)
                RenderDashboard(objs, current);

                // Wait for a key and react immediately (no Enter required)
                var key = Console.ReadKey(true);

                // Home-key handling (arrow Up/Down, Enter for applying, short-keys)
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
                            ApplySelection(objs[_selectedIndex]);
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
                    // ESC: only meaningful inside flows — in Home we ignore it
                    case ConsoleKey.Escape:
                        // If you ever want ESC to exit from Home, change this behavior.
                        break;
                    default:
                        // ignore unknown keys
                        break;
                }
            }
        }

        #region Renderers & helpers

        private void RenderDashboard(IList<DnsObjViewModel> objs, string currentDns)
        {
            Console.Clear();

            // Root table with two columns
            var root = new Table().Expand().Border(TableBorder.Rounded);
            root.AddColumn("[yellow]Available DNS[/]");
            root.AddColumn("[cyan]Current & History[/]");

            // Left: DNS list (selectable)
            var dnsTable = new Table().Border(TableBorder.Minimal).Expand();
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
                        // highlight selected row
                        dnsTable.AddRow(
                            $"[black on darkgreen]{prefix}{o.Id}[/]",
                            $"[black on darkgreen]{o.Name ?? "(no name)"}[/]",
                            $"[black on darkgreen]{o.DnsAddresses ?? "(no address)"}[/]");
                    }
                    else
                    {
                        dnsTable.AddRow(
                            $"{prefix}{o.Id}",
                            $"{o.Name ?? "(no name)"}",
                            $"{o.DnsAddresses ?? "(no address)"}");
                    }
                }
            }

            // Right: current + history
            var infoTable = new Table().Border(TableBorder.Minimal).Expand();
            infoTable.AddColumn("Info");
            infoTable.AddRow($"[green]Current:[/] {currentDns}");
            infoTable.AddRow("[grey]History (last 10):[/]");
            foreach (var h in _history.TakeLast(10))
            {
                infoTable.AddRow($" - {h}");
            }

            root.AddRow(dnsTable, infoTable);

            AnsiConsole.Write(root);

            // Instructions and state
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[grey]Use Up/Down Arrow to move selection. Enter = apply selected DNS.[/]");
            AnsiConsole.MarkupLine("[grey]Commands (press key): (C)reate | (M)odify | (D)elete | (U)nset | (E)xit[/]");
            AnsiConsole.MarkupLine("[grey]Press [underline]ESC[/] to cancel while you are in a Create/Edit/Delete Page.[/]");

            //Debug Only
            //AnsiConsole.MarkupLine($"[grey]Current state: [bold]{_state}[/][/]");
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

        private void ApplySelection(DnsObjViewModel selected)
        {
            Console.CursorVisible = false;
            try
            {
                var result = _dnsApp.SetDns(selected.Id);
                if (result.IsSucceeded)
                {
                    _history.Add($"{selected.Name} ({selected.DnsAddresses})");
                    AnsiConsole.MarkupLine($"[green]DNS applied: {selected.Name}[/]");
                }
                else
                {
                    AnsiConsole.MarkupLine($"[red]Failed: {result.Message}[/]");
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
            }

            AnsiConsole.MarkupLine("[grey]Press any key to continue...[/]");
            Console.ReadKey(true);
            Console.CursorVisible = true;
        }

        private void UnsetDns()
        {
            try
            {
                _dnsApp.UnSetDns();
                _history.Add("Unset DNS");
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
                    _history.Add($"{title} ({(string.IsNullOrEmpty(cmd.FirstDns) ? "" : (", " + cmd.FirstDns))} - {(string.IsNullOrEmpty(cmd.SecondDns) ? "" : (", " + cmd.SecondDns))})");
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
                    _history.Add($"Edited: {selected.Name} ({selected.DnsAddresses})");
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
                        _history.Add($"Deleted: {selected.Name}");
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
}
