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

        public override StockItem ToDbModel(StockItemUpdateVm vm)
        {
            return vm.ToModel();
        }

        public override StockItemDto ToDto(StockItem model)
        {
            return model.ToDto();
        }
    }

}