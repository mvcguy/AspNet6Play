using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlayWebApp.Services.Database;
using PlayWebApp.Services.Database.Model;
using PlayWebApp.Services.Logistics.InventoryMgt;
using PlayWebApp.Services.Logistics.InventoryMgt.ViewModels;
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
            AdaptModelStateForAddressesArray(model.ItemPrices, nameof(model.ItemPrices));
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var id = model.RefNbr;
            var existingItem = await service.GetById(new StockItemRequestDto { RefNbr = id });
            if (existingItem == null) return BadRequest($"Stock item with ID: {id} does not exist");

            var item = await service.Update(model);

            await service.SaveChanges();
            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Post(StockItemUpdateVm model)
        {
            AdaptModelStateForAddressesArray(model.ItemPrices, nameof(model.ItemPrices));

            if (!ModelState.IsValid) return BadRequest(ModelState);

            var id = model.RefNbr;
            var exists = await service.GetById(new StockItemRequestDto { RefNbr = id });
            if (exists != null) return BadRequest($"Stock item with ID: {id} exists from before");

            var item = await service.Add(model);

            await service.SaveChanges();
            return Ok(item);
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
        [Route("{refNbr}")]
        public async Task<IActionResult> Delete(string refNbr)
        {
            if (string.IsNullOrWhiteSpace(refNbr)) return BadRequest();

            var item = await service.GetById(new StockItemRequestDto { RefNbr = refNbr });
            if (item == null) return NotFound();

            var del = await service.Delete(new StockItemRequestDto { RefNbr = refNbr });
            await service.SaveChanges();
            return Ok(del);
        }

        #region Item prices

        [HttpGet]
        [Route("prices/{refNbr}/{page}")]
        public async Task<IActionResult> GetPricesByStockItem(string refNbr, int page = 1)
        {
            var result = await service.GetItemPrices(refNbr, page);
            if (result == null) return NotFound();

            return Ok(result);
        }

        #endregion

    }
}