using MVC_Project.Models;

namespace MVC_Project.Services
{
    public interface IAuthClientService
    {
        Task<bool> LoginAsync(LoginUserDto model);
        Task<bool> RegisterAsync(RegisterViewModel model);
        public Task Logout();
    }
}
