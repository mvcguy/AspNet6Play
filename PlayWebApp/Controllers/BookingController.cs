using Microsoft.AspNetCore.Mvc;
using PlayWebApp.Services.Logistics.BookingMgt;
using PlayWebApp.Services.Logistics.ViewModels;
using PlayWebApp.Services.Logistics.ViewModels.Requests;
#nullable disable

namespace PlayWebApp.Controllers
{

    [Route("api/v1/bookings")]
    public class BookingController : BaseController
    {
        private readonly BookingService bookingService;

        public BookingController(BookingService bookingService)
        {
            this.bookingService = bookingService;
        }

        [HttpPut]
        public async Task<IActionResult> Put(BookingUpdateVm model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var id = model.BookingNumber;
            var existingItem = await bookingService.GetById(new BookingRequestDto { RefNbr = id });
            if (existingItem == null) return BadRequest($"Booking with ID: {id} does not exist");

            var item = await bookingService.Update(model);

            await bookingService.SaveChanges();
            return Ok(item.BookingNumber);
        }

        [HttpPost]
        public async Task<IActionResult> Post(BookingUpdateVm model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var id = model.BookingNumber;
            var exists = await bookingService.GetById(new BookingRequestDto { RefNbr = id });
            if (exists != null) return BadRequest($"Booking with ID: {id} exists from before");
            
            var item = await bookingService.Add(model);
            await bookingService.SaveChanges();
            return Ok(item.BookingNumber); // TODO: return full URi
        }

        [HttpGet()]
        [Route("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return BadRequest();

            var record = await bookingService.GetById(new BookingRequestDto { RefNbr = id });
            if (record == null) return NotFound();

            return Ok(record);
        }

        [HttpGet()]
        [Route("{currentRecord}/next")]
        public async Task<IActionResult> GetNextRecord(string currentRecord)
        {
            var record = await bookingService.GetNext(new BookingRequestDto { RefNbr = currentRecord });
            if (record == null) return NotFound();
            return Ok(record);
        }

        [HttpGet()]
        [Route("{currentRecord}/previous")]
        public async Task<IActionResult> GetPreviousRecord(string currentRecord)
        {
            var record = await bookingService.GetPrevious(new BookingRequestDto { RefNbr = currentRecord });
            if (record == null) return NotFound();
            return Ok(record);
        }

        [HttpGet()]
        [Route("first")]
        public async Task<IActionResult> GetFirst()
        {
            var record = await bookingService.GetFirst();
            if (record == null) return NotFound();
            return Ok(record);
        }

        [HttpGet()]
        [Route("last")]
        public async Task<IActionResult> GetLast()
        {
            var record = await bookingService.GetLast();
            if (record == null) return NotFound();
            return Ok(record);
        }

        [HttpDelete]
        [Route("{bookingNumber}")]
        public async Task<IActionResult> Delete(string bookingNumber)
        {
            if (string.IsNullOrWhiteSpace(bookingNumber)) return BadRequest();

            var record = await bookingService.GetById(new BookingRequestDto { RefNbr = bookingNumber });
            if (record == null) return NotFound();

            var model = await bookingService.Delete(new BookingRequestDto { RefNbr = bookingNumber });
            await bookingService.SaveChanges();

            return Ok(model);
        }


    }
}