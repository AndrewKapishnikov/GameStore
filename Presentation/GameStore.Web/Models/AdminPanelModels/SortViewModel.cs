using GameStore.Web.App.Models;

namespace GameStore.Web.Models.AdminPanelModels
{
    public class SortViewModel
    {
        public SortGameState NameSort { get; private set; } 
        public SortGameState PublisherSort { get; private set; }  
        public SortGameState PriceSort { get; private set; }
        public SortGameState DateOfAddingSort { get; private set; }
        public SortGameState Current { get; private set; }
        public SortGameState CurrentForPagination { get; private set; }
        public bool Up { get; set; }

        public SortViewModel(SortGameState sortOrder)
        {

            NameSort = SortGameState.NameAsc;
            PublisherSort = SortGameState.PublisherAsc;
            DateOfAddingSort = SortGameState.DateOfAddingAsc;
            PriceSort = SortGameState.PriceAsc;
            Up = true;
            CurrentForPagination = sortOrder;

            if (sortOrder == SortGameState.DateOfAddingDesc || sortOrder == SortGameState.NameDesc
                || sortOrder == SortGameState.PublisherDesc || sortOrder == SortGameState.PriceDesc)
            {
                Up = false;
            }

            switch (sortOrder)
            {
                case SortGameState.NameDesc:
                    Current = NameSort = SortGameState.NameAsc;
                    break;
                case SortGameState.PublisherAsc:
                    Current = PublisherSort = SortGameState.PublisherDesc;
                    break;
                case SortGameState.PublisherDesc:
                    Current = PublisherSort = SortGameState.PublisherAsc;
                    break;
                case SortGameState.PriceAsc:
                    Current = PriceSort = SortGameState.PriceDesc;
                    break;
                case SortGameState.PriceDesc:
                    Current = PriceSort = SortGameState.PriceAsc;
                    break;
                case SortGameState.DateOfAddingAsc:
                    Current = DateOfAddingSort = SortGameState.DateOfAddingDesc;
                    break;
                case SortGameState.DateOfAddingDesc:
                    Current = DateOfAddingSort = SortGameState.DateOfAddingAsc;
                    break;
                default:
                    Current = NameSort = SortGameState.NameDesc;
                    break;
            }
        }



    }


}
