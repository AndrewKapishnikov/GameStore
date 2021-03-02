using GameStore.Web.App;
using GameStore.Web.Models.AdminPanelModels;
using System.Collections.Generic;

namespace GameStore.Web.Models
{
    public class CategoryGamesViewModel
    {
        public IReadOnlyCollection<GameModel> Games { get; set; }
        public PaginationViewModel PageViewModel { get; set; }
        public string Category { get; set; }
    }
}
