using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlayWebApp.Services.Database;
using PlayWebApp.Services.Database.Model;
using PlayWebApp.Services.Logistics.InventoryMgt;
using PlayWebApp.Services.Logistics.ViewModels;
#nullable disable

namespace PlayWebApp.Controllers
{
    [Route("api/v1/StockItems")]
    [ApiController]
    public class StockItemsController : BaseController
    {
        private readonly InventoryService service;

        public StockItemsController(InventoryService service)
        {
            this.service = service;
        }

        [HttpPut]
        public async Task<IActionResult> Put(StockItemUpdateVm model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var id = model.RefNbr;
            var existingItem = await service.GetById(new StockItemRequestDto { RefNbr = id });
            if (existingItem == null) return BadRequest($"Stock item with ID: {id} does not exist");

            var item = await service.Update(model);

            await service.SaveChanges();
            return Ok(item.RefNbr);
        }

        [HttpPost]
        public async Task<IActionResult> Post(StockItemUpdateVm model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var id = model.RefNbr;
            var exists = await service.GetById(new StockItemRequestDto { RefNbr = id });
            if (exists != null) return BadRequest($"Stock item with ID: {id} exists from before");

            var item = await service.Add(model);

            await service.SaveChanges();
            return Ok(item.RefNbr);
        }

        [HttpGet]
        [Route("paginated/{page}")]
        public async Task<IActionResult> GetAll(int page = 1)
        {
            var items = await service.GetAll(page);
            return Ok(items);
        }

        [HttpGet()]
        [Route("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return BadRequest();

            var record = await service.GetById(new StockItemRequestDto { RefNbr = id }); ;
            if (record == null) return NotFound();

            return Ok(record);
        }

        [HttpGet()]
        [Route("{currentRecord}/next")]
        public async Task<IActionResult> GetNextRecord(string currentRecord)
        {
            // select top 1, order by displayID, where displayID > currentRecord

            var record = await service.GetNext(new StockItemRequestDto { RefNbr = currentRecord });
            if (record == null) return NotFound();
            return Ok(record);

        }

        [HttpGet()]
        [Route("{currentRecord}/previous")]
        public async Task<IActionResult> GetPreviousRecord(string currentRecord)
        {
            // select top 1, order by displayID descending, where displayID < currentRecord
            var record = await service.GetPrevious(new StockItemRequestDto { RefNbr = currentRecord });
            if (record == null) return NotFound();
            return Ok(record);

        }

        [HttpGet()]
        [Route("first")]
        public async Task<IActionResult> GetFirst()
        {
            var record = await service.GetFirst();
            if (record == null) return NotFound();
            return Ok(record);
        }

        [HttpGet()]
        [Route("last")]
        public async Task<IActionResult> GetLast()
        {
            var record = await service.GetLast();
            if (record == null) return NotFound();
            return Ok(record);
        }

        [HttpDelete]
        [Route("{displayId}")]
        public async Task<IActionResult> Delete(string displayId)
        {
            if (string.IsNullOrWhiteSpace(displayId)) return BadRequest();

            var item = await service.GetById(new StockItemRequestDto { RefNbr = displayId });
            if (item == null) return NotFound();

            var del = await service.Delete(new StockItemRequestDto { RefNbr = displayId });
            await service.SaveChanges();
            return Ok(del);
        }

    }
}