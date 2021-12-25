using PlayWebApp.Services.Database.Model;
using PlayWebApp.Services.DataNavigation;
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

        public async override Task<BookingDto> Add(BookingUpdateVm model)
        {
            var record = await repository.GetById(model.RefNbr);
            if (record == null)
            {
                record = new Booking
                {
                    RefNbr = model.RefNbr,
                    Description = model.Description,
                    BookingItems = model.Lines?.Select(x => new BookingItem
                    {
                        BookingId = x.BookingRefNbr,
                        RefNbr = x.RefNbr,
                        Description = x.Description,
                        Discount = x.Discount,
                        ExtCost = x.ExtCost,
                        Quantity = x.Quantity,
                        StockItemId = x.StockItemId,
                        UnitCost = x.UnitCost,
                    }).ToList(),
                };

                var item = repository.Add(record);
                return item.Entity.ToDto();
            }

            throw new Exception("Record exist from before");
        }

        public override BookingDto ToDto(Booking model)
        {
            return model.ToDto();
        }

        public async override Task<BookingDto> Update(BookingUpdateVm model)
        {
            var record = await repository.GetById(model.RefNbr);
            if (record != null)
            {

                record.Description = model.Description;

                foreach (var item in model.Lines)
                {
                    var line = record.BookingItems.FirstOrDefault(x => x.RefNbr == item.RefNbr);
                    if (line == null) continue;

                    line.Description = item.Description;
                    line.Discount = item.Discount;
                    line.ExtCost = item.ExtCost;
                    line.Quantity = item.Quantity;
                    line.UnitCost = item.UnitCost;
                }

                var res = repository.Update(record);
                return res.Entity.ToDto();
            }

            throw new Exception("Record cannot be found");
        }
    }

}