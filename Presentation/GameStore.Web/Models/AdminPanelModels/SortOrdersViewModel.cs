using GameStore.Web.App.Models;

namespace GameStore.Web.Models.AdminPanelModels
{
    public class SortOrdersViewModel
    {
        public SortOrderStates DateSort { get; private set; }
        public SortOrderStates UserEmailSort { get; private set; }
        public SortOrderStates UserNameSort { get; private set; }
        public SortOrderStates Current { get; private set; }
        public SortOrderStates CurrentForPagination { get; private set; }
        public bool Up { get; set; }

        public SortOrdersViewModel(SortOrderStates sortOrder)
        {
            DateSort = SortOrderStates.OrderDateDesc;
            UserEmailSort = SortOrderStates.UserEmailAsc;
            UserNameSort = SortOrderStates.UserNameAsc;
            Up = true;
            CurrentForPagination = sortOrder;

            if (sortOrder == SortOrderStates.UserNameDesc || sortOrder == SortOrderStates.UserEmailDesc
                || sortOrder == SortOrderStates.OrderDateDesc)
            {
                Up = false;
            }

            switch (sortOrder)
            {
                case SortOrderStates.OrderDateDesc:
                    Current = DateSort = SortOrderStates.OrderDateAsc;
                    break;
                case SortOrderStates.UserEmailAsc:
                    Current = UserEmailSort = SortOrderStates.UserEmailDesc;
                    break;
                case SortOrderStates.UserEmailDesc:
                    Current = UserEmailSort = SortOrderStates.UserEmailAsc;
                    break;
                case SortOrderStates.UserNameAsc:
                    Current = UserNameSort = SortOrderStates.UserNameDesc;
                    break;
                case SortOrderStates.UserNameDesc:
                    Current = UserNameSort = SortOrderStates.UserNameAsc;
                    break;
                default:
                    Current = DateSort = SortOrderStates.OrderDateDesc;
                    break;
            }
        }
    }
}
