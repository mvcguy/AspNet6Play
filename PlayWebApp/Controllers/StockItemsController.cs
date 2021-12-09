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
    public class StockItemsController : NavigationBaseController
    {
        
        public StockItemsController(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        [HttpPut]
        public async Task<IActionResult> Put(StockItemUpdateVm model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var id = model.ItemDisplayId;
            var existingItem = await GetRecord<StockItem>(id);
            if (existingItem == null) return BadRequest($"Stock item with ID: {id} does not exist");

            //
            // update the db model
            //
            existingItem.Description = model.ItemDescription;
            var item = Update(existingItem);

            await SaveChanges();
            return Ok(item.Entity.Id);
        }

        [HttpPost]
        public async Task<IActionResult> Post(StockItemUpdateVm model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var id = model.ItemDisplayId;
            var exists = await GetRecord<Address>(id);
            if (exists != null) return BadRequest($"Stock item with ID: {id} exists from before");

            var item = Add(new StockItem { Id = Guid.NewGuid().ToString(), Code = id, Description = model.ItemDescription });

            await SaveChanges();
            return Ok(item.Entity.Id);
        }

        [HttpGet()]
        [Route("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return BadRequest();

            var record = await GetRecord<StockItem>(id);
            if (record == null) return NotFound();

            return Ok(new StockItemDto { ItemDisplayId = record.Code, ItemDescription = record.Description });
        }

        [HttpGet()]
        [Route("{currentRecord}/next")]
        public async Task<IActionResult> GetNextRecord(string currentRecord)
        {
            // select top 1, order by displayID, where displayID > currentRecord

            var record = await GetNextRecord<StockItem>(currentRecord);

            if (record == null) return NotFound();

            return Ok(new StockItemDto { ItemDisplayId = record.Code, ItemDescription = record.Description });

        }

        [HttpGet()]
        [Route("{currentRecord}/previous")]
        public async Task<IActionResult> GetPreviousRecord(string currentRecord)
        {
            // select top 1, order by displayID descending, where displayID < currentRecord

            var record = await GetPreviousRecord<StockItem>(currentRecord);
            if (record == null) return NotFound();

            return Ok(new StockItemDto { ItemDisplayId = record.Code, ItemDescription = record.Description });
        }

        [HttpGet()]
        [Route("first")]
        public async Task<IActionResult> GetFirst()
        {
            var record = await GetTopRecord<StockItem>();
            if (record == null) return NotFound();
            return Ok(new StockItemDto { ItemDisplayId = record.Code, ItemDescription = record.Description });
        }

        [HttpGet()]
        [Route("last")]
        public async Task<IActionResult> GetLast()
        {
            var record = await GetLastRecord<StockItem>();
            if (record == null) return NotFound();
            return Ok(new StockItemDto { ItemDisplayId = record.Code, ItemDescription = record.Description });
        }

        [HttpDelete]
        [Route("{displayId}")]
        public async Task<IActionResult> Delete(string displayId)
        {
            if (string.IsNullOrWhiteSpace(displayId)) return BadRequest();

            var item = await GetRecord<StockItem>(displayId);
            if (item == null) return NotFound();

            Delete(item);

            await SaveChanges();

            return Ok(new StockItemDto { ItemDisplayId = item.Code, ItemDescription = item.Description });
        }

    }
}