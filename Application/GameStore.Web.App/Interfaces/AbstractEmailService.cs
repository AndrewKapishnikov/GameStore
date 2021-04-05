using System.Threading.Tasks;

namespace GameStore.Web.App.Interfaces
{
    public abstract class AbstractEmailService
    {
        protected EmailConfiguration emailConfiguration;
        public abstract Task SendEmailAsync(string email, string subject, string messageBodу);
        public abstract Task SendOrderEmailAsync(OrderModel model);
    }
}
