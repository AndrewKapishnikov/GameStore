using GameStore.Web.App.Models;

namespace GameStore.Web.Models.AdminPanelModels
{
    public class SortGamesViewModel
    {
        public SortGameStates NameSort { get; private set; } 
        public SortGameStates PublisherSort { get; private set; }  
        public SortGameStates PriceSort { get; private set; }
        public SortGameStates DateOfAddingSort { get; private set; }
        public SortGameStates Current { get; private set; }
        public SortGameStates CurrentForPagination { get; private set; }
        public bool Up { get; set; }

        public SortGamesViewModel(SortGameStates sortOrder)
        {
            NameSort = SortGameStates.NameAsc;
            PublisherSort = SortGameStates.PublisherAsc;
            DateOfAddingSort = SortGameStates.DateOfAddingAsc;
            PriceSort = SortGameStates.PriceAsc;
            Up = true;
            CurrentForPagination = sortOrder;

            if (sortOrder == SortGameStates.DateOfAddingDesc || sortOrder == SortGameStates.NameDesc
                || sortOrder == SortGameStates.PublisherDesc || sortOrder == SortGameStates.PriceDesc)
            {
                Up = false;
            }

            switch (sortOrder)
            {
                case SortGameStates.NameDesc:
                    Current = NameSort = SortGameStates.NameAsc;
                    break;
                case SortGameStates.PublisherAsc:
                    Current = PublisherSort = SortGameStates.PublisherDesc;
                    break;
                case SortGameStates.PublisherDesc:
                    Current = PublisherSort = SortGameStates.PublisherAsc;
                    break;
                case SortGameStates.PriceAsc:
                    Current = PriceSort = SortGameStates.PriceDesc;
                    break;
                case SortGameStates.PriceDesc:
                    Current = PriceSort = SortGameStates.PriceAsc;
                    break;
                case SortGameStates.DateOfAddingAsc:
                    Current = DateOfAddingSort = SortGameStates.DateOfAddingDesc;
                    break;
                case SortGameStates.DateOfAddingDesc:
                    Current = DateOfAddingSort = SortGameStates.DateOfAddingAsc;
                    break;
                default:
                    Current = NameSort = SortGameStates.NameDesc;
                    break;
            }
        }



    }


}
