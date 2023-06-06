using System;
using System.Collections.Generic;
using DC.Application.Contracts.DnsObjContracts;

namespace DnsChangerConsole;

public class ConsoleApp
{

    private readonly IDnsObjApplication _dnsObjApplication;
    private List<DnsObjViewModel>? _objs;

    public ConsoleApp(IDnsObjApplication dnsObjApplication)
    {
        _dnsObjApplication = dnsObjApplication;
    }

    public void StartApp()
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.Clear();
        Console.WriteLine("[C]reate");
        Console.WriteLine("[U]nset Dns");
        Console.WriteLine("[M]odify");
        Console.WriteLine("[D]elete");
        Console.WriteLine("[E]xit");
    }

    public void GoHome()
    {
        StartApp();
        MainPage();
    }

    public void LoopDns(ConsoleColor color)
    {
        Console.WriteLine("-----------------------------------------");
        Console.ForegroundColor = color;
        _objs = _dnsObjApplication.GetAll();
        if (_objs.Count > 0)
        {
            int i = 1;
            foreach (DnsObjViewModel obj in _objs)
            {
                Console.WriteLine($"[{i}] - {obj.Name}",obj.Id);
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
        _dnsObjApplication.SetDns(id);
    }

    public void UnsetDns()
    {
        _dnsObjApplication.UnSetDns();
    }

    public int WhichRecordPressed(string pressedkey)
    {
        int Row;
        if (int.TryParse(pressedkey,out Row) && !pressedkey.StartsWith("0"))
        {
            return _objs[(Row-1)].Id;
        }
        return -1;
    }
    public void MainPage()
    {
        LoopDns(ConsoleColor.Green);
        while (true)
        {
            string PressedKey = Console.ReadLine();
            int Id = WhichRecordPressed(PressedKey);
            if (Id != -1)
            {
                SetThisDns(Id);
            }
            else
            {
                switch (PressedKey)
                {
                    case "C" or "c":
                        CreateDns();
                        break;
                    case "U" or "u":
                        _dnsObjApplication.UnSetDns();
                        break;
                    case "M" or "m":
                        ModifyDns();
                        break;
                    case "D" or "d":
                        DeleteDns();
                        break;
                    case "E" or "e":
                        Environment.Exit(0);
                        break;
                }
            }
        }
    }

    public void BackButton(string PressedKey)
    {
        if (PressedKey=="B" || PressedKey == "b")
            GoHome();
    }
    public void CreateDns()
    {
        Console.Clear();
        Console.WriteLine("- Delete Page -");
        Console.WriteLine("[B]ack");
        Console.WriteLine("-----------------------------------------");
        CreateDnsObj command = new CreateDnsObj();
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
        _dnsObjApplication.Create(command);
        GoHome();;
    }

    public void ModifyDns()
    {
        Console.Clear(); 
        Console.WriteLine("- Modify Page -");
        Console.WriteLine("[B]ack");
        
        LoopDns(ConsoleColor.Cyan);
        Console.ForegroundColor = ConsoleColor.White;
        while (true)
        {
            string PressedKey = Console.ReadLine();
            BackButton(PressedKey);
            int row;
            int Id = WhichRecordPressed(PressedKey);
            if (Id != -1)
            {
                EditDnsObj editDnsObj = _dnsObjApplication.GetDetail(Id);
                Console.WriteLine($" Enter new Value for Title [Previous value:{editDnsObj.Name}] ");
                editDnsObj.Name = Console.ReadLine();
                string[] DnsTemp = editDnsObj.DnsAddresses.Split(',');
                Console.WriteLine($" Enter new value for First Dns [Previous value:{DnsTemp[0]}] ");
                editDnsObj.DnsAddresses = Console.ReadLine() + ",";
                Console.WriteLine($" Enter new value for First Dns [Previous value:{DnsTemp[1]}] ");
                editDnsObj.DnsAddresses += Console.ReadLine();
                Console.WriteLine("-----------------------------------------");
                Console.WriteLine("Y/N - Y=Save | N=Cancel");
                PressedKey = Console.ReadLine();
                if (PressedKey=="y" || PressedKey == "Y")
                {
                    _dnsObjApplication.Edit(editDnsObj);
                    ModifyDns();
                }
                else if (PressedKey=="n" || PressedKey == "N")
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
        Console.WriteLine("[B]ack");
        
        LoopDns(ConsoleColor.Yellow);
        Console.ForegroundColor = ConsoleColor.White;
        while (true)
        {
            string PressedKey = Console.ReadLine();
            BackButton(PressedKey);
            int row;
            int Id = WhichRecordPressed(PressedKey);
            if (Id != -1)
            {
                
                Console.WriteLine("-----------------------------------------");
                Console.WriteLine($"Do you want to Delete Dns #{Id} ? Y/N - Y=Delete | N=Cancel");
                PressedKey = Console.ReadLine();
                if (PressedKey=="y" || PressedKey == "Y")
                {
                    _dnsObjApplication.Delete(Id);
                    DeleteDns();
                }
                else if (PressedKey=="n" || PressedKey == "N")
                {
                    DeleteDns();
                }
            }
        }
    }
}