using Hairhub.Domain.Dtos.Responses.StaticFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Responses.Feedbacks
{
    public class GetFeedbackResponse
    {
        public Guid Id { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? AppointmentDetailId { get; set; }
        public int? Rating { get; set; }
        public string? Comment { get; set; }
        public bool? IsActive { get; set; }
        public CustomerResponseF Customer { get; set; }
        public AppointmentDetailResponseF AppointmentDetail { get; set; }

        public AppointmentResponseF Appointment { get; set; }

        public StaticFileResponseF StaticFile {  get; set; }
    }

    public class StaticFileResponseF
    {
        public Guid Id { get; set; }
        public Guid? FeedbackId { get; set; }
        public Guid? ReportId { get; set; }
        public string? Img { get; set; }
        public string? Video { get; set; }
    }

    public class CustomerResponseF
    {
        public Guid Id { get; set; }
        public Guid? AccountId { get; set; }
        public DateTime? DayOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? HumanId { get; set; }
        public string? Img { get; set; }
        public string? BankAccount { get; set; }
        public string? BankName { get; set; }
    }

    public class AppointmentDetailResponseF
    {
        public Guid Id { get; set; }
        public Guid? SalonEmployeeId { get; set; }
        public Guid? ServiceHairId { get; set; }
        public Guid? AppointmentId { get; set; }
        public string? Description { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? Time { get; set; }
        public Decimal? OriginalPrice { get; set; }
        public Decimal? DiscountedPrice { get; set; }
        public bool? Status { get; set; }

    }

    public class AppointmentResponseF{
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime StartDate { get; set; }
        public Decimal TotalPrice { get; set; }
        public Decimal OriginalPrice { get; set; }
        public Decimal DiscountedPrice { get; set; }
        public bool? IsReportByCustomer { get; set; }
        public bool? IsReportBySalon { get; set; }
        public string? ReasonCancel { get; set; }
        public DateTime? CancelDate { get; set; }
        public string? QrCodeImg { get; set; }
        public string Status { get; set; }


    }
}
