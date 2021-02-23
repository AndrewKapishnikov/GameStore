using System.Collections.Generic;

namespace GameStore.Web.Models
{
    public class ConfirmationViewModel
    {
        public string Id { get; set; }  //UserId
        public string OrderUrl { get; set; }
        public Dictionary<string, string> Errors { get; set; } = new Dictionary<string, string>();
    }
}
