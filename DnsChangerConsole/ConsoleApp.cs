﻿using DC.Application.Contracts.DnsObjContracts;
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

    private void TextWhite()
    {
        Console.ForegroundColor = ConsoleColor.White;
    }
    
    private void TextGreen()
    {
        Console.ForegroundColor = ConsoleColor.Green;
    }

    private void StartApp()
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
        UpdateCurrentDns().Wait(timeout: TimeSpan.FromSeconds(5));
        StartApp();
        MainPage();
    }

    private void Dash()
    {
        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.WriteLine(ConsoleHelper.DashLine);
        Console.ForegroundColor = ConsoleColor.White;
    }

    private async void LoopDns(ConsoleColor color)
    {
        Console.WriteLine(ConsoleHelper.DashLine);
        Console.ForegroundColor = color;
        _objs = await _dnsObjApplication.GetAll();
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
            Console.WriteLine(ConsoleHelper.NoRecordsFound);
        }
    }

    private async void SetThisDns(int id)
    {
        var operationResult = await _dnsObjApplication.SetDns(id);
        if (!operationResult.IsSucceeded)
            Console.WriteLine(operationResult.Message);
        GoHome();
    }

    private async Task UpdateCurrentDns()
    {
        var currentDns = await _dnsObjApplication.GetCurrentDns();
        _currentDns = ConsoleHelper.DisplayCurrentDns(currentDns);
    }

    private void UnsetDns()
    {
        _dnsObjApplication.UnSetDns();
        GoHome();
    }

    private bool ValidateDnsIndex(int id)
    {
        if (id <= _objs.Count)
            return true;
        return false;
    }

    private int WhichRecordPressed(string pressedKey)
    {
        int row;
        int.TryParse(pressedKey, out row);
        if (ValidateDnsIndex(row) && row > 0)
            if (!pressedKey.StartsWith("0"))
                return _objs[row - 1].Id;

        return -1;
    }

    private void MainPage()
    {
        LoopDns(ConsoleColor.Green);
        while (true)
        {
            var pressedKey = Console.ReadLine();
            var Id = WhichRecordPressed(pressedKey);
            if (Id != -1)
                SetThisDns(Id);
            else
                switch (pressedKey)
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

    private void BackButton(string pressedKey)
    {
        if (pressedKey == "B" || pressedKey == "b")
            GoHome();
    }

    private async void CreateThisDns(CreateDnsObj command)
    {
        OperationResult operationResult = await _dnsObjApplication.Create(command);
        GoHome();
        if(operationResult.IsSucceeded==false)
            Console.WriteLine(operationResult.Message);
    }

    private void CreateDns()
    {
        Console.Clear();
        Console.WriteLine("- Create New DNS -");
        Console.WriteLine(ConsoleHelper.BackButton);
        Console.WriteLine(ConsoleHelper.DashLine);
        var command = new CreateDnsObj();
        string input;
        Console.WriteLine("Dns Title : ");
        TextWhite();
        input = Console.ReadLine();
        TextGreen();
         BackButton(input);
        command.Name = input;
        Console.WriteLine("First Dns : ");
        TextWhite();
        input = Console.ReadLine();
        TextGreen();
         BackButton(input);
        command.DnsAddresses = input + ",";
        Console.WriteLine("Second Dns : ");
        TextWhite();
        input = Console.ReadLine();
        TextGreen();
         BackButton(input);
        command.DnsAddresses += input;
        CreateThisDns(command);
    }

    private async void CreateDns(string newdnsAddress)
    {
        Console.Clear();
        Console.WriteLine("- Create New DNS -");
        Console.WriteLine(ConsoleHelper.BackButton);
        Console.WriteLine(ConsoleHelper.DashLine);
        var command = new CreateDnsObj();
        string input;
        Console.WriteLine("Dns Title : ");
        TextWhite();
        input = Console.ReadLine();
        TextGreen();
         BackButton(input);
        command.Name = input;
        command.DnsAddresses = newdnsAddress;
        OperationResult operationResult = await _dnsObjApplication.Create(command);
        CreateThisDns(command); 
    }

    private async void ModifyDns()
    {
        Console.Clear();
        Console.WriteLine("- Modify Page -");
        Console.WriteLine(ConsoleHelper.BackButton);

        LoopDns(ConsoleColor.Cyan);
        Console.ForegroundColor = ConsoleColor.White;
        while (true)
        {
            var pressedKey = Console.ReadLine();
             BackButton(pressedKey);
            int row;
            var id = WhichRecordPressed(pressedKey);
            if (id != -1)
            {
                string inputValue;
                var editDnsObj = await _dnsObjApplication.GetDetail(id);
                while (true)
                {
                    ConsoleHelper.ModifyTextFor("Title", editDnsObj.Name);
                    TextWhite();
                    inputValue = Console.ReadLine();
                    TextGreen();
                    if (string.IsNullOrWhiteSpace(inputValue))
                    {
                        ConsoleHelper.DisplayErrorFor(ConsoleHelper.NullInput, "Title");
                    }
                    else
                    {
                        editDnsObj.Name = inputValue;
                        break;
                    }
                }

                var dnsTemp = editDnsObj.DnsAddresses.Split(',');
                ConsoleHelper.ModifyTextFor("First DNS", dnsTemp[0]);
                TextWhite();
                inputValue = Console.ReadLine();
                TextGreen();
                if (string.IsNullOrWhiteSpace(inputValue))
                    editDnsObj.DnsAddresses = dnsTemp[0] + ",";
                else
                    editDnsObj.DnsAddresses = inputValue + ",";

                ConsoleHelper.ModifyTextFor("Second DNS", dnsTemp[0]);
                TextWhite();
                inputValue = Console.ReadLine();
                TextGreen();
                if (string.IsNullOrWhiteSpace(inputValue))
                    editDnsObj.DnsAddresses += dnsTemp[1];
                else
                    editDnsObj.DnsAddresses += inputValue;

                Console.WriteLine(ConsoleHelper.DashLine);
                if (ConsoleHelper.CheckConsentFor("Modify", editDnsObj.Id))
                {
                    OperationResult operationResult = await _dnsObjApplication.Edit(editDnsObj);
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

    private void DeleteDns()
    {
        Console.Clear();
        Console.WriteLine("- Delete Page -");
        Console.WriteLine(ConsoleHelper.BackButton);

        LoopDns(ConsoleColor.Yellow);
        Console.ForegroundColor = ConsoleColor.White;
        while (true)
        {
            var pressedKey = Console.ReadLine();
            BackButton(pressedKey);
            int row;
            var id = WhichRecordPressed(pressedKey);
            if (id != -1)
            {
                Console.WriteLine(ConsoleHelper.DashLine);
                if (ConsoleHelper.CheckConsentFor("Delete", id)) _dnsObjApplication.Delete(id);
                DeleteDns();
            }
        }
    }
}