using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PlayWebApp.Services.Logistics.CustomerManagement;
using PlayWebApp.Services.Logistics.CustomerManagement.ViewModels;
using PlayWebApp.Services.Logistics.LocationMgt.ViewModels;

namespace PlayWebApp.Controllers
{
    [Route("api/v1/customers")]
    public class CustomerController : BaseController
    {
        private readonly CustomerService service;
        public CustomerController(CustomerService service)
        {
            this.service = service;
        }

        [HttpPut]
        public async Task<IActionResult> Put(CustomerUpdateVm model)
        {
            AdaptModelStateForAddressesArray(model.Addresses, nameof(model.Addresses));

            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existingItem = await service.GetById(CreateRequest(model.RefNbr));
            if (existingItem == null) return BadRequest($"Customer with ID: {model.RefNbr} does not exist");

            var item = await service.Update(model);

            await service.SaveChanges();
            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Post(CustomerUpdateVm model)
        {

            AdaptModelStateForAddressesArray(model.Addresses, nameof(model.Addresses));

            if (!ModelState.IsValid) return BadRequest(ModelState);


            var exists = await service.GetById(CreateRequest(model.RefNbr));
            if (exists != null) return BadRequest($"Customer with ID: {model.RefNbr} exists from before");

            var item = await service.Add(model);
            await service.SaveChanges();
            return CreatedAtAction(nameof(GetById), new { id = item.RefNbr }, item);
            //return Ok(item.RefNbr); // TODO: need to return URI to the newly created item
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
        [Route("{refNbr}")]
        public async Task<IActionResult> Delete(string refNbr)
        {
            if (string.IsNullOrWhiteSpace(refNbr)) return BadRequest();

            var req = CreateRequest(refNbr);
            var record = await service.GetById(req);
            if (record == null) return NotFound();

            var model = await service.Delete(req);
            await service.SaveChanges();
            return Ok(model);
        }

        private CustomerRequestDto CreateRequest(string refNbr = null)
        {
            return new CustomerRequestDto { RefNbr = refNbr };
        }

    }
}