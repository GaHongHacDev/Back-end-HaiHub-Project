using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Entitities
{
    public class PaymentReport
    {
        [Key]
        public Guid PaymentId {  get; set; }
        public string? ReasonCancle { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ConfirmDate { get; set; }
        public string FullName { get; set; }
        public string NumberAccount { get; set; }
        public string BankName { get; set; }
        public decimal Balance { get; set; }

        public virtual ICollection<StaticFile> StaticFiles { get; set; }
        public virtual Payment Payment { get; set; }
    }
}
