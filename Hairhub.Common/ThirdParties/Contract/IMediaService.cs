using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Common.ThirdParties.Contract
{
    public interface IMediaService
    {
        Task<string> UploadAnImage(IFormFile file, string pathFolder, string nameOfImg);
        Task<string> UploadAVideo(IFormFile file, string pathFolder, string nameOfImg);
        Task<bool> DeleteImageAsync(string urlImage, string path);
    }
}
