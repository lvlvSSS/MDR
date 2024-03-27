namespace MDR.Data.Model.Dtos;

public class MdrResponseMessage<T> : MdrResponseMessage
{
    public T? Data { get; set; }
    public static MdrResponseMessage<T> Ok(T data)
    {
        return new MdrResponseMessage<T>() { Code = 0, Data = data };
    }
}

public class MdrResponseMessage
{
    public int Code { get; set; }
    public string? ErrorMessage { get; set; }
    public static MdrResponseMessage Error(int codeError, string? error)
    {
        return new MdrResponseMessage { Code = codeError, ErrorMessage = error, };
    }
}