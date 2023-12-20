namespace CityInfo.API.Services
{
    public class LocalMailingService : IMailingService
    {
        private readonly string mailFrom = string.Empty;

        private readonly string mailTo = string.Empty;

        public LocalMailingService(IConfiguration configuration)
        {
            mailTo = configuration["mailSettings:mailTo"];
            mailFrom = configuration["mailSettings:mailFrom"];
        }

        public void sendMail(string subject, string mailBody)
        {
            Console.WriteLine($"Mail sent from {mailFrom} to {mailTo}");
            Console.WriteLine($"With use of {nameof(LocalMailingService)}");
            Console.WriteLine($"Mail Subject: {subject}");
            Console.WriteLine($"Mail body: {mailBody}");
        }
    }
}
