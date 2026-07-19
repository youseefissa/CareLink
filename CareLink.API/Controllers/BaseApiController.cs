using CareLink.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace CareLink.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseApiController : ControllerBase
    {
        protected IActionResult HandleResult<T>(Result<T> result)
        {
            if (result.Succeeded)
                return Ok(result.Data);

            return BadRequest(new { errors = result.Errors });
        }

        protected IActionResult HandleResult(Result result)
        {
            if (result.Succeeded)
                return Ok();

            return BadRequest(new { errors = result.Errors });
        }
    }
}