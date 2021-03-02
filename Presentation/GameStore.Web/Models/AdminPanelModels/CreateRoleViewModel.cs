using System.ComponentModel.DataAnnotations;

namespace GameStore.Web.Models.AdminPanelModels
{
    public class CreateRoleViewModel
    {
        [Required(ErrorMessage = "Поле Роль должно быть заполнено")]
        [Display(Name = "Роль")]
        [StringLength(25, MinimumLength = 4, ErrorMessage = "От 4 до 30 символов")]
        public string RoleName { get; set; }
    }
}
