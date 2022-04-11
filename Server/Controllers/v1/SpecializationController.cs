using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;
using waPlanner.Database.Models;
using waPlanner.ModelViews;
using waPlanner.Services;


namespace waPlanner.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
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

        [HttpPost("{spec_id}")]
        public async Task UpdateSpecialization(int spec_id, [FromBody] viSpecialization spec)
        {
            await service.UpdateSpecializationAsync(spec_id, spec);
        }

        [HttpPost("change_status/{spec_id}/{status}")]
        public async Task ChangeStatus(int spec_id, int status)
        {
            await service.ChangeSpecializationStatus(spec_id, status);
        }

        [HttpGet]
        public async Task<spSpecialization[]> GetAllSpecializations()
        {
            return await service.GetSpecializationsAsync();
        }

        [HttpGet("get_spec/{spec_id}")]
        public async Task<spSpecialization> GetSpecialization(int spec_id)
        {
            return await service.GetSpecializationByIdAsync(spec_id);
        }
    }
}
