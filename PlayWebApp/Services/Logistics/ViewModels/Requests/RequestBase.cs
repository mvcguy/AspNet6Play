#nullable disable

using PlayWebApp;

namespace PlayWebApp.Services.Logistics.ViewModels.Requests
{
    public class RequestBase
    {

        public string RefNbr { get; set; }

        public string TenantId { get; set; }

        public string UserId { get; set; }

        public bool ExpandLines { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }

    }


}