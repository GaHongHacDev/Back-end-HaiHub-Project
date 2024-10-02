using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Requests.Accounts
{
    public class CreateAccountLoginGoogleRequest
    {
        public string IdToken { get; set; }
        [Required]
        public string RoleName { get; set; }
        [Required]
        public string Phone { get; set; }
    }
}
