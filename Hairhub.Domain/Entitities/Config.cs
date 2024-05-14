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
        [Key]
        public Guid ConfigId { get; set; }
        public decimal? CommissionRate { get; set; }
        public decimal? MaintenanceFee { get; set; }
        public DateTime? DateCreate { get; set; }
        public Guid AdminId { get; set; }

        public virtual Admin Admin { get; set; }
    }
}
