using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Entitities
{
    public class Approval
    {
        public Guid Id {  get; set; }
        public Guid SalonInformationId {  get; set; }
        public Guid AdminId {  get; set; }
        public string? ReasonReject {  get; set; }
        public DateTime CreateDate { get; set; }

        public virtual SalonInformation SalonInformation { get; set; }
        public virtual Admin Admin { get; set; }
    }
}
