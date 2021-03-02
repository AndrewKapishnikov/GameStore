using System.ComponentModel.DataAnnotations;

namespace GameStore.Web.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Поле 'Email' должно быть заполнено")]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Некорректный адрес электронной почты")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Поле 'Пароль' должно быть заполнено")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }
}
