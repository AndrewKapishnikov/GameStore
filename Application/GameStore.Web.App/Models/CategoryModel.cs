using System.ComponentModel.DataAnnotations;

namespace GameStore.Web.App.Models
{
    public class CategoryModel
    {
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Заполните поле Название")]
        [Display(Name = "Название")]
        [StringLength(40, MinimumLength = 3, ErrorMessage = "От 3 до 40 символов")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Заполните поле URL Slug")]
        [Display(Name = "URL Slug")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "От 3 до 30 символов")]
        [RegularExpression(@"^[a-z0-9]{3,30}$", ErrorMessage = "Только не заглавные английские буквы или цифры без пробелов от 3 до 30")]
        public string CategoryUrlSlug { get; set; }
    }
}
