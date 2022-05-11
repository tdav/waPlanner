﻿using Microsoft.AspNetCore.Authorization;
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
        public async Task<Answer<viSpecialization>> AddSpecialization([FromBody] viSpecialization spec)
        {
            return await service.AddSpecializationAsync(spec);
        }

        [HttpPost("change")]
        public async Task<Answer<viSpecialization>> UpdateSpecialization([FromBody] viSpecialization spec)
        {
            return await service.UpdateSpecializationAsync( spec);
        }

        [HttpPost("change/{spec}/{status}")]
        public async Task<AnswerBasic> ChangeStatus(int spec, int status)
        {
            return await service.ChangeSpecializationStatus(spec, status);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<Answer<spSpecialization[]>> GetAllSpecializations()
        {
            return await service.GetSpecializationsAsync();
        }

        [HttpGet("{spec_id}")]
        public async Task<Answer<spSpecialization>> GetSpecialization(int spec_id)
        {
            return await service.GetSpecializationByIdAsync(spec_id);
        }
    }
}
