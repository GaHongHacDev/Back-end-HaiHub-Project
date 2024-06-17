using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Hairhub.Domain.Exceptions;
using Hairhub.Service.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Service.Services.Services
{
    public class MediaService : IMediaService
    {
        private Cloudinary _cloudinary;
        private string _cloudName;
        private string _apiKey;
        private string apiSecret;
        private readonly IConfiguration _configuration;

        public MediaService(IConfiguration configuration)
        {
            _configuration = configuration;
            _cloudName = _configuration["Cloudinary:cloud_name"];
            _apiKey = _configuration["Cloudinary:api_key"];
            apiSecret = _configuration["Cloudinary:api_secret"];
        }
        public async Task<string> UploadAnImage(IFormFile file, string pathFolder, string nameOfImg)
        {
            if (file == null || file.Length == 0)
            {
                throw new NotFoundException("No file uploaded.");
            }

            if (!file.ContentType.ToLower().StartsWith("image/"))
            {
                throw new Exception("File is not a image!");
            }
            var account = new Account(_cloudName, _apiKey, apiSecret);
            _cloudinary = new Cloudinary(account);

            var uploadParameters = new ImageUploadParams();

            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                uploadParameters.File = new FileDescription(file.FileName, new MemoryStream(memoryStream.ToArray()));
            }

            uploadParameters.Folder = pathFolder;
            uploadParameters.PublicId = nameOfImg;
            var result = await _cloudinary.UploadAsync(uploadParameters);

            if (result.Error != null)
            {
                throw new Exception(result.Error.Message);
            }

            return result.SecureUrl.ToString();
        }

        public async Task<string> UploadAVideo(IFormFile file, string pathFolder, string nameOfImg)
        {
            
            if (file == null || file.Length == 0)
            {
                throw new NotFoundException("No file uploaded!");
            }

            if (!file.ContentType.ToLower().StartsWith("video/"))
            {
                throw new NotFoundException("File is not a video!");
            }

            var account = new CloudinaryDotNet.Account(_cloudName, _apiKey, apiSecret);
            _cloudinary = new Cloudinary(account);

            var uploadParams = new VideoUploadParams();
            uploadParams.Folder = pathFolder;
            uploadParams.PublicId = nameOfImg;
            using (var stream = file.OpenReadStream())
            {
                uploadParams.File = new FileDescription(file.FileName, stream);
                var uploadResult = _cloudinary.Upload(uploadParams);

                if (uploadResult.Error != null)
                {
                    throw new Exception(uploadResult.Error.Message);
                }

                return uploadResult.SecureUrl.ToString();
            }
        }
    }
}
