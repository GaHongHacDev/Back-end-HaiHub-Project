using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Responses.AppointmentDetailVoucher
{
    public class GetAppointmentDetailVoucherResponse
    {
        public Guid Id { get; set; }
        public Guid? VoucherId { get; set; }
        public Guid? AppointmenId { get; set; }
        public Decimal? AppliedAmount { get; set; }
        public DateTime? AppliedDate { get; set; }
        public VoucherResponseA Voucher {  get; set; }
        public AppointmentResponseA Appointment { get; set; }
    }

    public class VoucherResponseA
    {
        public Guid Id { get; set; }
        public Guid? SalonInformationId { get; set; }
        public string? Code { get; set; }
        public string? Description { get; set; }
        public decimal? MinimumOrderAmount { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? IsSystemCreated { get; set; }
        public bool? IsActive { get; set; }
    }

    public class AppointmentResponseA
    {
        public Guid Id { get; set; }
        public DateTime? Date { get; set; }
        public Decimal? TotalPrice { get; set; }
        public Guid? CustomerId { get; set; }
        public Decimal? OriginalPrice { get; set; }
        public Decimal? DiscountedPrice { get; set; }
        public bool? IsActive { get; set; }
    }
}
