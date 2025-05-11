namespace EasyFly.Application.Abstractions
{
    public interface IQrCodeService
    {
        public byte[] GenerateQRCode(string inputText, int size);
    }
}
