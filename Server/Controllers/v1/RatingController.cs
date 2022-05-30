using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;
using waPlanner.ModelViews;
using waPlanner.Services;


namespace waPlanner.Controllers.v1
{
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("[controller]")]
    [SwaggerTag("Рейтинг")]

    public class RatingController: ControllerBase
    {
        private readonly IRatingService service;

        public RatingController(IRatingService service)
        {
            this.service = service;
        }

        [HttpPost("set_staff_rating")]
        public ValueTask<AnswerBasic> SetStaffRating(viRating rating)
        {
            return service.AddStaffRatingAsync(rating);
        }

        [HttpPost("set_organization_rating")]
        public ValueTask<AnswerBasic> SetOrganizationRating(viRating rating)
        {
            return service.AddOrganizationRatingAsync(rating);
        }

        [HttpGet("get_staff_rating/{staff_id}")]
        public async ValueTask<Answer<viRating[]>> GetStaffRating(int staff_id)
        {
            return await service.GetStaffRating(staff_id);
        }

        [HttpGet("get_organization_rating/{organization_id}")]
        public async ValueTask<Answer<viRating[]>> GetOrganizationRating(int organization_id)
        {
            return await service.GetOrganizationRating(organization_id);
        }
    }
}
