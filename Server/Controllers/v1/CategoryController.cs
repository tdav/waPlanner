﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Threading.Tasks;
using waPlanner.ModelViews;
using waPlanner.Services;

namespace waPlanner.Controllers.v1
{
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("[controller]")]
    [SwaggerTag("Категория")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService service;

        public CategoryController(ICategoryService service)
        {
            this.service = service;
        }

        [HttpGet("{category_id}")]
        public async Task<Answer<viCategory>> Get(int category_id)
        {
            return await service.GetCategoryByIdAsync(category_id);
        }

        [AllowAnonymous]
        [HttpGet("all_categories")]
        public async Task<Answer<List<viCategory>>> GetAll()
        {

            return await service.GetAllCategoriesAsync();
        }

        [HttpPost]
        public async Task<Answer<int>> Insert([FromBody] viCategory value)
        {
            return await service.AddCategoryAsync(value);
        }

        [HttpPost("change")]
        public async Task<AnswerBasic> Update([FromBody] viCategory value)
        {
            return await service.UpdateAsync(value);
        }

        [HttpPost("change_status")]
        public async Task<AnswerBasic> ChangeCategoryStatus(viSetStatus status)
        {
            return await service.ChangeCategoryStatus(status);
        }

        [AllowAnonymous]
        [HttpGet("search/{name}")]
        public async Task<Answer<List<viCategory>>> SearchCategory(string name)
        {
            return await service.SearchCategory(name);
        }
    }
}