using Hairhub.Domain.Dtos.Requests.SMS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Service.Services.IServices
{
    public interface ISMSService
    {
        Task<bool> SendOtpSMS(SendSMS reqeust);
    }
}
