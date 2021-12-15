using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PlayWebApp.Services.Database;
using PlayWebApp.Services.Database.Model;
using PlayWebApp.Services.Identity;
#nullable disable

namespace PlayWebApp.Controllers
{
    [Authorize]
    [ApiController]
    public class BaseController : Controller
    {
    }
}