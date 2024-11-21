using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Entitities
{
    public class Config
    {
        public Guid Id { get; set; }
        public string PakageName { get; set; }
        public string Description { get; set; }
        public decimal PakageFee { get; set; }
        public DateTime DateCreate { get; set; }
        public bool IsActive { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
    }
}
