using IronBarCode;

namespace waPlanner.Services
{
    public interface IGenerateQrCodeService
    {
        byte[] Run(string url);
    }


    public class GenerateQrCodeService : IGenerateQrCodeService
    {
        public byte[] Run(string url)
        {
            return QRCodeWriter.CreateQrCode(url, 500, QRCodeWriter.QrErrorCorrectionLevel.Medium).ToPngBinaryData();
        }
    }
}
