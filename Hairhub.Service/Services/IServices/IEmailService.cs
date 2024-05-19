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
    }
}
