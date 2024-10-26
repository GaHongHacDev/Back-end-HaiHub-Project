using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Requests.Payment
{
    public class WithdrawConfirmRequest
    {
        public Guid Id;
        public List<IFormFile>? BankingImgs { get; set; }
        public string? ReasonCancel { get; set; }
        public string StatusConfirm { get; set; }
    }
}
