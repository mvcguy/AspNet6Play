using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PlayWebApp.Services.Database;
using PlayWebApp.Services.Database.Model;
using PlayWebApp.Services.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using System.Globalization;
using PlayWebApp.Services.Logistics.LocationMgt.ViewModels;
using PlayWebApp.Services.Logistics.ViewModels;
#nullable disable

namespace PlayWebApp.Controllers
{
    [Authorize(AuthenticationSchemes = fk)]
    [ApiController]
    public class BaseController : Controller
    {
        // private const string fk = CookieAuthenticationDefaults.AuthenticationScheme + "," +
        // JwtBearerDefaults.AuthenticationScheme;      
        private const string fk = CookieAuthenticationDefaults.AuthenticationScheme + "," + JwtBearerDefaults.AuthenticationScheme + "," + "oidc";



        private List<string> GetModelKeysWithPrefix(List<string> props, string prefix)
        {
            return props.Select(prop => $"{prefix}.{prop}").ToList();
        }

        private void RemoveErrorsFromModelState(List<string> keys)
        {
            if (ModelState.IsValid) return;
            foreach (var key in keys)
            {
                if (ModelState.Keys.Contains(key))
                    ModelState.Remove(key);
            }

        }

        protected virtual void AdaptModelStateForAddressesArray<TModel>(IList<TModel> model, string pref) where TModel : ViewModelBase
        {
            if (!ModelState.IsValid)
            {
                var props = typeof(TModel).GetProperties().Select(x => x.Name).ToList();
                for (int i = 0; i < model.Count; i++)
                {
                    var prefix = $"{pref}[{i}]";
                    var row = model[i];
                    if (row.UpdateType == UpdateType.Delete && !string.IsNullOrWhiteSpace(row.RefNbr))
                    {
                        RemoveErrorsFromModelState(GetModelKeysWithPrefix(props, prefix));
                    }
                }

                var addressErrors = ModelState.FindKeysWithPrefix(pref);

                if (addressErrors.Any())
                {
                    //
                    // provide a mapping of server and client array indexes
                    //
                    for (int i = 0; i < model.Count; i++)
                    {
                        ModelState
                        .AddModelError($"{pref}[{i}]",
                                model[i].ClientRowNumber.ToString(CultureInfo.InvariantCulture));
                    }
                }
            }
        }

    }
}