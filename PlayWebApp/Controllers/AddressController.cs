using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlayWebApp.Services.Database;
using PlayWebApp.Services.Database.Model;
using PlayWebApp.Services.Logistics.ViewModels;
using PlayWebApp.Services.ModelExtentions;
#nullable disable

namespace PlayWebApp.Controllers
{
    [Route("api/v1/addresses")]
    public class AddressController : NavigationBaseController
    {
        private bool IsDefaultAddress(Address address)
        {
            return address.Id == DefaultAddressId && !string.IsNullOrWhiteSpace(DefaultAddressId);
        }

        public AddressController(ApplicationDbContext dbContext) : base(dbContext) { }

        [HttpPut]
        public async Task<IActionResult> Put(AddressUpdateVm model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var id = model.AddressCode;

            var existingItem = await GetRecord<Address>(id);
            if (existingItem == null) return BadRequest($"Address with ID: {id} does not exist");

            //
            // update the db model
            //
            existingItem.StreetAddress = model.StreetAddress;
            existingItem.City = model.City;
            existingItem.PostalCode = model.PostalCode;
            existingItem.Country = model.Country;

            var item = Update(existingItem);

            await SaveChanges();
            return Ok(item.Entity.Id);
        }

        [HttpPost]
        public async Task<IActionResult> Post(AddressUpdateVm model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (string.IsNullOrWhiteSpace(UserId)) return BadRequest("User not found");
            var id = model.AddressCode;

            var exists = await GetRecord<Address>(id);
            if (exists != null) return BadRequest($"Address with ID: {id} exists from before");

            var item = Add(new Address
            {
                Id = Guid.NewGuid().ToString(),
                Code = id,
                StreetAddress = model.StreetAddress,
                City = model.City,
                PostalCode = model.PostalCode,
                Country = model.Country,
                UserId = UserId
            });

            await SaveChanges();
            return Ok(item.Entity.Id);
        }

        [HttpGet()]
        [Route("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return BadRequest();
            if (string.IsNullOrWhiteSpace(UserId)) return BadRequest("User not found");

            var record = await GetRecord<Address>(id);
            if (record == null) return NotFound();

            var model = record.ToAddressDto();
            model.PreferredAddress = IsDefaultAddress(record);
            return Ok(model);
        }

        [HttpGet()]
        [Route("{currentRecord}/next")]
        public async Task<IActionResult> GetNextRecord(string currentRecord)
        {

            if (string.IsNullOrWhiteSpace(UserId)) return BadRequest("User not found");

            var record = await GetNextRecord<Address>(currentRecord);

            if (record == null) return NotFound();

            var model = record.ToAddressDto();
            model.PreferredAddress = IsDefaultAddress(record);
            return Ok(model);

        }

        [HttpGet()]
        [Route("{currentRecord}/previous")]
        public async Task<IActionResult> GetPreviousRecord(string currentRecord)
        {
            if (string.IsNullOrWhiteSpace(UserId)) return BadRequest("User not found");
            var record = await GetPreviousRecord<Address>(currentRecord);
            if (record == null) return NotFound();

            var model = record.ToAddressDto();
            model.PreferredAddress = IsDefaultAddress(record);
            return Ok(model);
        }

        [HttpGet()]
        [Route("first")]
        public async Task<IActionResult> GetFirst()
        {
            if (string.IsNullOrWhiteSpace(UserId)) return BadRequest("User not found");
            var record = await GetTopRecord<Address>();
            if (record == null) return NotFound();
            var model = record.ToAddressDto();
            model.PreferredAddress = IsDefaultAddress(record);
            return Ok(model);
        }

        [HttpGet()]
        [Route("last")]
        public async Task<IActionResult> GetLast()
        {
            if (string.IsNullOrWhiteSpace(UserId)) return BadRequest("User not found");
            var record = await GetLastRecord<Address>();
            if (record == null) return NotFound();
            var model = record.ToAddressDto();
            model.PreferredAddress = IsDefaultAddress(record);
            return Ok(model);
        }

        [HttpDelete]
        [Route("{addressCode}")]
        public async Task<IActionResult> Delete(string addressCode)
        {
            if (string.IsNullOrWhiteSpace(addressCode)) return BadRequest();
            if (string.IsNullOrWhiteSpace(UserId)) return BadRequest("User not found");

            var record = await GetRecord<Address>(addressCode);
            if (record == null) return NotFound();

            //
            // default address cannot be deleted
            //    
            if (IsDefaultAddress(record))
            {
                return BadRequest("Default address cannot be deleted. Please change the default address from the profile page first!");
            }

            Delete(record);

            await SaveChanges();

            var model = record.ToAddressDto();
            model.PreferredAddress = IsDefaultAddress(record);
            return Ok(model);
        }

    }
}