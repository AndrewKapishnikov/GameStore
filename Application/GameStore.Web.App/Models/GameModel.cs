using System;
using System.ComponentModel.DataAnnotations;


namespace GameStore.Web.App
{
    public class GameModel
    {
        public int GameId { get; set; }

        [Required(ErrorMessage = "Заполните поле Название")]
        [Display(Name = "Название")]
        [StringLength(40, MinimumLength = 3, ErrorMessage = "От 3 до 40 символов")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Заполните поле Разработчик")]
        [Display(Name = "Разработчик")]
        [StringLength(40, MinimumLength = 3, ErrorMessage = "От 3 до 40 символов")]
        public string Publisher { get; set; }

        public string Category { get; set; }
        public int? CategoryId { get; set; }

        [Required(ErrorMessage = "Заполните поле Краткое описание")]
        [Display(Name = "Краткое описание")]
        [StringLength(170, MinimumLength = 20, ErrorMessage = "От 20 до 170 символов")]
        public string ShortDescription { get; set; }

        [Required(ErrorMessage = "Заполните поле Подробное описание игры")]
        [Display(Name = "Описание игры")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Заполните поле Цена")]
        [Display(Name = "Цена")]
        [Range(typeof(decimal), "0.00", "100000.00", ErrorMessage = "Цена должна быть в диапазоне от 0 до 100000",
               ParseLimitsInInvariantCulture = true, ConvertValueInInvariantCulture = true)]
        [RegularExpression(@"^[0-9]{1,6}$", ErrorMessage = "От 0 до 100000")]
        public decimal Price { get; set; }
       
        public byte[] ImageData { get; set; }

        [Required(ErrorMessage = "Заполните поле Дата выхода")]
        [Display(Name = "Дата выхода:")]
        public DateTime ReleaseDate { get; set; }
        public DateTime DateOfAdding { get; set; }

        [Display(Name = "Игра находится в продаже")]
        public bool OnSale { get; set; }
    }
}
