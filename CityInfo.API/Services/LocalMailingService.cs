﻿namespace CityInfo.API.Services
{
    public class LocalMailingService : IMailingService
    {
        public string mailFrom { get; set; } = "noreply@myCompany.foobar";

        public string mailTo { get; set; } = "admin@myCompany.foobar";

        public void sendMail(string subject, string mailBody)
        {
            Console.WriteLine($"Mail sent from {mailFrom} to {mailTo}");
            Console.WriteLine($"With use of {nameof(LocalMailingService)}");
            Console.WriteLine($"Mail Subject: {subject}");
            Console.WriteLine($"Mail body: {mailBody}");
        }
    }
}
