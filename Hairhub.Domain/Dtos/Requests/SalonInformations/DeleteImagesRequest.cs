using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Requests.SalonInformations
{
    public class DeleteImagesRequest
    {
        public List<Guid>? ImagesId { get; set; }
    }
}
