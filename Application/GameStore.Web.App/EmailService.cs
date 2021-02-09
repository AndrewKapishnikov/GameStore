using MimeKit;
using MailKit.Net.Smtp;
using System.Text;
using System.Threading.Tasks;
using System;
//using System.Net.Mail;


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


        public async Task SendOrderEmailAsync(OrderModel model)
        {
            var message = new StringBuilder();
            message.Append("<h2>Заказ передан в службу доставки</h2><div>");
            foreach (var item in model.OrderItems)
            {
                message.Append("<div style=\"margin-top:10px;margin-right:15px;float:left\">");
                message.Append(item.GameName);
                message.Append("</div><div style=\"margin-top:10px;margin-right:15px;float:left\">");
                message.Append(item.Count.ToString() + " шт.");
                message.Append("</div><div style = \"margin-top:10px;float:left\">");
                message.Append(Math.Truncate(item.Price) + " руб.</div><div style=\"clear: both\"></div>");
            }
            message.Append("</div><div style=\"clear:both\"></div><div style=\"margin-top: 30px;\">" +
                           "<div style=\"margin-top:10px\">Доставка:</div><div style=\"margin-top:10px;margin-right:15px;\">");
            message.Append(model.DeliveryDescription + "</div>");
            if (model.DeliveryName == "Courier")
            {
                message.Append("<div style=\"margin-top:10px;margin-right:10px;float:left\">По адресу: ");
                message.Append(model.UserCity + "</div><div style=\"margin-top:10px; margin-right:15px;float:left\">");
                message.Append(model.UserAddress + "</div><div style=\"clear:both\"></div><div style=\"margin-top:10px;margin-right:15px;\">");
            }
            message.Append("Стоимость доставки: " + Math.Truncate(model.DeliveryPrice) + " руб.</div>");
            message.Append("</div><div style=\"margin-top:30px;\"><div style=\"margin-top:10px\"> Оплата:</div><div style=\"margin-top:10px;margin-right:15px;\"> ");
            message.Append(model.PaymentDescription + "</div><div style=\"margin-top:10px\">Общая стоимость заказа: ");
            message.Append(Math.Truncate(model.TotalPrice) + " руб.</div></div>");

            await SendEmailAsync(model.UserEmail,"Данные оформленного заказа", message.ToString());


        }


        }
    }
