using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Common.CommonService.Contract
{
    public interface IQRCodeService
    {
        Task<string> GenerateQR(Guid AppointmentId);
        string DecodeQR(string hashedAppointmentData);
    }
}
