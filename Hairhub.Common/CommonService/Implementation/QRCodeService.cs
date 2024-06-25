using Hairhub.Domain.Enums;
using IronBarCode;
using System;
using System.IO;
using System.Threading.Tasks;
using Hairhub.Common.ThirdParties.Contract;
using Hairhub.Common.CommonService.Contract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;

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
            string pathName = MediaPath.QR_APPOINTMENT;
            IFormFile qr = GenerateQRCodeImage(qrAppointment);
            var url = await _mediaService.UploadAnImage(qr, pathName, AppointmentId.ToString());
            if (String.IsNullOrEmpty(url))
            {
                return null;
            }
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
}
