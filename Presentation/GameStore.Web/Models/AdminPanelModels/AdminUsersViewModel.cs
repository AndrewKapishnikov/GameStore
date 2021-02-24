using GameStore.Web.App.Models;
using System.Collections.Generic;

namespace GameStore.Web.Models.AdminPanelModels
{
    public class AdminUsersViewModel
    {
        public IReadOnlyCollection<UserModel> Users { get; set; }
        public PaginationViewModel PageViewModel { get; set; }
        public FilterUsersViewModel FilterViewModel { get; set; }
        public SortUsersViewModel SortViewModel { get; set; }
    }
}
