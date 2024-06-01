using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Requests.Otps
{
    public class CheckExistEmailResrequest
    {
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}
