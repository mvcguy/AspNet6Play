using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlayWebApp.Services.Database;
using PlayWebApp.Services.Database.Model;
using PlayWebApp.Services.Logistics.ViewModels;
#nullable disable

namespace PlayWebApp.Controllers
{
    [Route("api/v1/StockItems")]
    [ApiController]
    public class StockItemsController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public StockItemsController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpPut]
        public async Task<IActionResult> Put(StockItemUpdateVm model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var id = model.ItemDisplayId;
            var existingItem = await dbContext.StockItems.FirstOrDefaultAsync(x => x.DisplayId == id);
            if (existingItem == null) return BadRequest($"Stock item with ID: {id} does not exist");

            //
            // update the db model
            //
            existingItem.Description = model.ItemDescription;
            var item = dbContext.StockItems.Update(existingItem);

            await dbContext.SaveChangesAsync();
            return Ok(item.Entity.Id);
        }

        [HttpPost]
        public async Task<IActionResult> Post(StockItemUpdateVm model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var id = model.ItemDisplayId;
            var exists = await dbContext.StockItems.FirstOrDefaultAsync(x => x.DisplayId == id);
            if (exists != null) return BadRequest($"Stock item with ID: {id} exists from before");

            var item = dbContext.StockItems.Add(new StockItem { Id = Guid.NewGuid(), DisplayId = id, Description = model.ItemDescription });

            await dbContext.SaveChangesAsync();
            return Ok(item.Entity.Id);
        }

        [HttpGet()]
        [Route("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return BadRequest();

            var record = await dbContext.StockItems.FirstOrDefaultAsync(x => x.DisplayId == id);
            if (record == null) return NotFound();

            return Ok(new StockItemDto { ItemDisplayId = record.DisplayId, ItemDescription = record.Description });
        }

        [HttpGet()]
        [Route("{currentRecord}/next")]
        public async Task<IActionResult> GetNextRecord(string currentRecord)
        {
            // select top 1, order by displayID, where displayID > currentRecord

            StockItem nextRecord;

            if (string.IsNullOrWhiteSpace(currentRecord))
            {
                nextRecord = await dbContext.StockItems.OrderBy(x => x.DisplayId).Take(1).FirstOrDefaultAsync();
            }
            else
            {
                nextRecord = await dbContext.StockItems.OrderBy(x => x.DisplayId)
                            .Where(x => x.DisplayId.CompareTo(currentRecord) > 0).
                            Take(1).FirstOrDefaultAsync();
            }

            if (nextRecord == null) return NotFound();

            return Ok(new StockItemDto { ItemDisplayId = nextRecord.DisplayId, ItemDescription = nextRecord.Description });

        }

        [HttpGet()]
        [Route("{currentRecord}/previous")]
        public async Task<IActionResult> GetPreviousRecord(string currentRecord)
        {
            // select top 1, order by displayID descending, where displayID < currentRecord

            StockItem nextRecord;

            if (string.IsNullOrWhiteSpace(currentRecord))
            {
                nextRecord = await dbContext.StockItems.OrderByDescending(x => x.DisplayId).Take(1).FirstOrDefaultAsync();
            }
            else
            {
                nextRecord = await dbContext.StockItems.OrderByDescending(x => x.DisplayId)
                            .Where(x => x.DisplayId.CompareTo(currentRecord) < 0).
                            Take(1).FirstOrDefaultAsync();
            }

            if (nextRecord == null) return NotFound();

            return Ok(new StockItemDto { ItemDisplayId = nextRecord.DisplayId, ItemDescription = nextRecord.Description });
        }

        [HttpGet()]
        [Route("first")]
        public async Task<IActionResult> GetFirst()
        {
            var first = await dbContext.StockItems.OrderBy(x => x.DisplayId).FirstOrDefaultAsync();
            if (first == null) return NotFound();
            return Ok(new StockItemDto { ItemDisplayId = first.DisplayId, ItemDescription = first.Description });
        }

        [HttpGet()]
        [Route("last")]
        public async Task<IActionResult> GetLast()
        {
            var last = await dbContext.StockItems.OrderByDescending(x => x.DisplayId).FirstOrDefaultAsync();
            if (last == null) return NotFound();
            return Ok(new StockItemDto { ItemDisplayId = last.DisplayId, ItemDescription = last.Description });
        }

        [HttpDelete]
        [Route("{displayId}")]
        public async Task<IActionResult> Delete(string displayId)
        {
            if (string.IsNullOrWhiteSpace(displayId)) return BadRequest();

            var item = await dbContext.StockItems.FirstOrDefaultAsync(x => x.DisplayId == displayId);
            if (item == null) return NotFound();

            dbContext.StockItems.Remove(item);

            await dbContext.SaveChangesAsync();

            return Ok(new StockItemDto { ItemDisplayId = item.DisplayId, ItemDescription = item.Description });
        }

    }
}