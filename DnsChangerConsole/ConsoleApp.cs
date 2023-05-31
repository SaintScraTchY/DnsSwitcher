using DC.Application.Contracts.DnsObjContracts;

namespace DnsChangerConsole;

public class ConsoleApp
{

    private readonly IDnsObjApplication _dnsObjApplication;
    public ConsoleApp(IDnsObjApplication dnsObjApplication)
    {
        _dnsObjApplication = dnsObjApplication;
        StartApp();
    }

    public void StartApp()
    {
        var s =_dnsObjApplication.UnSetDns();
        if(s.IsSucceeded=false)
            Console.WriteLine(s.Message);   
    }
}