using System.ComponentModel.DataAnnotations;

namespace ERP_MVC.Models.ViewModels.User
{
    public class UserLoginViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; }


    }
}
