#nullable disable

using PlayWebApp;

namespace PlayWebApp.Services.Logistics.ViewModels.Dtos
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
        public int Page { get; set; }

        public int TotalPages { get; set; }

        public int ItemsPerPage { get; set; }
    }


}