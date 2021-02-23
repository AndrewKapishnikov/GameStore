using GameStore.Web.App.Models;
using System.Collections.Generic;

namespace GameStore.Web.Models.AdminPanelModels
{
    public class AdminOrdersViewModel
    {
        public IReadOnlyCollection<ShortOrderModel> Orders { get; set; }
        public PaginationViewModel PageViewModel { get; set; }
        public FilterOrdersViewModel FilterViewModel { get; set; }
        public SortOrdersViewModel SortViewModel { get; set; }
    }
}
