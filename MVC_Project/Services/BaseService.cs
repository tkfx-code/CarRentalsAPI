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

        if (httpContext == null || !httpContext.User.Identity!.IsAuthenticated)
        {
            //could log case of user trying to reach secured API endpoint here
            return;
        }
        //fetch local claimsprincipal JWT token from SignInAsync
        var token = httpContext.User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value;

        if (string.IsNullOrEmpty(token))
        {
            //alt 2. fetch from session if not in claims as backup
            token = httpContext.Session.GetString("JWTtoken");
            if (string.IsNullOrEmpty(token))
            {
                return;
            }
            return; 
        }

        _client.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
}