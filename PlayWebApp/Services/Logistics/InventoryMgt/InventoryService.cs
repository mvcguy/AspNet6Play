using PlayWebApp.Services.Database.Model;
using PlayWebApp.Services.DataNavigation;
using PlayWebApp.Services.Logistics.InventoryMgt.Repository;
using PlayWebApp.Services.Logistics.ViewModels;
using PlayWebApp.Services.Logistics.ViewModels.Dtos;
using PlayWebApp.Services.Logistics.ViewModels.Requests;
using PlayWebApp.Services.ModelExtentions;

#nullable disable

namespace PlayWebApp.Services.Logistics.InventoryMgt
{

    public class InventoryService : NavigationService<StockItem, StockItemRequestDto, StockItemUpdateVm, StockItemDto>
    {
        public InventoryService(INavigationRepository<StockItem> repository) : base(repository)
        {
        }

        public async override Task<StockItemDto> Add(StockItemUpdateVm vm)
        {


            // TODO: do not allow to add two lines with same CODE

            var record = await repository.GetById(vm.ItemDisplayId);
            if (record == null)
            {
                record = new StockItem
                {
                    Code = vm.ItemDisplayId,
                    Description = vm.ItemDescription,
                    StockItemPrices = vm.ItemPrices?.Select(x => new StockItemPrice
                    {
                        BreakQty = x.BreakQty,
                        Code = x.Code,
                        EffectiveFrom = x.EffectiveFrom,
                        ExpiresAt = x.ExpiresAt,
                        UnitCost = x.UnitCost,
                        UnitOfMeasure = x.UnitOfMeasure,
                    }).ToList(),
                };
                var item = repository.Add(record);
                return item.Entity.ToDto();
            }

            throw new Exception("Record exist from before");

        }


        public override StockItemDto ToDto(StockItem model)
        {
            return model.ToDto();
        }

        public async override Task<StockItemDto> Update(StockItemUpdateVm model)
        {
            var record = await repository.GetById(model.ItemDisplayId);
            if (record != null)
            {
                record.Description = model.ItemDescription;

                foreach (var item in model.ItemPrices)
                {
                    var line = record.StockItemPrices.FirstOrDefault(x => x.Code == item.Code);
                    if (line == null) continue;

                    line.BreakQty = item.BreakQty;
                    line.EffectiveFrom = item.EffectiveFrom;
                    line.ExpiresAt = item.ExpiresAt;
                    line.UnitCost = item.UnitCost;
                    line.UnitOfMeasure = item.UnitOfMeasure;
                }
                var res = repository.Update(record);
                return res.Entity.ToDto();
            }

            throw new Exception("Record cannot be found");
        }
    }

}