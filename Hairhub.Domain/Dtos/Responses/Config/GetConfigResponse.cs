using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Responses.Config
{
    public class GetConfigResponse
    {
        public GetConfigResponse(string? packagename, decimal? packagefee,string? description , DateTime? datecreate, bool? isactive) { 
            PackageName = packagename;
            PackageFee = packagefee;
            Description = description;
            DateCreate = datecreate;
            IsActive = isactive;
        
        
        }

        public string? PackageName { get; set; }
        public decimal? PackageFee { get; set; }

        public string? Description { get; set; }
        public DateTime? DateCreate { get; set; }
        public bool? IsActive { get; set; }
    }
}
