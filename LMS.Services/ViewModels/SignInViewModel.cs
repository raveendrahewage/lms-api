using System.ComponentModel.DataAnnotations;

namespace LMS.Api.ViewModels
{
    public class SignInViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
        public bool RememberMe { get; set; }
    }
}
