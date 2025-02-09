using EasyFly.Application.Abstractions;
using IronBarCode;

namespace EasyFly.Infrastructure.Services
{
    internal class QrCodeService : IQrCodeService
    {
        public byte[] GenerateQRCode(string inputText, int size)
        {
            var temp = QRCodeWriter
                .CreateQrCode(inputText, size, QRCodeWriter.QrErrorCorrectionLevel.Medium)
                .ToJpegBinaryData();

            return temp;
        }
    }
}
