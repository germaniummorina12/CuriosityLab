using CodeInvention.CuriosityLab.Web.API.Controllers.Base;
using Microsoft.AspNetCore.Mvc;
using CodeInvention.CuriosityLab.Application.Features.ContactTypes;

namespace CodeInvention.CuriosityLab.Web.API.Controllers
{
    [Route("api/v1/contactTypes")]
    public class ContactTypesController : BaseController
    {
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await Mediator.Send(new Get.Query());
            return Ok(result);
        }
    }
}
