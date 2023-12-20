namespace CityInfo.API.Services
{
    public interface IMailingService
    {
        void sendMail(string subject, string mailBody);
    }
}