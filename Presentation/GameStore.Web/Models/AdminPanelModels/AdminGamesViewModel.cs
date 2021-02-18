using GameStore.Web.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameStore.Web.Models.AdminPanelModels
{
    public class AdminGamesViewModel
    {
        public IReadOnlyCollection<GameModel> Games { get; set; }
        public PaginationViewModel PageViewModel { get; set; }
        public FilterViewModel FilterViewModel { get; set; }
        public SortViewModel SortViewModel { get; set; }
    
    }
}
