#nullable disable

using System.ComponentModel.DataAnnotations;
using PlayWebApp.Services.Logistics.LocationMgt.ViewModels;

namespace PlayWebApp.Services.Logistics.ViewModels
{
    public class ViewModelBase
    {
        public virtual string RefNbr { get; set; }

        public UpdateType UpdateType { get; set; }

        [Required]
        [Range(0, 20000)]
        public int ClientRowNumber { get; set; }
    }


}