using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Requests.SMS
{
    public class SendSMS
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Text { get; set; }
    }
}
