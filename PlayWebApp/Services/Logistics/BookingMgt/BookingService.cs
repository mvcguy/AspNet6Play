using PlayWebApp.Services.Database.Model;
using PlayWebApp.Services.DataNavigation;
using PlayWebApp.Services.Logistics.BookingMgt.Repository;
using PlayWebApp.Services.Logistics.BookingMgt.ViewModels;
using PlayWebApp.Services.Logistics.LocationMgt.ViewModels;
using PlayWebApp.Services.Logistics.ViewModels;
using PlayWebApp.Services.ModelExtentions;

#nullable disable

namespace PlayWebApp.Services.Logistics.BookingMgt
{
    public class BookingService : NavigationService<Booking, BookingRequestDto, BookingUpdateVm, BookingDto>
    {
        private readonly BookingRepository bookingRepository;
        private readonly INavigationRepository<Customer> customerRepo;
        private readonly INavigationRepository<StockItem> stockRepository;

        public BookingService(BookingRepository repository,
            INavigationRepository<Customer> customerRepo,
            INavigationRepository<StockItem> stockRepository) : base(repository)
        {
            this.bookingRepository = repository;
            this.customerRepo = customerRepo;
            this.stockRepository = stockRepository;
        }

        public async Task<DtoCollection<BookingItemDto>> GetBookingLines(string bookingRefNbr, int page = 1)
        {
            var pagedResult = await bookingRepository.GetBookingLinesPaginated(bookingRefNbr, MaxPerPage, page);
            if (pagedResult == null) return null;

            return new DtoCollection<BookingItemDto>()
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

        public void UpdateSummary(Booking booking)
        {
            var lt = 0.0m;
            var ds = 0.0m;
            foreach (var item in booking.BookingItems)
            {
                lt += item.ExtCost.GetValueOrDefault();
                ds += item.Discount.GetValueOrDefault();
            }

            var tx = (lt - ds) * .025m;

            booking.Discount = ds;
            booking.LinesTotal = lt;
            booking.TaxAmount = tx;
            booking.TaxableAmount = lt - ds;
            booking.TotalAmount = (lt - ds) + tx;
            booking.Balance = (lt - ds) + tx;

        }

        public async override Task<BookingDto> Add(BookingUpdateVm model)
        {
            var record = await repository.GetById(model.RefNbr);
            var customer = await customerRepo.GetById(model.CustomerRefNbr);
            if (customer == null) throw new Exception("Customer cannot be found");
            var defAddress = customer.DefaultAddress;
            if (defAddress == null)
                throw new Exception("Shipping address not found. Please select a default address for the customer frist");

            if (record != null) throw new Exception("Record exist from before");
            record = new Booking
            {
                RefNbr = model.RefNbr,
                Description = model.Description,
                ShippingAddress = defAddress,
                Customer = customer,
                BookingItems = new List<BookingItem>()
            };
            await UpdateLines(model, record);
            UpdateSummary(record);
            var item = repository.Add(record);
            return item.Entity.ToDto();
        }

        public override BookingDto ToDto(Booking model)
        {
            return model.ToDto();
        }

        public async override Task<BookingDto> Update(BookingUpdateVm model)
        {
            var record = await repository.GetById(model.RefNbr);
            if (record == null) throw new Exception("Record does not exist");
            var customer = await customerRepo.GetById(model.CustomerRefNbr);
            if (customer.DefaultAddress == null)
                throw new Exception("Shipping address not found. Please select a default address for the customer frist");

            record.Description = model.Description;
            record.Customer = customer;
            record.ShippingAddress = customer.DefaultAddress;
            record.BookingItems = (await bookingRepository.GetBookingLines(record.RefNbr)).ToList();

            await UpdateLines(model, record);
            UpdateSummary(record);
            var res = repository.Update(record);
            return res.Entity.ToDto();
        }

        private async Task UpdateLines(BookingUpdateVm vm, Booking dbModel)
        {
            if(vm.Lines == null) return;
            foreach (var lineVm in vm.Lines)
            {
                BookingItem line = null;
                switch (lineVm.UpdateType)
                {
                    case UpdateType.New:
                        line = await AddNewLine(dbModel, lineVm);
                        break;
                    case UpdateType.Update:
                        line = await UpdateExistingLine(dbModel, lineVm);
                        break;
                    case UpdateType.Delete:
                        line = DeleteExistingLine(dbModel, lineVm);
                        break;
                }
            }
        }

        private BookingItem DeleteExistingLine(Booking dbModel, BookingItemUpdateVm vm)
        {
            var line = dbModel.BookingItems.FirstOrDefault(x => x.RefNbr == vm.RefNbr);
            if (line != null)
            {
                dbModel.BookingItems.Remove(line);
            }
            return line;
        }

        private async Task<BookingItem> UpdateExistingLine(Booking dbModel, BookingItemUpdateVm vm)
        {
            var line = dbModel.BookingItems.FirstOrDefault(x => x.RefNbr == vm.RefNbr);
            if (line != null)
            {
                line.Description = vm.Description;
                line.Discount = vm.Discount;
                line.Quantity = vm.Quantity;
                line.UnitCost = vm.UnitCost;
                line.ExtCost = vm.ExtCost;

                if (!string.IsNullOrWhiteSpace(vm.StockItemRefNbr))
                {
                    var stockItem = await stockRepository.GetById(vm.StockItemRefNbr);
                    if (stockItem == null) throw new Exception($"Stock Item with ID: {vm.StockItemRefNbr} cannot be found");
                    if (stockItem != null)
                    {
                        line.StockItem = stockItem;
                        line.StockItemId = stockItem.Id;
                    }
                }

                repository.UpdateAuditData(line);
            }
            return line;
        }

        private async Task<BookingItem> AddNewLine(Booking dbModel, BookingItemUpdateVm vm)
        {

            var line = new BookingItem
            {
                Id = Guid.NewGuid().ToString(),
                Description = vm.Description,
                Discount = vm.Discount,
                ExtCost = vm.ExtCost,
                UnitCost = vm.UnitCost,
                Quantity = vm.Quantity,
                RefNbr = vm.RefNbr,
                Booking = dbModel,
                BookingId = dbModel.Id,
            };

            if (!string.IsNullOrWhiteSpace(vm.StockItemRefNbr))
            {
                var stockItem = await stockRepository.GetById(vm.StockItemRefNbr);
                if (stockItem == null) throw new Exception($"Stock Item with ID: {vm.StockItemRefNbr} cannot be found");
                if (stockItem != null)
                {
                    line.StockItem = stockItem;
                    line.StockItemId = stockItem.Id;
                }
            }
            repository.AddAuditData(line);
            dbModel.BookingItems.Add(line);
            return line;
        }
    
    }

}