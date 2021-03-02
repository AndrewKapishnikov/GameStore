using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace GameStore.Web.Models
{
     public class RegisterViewModel
     {

         [Required(ErrorMessage = "Заполните поле Имя")]
         [Display(Name = "Имя")] 
         [StringLength(30, MinimumLength = 3, ErrorMessage = "От 3 до 30 символов")]
         public string Name { get; set; }

         [Required(ErrorMessage = "Заполните поле Фамилия")]
         [Display(Name = "Фамилия")]
         [StringLength(40, MinimumLength = 3, ErrorMessage = "От 3 до 40 символов")]
         public string Surname { get; set; }

         [Required(ErrorMessage = "Не указан электронный адрес")]
         [Remote(action: "IsEmailInUse", controller: "Account")]
         [Display(Name = "Электронная почта")]
         [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Некорректный адрес электронной почты")]
         public string Email { get; set; }

         [Required(ErrorMessage = "Поле должно быть заполнено")]
         [Display(Name = "Насел. пункт")]
         [StringLength(30, MinimumLength = 3, ErrorMessage = "От 3 до 30 символов")] 
         public string City { get; set; }

         [Required(ErrorMessage = "Поле должно быть заполнено")]
         [Display(Name = "Телефон")]
         public string Telephone { get; set; }

         [Required(ErrorMessage = "Поле Адреса должно быть заполнено")]
         [Display(Name = "Адрес")]
         [StringLength(50, MinimumLength = 7, ErrorMessage = "Длина строки должна быть от 7 до 50 символов")]
         public string Address { get; set; }

         [Required(ErrorMessage = "Поле должно быть заполнено")]
         [Display(Name = "Пароль")]
         [DataType(DataType.Password)]
         public string Password { get; set; }
    
         [DataType(DataType.Password)]
         [Display(Name = "Подтвердить пароль")]
         [Compare("Password", ErrorMessage = "Не соответствует паролю")]
         public string ConfirmPassword { get; set; }

        
    }
}
