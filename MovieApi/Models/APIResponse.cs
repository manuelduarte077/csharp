using System.Net;

namespace MovieApi.Models;

public class APIResponse
{
    public APIResponse()
    {
        ErrorMessages = new List<string>();
    }

    public HttpStatusCode StatusCode { get; set; }
    public bool Success { get; set; } = true;
    public List<string> ErrorMessages { get; set; }
    public object Result { get; set; }
}