using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Entitities
{
    public class Admin
    {
        [Key]
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public string Phone {  get; set; }
        public string FullName { get; set; }
        public string Email {  get; set; }
        public string img {  get; set; }

        public virtual Account Account { get; set; }
        public virtual ICollection<Approval> Approvals { get; set; }
    }
}
