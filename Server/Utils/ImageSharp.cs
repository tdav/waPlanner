using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;
using System.IO;
using System.Threading.Tasks;

namespace waPlanner.Utils
{
    public class ImageSharp
    {
        public static async Task<MemoryStream> ResizeAsync(MemoryStream stream, int newWidth= 240, int newHeight=200)
        {
            stream.Position = 0;
            stream.Seek(0, SeekOrigin.Begin);

            var outputStream = new MemoryStream();
            using var image = Image.Load(stream);

            var ro = new ResizeOptions { Mode = ResizeMode.BoxPad, Size = new Size(newWidth, newHeight) };

            image.Mutate(i => i.Resize(ro));

            var pe = new WebpEncoder
            {
                EntropyPasses = 1,
                FileFormat = WebpFileFormatType.Lossy,
                Method = WebpEncodingMethod.BestQuality,
                NearLossless = false,
                NearLosslessQuality = 50,
                Quality = 50,
                SpatialNoiseShaping = 30,
                UseAlphaCompression = false
            };

            await image.SaveAsWebpAsync(outputStream, pe);

            outputStream.Seek(0, SeekOrigin.Begin);
            return outputStream;
        }        
    }
}
