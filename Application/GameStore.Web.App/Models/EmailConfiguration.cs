
namespace GameStore.Web.App
{
    public class EmailConfiguration
    {
        public string MessageSenderEmail { get; set;}
        public string MessageSenderCompany { get; set; }
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public string SmtpUsername { get; set; }
        public string SmtpPassword { get; set; }
        public bool TestMode { get; set; }
    }
}
