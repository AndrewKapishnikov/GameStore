using MimeKit;
using MailKit.Net.Smtp;
using System.Text;
using System.Threading.Tasks;
using MailKit.Security;
using System;

//using System.Net.Mail;
using System.Net;

namespace GameStore.Web.App
{
    public class EmailService
    {
        private readonly EmailConfiguration emailConfiguration;

        public EmailService(EmailConfiguration emailConfiguration)
        {
            this.emailConfiguration = emailConfiguration;
        }
        public async Task SendEmailAsync(string email, string subject, string messageBodу)
        {
            
            MimeMessage message = new MimeMessage();
            message.From.Add(new MailboxAddress(emailConfiguration.MessageSenderCompany, emailConfiguration.MessageSenderEmail));
            message.To.Add(new MailboxAddress("", email)); //addressee of the message
            message.Subject = subject; //Message subject
            message.Body = new BodyBuilder() { HtmlBody = messageBodу }.ToMessageBody();

            using (SmtpClient client = new SmtpClient())
            {
                if(emailConfiguration.TestMode)
                {
                    client.ServerCertificateValidationCallback = (mysender, certificate, chain, sslPolicyErrors) => { return true; };
                    client.CheckCertificateRevocation = false;
                }
                client.AuthenticationMechanisms.Remove("XOAUTH2");

                await client.ConnectAsync(emailConfiguration.SmtpServer, emailConfiguration.SmtpPort, true);
                await client.AuthenticateAsync(emailConfiguration.SmtpUsername, emailConfiguration.SmtpPassword);
                await client.SendAsync(message);

                await client.DisconnectAsync(true);
               
            }

   
        }
    }
}
