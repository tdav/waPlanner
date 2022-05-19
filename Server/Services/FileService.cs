using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;
using waPlanner.Database;
using waPlanner.Database.Models;
using waPlanner.Extensions;
using waPlanner.Interfaces;
using waPlanner.ModelViews;
using waPlanner.Utils;

namespace waPlanner.Controllers.v1
{
    public interface IFileService
    {
        ValueTask<Answer<string>> SaveFile(IFormFile fileForm, int seller);
        ValueTask<Answer<tbAnalizeResult>> SaveAnalizeResultFile(viAnalizeResultFile fileForm);
    }

    public class FileService : IFileService, IAutoRegistrationScopedLifetimeService
    {
        private readonly IHttpContextAccessorExtensions accessor;
        private readonly MyDbContext db;
        private readonly ILogger<FileService> logger;

        public FileService(IHttpContextAccessorExtensions accessor, ILogger<FileService> logger, MyDbContext db)
        {
            this.accessor = accessor;
            this.logger = logger;
            this.db = db;
        }

        public async ValueTask<Answer<tbAnalizeResult>> SaveAnalizeResultFile(viAnalizeResultFile fileForm)
        {
            try
            {
                int org_id = accessor.GetOrgId();
                int user_id = accessor.GetId();

                var path = $"{AppDomain.CurrentDomain.BaseDirectory}wwwroot{Path.DirectorySeparatorChar}store{Path.DirectorySeparatorChar}analysis{Path.DirectorySeparatorChar}{org_id}";
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);

                path += $"{Path.DirectorySeparatorChar}{fileForm.UserId}";
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);

                path += $"{Path.DirectorySeparatorChar}{DateTime.Now.Date:yyyy-MM-dd}{Path.DirectorySeparatorChar}";
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);

                var fileName = $"{fileForm.UserId}_{fileForm.StaffId}_{Guid.NewGuid()}.pdf";
                using (var ms = new MemoryStream())
                {
                    var fileUrl = $"/store/analysis/{org_id}/{fileForm.UserId}/{DateTime.Now.Date:yyyy-MM-dd}/{fileName}";

                    var analysisResult = new tbAnalizeResult
                    {
                        UserId = fileForm.UserId,
                        StaffId = fileForm.StaffId,
                        OrganizationId = org_id,
                        AdInfo = fileForm.AdInfo,
                        Url = fileUrl,
                        CreateDate = DateTime.Now,
                        CreateUser = user_id,
                        Status = 1,
                    };
                    
                    await db.AddAsync(analysisResult);
                    await db.SaveChangesAsync();

                    await fileForm.FileData.CopyToAsync(ms);
                    await File.WriteAllBytesAsync(path + fileName, ms.ToArray());
                    return new Answer<tbAnalizeResult>(true, "Downloaded", analysisResult);
                }
            }
            catch (Exception e)
            {
                logger.LogError($"FileService.SaveAnalizeResultFile Error:{e.Message} Model:{fileForm.ToJson()}");
                return new Answer<tbAnalizeResult>(false, "Ошибка программы", null);
            }
        }

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

                await File.WriteAllBytesAsync(path + fileName, ms.ToArray());

                var fileUrl = $"/store/{fileName}";
                return new Answer<string>(true, "Downloaded", fileUrl);
            }
        }
    }
}
