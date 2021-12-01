using Microsoft.AspNetCore.Mvc;
using PlayWebApp.Services.Database;
using PlayWebApp.Services.Logistics.Model;
using PlayWebApp.Services.ViewModels;
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

        [HttpPost]
        public async Task<IActionResult> Post(StockItemUpdateVm model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var id = model.ItemDisplayId;
            var exists = dbContext.StockItems.FirstOrDefault(x => x.DisplayId == id);
            if (exists != null) return BadRequest($"Stock item with ID: {id} exists from before");

            var item = dbContext.StockItems.Add(new StockItem { Id = Guid.NewGuid(), DisplayId = id, Description = model.ItemDescription });

            await dbContext.SaveChangesAsync();
            return Ok(item.Entity.Id);
        }
    }
}