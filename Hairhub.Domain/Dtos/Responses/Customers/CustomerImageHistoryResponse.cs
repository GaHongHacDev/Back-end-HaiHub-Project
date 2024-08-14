using Hairhub.Domain.Entitities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Responses.Customers
{
    public class CustomerImageHistoryResponse
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdateddAt { get; set; }

        public bool IsActive { get; set; }

        public virtual Customers Customer { get; set; }
        public virtual ICollection<ImageStyles> ImageStyles { get; set; }
    }


    public class Customers
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public string Phone { get; set; }
        public string FullName { get; set; }
        public DateTime? DayOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? Img { get; set; }
        public int? NumberOfReported { get; set; }
    }

    public class ImageStyles
    {
        public Guid Id { get; set; }
        public Guid StyleHairCustomerId { get; set; }
        public string UrlImage { get; set; }
        public bool IsActive { get; set; }
    }
}
