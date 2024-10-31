namespace SmartCharge.Domain.Response;

public class Result<T>
{
    public T Data { get; set; }
    public string Error { get; set; }

    public bool IsSuccess => string.IsNullOrEmpty(Error);
    
    
}