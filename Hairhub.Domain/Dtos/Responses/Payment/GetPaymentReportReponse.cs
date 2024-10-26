using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Responses.Payment
{
    public class GetPaymentReportReponse
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public string Email { get; set; }
        public string RoleName { get; set; }
        public decimal Balance { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? ConfirmDate { get; set; }
        public string Status { get; set; }
    }
}
