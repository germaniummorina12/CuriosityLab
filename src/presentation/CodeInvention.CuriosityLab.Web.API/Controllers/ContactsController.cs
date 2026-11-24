using CodeInvention.CuriosityLab.Application.Features.Contacts;
using CodeInvention.CuriosityLab.Domain.Entities;
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
    }
}
