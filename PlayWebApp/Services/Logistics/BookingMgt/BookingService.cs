using PlayWebApp.Services.Database.Model;
using PlayWebApp.Services.DataNavigation;
using PlayWebApp.Services.Logistics.BookingMgt.Repository;
using PlayWebApp.Services.Logistics.ViewModels;
using PlayWebApp.Services.Logistics.ViewModels.Dtos;
using PlayWebApp.Services.Logistics.ViewModels.Requests;
using PlayWebApp.Services.ModelExtentions;

#nullable disable

namespace PlayWebApp.Services.Logistics.BookingMgt
{
    public class BookingService : NavigationService<Booking, BookingRequestDto, BookingUpdateVm, BookingDto>
    {
        public BookingService(INavigationRepository<Booking> repository) : base(repository)
        {
        }

        public override Booking ToDbModel(BookingUpdateVm vm)
        {
            return vm.ToModel();
        }

        public override BookingDto ToDto(Booking model)
        {
            return model.ToDto();
        }
    }

}