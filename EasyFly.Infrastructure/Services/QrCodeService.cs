using EasyFly.Application.Abstractions;
using IronBarCode;
using QRCoder;

namespace EasyFly.Infrastructure.Services
{
    internal class QrCodeService : IQrCodeService
    {
        public byte[] GenerateQRCode(string inputText, int size)
        {
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(inputText, QRCodeGenerator.ECCLevel.Q))
            using (PngByteQRCode qrCode = new PngByteQRCode(qrCodeData))
            {
                byte[] qrCodeImage = qrCode.GetGraphic(size);
                return qrCodeImage;
            }
        }
    }
}
