using MailKit.Net.Smtp;
using MimeKit;

namespace bt_api.Helpers
{
    public class Mail
    {
        public readonly IConfigurationRoot Config;

        public Mail()
        {
            this.Config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        }

        public void Send(MimeMessage message)
        {
            var username = this.Config.GetValue<string>("Smtp:Username");
            var password = this.Config.GetValue<string>("Smtp:Password");

            if (message.To.Count == 0) 
            {
                message.To.Add(new MailboxAddress("Baldur's Trials", this.Config.GetValue<string>("Smtp:Username")));
            }

            using var client = new SmtpClient();

            client.Connect(
                this.Config.GetValue<string>("Smtp:ServerAddress"),
                this.Config.GetValue<int>("Smtp:PortSSL"),
                true
            );

            // Note: only needed if the SMTP server requires authentication
            client.Authenticate(
                username,
                password
            );

            client.Send(message);
            client.Disconnect(true);
        }
    }
}
