using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlayWebApp.Services.Database;
using PlayWebApp.Services.Database.Model;
using PlayWebApp.Services.ViewModels;
#nullable disable

namespace PlayWebApp.Controllers
{
    [Route("api/v1/bookings")]
    [ApiController]
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public BookingController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpPut]
        public async Task<IActionResult> Put(BookingUpdateVm model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var id = model.BookingNumber;
            var existingItem = await dbContext.Bookings.FirstOrDefaultAsync(x => x.BookingNumber == id);
            if (existingItem == null) return BadRequest($"Booking with ID: {id} does not exist");

            //
            // update the db model
            //
            existingItem.Description = model.Description;
            var item = dbContext.Bookings.Update(existingItem);

            await dbContext.SaveChangesAsync();
            return Ok(item.Entity.Id);
        }

        [HttpPost]
        public async Task<IActionResult> Post(BookingUpdateVm model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var id = model.BookingNumber;
            var exists = await dbContext.Bookings.FirstOrDefaultAsync(x => x.BookingNumber == id);
            if (exists != null) return BadRequest($"Booking with ID: {id} exists from before");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(!Guid.TryParse(userId, out var userGuid)) return BadRequest("User not found");
            var item = dbContext.Bookings.Add(new Booking
            {
                Id = Guid.NewGuid(),
                BookingNumber = id,
                Description = model.Description,
                UserId = userId,
                ShippingAddressId = Guid.Parse("055CC167-A9FC-4932-AC2E-46582881A740")
            });

            await dbContext.SaveChangesAsync();
            return Ok(item.Entity.Id);
        }

        [HttpGet()]
        [Route("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return BadRequest();

            var record = await dbContext.Bookings.FirstOrDefaultAsync(x => x.BookingNumber == id);
            if (record == null) return NotFound();

            return Ok(new BookingDto { BookingNumber = record.BookingNumber, Description = record.Description });
        }

        [HttpGet()]
        [Route("{currentRecord}/next")]
        public async Task<IActionResult> GetNextRecord(string currentRecord)
        {
            // select top 1, order by bookingNumber, where bookingNumber > currentRecord

            Booking nextRecord;

            if (string.IsNullOrWhiteSpace(currentRecord))
            {
                nextRecord = await dbContext.Bookings.OrderBy(x => x.BookingNumber).Take(1).FirstOrDefaultAsync();
            }
            else
            {
                nextRecord = await dbContext.Bookings.OrderBy(x => x.BookingNumber)
                            .Where(x => x.BookingNumber.CompareTo(currentRecord) > 0).
                            Take(1).FirstOrDefaultAsync();
            }

            if (nextRecord == null) return NotFound();

            return Ok(new BookingDto { BookingNumber = nextRecord.BookingNumber, Description = nextRecord.Description });

        }

        [HttpGet()]
        [Route("{currentRecord}/previous")]
        public async Task<IActionResult> GetPreviousRecord(string currentRecord)
        {
            // select top 1, order by bookingNumber descending, where bookingNumber < currentRecord

            Booking nextRecord;

            if (string.IsNullOrWhiteSpace(currentRecord))
            {
                nextRecord = await dbContext.Bookings.OrderByDescending(x => x.BookingNumber).Take(1).FirstOrDefaultAsync();
            }
            else
            {
                nextRecord = await dbContext.Bookings.OrderByDescending(x => x.BookingNumber)
                            .Where(x => x.BookingNumber.CompareTo(currentRecord) < 0).
                            Take(1).FirstOrDefaultAsync();
            }

            if (nextRecord == null) return NotFound();

            return Ok(new BookingDto { BookingNumber = nextRecord.BookingNumber, Description = nextRecord.Description });
        }

        [HttpGet()]
        [Route("first")]
        public async Task<IActionResult> GetFirst()
        {
            var first = await dbContext.Bookings.OrderBy(x => x.BookingNumber).FirstOrDefaultAsync();
            if (first == null) return NotFound();
            return Ok(new BookingDto { BookingNumber = first.BookingNumber, Description = first.Description });
        }

        [HttpGet()]
        [Route("last")]
        public async Task<IActionResult> GetLast()
        {
            var last = await dbContext.Bookings.OrderByDescending(x => x.BookingNumber).FirstOrDefaultAsync();
            if (last == null) return NotFound();
            return Ok(new BookingDto { BookingNumber = last.BookingNumber, Description = last.Description });
        }

        [HttpDelete]
        [Route("{bookingNumber}")]
        public async Task<IActionResult> Delete(string bookingNumber)
        {
            if (string.IsNullOrWhiteSpace(bookingNumber)) return BadRequest();

            var item = await dbContext.Bookings.FirstOrDefaultAsync(x => x.BookingNumber == bookingNumber);
            if (item == null) return NotFound();

            dbContext.Bookings.Remove(item);

            await dbContext.SaveChangesAsync();

            return Ok(new BookingDto { BookingNumber = item.BookingNumber, Description = item.Description });
        }

    }
}