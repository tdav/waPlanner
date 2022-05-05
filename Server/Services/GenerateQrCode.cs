using IronBarCode;

namespace waPlanner.Services
{
    public interface IGenerateQrCode
    {
        byte[] Run(string url);
    }


    public class GenerateQrCode : IGenerateQrCode
    {
        public byte[] Run(string url)
        {
            return QRCodeWriter.CreateQrCode(url, 500, QRCodeWriter.QrErrorCorrectionLevel.Medium).ToPngBinaryData();
        }
    }
}
