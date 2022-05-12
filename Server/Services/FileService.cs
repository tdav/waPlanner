using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;
using waPlanner.Interfaces;
using waPlanner.ModelViews;
using waPlanner.Utils;

namespace waPlanner.Controllers.v1
{
    [DInjectionAttribute]
    public interface IFileService
    {
        ValueTask<Answer<string>> SaveFile(IFormFile fileForm, int seller);
    }
        
    public class FileService : IFileService, IAutoRegistrationScopedLifetimeService
    {
        public async ValueTask<Answer<string>> SaveFile(IFormFile fileForm, int seller)
        {
            if (fileForm == null || fileForm.FileName == "") return new Answer<string>(false, "Юборилган файл келмади", null);
            if (fileForm.Length > 5_000_000) return new Answer<string>(false, "Файл хажми 5MB-дан катта булмасин...", null);

            var path = $"{AppDomain.CurrentDomain.BaseDirectory}wwwroot{Path.DirectorySeparatorChar}store{Path.DirectorySeparatorChar}";
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            var fileName = $"{seller}_{Guid.NewGuid().ToString("N")}.jpg";

            using (var ms = new MemoryStream())
            {
                await fileForm.CopyToAsync(ms);                
               // var ba = CImage.ResizeAndSave(ms.ToArray(), 750, 1334, 70);                

                await File.WriteAllBytesAsync(path+fileName, ms.ToArray());    
                
                var fileUrl = $"/store/{fileName}";
                return new Answer<string>(true, "Downloaded", fileUrl);
            }
        }
    }
}
