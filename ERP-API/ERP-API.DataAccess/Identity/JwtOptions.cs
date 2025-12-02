using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_API.DataAccess.Identity
{
    public class JwtOptions
    {
        public const string sectionName = "JwtOptions";
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string SecretKey { get; set; }

        public int ExpiryMinutes { get; set; }

        public int RefreshTokenExpiryHours { get; set; }
    }
}
