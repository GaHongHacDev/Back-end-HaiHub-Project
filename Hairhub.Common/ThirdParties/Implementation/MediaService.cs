using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Hairhub.Common.ThirdParties.Contract;
using Hairhub.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Common.ThirdParties.Implementation
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
            _cloudName = _configuration["Cloudinary:cloud_name"]!;
            _apiKey = _configuration["Cloudinary:api_key"]!;
            apiSecret = _configuration["Cloudinary:api_secret"]!;
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
                throw new Exception($"Error upload image: {result.Error.Message}");
            }

            return result.SecureUrl.ToString();
        }
        private string ExtractPublicIdFromUrl(string url, string path)
        {
            // Delete origin url
            int index = url.IndexOf(path);
            if (index != -1)
            {
                url = url.Substring(index);
            }
            else
            {
                throw new Exception("Đường dẫn tệp bị sai");
            }
            // Delete extension url
            string[] extensions = { ".bmp", ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            foreach (string extension in extensions)
                if (url.EndsWith(extension))
                {
                    url = url.Substring(0, url.Length - extension.Length);
                    break;
                }
            return url;
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
                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.Error != null)
                {
                    throw new Exception(uploadResult.Error.Message);
                }

                return uploadResult.SecureUrl.ToString();
            }
        }

        public async Task<bool> DeleteImageAsync(string urlImage, string path)
        {
            try
            {
                string publicId = ExtractPublicIdFromUrl(urlImage, path);

                var account = new Account(_cloudName, _apiKey, apiSecret);
                _cloudinary = new Cloudinary(account);

                var deleteParams = new DeletionParams(publicId);
                var result = await _cloudinary.DestroyAsync(deleteParams);

                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return true;
                }
                else
                {
                    throw new Exception($"Error deleting image: {result.Error.Message}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting image: {ex.Message}");
            }
        }
    }
}
