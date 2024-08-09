using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Entitities
{
    public class ImageStyle
    {
        [Key]
        public Guid Id { get; set; }
        public Guid StyleHairCustomerId { get; set; }
        public string UrlImage { get; set; }
        public bool IsActive { get; set; }

        public virtual StyleHairCustomer StyleHairCustomer { get; set; }
    }
}
