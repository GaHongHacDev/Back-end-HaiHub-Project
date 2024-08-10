using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Entitities
{
    public class Customer
    {
        public Guid Id {  get; set; }
        public Guid AccountId { get; set; }
        public string Phone { get; set; }
        public string FullName { get; set; }
        public DateTime? DayOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? Img { get; set; }
        public int? NumberOfReported { get; set; }

        public Account Account { get; set; }
        public virtual ICollection<Appointment> Appointments { get; set; }
        public virtual ICollection<Feedback> Feedbacks { get; set; }
        public virtual ICollection<Report> Report { get; set; }
        //public virtual ICollection<StyleHairCustomer> StyleHairCustomers { get; set; }
    }
}
