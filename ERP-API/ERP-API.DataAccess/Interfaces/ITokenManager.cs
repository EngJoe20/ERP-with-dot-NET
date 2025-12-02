using ERP_API.DataAccess.Entities.User;
using ERP_API.DataAccess.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_API.DataAccess.Interfaces
{
    public interface ITokenManager
    {
        Task<TokenResult> GetTokenAsync(AppUser user);
        public TokenResult GetRefreshToken();

        Task<TokenResult> RefreshToken(string accessToken, string refreshToken);
    }
}
