using CodeInvention.CuriosityLab.Application.Features.Users;
using CodeInvention.CuriosityLab.Web.API.Controllers.Base;
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

        [HttpGet("search-name-email/{text}")]
        public async Task<IActionResult> GetByNameOrEmail(string text)
        {
            var result = await Mediator.Send(new GetByNameOrEmail.Query(text));
            return Ok(result);
        }

        [HttpGet("search-email-domain/{email}")]
        public async Task<IActionResult> GetByEmailDomain(string email)
        {
            var result = await Mediator.Send(new GetByEmailDomain.Query(email));
            return Ok(result);
        }

        [HttpGet("user-roles/{id}")]
        public async Task<IActionResult> GetUserRoles(int id)
        {
            var result = await Mediator.Send(new GetUserRoles.Query(id));
            return Ok(result);
        }

        [HttpPut("update-age/{id}")]
        public async Task<IActionResult> UpdateAge(UpdateAge.Command command)
        {
            await Mediator.Send(command);
            return NoContent();
        }

        [HttpPut("remove-set-roles/{userId}")]
        public async Task<IActionResult> UpdateUserSetRoles(UpdateUserSetRoles.Command command)
        {
            await Mediator.Send(command);
            return NoContent();
        }

        [HttpPut("restore-user/{id}")]
        public async Task<IActionResult> RestoreUser(RestoreUser.Command command)
        {
            await Mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("bulk-delete")]
        public async Task<IActionResult> BulkDelete(List<int> Ids)
        {
            await Mediator.Send(new BulkDelete.Command(Ids));
            return NoContent();
        }

        [HttpGet("range-age")]
        public async Task<IActionResult> GetUserAgeRange(int minAge, int maxAge)
        {
            var result = await Mediator.Send(new GetUserAgeRange.Query(minAge, maxAge));
            return Ok(result);
        }

        [HttpGet("range-date")]
        public async Task<IActionResult> GetUserDateRange(DateTime fromDate, DateTime UntilDate)
        {
            var result = await Mediator.Send(new GetUserDateRange.Query(fromDate, UntilDate));
            return Ok(result);
        }

        [HttpGet("get-user-no-roles")]
        public async Task<IActionResult> GetUserNoRole()
        {
            var result = await Mediator.Send(new GetUserNoRole.Query());
            return Ok(result);
        }

        [HttpGet("get-user-no-age")]
        public async Task<IActionResult> GetUserNoAge()
        {
            var result = await Mediator.Send(new GetUserNoAge.Query());
            return Ok(result);
        }

        [HttpGet("sort-user-by-date")]
        public async Task<IActionResult> GetUserDateSort()
        {
            var result = await Mediator.Send(new GetUserDateSort.Query());
            return Ok(result);
        }

        [HttpGet("get-user-by-name/{input}")]
        public async Task<IActionResult> GetUserByName(string input)
        {
            var result = await Mediator.Send(new GetUserByName.Query(input));
            return Ok(result);
        }

        [HttpGet("get-total-number-of-users-and-roles")]
        public async Task<IActionResult> GetTotalUserRole()
        {
            var result = await Mediator.Send(new GetTotalUserRole.Query());
            return Ok(result);
        }

        [HttpGet("user-profile/{id}")]
        public async Task<IActionResult> GetUserProfile(int id)
        {
            var result = await Mediator.Send(new GetUserProfile.Query(id));
            return Ok(result);
        }
    }
}
