using Hairhub.Domain.Dtos.Requests.Otps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Service.Services.IServices
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(SendOtpEmailRequest sendEmailRequest);
        Task<bool> CheckOtpEmail(CheckOtpRequest checkOtpRequest);
        public Task<bool> CheckExistEmail(CheckExistEmailResrequest checkExistEmailResrequest);
        Task<bool> SendEmailAsyncNotifyOfExpired(string emailIndividual, string fullname, int REMAINING_DAY, DateTime EXPIRATION_DATE, string LINK_PAYMENT);
        Task<bool> SendEmailWithBodyAsync(string emailRequest, string subjectEmail, string fullName, string bodyEmail);
        Task<bool> SendEmailRegisterAccountAsync(string emailRequest, string subjectEmail, string fullName, string userNameAccount, string passwordAccount);
    }
}
