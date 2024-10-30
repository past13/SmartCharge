namespace SmartCharge.Domain.Response;

public class ApiResponse<T>
{
    public T Data { get; set; }
    public string Error { get; set; }

    public bool IsSuccess => string.IsNullOrEmpty(Error);
}