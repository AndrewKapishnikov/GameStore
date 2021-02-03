using MimeKit;

using MailKit.Net.Smtp;
using System.Text;
using System.Threading.Tasks;

namespace GameStore.Web.App
{
    public class EmailService
    {
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            await Task.Yield();
        }
    }
}
