using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlayWebApp.Services.Database;
using PlayWebApp.Services.Database.Model;
using PlayWebApp.Services.Logistics.LocationMgt;
using PlayWebApp.Services.Logistics.ViewModels;
using PlayWebApp.Services.Logistics.ViewModels.Requests;
using PlayWebApp.Services.ModelExtentions;
#nullable disable

namespace PlayWebApp.Controllers
{
    [Route("api/v1/addresses")]
    public class AddressController : BaseController
    {
        private readonly LocationService addressMgtService;

        private string DefaultAddressId = "HOME";

        private bool IsDefaultAddress(Address address)
        {
            return address.Id == DefaultAddressId && !string.IsNullOrWhiteSpace(DefaultAddressId);
        }

        public AddressController(LocationService addressMgtService)
        {
            this.addressMgtService = addressMgtService;
        }

        [HttpPut]
        public async Task<IActionResult> Put(AddressUpdateVm model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existingItem = await addressMgtService.GetById(new AddressRequestDto { RefNbr = model.AddressCode });
            if (existingItem == null) return BadRequest($"Address with ID: {model.AddressCode} does not exist");

            var item = addressMgtService.Update(model, UserId);

            await addressMgtService.SaveChanges();
            return Ok(item.AddressCode); // user need to verify the address code to diff; b/w OK and Redirects
        }

        [HttpPost]
        public async Task<IActionResult> Post(AddressUpdateVm model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (string.IsNullOrWhiteSpace(UserId)) return BadRequest("User not found");
            

            var exists = addressMgtService.GetById(new AddressRequestDto { RefNbr = model.AddressCode });
            if (exists != null) return BadRequest($"Address with ID: {model.AddressCode} exists from before");

            var item = addressMgtService.Add(model, UserId);
            await addressMgtService.SaveChanges();
            return Ok(item.AddressCode); // TODO: need to return URI to the newly created item
        }

        [HttpGet()]
        [Route("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return BadRequest();
            if (string.IsNullOrWhiteSpace(UserId)) return BadRequest("User not found");

            var item = await addressMgtService.GetById(new AddressRequestDto { RefNbr = id });
            if (item == null) return NotFound();

            return Ok(item);
        }

        [HttpGet()]
        [Route("{currentRecord}/next")]
        public async Task<IActionResult> GetNextRecord(string currentRecord)
        {

            if (string.IsNullOrWhiteSpace(UserId)) return BadRequest("User not found");
            var model = await addressMgtService.GetNext(new AddressRequestDto{RefNbr = currentRecord});
            return Ok(model);

        }

        [HttpGet()]
        [Route("{currentRecord}/previous")]
        public async Task<IActionResult> GetPreviousRecord(string currentRecord)
        {
            if (string.IsNullOrWhiteSpace(UserId)) return BadRequest("User not found");
            var model = await addressMgtService.GetPrevious(new AddressRequestDto{RefNbr = currentRecord});
            return Ok(model);
        }

        [HttpGet()]
        [Route("first")]
        public async Task<IActionResult> GetFirst()
        {
            if (string.IsNullOrWhiteSpace(UserId)) return BadRequest("User not found");
            var model = await addressMgtService.GetFirst();
            if (model == null) return NotFound();            
            return Ok(model);
        }

        [HttpGet()]
        [Route("last")]
        public async Task<IActionResult> GetLast()
        {
            if (string.IsNullOrWhiteSpace(UserId)) return BadRequest("User not found");
            var model = await addressMgtService.GetLast();
            if (model == null) return NotFound();            
            return Ok(model);
        }

        [HttpDelete]
        [Route("{addressCode}")]
        public async Task<IActionResult> Delete(string addressCode)
        {
            if (string.IsNullOrWhiteSpace(addressCode)) return BadRequest();
            if (string.IsNullOrWhiteSpace(UserId)) return BadRequest("User not found");

            var record = await addressMgtService.GetById(new AddressRequestDto { RefNbr = addressCode });
            if (record == null) return NotFound();

            //
            // default address cannot be deleted
            //    
            if (record.PreferredAddress)
            {
                return BadRequest("Default address cannot be deleted. Please change the default address from the profile page first!");
            }

            var model = await addressMgtService.Delete(new AddressRequestDto { RefNbr = addressCode });
            await addressMgtService.SaveChanges();
            return Ok(model);
        }

    }
}