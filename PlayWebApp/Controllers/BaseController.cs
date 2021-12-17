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
#nullable disable

namespace PlayWebApp.Controllers
{
    [Authorize(AuthenticationSchemes = fk)]
    [ApiController]
    public class BaseController : Controller
    {
        // private const string fk = CookieAuthenticationDefaults.AuthenticationScheme + "," +
        // JwtBearerDefaults.AuthenticationScheme;      
        private const string fk = "Identity.Application" + "," + JwtBearerDefaults.AuthenticationScheme;
    }
}