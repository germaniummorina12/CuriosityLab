using CodeInvention.CuriosityLab.Application.Features.Addresses;
using CodeInvention.DesireERP.Web.API.Controllers.Base;
using Microsoft.AspNetCore.Mvc;

namespace CodeInvention.CuriosityLab.Web.API.Controllers
{
    [Route("api/v1/addresses")]
    public class AddressesController : BaseController
    {

        [HttpPost("{id}")]
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


        [HttpGet("{id}")]
        public async Task<IActionResult> GetAllById(int id)
        {
            var result = await Mediator.Send(new GetAllById.Query(id));
            return Ok(result);
        }

        [HttpGet("get-addresses-by-postalCode/{postalCode}")]
        public async Task<IActionResult> GetAddressByPostalCode(string postalCode)
        {
            var result = await Mediator.Send(new GetAddressByPostalCode.Query(postalCode));
            return Ok(result);
        }

        [HttpGet("get-users-per-city")]
        public async Task<IActionResult> GetUserByCity()
        {
            var result = await Mediator.Send(new GetUserByCity.Query());
            return Ok(result);
        }

        [HttpGet("get-user-addresses")]
        public async Task<IActionResult> GetAddressesByUsers()
        {
            var result = await Mediator.Send(new GetAddressesByUsers.Query());
            return Ok(result);
        }

        [HttpGet("get-user-no-Address")]
        public async Task<IActionResult> GetUserNoAddress()
        {
            var result = await Mediator.Send(new GetUserNoAddress.Query());
            return Ok(result);
        }

        [HttpGet("get-user-with-Addresses")]
        public async Task<IActionResult> GetUserAddresses()
        {
            var result = await Mediator.Send(new GetUserAddresses.Query());
            return Ok(result);
        }


        [HttpGet("Search-street/{street}")]
        public async Task<IActionResult> GetByStreet(string street)
        {
            var result = await Mediator.Send(new GetByStreet.Query(street));
            return Ok(result);
        }

        [HttpGet("Search-city/{city}")]
        public async Task<IActionResult> GetByCity(string city)
        {
            var result = await Mediator.Send(new GetByCity.Query(city));
            return Ok(result);
        }
        [HttpGet("Common-city")]
        public async Task<IActionResult> GetCommonCity()
        {
            var result = await Mediator.Send(new GetCommonCity.Query());
            return Ok(result);
        }

        [HttpGet("range-date")]
        public async Task<IActionResult> GetDateRange(DateTime fromDate, DateTime UntilDate)
        {
            var result = await Mediator.Send(new GetDateRange.Query(fromDate, UntilDate));
            return Ok(result);
        }

        [HttpGet("Get-user-specific-city/{city}")]
        public async Task<IActionResult> GetSpecificCity(string city)
        {
            var result = await Mediator.Send(new GetSpecificCity.Query(city));
            return Ok(result);
        }
    }
}