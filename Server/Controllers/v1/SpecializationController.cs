using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;
using waPlanner.Database.Models;
using waPlanner.ModelViews;
using waPlanner.Services;


namespace waPlanner.Controllers.v1
{
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("[controller]")]
    [SwaggerTag("Категория")]
    public class SpecializationController
    {
        private readonly ISpecializationService service;
        public SpecializationController(ISpecializationService service)
        {
            this.service = service;
        }

        [HttpPost]
        public async Task AddSpecialization([FromBody] viSpecialization spec)
        {
            await service.AddSpecializationAsync(spec);
        }

        [HttpPost("change")]
        public async Task UpdateSpecialization([FromBody] viSpecialization spec)
        {
            await service.UpdateSpecializationAsync( spec);
        }

        [HttpPost("change_status/{status}")]
        public async Task ChangeStatus([FromBody] viSpecialization spec, int status)
        {
            await service.ChangeSpecializationStatus(spec, status);
        }

        [HttpGet]
        public async Task<spSpecialization[]> GetAllSpecializations()
        {
            return await service.GetSpecializationsAsync();
        }

        [HttpGet("get/{spec_id}")]
        public async Task<spSpecialization> GetSpecialization(int spec_id)
        {
            return await service.GetSpecializationByIdAsync(spec_id);
        }
    }
}
