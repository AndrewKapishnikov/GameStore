using System;


namespace GameStore.Web.App.Models
{
    public class ShortOrderModel
    {
        public int Id { get; set; }
        public bool OrderReviewed { get; set; }
        public DateTime OrderDateAndTime { get; set; }
        public int TotalCount { get; set; }
        public decimal TotalPrice { get; set; }
        public string UserName { get; set; }
        public string UserSurname { get; set; }
        public string UserEmail { get; set; }

     
    }
}
