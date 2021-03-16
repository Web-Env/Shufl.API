namespace Shufl.API.Infrastructure.Settings
{
    public class SmtpSettings
    {
        public string EmailFromName { get; set; }

        public string EmailFromAddress { get; set; }

        public string EmailSmtpHost { get; set; }

        public int EmailSmtpPort { get; set; }

        public string EmailSmtpUsername { get; set; }

        public string EmailSmtpPassword { get; set; }
    }
}
