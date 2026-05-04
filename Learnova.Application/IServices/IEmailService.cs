namespace Learnova.Application.IServices
{
    public interface IEmailService
    {
        Task SendEmail(string receptor, string subject, string body);
    }
}