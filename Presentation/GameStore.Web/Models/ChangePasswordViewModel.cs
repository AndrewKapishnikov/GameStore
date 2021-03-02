using System.ComponentModel.DataAnnotations;

namespace GameStore.Web.Models
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Заполните поле Текущий пароль")]
        [DataType(DataType.Password)]
        [Display(Name = "Текущий пароль")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "Заполните поле Новый пароль")]
        [DataType(DataType.Password)]
        [Display(Name = "Новый пароль")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Подтвердите новый пароль")]
        [Compare("NewPassword", ErrorMessage = "Поле Новый пароль и Подтверждение пароля не соответствуют")]
        public string ConfirmPassword { get; set; }
    }
}
