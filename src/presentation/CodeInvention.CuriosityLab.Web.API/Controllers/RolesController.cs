using CodeInvention.CuriosityLab.Application.Features.Roles;
using CodeInvention.DesireERP.Web.API.Controllers.Base;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CodeInvention.CuriosityLab.Web.API.Controllers
{
        [Route("api/v1/roles")]
        public class RolesController : BaseController
        {
            [HttpGet]
            public async Task<IActionResult> Get()
            {
                var result = await Mediator.Send(new Get.Query());
                return Ok(result);
            }

            [HttpGet("{id}")]
            public async Task<IActionResult> GetById(int id)
            {
                var result = await Mediator.Send(new GetById.Query(id));
                return Ok(result);
            }

            [HttpPost]
            public async Task<IActionResult> Add(Add.Command command)
            {
                await Mediator.Send(command);
                return NoContent();
            }

            [HttpPut("{id}")]
            public async Task<IActionResult> Update(Update.Command command)
            {
                await Mediator.Send(command);
                return NoContent();
            }

            [HttpDelete("{id}")]
            public async Task<IActionResult> Delete(int id)
            {
                await Mediator.Send(new Delete.Command(id));
                return NoContent();
            }

            [HttpGet("role-user/{id}")]
            public async Task<IActionResult> GetRoleUser(int id)
            {
                var result = await Mediator.Send(new GetRoleUser.Query(id));
                return Ok(result);
            }

            [HttpGet("count-users-roles")]
            public async Task<IActionResult> GetCountUser()
            {
                var result = await Mediator.Send(new GetCountUser.Query());
                return Ok(result);
            }

            [HttpGet("roles-no-users")]
            public async Task<IActionResult> GetNoUser()
            {
                var result = await Mediator.Send(new GetNoUser.Query());
                return Ok(result);
            }

            [HttpGet("role-stats-age")]
            public async Task<IActionResult> GetStatsAge()
            {
                var result = await Mediator.Send(new GetStatsAge.Query());
                return Ok(result);
            }
    }
}
