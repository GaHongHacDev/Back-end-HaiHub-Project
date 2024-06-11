using Hairhub.Domain.Entitities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Requests.Payment
{
    public class CreatePaymentRequest
    {
        public Guid ConfigId { get; set; }
        public Guid SalonOWnerID { get; set; }
        public string? Description { get; set; }



        public Config Configs { get; set; }

        public SalonOwners SalonOwner { get; set; }
    }

    public class Config
    {
        public Guid Id { get; set; }

        public string PackageName { get; set; }
        public decimal PackageFee { get; set; }
    }

    public class SalonOwners
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
    }
}
