using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Requests.Authentication
{
    public class LoginRequest
    {
        [Required]
        public string Username { get; set; }
        [Required]
        [MinLength(8, ErrorMessage = "Password must have at least 8 characters")]
        public string Password { get; set; }
    }
}
