using DC.Core.Cotracts.DnsObjContracts;
using Microsoft.Extensions.Hosting;
using Spectre.Console;

namespace DnsChangerConsole;

public class ConsoleApp
{
    private readonly IDnsObjApplication _dnsApp;
    private readonly CancellationToken _ct;

    public ConsoleApp(IDnsObjApplication dnsApp, IHostApplicationLifetime lifetime)
    {
        _dnsApp = dnsApp;
        _ct = lifetime.ApplicationStopping;
        // start automatically
        Task.Run(() => RunAsync(_ct));
    }

    public void RunAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            var objs = _dnsApp.GetAll() ?? new List<DnsObjViewModel>();
            var currentDns = _dnsApp.GetCurrentDns();

            AnsiConsole.MarkupLine($"[green]Current DNS:[/] {currentDns.DnsAddresses ?? "(none)"}");
            AnsiConsole.WriteLine();

            if (objs.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]No DNS records found.[/]");
            }
            else
            {
                var prompt = new SelectionPrompt<string>()
                    .Title("Select DNS to apply (or type command):")
                    .PageSize(10)
                    .MoreChoicesText("[grey](Move up and down to reveal more)[/]")
                    .AddChoices(objs.Select(o => $"{o.Id}: {o.Name}"));

                var choice = AnsiConsole.Prompt(prompt);

                // parse id
                var idStr = choice.Split(':')[0];
                if (int.TryParse(idStr, out var id))
                {
                    var result = _dnsApp.SetDns(id);
                    if (!result.IsSucceeded)
                        AnsiConsole.MarkupLine($"[red]{result.Message}[/]");
                }
            }

            AnsiConsole.WriteLine();
            var cmd = AnsiConsole.Prompt(new TextPrompt<string>("Type [grey](C)reate/(U)nset/(M)odify/(D)elete/(E)xit[/]"));
            switch (cmd?.ToLowerInvariant())
            {
                case "c":
                case "create":
                    CreateFlowAsync();
                    break;
                case "u":
                case "unset":
                    _dnsApp.UnSetDns();
                    break;
                case "e":
                case "exit":
                    return; // exit loop -> host will stop
                // handle other commands similarly...
                default:
                    AnsiConsole.MarkupLine("[red]Invalid command[/]");
                    break;
            }
        }
    }

    private void CreateFlowAsync()
    {
        var title = AnsiConsole.Ask<string>("Dns Title:");
        var first = AnsiConsole.Ask<string>("First DNS:");
        var second = AnsiConsole.Ask<string>("Second DNS:");
        var cmd = new CreateDnsObj(first, second, title);
        var r =  _dnsApp.Create(cmd);
        if (!r.IsSucceeded) AnsiConsole.MarkupLine($"[red]{r.Message}[/]");
    }
}
