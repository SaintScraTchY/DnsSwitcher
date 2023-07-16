﻿namespace HelperClass.Application;

public class OperationResult
{
    public OperationResult()
    {
        IsSucceeded = false;
    }

    public bool IsSucceeded { get; set; }
    public string Message { get; set; }

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