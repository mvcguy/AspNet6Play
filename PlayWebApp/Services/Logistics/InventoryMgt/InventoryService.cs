using Microsoft.EntityFrameworkCore;
using PlayWebApp.Services.Database.Model;
using PlayWebApp.Services.DataNavigation;
using PlayWebApp.Services.Logistics.InventoryMgt.Repository;
using PlayWebApp.Services.Logistics.InventoryMgt.ViewModels;
using PlayWebApp.Services.Logistics.LocationMgt.ViewModels;
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

        public async Task<StockItemPriceDto> GetItemPrice(string refNbr)
        {
            var price = await repository
                .GetCollection<StockItemPrice>(x => x.StockItem.RefNbr == refNbr)
                .OrderByDescending(x => x.ExpiresAt)
                .FirstOrDefaultAsync();

            return price.ToDto();

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
            if (record != null)
            {
                throw new Exception("Record exist from before");
            }

            record = new StockItem
            {
                RefNbr = vm.RefNbr,
                Description = vm.ItemDescription
            };
            UpdateLines(vm, record);
            var item = repository.Add(record);
            return item.Entity.ToDto();

        }

        public async override Task<StockItemDto> Update(StockItemUpdateVm model)
        {
            var record = await repository.GetById(model.RefNbr);
            if (record == null)
            {
                throw new Exception("Record cannot be found");
            }

            record.Description = model.ItemDescription;
            record.StockItemPrices = (await inventoryRepository.GetItemPrices(model.RefNbr)).ToList();
            UpdateLines(model, record);

            var res = repository.Update(record);
            return res.Entity.ToDto();
        }

        private void UpdateLines(StockItemUpdateVm vm, StockItem dbModel)
        {
            if (vm.ItemPrices == null) return;

            foreach (var lineVm in vm.ItemPrices)
            {
                StockItemPrice line = null;
                switch (lineVm.UpdateType)
                {
                    case UpdateType.New:
                        line = AddNewLine(dbModel, lineVm);
                        break;
                    case UpdateType.Update:
                        line = UpdateExistingLine(dbModel, lineVm);
                        break;
                    case UpdateType.Delete:
                        line = DeleteExistingLine(dbModel, lineVm);
                        break;
                }
            }
        }

        private StockItemPrice DeleteExistingLine(StockItem dbModel, StockItemPriceUpdateVm vm)
        {
            var line = dbModel.StockItemPrices.FirstOrDefault(x => x.RefNbr == vm.RefNbr);
            if (line != null)
            {
                dbModel.StockItemPrices.Remove(line);
            }
            return line;
        }

        private StockItemPrice UpdateExistingLine(StockItem dbModel, StockItemPriceUpdateVm vm)
        {
            var line = dbModel.StockItemPrices.FirstOrDefault(x => x.RefNbr == vm.RefNbr);
            if (line != null)
            {
                line.UnitCost = vm.UnitCost.Value;
                line.BreakQty = vm.BreakQty.Value;
                line.EffectiveFrom = vm.EffectiveFrom.Value;
                line.ExpiresAt = vm.ExpiresAt.Value;
                line.UnitOfMeasure = vm.UnitOfMeasure;

                repository.UpdateAuditData(line);
            }
            return line;
        }

        private StockItemPrice AddNewLine(StockItem dbModel, StockItemPriceUpdateVm vm)
        {
            var line = new StockItemPrice
            {
                Id = Guid.NewGuid().ToString(),
                BreakQty = vm.BreakQty.Value,
                UnitOfMeasure = vm.UnitOfMeasure,
                UnitCost = vm.UnitCost.Value,
                EffectiveFrom = vm.EffectiveFrom.Value,
                ExpiresAt = vm.ExpiresAt.Value,
                RefNbr = vm.RefNbr
            };

            repository.AddAuditData(line);
            dbModel.StockItemPrices.Add(line);
            return line;
        }


    }

}