namespace DnsChangerConsole;

public static class ConsoleHelper
{
    public const string DashLine = "-----------------------------------------";
    public const string NullInput = "Your Input Cannot Be Null";
    public const string BackButton = "[B]ack";
    public const string InvalidInput = "There was An Error in your Input";

    public static void ModifyTextFor(string field, string value)
    {
        Console.WriteLine($"Enter New Value for {field} | [Previous value:{value}] ");
    }

    public static bool CheckConsentFor(string operationType, int id)
    {
        Console.WriteLine($"Do you want to {operationType} Dns #{id} ? Y/N - Y=Yes | N=No");
        while (true)
        {
            var PressedKey = Console.ReadLine();
            if (PressedKey == "y" || PressedKey == "Y")
                return true;
            if (PressedKey == "n" || PressedKey == "N") return false;
        }
    }

    public static void DisplayErrorFor(string error, string field = "")
    {
        var Text = error;
        if (!string.IsNullOrWhiteSpace(field))
            Text += " For " + field;
        Console.WriteLine(Text);
    }
}