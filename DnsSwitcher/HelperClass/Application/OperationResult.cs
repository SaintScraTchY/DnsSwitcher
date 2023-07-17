namespace HelperClass.Application;

public class OperationResult
{
    public OperationResult()
    {
        Message = string.Empty;
        IsSucceeded = false;
    }

    public bool IsSucceeded { get; private set; }
    public string Message { get; private set; }

    public OperationResult Succeeded(string message = "Task Succeeded")
    {
        IsSucceeded = true;
        Message = message;
        return this;
    }

    public OperationResult Failed(string message)
    {
        IsSucceeded = false;
        Message = message;
        return this;
    }
}