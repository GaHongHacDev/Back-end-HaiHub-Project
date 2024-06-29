using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Requests.StaticFile
{
    public class StaticFileRequest
    {
        public IFormFile? Img { get; set; }
        public IFormFile? Video { get; set; }
    }
}
