using System.ComponentModel.DataAnnotations;

namespace HM_byDH.Models.ViewModels.Authentication
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Email là bắt buộc.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        public string Email { get; set; }
    }
}
