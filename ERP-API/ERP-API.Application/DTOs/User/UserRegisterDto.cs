using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_API.Application.DTOs.User
{
    public class UserRegisterDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? BirthDate { get; set; }

        public string Email { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }

        public string[]? Roles { get; set; }
    }
}
