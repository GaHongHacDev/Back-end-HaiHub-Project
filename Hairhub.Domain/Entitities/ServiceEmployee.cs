using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Entitities
{
    public class ServiceEmployee
    {
        [Key]
        public Guid Id { get; set; }
        public Guid? ServiceHairId { get; set; }
        public Guid? SalonEmployeeId { get; set; }

        public virtual ServiceHair ServiceHair { get; set; }
        public virtual SalonEmployee SalonEmployee { get; set; }
    }
}
