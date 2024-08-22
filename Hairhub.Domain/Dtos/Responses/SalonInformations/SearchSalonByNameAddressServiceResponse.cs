using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Responses.SalonInformations
{
    public class SearchSalonByNameAddressServiceResponse
    {
        public Guid Id { get; set; }
        public Guid? OwnerId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string? Description { get; set; }
        public string? Img { get; set; }
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }
        public int TotalRating { get; set; }
        public decimal Rate { get; set; }
        public int TotalReviewer { get; set; }
        public bool IsActive { get; set; }
        public List<SearchSalonServiceResponse> Services { get; set; } = new List<SearchSalonServiceResponse>();
        public List<SearchSalonVoucherRespnse> Vouchers { get; set; } = new List<SearchSalonVoucherRespnse>();
    }

    public class SearchSalonServiceResponse
    {
        public Guid Id { get; set; }
        public Guid SalonInformationId { get; set; }
        public string ServiceName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string? Img { get; set; }
        public decimal Time { get; set; }
        public bool IsActive { get; set; }
    }

    public class SearchSalonVoucherRespnse
    {
        public Guid Id { get; set; }
        public Guid SalonInformationId { get; set; }
        public string? Code { get; set; }
        public string? Description { get; set; }
        public decimal? MinimumOrderAmount { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsSystemCreated { get; set; }
        public bool IsActive { get; set; }
    }
}
