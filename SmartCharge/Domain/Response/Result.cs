namespace SmartCharge.Domain.Response;

public class Result<T>
{
    public T Data { get; private set; }
    public string Error { get; private set; }
    public bool IsSuccess => string.IsNullOrEmpty(Error);
    
    private Result(T value, string errorMessage)
    {
        Data = value;
        Error = errorMessage;
    }
    public static Result<T> Success(T value) => new(value, null);
    public static Result<T> Failure(string errorMessage) => new(default, errorMessage);
}
