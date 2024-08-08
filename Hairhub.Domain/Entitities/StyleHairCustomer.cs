using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Entitities
{
    public class StyleHairCustomer
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid CustomerId { get; set; }
        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdateddAt { get; set; }

        public bool IsActive { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual ICollection<ImageStyle> ImageStyles { get; set; }
    }
}
