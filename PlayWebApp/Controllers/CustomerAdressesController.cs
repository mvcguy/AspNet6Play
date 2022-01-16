using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlayWebApp.Services.Database;
using PlayWebApp.Services.Database.Model;
using PlayWebApp.Services.Logistics.CustomerManagement;
using PlayWebApp.Services.Logistics.ViewModels;
using PlayWebApp.Services.Logistics.ViewModels.Requests;
using PlayWebApp.Services.ModelExtentions;
#nullable disable

namespace PlayWebApp.Controllers
{
    [Route("api/v1/customers/address")]
    public class CustomerAdressesController : BaseController
    {
        private readonly CustomerLocationService service;
        public CustomerAdressesController(CustomerLocationService addressMgtService)
        {
            this.service = addressMgtService;
        }

        [HttpPut]
        public async Task<IActionResult> Put(AddressUpdateVm model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existingItem = await service.GetById(CreateRequest(model.RefNbr));
            if (existingItem == null) return BadRequest($"Address with ID: {model.RefNbr} does not exist");

            var item = await service.Update(model);

            await service.SaveChanges();
            return Ok(item.RefNbr); // user need to verify the address code to diff; b/w OK and Redirects
        }

        [HttpPost]
        public async Task<IActionResult> Post(AddressUpdateVm model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);


            var exists = await service.GetById(CreateRequest(model.RefNbr));
            if (exists != null) return BadRequest($"Address with ID: {model.RefNbr} exists from before");

            var item = await service.Add(model);
            await service.SaveChanges();
            return Ok(item.RefNbr); // TODO: need to return URI to the newly created item
        }

        [HttpGet]
        [Route("{customerId}/{page}")]
        public async Task<IActionResult> GetAll(string customerId, int page = 1)
        {
            if(string.IsNullOrWhiteSpace(customerId) || page<0) return BadRequest("Params CustomerID or Page are invalid");
            var items = await service.GetAllByParentId(customerId.ToString(), page);
            
            return Ok(items);
        }

        [HttpGet()]
        [Route("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return BadRequest();

            var item = await service.GetById(CreateRequest(id));
            if (item == null) return NotFound();

            return Ok(item);
        }

        [HttpGet()]
        [Route("{currentRecord}/next")]
        public async Task<IActionResult> GetNextRecord(string currentRecord)
        {
            var model = await service.GetNext(CreateRequest(currentRecord));
            if (model == null) return NotFound();
            return Ok(model);

        }

        [HttpGet()]
        [Route("{currentRecord}/previous")]
        public async Task<IActionResult> GetPreviousRecord(string currentRecord)
        {
            var model = await service.GetPrevious(CreateRequest(currentRecord));
            if (model == null) return NotFound();
            return Ok(model);
        }

        [HttpGet()]
        [Route("first")]
        public async Task<IActionResult> GetFirst()
        {
            var model = await service.GetFirst(CreateRequest());
            if (model == null) return NotFound();
            return Ok(model);
        }

        [HttpGet()]
        [Route("last")]
        public async Task<IActionResult> GetLast()
        {
            var model = await service.GetLast(CreateRequest());
            if (model == null) return NotFound();
            return Ok(model);
        }

        [HttpDelete]
        [Route("{addressCode}")]
        public async Task<IActionResult> Delete(string addressCode)
        {
            if (string.IsNullOrWhiteSpace(addressCode)) return BadRequest();

            var req = CreateRequest(addressCode);
            var record = await service.GetById(req);
            if (record == null) return NotFound();

            //
            // default address cannot be deleted
            //    
            if (record.PreferredAddress)
            {
                return BadRequest("Default address cannot be deleted. Please change the default address from the profile page first!");
            }

            var model = await service.Delete(req);
            await service.SaveChanges();
            return Ok(model);
        }

        private AddressRequestDto CreateRequest(string refNbr = null)
        {
            return new AddressRequestDto { RefNbr = refNbr };
        }

    }
}