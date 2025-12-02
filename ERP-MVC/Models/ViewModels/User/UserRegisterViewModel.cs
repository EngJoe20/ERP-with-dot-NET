using System.ComponentModel.DataAnnotations;

namespace ERP_MVC.Models.ViewModels.User
{
    public class UserRegisterViewModel
    {
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }


        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }


        [Display(Name = "Birth Date")]
        public DateTime? BirthDate { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        public string[]? Roles { get; set; }
    }
}
