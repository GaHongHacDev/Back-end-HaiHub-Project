using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Requests.Approval
{
    public class UpdateApprovalRequest
    {
        public Guid SalonInformationId { get; set; }
        public Guid AdminId { get; set; }
        public string? ReasonReject { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string Status { get; set; }
    }
}
