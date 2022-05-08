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
    [Route("[controller]")]
    [SwaggerTag("Категория")]
    public class SpecializationController
    {
        private readonly ISpecializationService service;
        public SpecializationController(ISpecializationService service)
        {
            this.service = service;
        }

        [Authorize]
        [HttpPost]
        public async Task AddSpecialization([FromBody] viSpecialization spec)
        {
            await service.AddSpecializationAsync(spec);
        }

        [Authorize]
        [HttpPost("change")]
        public async Task UpdateSpecialization([FromBody] viSpecialization spec)
        {
            await service.UpdateSpecializationAsync( spec);
        }

        [Authorize]
        [HttpPost("change/{spec}/{status}")]
        public async Task ChangeStatus(int spec, int status)
        {
            await service.ChangeSpecializationStatus(spec, status);
        }

        [HttpGet]
        public async Task<Answer<spSpecialization[]>> GetAllSpecializations()
        {
            return await service.GetSpecializationsAsync();
        }

        [Authorize]
        [HttpGet("{spec_id}")]
        public async Task<Answer<spSpecialization>> GetSpecialization(int spec_id)
        {
            return await service.GetSpecializationByIdAsync(spec_id);
        }
    }
}
