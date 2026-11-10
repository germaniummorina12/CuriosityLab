using CodeInvention.CuriosityLab.Application.Features.Users;
using CodeInvention.DesireERP.Web.API.Controllers.Base;
using Microsoft.AspNetCore.Mvc;

namespace CodeInvention.CuriosityLab.Web.API.Controllers
{
    [Route("api/v1/users")]
    public class UsersController : BaseController
    {
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await Mediator.Send(new GetAll.Query());
            return Ok(result);
        }

        [HttpGet("age/{age}")]
        public async Task<IActionResult> GetByAge(int age)
        {
            var result = await Mediator.Send(new GetByAge.Query(age));
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await Mediator.Send(new GetById.Query(id));
            return Ok(result);
        }

        [HttpGet("average-age")]
        public async Task<IActionResult> GetAverageAge()
        {
            var result = await Mediator.Send(new GetAverageAge.Query());
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await Mediator.Send(new Delete.Command(id));
            return NoContent();
        }

        [HttpGet("name-length/{length}")]
        public async Task<IActionResult> GetByLengthName(int length)
        {
            var result = await Mediator.Send(new GetByLengthName.Query(length));
            return Ok(result);
        }

        [HttpGet("search-name-email/{input}")]
        public async Task<IActionResult> GetByNameOrEmail(string input)
        {
            var result = await Mediator.Send(new GetByNameOrEmail.Query(input));
            return Ok(result);
        }

        [HttpGet("search-email-domain/{input}")]
        public async Task<IActionResult> GetByEmailDomain(string input)
        {
            var result = await Mediator.Send(new GetByEmailDomain.Query(input));
            return Ok(result);
        }
        [HttpGet("user-roles/{id}")]
        public async Task<IActionResult> GetUserRoles(int id)
        {
            var result = await Mediator.Send(new GetUserRoles.Query(id));
            return Ok(result);
        }
    }
}
