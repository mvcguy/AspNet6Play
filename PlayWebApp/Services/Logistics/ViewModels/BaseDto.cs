#nullable disable

using PlayWebApp;

namespace PlayWebApp.Services.Logistics.ViewModels
{
    public class BaseDto
    {
        public string RefNbr { get; set; }

        public string InternalId { get; set; }
    }

    public class DtoCollection<TDto>
    {
        public IEnumerable<TDto> Items { get; set; }

        public CollectionMetaData MetaData { get; set; }

    }

    public class CollectionMetaData
    {
        public int PageIndex { get; set; }

        public int TotalRecords { get; set; }

        public int PageSize { get; set; }
    }


}