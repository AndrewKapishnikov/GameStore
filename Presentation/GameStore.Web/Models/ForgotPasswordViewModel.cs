using System.ComponentModel.DataAnnotations;

namespace GameStore.Web.Models
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Не указан электронный адрес")]
        [Display(Name = "Электронная почта")]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Некорректный адрес электронной почты")]
        public string Email { get; set; }
    }
}
