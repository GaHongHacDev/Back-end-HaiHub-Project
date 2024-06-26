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
using Hairhub.Common.Security;

namespace Hairhub.Common.CommonService.Implementation
{
    public class QRCodeService : IQRCodeService
    {
        private readonly IMediaService _mediaService;

        public QRCodeService(IMediaService media)
        {
            _mediaService = media;
        }

        public async Task<string> DecodeQR(string hashedAppointmentData)
        {
            return String.IsNullOrEmpty(hashedAppointmentData) == true ? null : hashedAppointmentData;
        }


        public async Task<string> GenerateQR(Guid AppointmentId)
        {
            string qrAppointment = $"{AppointmentId}";
            //string dataEncrypt = AesEncoding.EncryptAES(qrAppointment);
            //string decryptedQrAppointment = AesEncoding.DecryptAES(dataEncrypt);
            string pathName = MediaPath.QR_APPOINTMENT;
            IFormFile qr = GenerateQRCodeImage(qrAppointment);
            var url = await _mediaService.UploadAnImage(qr, pathName, qrAppointment);
            return url;
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
