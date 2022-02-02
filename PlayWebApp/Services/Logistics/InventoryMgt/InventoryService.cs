using PlayWebApp.Services.Database.Model;
using PlayWebApp.Services.DataNavigation;
using PlayWebApp.Services.Logistics.InventoryMgt.Repository;
using PlayWebApp.Services.Logistics.InventoryMgt.ViewModels;
using PlayWebApp.Services.Logistics.ViewModels;
using PlayWebApp.Services.ModelExtentions;

namespace PlayWebApp.Services.Logistics.InventoryMgt
{

    public class InventoryService : NavigationService<StockItem, StockItemRequestDto, StockItemUpdateVm, StockItemDto>
    {
        private readonly InventoryRepository inventoryRepository;

        public InventoryService(InventoryRepository repository) : base(repository)
        {
            this.inventoryRepository = repository;
        }
        
        public override StockItemDto ToDto(StockItem model)
        {
            return model.ToDto();
        }

        public async Task<DtoCollection<StockItemPriceDto>> GetItemPrices(string refNbr, int page = 1)
        {
            var pagedResult = await inventoryRepository.GetItemPricesPaginated(refNbr, MaxPerPage, page);
            if (pagedResult == null) return null;

            return new DtoCollection<StockItemPriceDto>()
            {
                Items = pagedResult.Records.Select(x => x.ToDto()).ToList(),
                MetaData = new CollectionMetaData
                {
                    PageSize = pagedResult.PageSize,
                    PageIndex = pagedResult.PageIndex,
                    TotalRecords = pagedResult.TotalRecords,
                }
            };

        }

        public async override Task<StockItemDto> Add(StockItemUpdateVm vm)
        {


            // TODO: do not allow to add two lines with same CODE

            var record = await repository.GetById(vm.RefNbr);
            if (record == null)
            {
                record = new StockItem
                {
                    RefNbr = vm.RefNbr,
                    Description = vm.ItemDescription,
                    StockItemPrices = vm.ItemPrices?.Select(x => new StockItemPrice
                    {
                        BreakQty = x.BreakQty,
                        RefNbr = x.RefNbr,
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


        public async override Task<StockItemDto> Update(StockItemUpdateVm model)
        {
            var record = await repository.GetById(model.RefNbr);
            if (record != null)
            {
                record.Description = model.ItemDescription;

                foreach (var item in model.ItemPrices)
                {
                    var line = record.StockItemPrices.FirstOrDefault(x => x.RefNbr == item.RefNbr);
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