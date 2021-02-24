using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace GameStore.Web.Models.AdminPanelModels
{
    public class FilterUsersViewModel
    {
        private List<PagePair> pagePair = new List<PagePair>()
        {
            new PagePair() { Number = 5, NumberName = "5"},
            new PagePair() { Number = 10, NumberName = "10"},
            new PagePair() { Number = 20, NumberName = "20"},
            new PagePair() { Number = 50, NumberName = "50"},
            new PagePair() { Number = 100, NumberName = "100"}
        };
        public FilterUsersViewModel(string userName, string userEmail, int pageSize, bool confirmed)
        {
            SelectedUserName = userName;
            SelectedUserEmail = userEmail;
            CountPages = new SelectList(pagePair, "Number", "NumberName", pageSize);
            SelectedPageSize = pageSize;
            ConfirmedEmail = confirmed;

        }
        public SelectList CountPages { get; private set; }
        public int SelectedPageSize { get; private set; }
        public string SelectedUserName { get; private set; }
        public string SelectedUserEmail { get; private set; }
        public bool ConfirmedEmail { get; private set; }
    }
}
