
namespace GameStore.Web.App.Models
{
    public class UserWithOrderModel
    {
        public UserModel UserModel { get; set; }
        public ShortOrderModel[] UserOrdersModels { get; set; }
    }
}
