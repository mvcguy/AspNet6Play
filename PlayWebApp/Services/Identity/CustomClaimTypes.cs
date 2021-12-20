using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;

namespace PlayWebApp.Services.Identity
{
    public static class CustomClaimTypes
    {
        public const string TenantId = "PlayWebApp.TenantId";
    }

    public class CustomerClaimsAction : ClaimAction
    {
        public CustomerClaimsAction(string claimType, string valueType) : base(claimType, valueType)
        {
        }

        public override void Run(JsonElement userData, ClaimsIdentity identity, string issuer)
        {
            
        }
    }
}