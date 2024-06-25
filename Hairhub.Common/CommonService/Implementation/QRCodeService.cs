using IronBarCode;
using System;
using System.IO;
using System.Threading.Tasks;
using Hairhub.Common.ThirdParties.Contract;
using Hairhub.Common.CommonService.Contract;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using System.Text;
using Hairhub.Domain.Enums;

namespace Hairhub.Common.CommonService.Implementation
{
    public class QRCodeService : IQRCodeService
    {
        private readonly IMediaService _mediaService;
        private static readonly string encryptionKey = "hairhub";

        public QRCodeService(IMediaService media)
        {
            _mediaService = media;
        }

        private string EncryptAES(string plainText)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = GetAesKey(encryptionKey);
                aes.IV = new byte[16]; // Default IV to 0

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(cs))
                        {
                            sw.Write(plainText);
                        }
                    }
                    byte[] encryptedBytes = ms.ToArray();
                    return Convert.ToBase64String(encryptedBytes).Replace("+", "-").Replace("/", "_").Replace("=", "");
                }
            }
        }

        private string DecryptAES(string cipherText)
        {
            string modifiedCipherText = cipherText.Replace("-", "+").Replace("_", "/") + "==".Substring(0, (4 - cipherText.Length % 4) % 4);
            using (Aes aes = Aes.Create())
            {
                aes.Key = GetAesKey(encryptionKey);
                aes.IV = new byte[16]; // Default IV to 0

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(modifiedCipherText)))
                {
                    using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader sr = new StreamReader(cs))
                        {
                            return sr.ReadToEnd();
                        }
                    }
                }
            }
        }

        private byte[] GetAesKey(string key)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] keyBytes = Encoding.UTF8.GetBytes(key);
                byte[] hash = sha256.ComputeHash(keyBytes);
                Array.Resize(ref hash, 32); // Ensure the key is 32 bytes long
                return hash;
            }
        }

        public async Task<string> DecodeQR(string hashedAppointmentData)
        {
            return String.IsNullOrEmpty(hashedAppointmentData) == true ? null : hashedAppointmentData;
        }


        public async Task<string> GenerateQR(Guid AppointmentId)
        {
            try
            {
                string qrAppointment = $"{AppointmentId}";
                string dataEncrypt = EncryptAES(qrAppointment);
                //string decryptedQrAppointment = DecryptAES(dataEncrypt);
                string pathName = MediaPath.QR_APPOINTMENT;
                IFormFile qr = GenerateQRCodeImage(dataEncrypt);
                var url = await _mediaService.UploadAnImage(qr, pathName, dataEncrypt);
                if (String.IsNullOrEmpty(url))
                {
                    return null;
                }
                return url;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        private IFormFile GenerateQRCodeImage(string data)
        {
            GeneratedBarcode barcode = QRCodeWriter.CreateQrCode(data, 500, QRCodeWriter.QrErrorCorrectionLevel.Medium);

            // Save barcode as PNG in memory
            byte[] barcodeBytes = barcode.ToPngBinaryData();

            // Create a MemoryStream from the barcode bytes
            MemoryStream ms = new MemoryStream(barcodeBytes);

            // Create an IFormFile from the MemoryStream
            IFormFile formFile = new FormFile(ms, 0, ms.Length, "barcode", "barcode.png")
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/png"
            };

            // Set the position of the MemoryStream back to the beginning for subsequent reads
            ms.Position = 0;

            return formFile;
        }
    }

    public class FormFile : IFormFile
    {
        private readonly Stream _baseStream;

        public FormFile(Stream baseStream, long baseStreamOffset, long length, string name, string fileName)
        {
            _baseStream = baseStream;
            Length = length;
            Name = name;
            FileName = fileName;
            Headers = new HeaderDictionary();
            _baseStream.Position = baseStreamOffset;
        }

        public string ContentType { get; set; }
        public string ContentDisposition { get; set; }
        public IHeaderDictionary Headers { get; set; }
        public long Length { get; }
        public string Name { get; }
        public string FileName { get; }

        public void CopyTo(Stream target)
        {
            _baseStream.CopyTo(target);
        }

        public async Task CopyToAsync(Stream target, CancellationToken cancellationToken = default)
        {
            await _baseStream.CopyToAsync(target, cancellationToken);
        }

        public Stream OpenReadStream()
        {
            return _baseStream;
        }
    }
}
