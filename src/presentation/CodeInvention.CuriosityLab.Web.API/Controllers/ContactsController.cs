using CodeInvention.CuriosityLab.Application.Features.Contacts;
using CodeInvention.CuriosityLab.Web.API.Controllers.Base;
using Microsoft.AspNetCore.Mvc;

namespace CodeInvention.CuriosityLab.Web.API.Controllers
{
    [Route("api/v1/contacts")]
    public class ContactsController : BaseController
    {

        [HttpPost("create-contact/{id}")]
        public async Task<IActionResult> Add(Add.Command command)
        {
            await Mediator.Send(command);
            return NoContent();
        }


        [HttpPut("update-contact/{id}")]
        public async Task<IActionResult> Update(Update.Command command)
        {
            await Mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("delete-contact/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await Mediator.Send(new Delete.Command(id));
            return NoContent();
        }


        [HttpGet("read-contact/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await Mediator.Send(new GetById.Query(id));
            return Ok(result);
        }

        [HttpGet("by-user/{userId}")]
        public async Task<IActionResult> GetByUser(int userId)
        {
            var result = await Mediator.Send(new GetByUser.Query(userId));
            return Ok(result);
        }

        [HttpPost("bulk")]
        public async Task<IActionResult> BulkAdd(BulkAdd.Command command)
        {
            await Mediator.Send(command);
            return NoContent();
        }

        [HttpPut("bulk")]
        public async Task<IActionResult> BulkUpdate(BulkUpdate.Command command)
        {
            await Mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("bulk")]
        public async Task<IActionResult> BulkDelete(BulkDelete.Command command)
        {
            await Mediator.Send(command);
            return NoContent();
        }

        [HttpGet("by-type/{contactTypeId}")]
        public async Task<IActionResult> GetByType(int contactTypeId)
        {
            var result = await Mediator.Send(new GetByType.Query(contactTypeId));
            return Ok(result);
        }

        [HttpGet("users-without-type/{contactTypeId}")]
        public async Task<IActionResult> GetUsersWithoutType(int contactTypeId)
        {
            var result = await Mediator.Send(new GetUsersWithoutType.Query(contactTypeId));
            return Ok(result);
        }

        [HttpGet("date-range")]
        public async Task<IActionResult> GetByDateRange(DateTime fromDate, DateTime UntilDate)
        {
            var result = await Mediator.Send(new GetByDateRange.Query(fromDate, UntilDate));
            return Ok(result);
        }


        [HttpGet("search/{value}")]
        public async Task<IActionResult> Search(string value)
        {
            var result = await Mediator.Send(new Search.Query(value));
            return Ok(result);
        }


        [HttpGet("grouped-by-type/{userId}")]
        public async Task<IActionResult> GetGroupedByType(int userId)
        {
            var result = await Mediator.Send(new GetGroupedByType.Query(userId));
            return Ok(result);
        }


        [HttpGet("contact-types")]
        public async Task<IActionResult> GetAll()
        {
            var result = await Mediator.Send(new GetAll.Query());
            return Ok(result);
        }
    }
}
