using ERP_API.DataAccess.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_API.Application.DTOs.User
{
    public class UserLoginResult
    {
        public UserLoginResult()
        {
            Errors = new List<string>();
            TokenResult = new TokenResultDto
            {
                Token = string.Empty,
                RefreshToken = string.Empty
            };
            Claims = new List<AppClaim>();
            Roles = Array.Empty<string>();
            UserName = string.Empty;
        }

        public string UserName { get; set; }

        public TokenResultDto TokenResult { get; set; }

        public List<AppClaim> Claims { get; set; }

        public string[] Roles { get; set; }

        public List<string> Errors { get; set; }

        public bool Succeeded { get; set; } = false;

    }
}
