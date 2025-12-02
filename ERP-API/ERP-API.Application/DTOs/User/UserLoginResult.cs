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
            TokenResult = new TokenResultDto();
        }

        public string UserName { get; set; }

        public TokenResultDto TokenResult { get; set; }

        public List<AppClaim> Claims { get; set; }

        public string[] Roles { get; set; }

        public List<string> Errors { get; set; }

        public bool Succeeded { get; set; } = false;

    }
}
