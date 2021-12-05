using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlayWebApp.Services.Database;
using PlayWebApp.Services.Database.Model;
using PlayWebApp.Services.Logistics.ViewModels;
#nullable disable

namespace PlayWebApp.Controllers
{
    [Route("api/v1/addresses")]
    [ApiController]
    public class AddressController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        private Guid UserId
        {
            get
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                try
                {
                    return Guid.Parse(userId);
                }
                catch
                {
                    return default(Guid);
                }

            }
        }

        public AddressController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;

        }

        [HttpPut]
        public async Task<IActionResult> Put(AddressUpdateVm model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var id = model.AddressCode;
            var usr = UserId.ToString();
            var existingItem = await dbContext.Addresses.FirstOrDefaultAsync(x => x.AddressCode == id && x.UserId == usr);
            if (existingItem == null) return BadRequest($"Address with ID: {id} does not exist");

            //
            // update the db model
            //
            existingItem.StreetAddress = model.StreetAddress;
            existingItem.City = model.City;
            existingItem.PostalCode = model.PostalCode;
            existingItem.Country = model.Country;

            var item = dbContext.Addresses.Update(existingItem);

            await dbContext.SaveChangesAsync();
            return Ok(item.Entity.Id);
        }

        [HttpPost]
        public async Task<IActionResult> Post(AddressUpdateVm model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (UserId == default(Guid)) return BadRequest("User not found");
            var id = model.AddressCode;
            var usr = UserId.ToString();
            var exists = await dbContext.Addresses.FirstOrDefaultAsync(x => x.AddressCode == id && x.UserId == usr);
            if (exists != null) return BadRequest($"Address with ID: {id} exists from before");

            var item = dbContext.Addresses.Add(new Address
            {
                Id = Guid.NewGuid(),
                AddressCode = id,
                StreetAddress = model.StreetAddress,
                City = model.City,
                PostalCode = model.PostalCode,
                Country = model.Country,
                UserId = usr
            });

            await dbContext.SaveChangesAsync();
            return Ok(item.Entity.Id);
        }

        [HttpGet()]
        [Route("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return BadRequest();
            var usr = UserId.ToString();
            var record = await dbContext.Addresses.FirstOrDefaultAsync(x => x.AddressCode == id && x.UserId == usr);
            if (record == null) return NotFound();

            return Ok(new AddressDto
            {
                AddressCode = record.AddressCode,
                StreetAddress = record.StreetAddress,
                City = record.City,
                PostalCode = record.PostalCode,
                Country = record.Country
            });
        }

        [HttpGet()]
        [Route("{currentRecord}/next")]
        public async Task<IActionResult> GetNextRecord(string currentRecord)
        {
            // select top 1, order by AddressCode, where AddressCode > currentRecord

            Address record;
            var usr = UserId.ToString();

            if (string.IsNullOrWhiteSpace(currentRecord))
            {
                record = await dbContext.Addresses.Where(x => x.UserId == usr).OrderBy(x => x.AddressCode).Take(1).FirstOrDefaultAsync();
            }
            else
            {
                record = await dbContext.Addresses.OrderBy(x => x.AddressCode)
                            .Where(x => x.AddressCode.CompareTo(currentRecord) > 0 && x.UserId == usr).
                            Take(1).FirstOrDefaultAsync();
            }

            if (record == null) return NotFound();

            return Ok(new AddressDto
            {
                AddressCode = record.AddressCode,
                StreetAddress = record.StreetAddress,
                City = record.City,
                PostalCode = record.PostalCode,
                Country = record.Country
            });

        }

        [HttpGet()]
        [Route("{currentRecord}/previous")]
        public async Task<IActionResult> GetPreviousRecord(string currentRecord)
        {
            // select top 1, order by AddressCode descending, where AddressCode < currentRecord

            Address record;
            var usr = UserId.ToString();

            if (string.IsNullOrWhiteSpace(currentRecord))
            {
                record = await dbContext.Addresses.Where(x => x.UserId == usr).OrderByDescending(x => x.AddressCode).Take(1).FirstOrDefaultAsync();
            }
            else
            {
                record = await dbContext.Addresses.OrderByDescending(x => x.AddressCode)
                            .Where(x => x.AddressCode.CompareTo(currentRecord) < 0 && x.UserId == usr).
                            Take(1).FirstOrDefaultAsync();
            }

            if (record == null) return NotFound();

            return Ok(new AddressDto
            {
                AddressCode = record.AddressCode,
                StreetAddress = record.StreetAddress,
                City = record.City,
                PostalCode = record.PostalCode,
                Country = record.Country
            });
        }

        [HttpGet()]
        [Route("first")]
        public async Task<IActionResult> GetFirst()
        {
            var usr = UserId.ToString();
            var record = await dbContext.Addresses.Where(x => x.UserId == usr).OrderBy(x => x.AddressCode).FirstOrDefaultAsync();
            if (record == null) return NotFound();
            return Ok(new AddressDto
            {
                AddressCode = record.AddressCode,
                StreetAddress = record.StreetAddress,
                City = record.City,
                PostalCode = record.PostalCode,
                Country = record.Country
            });
        }

        [HttpGet()]
        [Route("last")]
        public async Task<IActionResult> GetLast()
        {
            var usr = UserId.ToString();
            var record = await dbContext.Addresses.Where(x => x.UserId == usr).OrderByDescending(x => x.AddressCode).FirstOrDefaultAsync();
            if (record == null) return NotFound();
            return Ok(new AddressDto
            {
                AddressCode = record.AddressCode,
                StreetAddress = record.StreetAddress,
                City = record.City,
                PostalCode = record.PostalCode,
                Country = record.Country
            });
        }

        [HttpDelete]
        [Route("{addressCode}")]
        public async Task<IActionResult> Delete(string addressCode)
        {
            if (string.IsNullOrWhiteSpace(addressCode)) return BadRequest();

            var usr = UserId.ToString();
            var record = await dbContext.Addresses.FirstOrDefaultAsync(x => x.AddressCode == addressCode && x.UserId == usr);
            if (record == null) return NotFound();

            dbContext.Addresses.Remove(record);

            await dbContext.SaveChangesAsync();

            return Ok(new AddressDto
            {
                AddressCode = record.AddressCode,
                StreetAddress = record.StreetAddress,
                City = record.City,
                PostalCode = record.PostalCode,
                Country = record.Country
            });
        }

    }
}