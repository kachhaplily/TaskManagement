using TaskManagement.Model;

namespace TaskManagement.UtlityServices
{


    public interface IEmailService
    {
        void SendEmail(EmailModel emailModel);
    }

}
