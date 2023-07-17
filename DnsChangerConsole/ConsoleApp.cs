using DC.Application.Contracts.DnsObjContracts;
using HelperClass.Application;

namespace DnsChangerConsole;

public class ConsoleApp
{
    private readonly IDnsObjApplication _dnsObjApplication;
    private List<DnsObjViewModel>? _objs;
    private string _currentDns;

    public ConsoleApp(IDnsObjApplication dnsObjApplication)
    {
        _dnsObjApplication = dnsObjApplication;
    }

    public void StartApp()
    {
        Console.WriteLine(ConsoleHelper.DashLine);
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("[C]reate");
        Console.WriteLine("[U]nset Dns");
        Console.WriteLine("[M]odify");
        Console.WriteLine("[D]elete");
        Console.WriteLine("[E]xit");
    }

    public void GoHome()
    {
        Console.Clear();
        Dash();
        UpdateCurrentDns();
        StartApp();
        MainPage();
    }

    public void Dash()
    {
        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.WriteLine(ConsoleHelper.DashLine);
        Console.ForegroundColor = ConsoleColor.White;
    }

    public void LoopDns(ConsoleColor color)
    {
        Console.WriteLine(ConsoleHelper.DashLine);
        Console.ForegroundColor = color;
        _objs = _dnsObjApplication.GetAll();
        if (_objs.Count > 0)
        {
            var i = 1;
            foreach (var obj in _objs)
            {
                Console.WriteLine($"[{i}] - {obj.Name}", obj.Id);
                i++;
            }
        }
        else
        {
            Console.WriteLine("No Records were Found try Creating some");
        }
    }

    public void SetThisDns(int id)
    {
        var operationResult = _dnsObjApplication.SetDns(id);
        if (!operationResult.IsSucceeded)
            Console.WriteLine(operationResult.Message);
        GoHome();
    }

    public void UpdateCurrentDns()
    {
        var CurrentDns = _dnsObjApplication.GetCurrentDns();
        _currentDns = CurrentDns.DnsAddresses;
        string CurrentDnsText;
        if (!string.IsNullOrWhiteSpace(CurrentDns.DnsAddresses))
        {
            CurrentDnsText = CurrentDns.DnsAddresses;
            if (!string.IsNullOrWhiteSpace(CurrentDns.Name))
            {
                CurrentDnsText += " | " + CurrentDns.Name;
                
            }
            else
            {
                CurrentDnsText += " | " + "This Dns Does Exists in Database , You can Add it by Pressing 'N'";
            }
        }
        else
        {
            CurrentDnsText = "No Dns Have Been Set";
        }

        Console.WriteLine(CurrentDnsText);
    }

    public void UnsetDns()
    {
        _dnsObjApplication.UnSetDns();
        GoHome();
    }

    public bool ValidateDnsIndex(int id)
    {
        if (id <= _objs.Count)
            return true;
        return false;
    }

    public int WhichRecordPressed(string pressedkey)
    {
        int Row;
        int.TryParse(pressedkey, out Row);
        if (ValidateDnsIndex(Row) && Row > 0)
            if (!pressedkey.StartsWith("0"))
                return _objs[Row - 1].Id;

        return -1;
    }

    public void MainPage()
    {
        LoopDns(ConsoleColor.Green);
        while (true)
        {
            var PressedKey = Console.ReadLine();
            var Id = WhichRecordPressed(PressedKey);
            if (Id != -1)
                SetThisDns(Id);
            else
                switch (PressedKey)
                {
                    case "C" or "c":
                        CreateDns();
                        break;
                    case "U" or "u":
                        UnsetDns();
                        break;
                    case "M" or "m":
                        ModifyDns();
                        break;
                    case "D" or "d":
                        DeleteDns();
                        break;
                    case "N" or "n":
                        CreateDns(_currentDns);
                        break;
                    case "E" or "e":
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine(ConsoleHelper.InvalidInput);
                        break;
                }
        }
    }

    public void BackButton(string PressedKey)
    {
        if (PressedKey == "B" || PressedKey == "b")
            GoHome();
    }

    public void CreateDns()
    {
        Console.Clear();
        Console.WriteLine("- Create New DNS -");
        Console.WriteLine(ConsoleHelper.BackButton);
        Console.WriteLine(ConsoleHelper.DashLine);
        var command = new CreateDnsObj();
        string input;
        Console.WriteLine("Dns Title : ");
        input = Console.ReadLine();
        BackButton(input);
        command.Name = input;
        Console.WriteLine("First Dns : ");
        input = Console.ReadLine();
        BackButton(input);
        command.DnsAddresses = input + ",";
        Console.WriteLine("Second Dns : ");
        input = Console.ReadLine();
        BackButton(input);
        command.DnsAddresses += input;
        OperationResult operationResult =_dnsObjApplication.Create(command);
        GoHome();
        if(operationResult.IsSucceeded==false)
            Console.WriteLine(operationResult.Message);
    }

    public void CreateDns(string newdnsAddress)
    {
        Console.Clear();
        Console.WriteLine("- Create New DNS -");
        Console.WriteLine(ConsoleHelper.BackButton);
        Console.WriteLine(ConsoleHelper.DashLine);
        var command = new CreateDnsObj();
        string input;
        Console.WriteLine("Dns Title : ");
        input = Console.ReadLine();
        BackButton(input);
        command.Name = input;
        command.DnsAddresses = newdnsAddress;
        OperationResult operationResult =_dnsObjApplication.Create(command);
        GoHome();
        if(operationResult.IsSucceeded==false)
            Console.WriteLine(operationResult.Message);
    }

    public void ModifyDns()
    {
        Console.Clear();
        Console.WriteLine("- Modify Page -");
        Console.WriteLine(ConsoleHelper.BackButton);

        LoopDns(ConsoleColor.Cyan);
        Console.ForegroundColor = ConsoleColor.White;
        while (true)
        {
            var PressedKey = Console.ReadLine();
            BackButton(PressedKey);
            int row;
            var Id = WhichRecordPressed(PressedKey);
            if (Id != -1)
            {
                string InputValue;
                var editDnsObj = _dnsObjApplication.GetDetail(Id);
                while (true)
                {
                    ConsoleHelper.ModifyTextFor("Title", editDnsObj.Name);
                    InputValue = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(InputValue))
                    {
                        ConsoleHelper.DisplayErrorFor(ConsoleHelper.NullInput, "Title");
                    }
                    else
                    {
                        editDnsObj.Name = InputValue;
                        break;
                    }
                }

                var DnsTemp = editDnsObj.DnsAddresses.Split(',');
                ConsoleHelper.ModifyTextFor("First DNS", DnsTemp[0]);
                InputValue = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(InputValue))
                    editDnsObj.DnsAddresses = DnsTemp[0] + ",";
                else
                    editDnsObj.DnsAddresses = InputValue + ",";

                ConsoleHelper.ModifyTextFor("Second DNS", DnsTemp[0]);
                InputValue = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(InputValue))
                    editDnsObj.DnsAddresses += DnsTemp[1];
                else
                    editDnsObj.DnsAddresses += InputValue;

                Console.WriteLine(ConsoleHelper.DashLine);
                if (ConsoleHelper.CheckConsentFor("Modify", editDnsObj.Id))
                {
                    OperationResult operationResult = _dnsObjApplication.Edit(editDnsObj);
                    if (operationResult.IsSucceeded == false)
                    {
                        ModifyDns();
                        Console.WriteLine(operationResult.Message);
                    }
                    else
                    {
                        ModifyDns();
                    }
                        
                }
                else
                {
                    ModifyDns();
                }
                
            }
        }
    }

    public void DeleteDns()
    {
        Console.Clear();
        Console.WriteLine("- Delete Page -");
        Console.WriteLine(ConsoleHelper.BackButton);

        LoopDns(ConsoleColor.Yellow);
        Console.ForegroundColor = ConsoleColor.White;
        while (true)
        {
            var PressedKey = Console.ReadLine();
            BackButton(PressedKey);
            int row;
            var Id = WhichRecordPressed(PressedKey);
            if (Id != -1)
            {
                Console.WriteLine(ConsoleHelper.DashLine);
                if (ConsoleHelper.CheckConsentFor("Delete", Id)) _dnsObjApplication.Delete(Id);

                DeleteDns();
            }
        }
    }
}