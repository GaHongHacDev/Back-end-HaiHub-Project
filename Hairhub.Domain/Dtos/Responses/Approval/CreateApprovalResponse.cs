using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Responses.Approval
{
    public class CreateApprovalResponse
    {
        public Guid Id { get; set; }
        public Guid SalonInformationId { get; set; }
        public Guid AdminId { get; set; }
        public string? ReasonReject { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
