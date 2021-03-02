using System.ComponentModel.DataAnnotations;

namespace GameStore.Web.Models
{
    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "Не указан электронный адрес")]
        [Display(Name = "Электронная почта")]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Некорректный адрес электронной почты")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Не задан пароль")]
        [Display(Name = "Пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Подтвердите пароль")]
        [Compare("Password", ErrorMessage = "Пароль и подтверждённый пароль не соответствуют")]
        public string ConfirmPassword { get; set; }

        public string Token { get; set; }
    }
}
