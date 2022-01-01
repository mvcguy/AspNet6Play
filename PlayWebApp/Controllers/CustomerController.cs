using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PlayWebApp.Services.CustomerManagement;
using PlayWebApp.Services.CustomerManagement.ViewModels;
using PlayWebApp.Services.Logistics.ViewModels;

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

        private List<string> GetModelKeys(string prefix)
        {
            return new List<string>
                            {
                                $"{prefix}.{nameof(AddressUpdateVm.StreetAddress)}",
                                $"{prefix}.{nameof(AddressUpdateVm.PostalCode)}",
                                $"{prefix}.{nameof(AddressUpdateVm.City)}",
                                $"{prefix}.{nameof(AddressUpdateVm.Country)}",
                            };
        }

        private void RemoveErrorsFromModelState(List<string> keys)
        {
            if (ModelState.IsValid) return;
            foreach (var key in keys)
            {
                if (ModelState.Keys.Contains(key))
                    ModelState.Remove(key);
            }

        }

        [HttpPut]
        public async Task<IActionResult> Put(CustomerUpdateVm model)
        {
            AdaptModelStateForAddressesArray(model);

            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existingItem = await service.GetById(CreateRequest(model.RefNbr));
            if (existingItem == null) return BadRequest($"Customer with ID: {model.RefNbr} does not exist");

            var item = await service.Update(model);

            await service.SaveChanges();
            return Ok(item);
        }

        private void AdaptModelStateForAddressesArray(CustomerUpdateVm model)
        {
            if (!ModelState.IsValid)
            {
                for (int i = 0; i < model.Addresses.Count; i++)
                {
                    var prefix = $"{nameof(model.Addresses)}[{i}]";
                    var row = model.Addresses[i];
                    if (row.UpdateType == UpdateType.Delete && !string.IsNullOrWhiteSpace(row.RefNbr))
                    {
                        RemoveErrorsFromModelState(GetModelKeys(prefix));
                    }
                }

                var addressErrors = ModelState.FindKeysWithPrefix("Addresses");

                if (addressErrors.Any())
                {
                    //
                    // provide a mapping of server and client array indexes
                    //
                    for (int i = 0; i < model.Addresses.Count; i++)
                    {
                        ModelState
                        .AddModelError($"{nameof(model.Addresses)}[{i}]",
                                model.Addresses[i].ClientRowNumber.ToString(CultureInfo.InvariantCulture));
                    }
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post(CustomerUpdateVm model)
        {

            AdaptModelStateForAddressesArray(model);

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