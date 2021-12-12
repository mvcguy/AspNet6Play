using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlayWebApp.Services.Database;
using PlayWebApp.Services.Database.Model;
using PlayWebApp.Services.Logistics.ViewModels;
using PlayWebApp.Services.ModelExtentions;
#nullable disable

namespace PlayWebApp.Controllers
{

    [Route("api/v1/bookings")]
    public class BookingController : NavigationBaseController
    {
        public BookingController(ApplicationDbContext dbContext) : base(dbContext)
        {
            
        }

        [HttpPut]
        public async Task<IActionResult> Put(BookingUpdateVm model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (string.IsNullOrWhiteSpace(UserId)) return BadRequest("User not found");

            var id = model.BookingNumber;
            var existingItem = await GetRecord<Booking>(id);
            if (existingItem == null) return BadRequest($"Booking with ID: {id} does not exist");

            //
            // update the db model
            //
            existingItem.Description = model.Description;
            var item = Update(existingItem);

            await SaveChanges();
            return Ok(item.Entity.Id);
        }

        [HttpPost]
        public async Task<IActionResult> Post(BookingUpdateVm model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (string.IsNullOrWhiteSpace(UserId)) return BadRequest("User not found");

            var id = model.BookingNumber;
            var exists = await GetRecord<Booking>(id);
            if (exists != null) return BadRequest($"Booking with ID: {id} exists from before");

            
            if (!Guid.TryParse(UserId, out var userGuid)) return BadRequest("User not found");
            var item = Add(new Booking
            {
                Id = Guid.NewGuid().ToString(),
                Code = id,
                Description = model.Description,
                UserId = UserId,
                ShippingAddressId = DefaultAddressId
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

            var record = await GetRecord<Booking>(id);
            if (record == null) return NotFound();

            return Ok(record.ToDto());
        }

        [HttpGet()]
        [Route("{currentRecord}/next")]
        public async Task<IActionResult> GetNextRecord(string currentRecord)
        {

            if (string.IsNullOrWhiteSpace(UserId)) return BadRequest("User not found");
            var record = await GetNextRecord<Booking>(currentRecord);
            if (record == null) return NotFound();
            return Ok(record.ToDto());

        }

        [HttpGet()]
        [Route("{currentRecord}/previous")]
        public async Task<IActionResult> GetPreviousRecord(string currentRecord)
        {
            if (string.IsNullOrWhiteSpace(UserId)) return BadRequest("User not found");
            
            var record = await GetPreviousRecord<Booking>(currentRecord);
            if (record == null) return NotFound();

            return Ok(record.ToDto());
        }

        [HttpGet()]
        [Route("first")]
        public async Task<IActionResult> GetFirst()
        {
            if (string.IsNullOrWhiteSpace(UserId)) return BadRequest("User not found");
            var record = await GetTopRecord<Booking>();
            if (record == null) return NotFound();
            return Ok(record.ToDto());
        }

        [HttpGet()]
        [Route("last")]
        public async Task<IActionResult> GetLast()
        {
            if (string.IsNullOrWhiteSpace(UserId)) return BadRequest("User not found");
            var record = await GetLastRecord<Booking>();
            if (record == null) return NotFound();
            return Ok(record.ToDto());
        }

        [HttpDelete]
        [Route("{bookingNumber}")]
        public async Task<IActionResult> Delete(string bookingNumber)
        {
            if (string.IsNullOrWhiteSpace(UserId)) return BadRequest("User not found");
            if (string.IsNullOrWhiteSpace(bookingNumber)) return BadRequest();

            var record = await GetRecord<Booking>(bookingNumber);
            if (record == null) return NotFound();

            Delete(record);
            await SaveChanges();

            return Ok(record.ToDto());
        }


    }
}