using Hairhub.Domain.Dtos.Responses.Appointments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Responses.SalonInformations
{
    public class SalonInformationImagesResponse
    {       
       public Guid SalonInformationId {  get; set; } 
       public List<FileSalonResponse> SalonImages { get; set; }  
    }

    public class FileSalonResponse
    {
        public Guid Id { get; set; }
        public string? Img { get; set; }
        public string? Video { get; set; }
    }
}
