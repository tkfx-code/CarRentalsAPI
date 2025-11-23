using System.Net.Http.Headers;
using Microsoft.AspNet.SignalR.Hubs;
using MVC_Project.Services;

public class BaseService
{
    protected readonly IHttpContextAccessor _httpContextAccessor;
    protected readonly IClient _client;    

    protected BaseService(IHttpContextAccessor httpContextAccessor, IClient client)
    {
        _httpContextAccessor = httpContextAccessor;
        _client = client;
    }

    protected void CarryAccessToken()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var token = httpContext.User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value;

        if (string.IsNullOrEmpty(token))
        {
            return; 
        }

        _client.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
}