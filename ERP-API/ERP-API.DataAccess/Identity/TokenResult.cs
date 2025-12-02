using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_API.DataAccess.Identity
{
    public class TokenResult
    {
        public string Token { get; set; }
        public DateTime TokenExpiryTime { get; set; }

        public List<AppClaim> Claims { get; set; }


    }

    public class AppClaim
    {
        public string Type { get; set; }
        public string Value { get; set; }
    }
}
