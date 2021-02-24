using GameStore.Web.App.Models;

namespace GameStore.Web.Models.AdminPanelModels
{
    public class SortUsersViewModel
    {
        public SortUserStates UserEmailSort { get; private set; }
        public SortUserStates UserNameSort { get; private set; }
        public SortUserStates Current { get; private set; }
        public SortUserStates CurrentForPagination { get; private set; }
        public bool Up { get; set; }

        public SortUsersViewModel(SortUserStates sortUser)
        {
            UserEmailSort = SortUserStates.UserEmailAsc;
            UserNameSort = SortUserStates.UserNameAsc;
            Up = true;
            CurrentForPagination = sortUser;

            if (sortUser == SortUserStates.UserNameDesc || sortUser == SortUserStates.UserEmailDesc)
            {
                Up = false;
            }

            switch (sortUser)
            {
                case SortUserStates.UserEmailAsc:
                    Current = UserEmailSort = SortUserStates.UserEmailDesc;
                    break;
                case SortUserStates.UserEmailDesc:
                    Current = UserEmailSort = SortUserStates.UserEmailAsc;
                    break;
                case SortUserStates.UserNameAsc:
                    Current = UserNameSort = SortUserStates.UserNameDesc;
                    break;
                default:
                    Current = UserNameSort = SortUserStates.UserNameAsc;
                    break;
            }
        }
    }
}
