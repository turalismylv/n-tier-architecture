using System.ComponentModel.DataAnnotations;

namespace Web.Areas.Admin.ViewModels.Account
{
    public class AccountLoginVM
    {

        [Required]
        public string Username { get; set; }

        [Required, MaxLength(100), DataType(DataType.Password)]
        public string Password { get; set; }

        public string? ReturnUrl { get; set; }
    }
}
