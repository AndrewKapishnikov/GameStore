using System;

namespace GameStore.Web.Models.AdminPanelModels
{
    public class PaginationViewModel
    {
        public int PageNumber { get; private set; }
        public int TotalPages { get; private set; }

        public PaginationViewModel(int count, int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        }

        public bool HasHome
        {
            get
            {
                return ((PageNumber - 2) > 1);
            }
        }
        public bool HasPreviousPage
        {
            get
            {
                return (PageNumber > 1);
            }
        }
        public bool HasPreviousPreviousPage
        {
            get
            {
                return ( (PageNumber - 1) > 1);
            }
        }

        public bool HasNextPage
        {
            get
            {
                return (PageNumber < TotalPages);
            }
        }
        public bool HasNextNextPage
        {
            get
            {
                return ((PageNumber + 1)< TotalPages);
            }
        }
        public bool HasEnd
        {
            get
            {
                return ((PageNumber + 2) < TotalPages);
            }
        }
    }
}
