using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using MVC_Project.Models;
using MVC_Project.Services;

namespace MVC_Project.Services
{
    public partial class Client : IClient
    {
        public HttpClient HttpClient
        {
            get
            {
                return _httpClient;
            }
        }
    }
}
