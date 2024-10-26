using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Requests.Payment
{
    public class CreateWithdrawPaymentRequest
    {
        public Guid AccountId { get; set; }
        public string FullName { get; set; }
        public string NumberAccount { get; set; }
        public string BankName { get; set; }
        public decimal Balance { get; set; }
        public List<IFormFile> IdentityCard {get; set;}
    }
}
