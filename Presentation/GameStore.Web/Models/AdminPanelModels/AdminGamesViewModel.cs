using GameStore.Web.App;
using System.Collections.Generic;

namespace GameStore.Web.Models.AdminPanelModels
{
    public class AdminGamesViewModel
    {
        public IReadOnlyCollection<GameModel> Games { get; set; }
        public PaginationViewModel PageViewModel { get; set; }
        public FilterGamesViewModel FilterViewModel { get; set; }
        public SortGamesViewModel SortViewModel { get; set; }
    
    }
}
