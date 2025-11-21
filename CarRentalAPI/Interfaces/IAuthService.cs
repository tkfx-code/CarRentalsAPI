using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Collections.Generic;
using CarRentalAPI.Data;

namespace CarRentalAPI.Interfaces
{
    public interface IAuthService
    {
        string CreateJwtToken(APIUser user, IList<string> roles);
    }
}
