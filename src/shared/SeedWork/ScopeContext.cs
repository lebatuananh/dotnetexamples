using System;
using System.Linq;
using AspNet.Security.OpenIdConnect.Primitives;
using Microsoft.AspNetCore.Http;

namespace Shared.SeedWork;

public class ScopeContext : IScopeContext
{
    public ScopeContext(IHttpContextAccessor contextAccessor)
    {
        var claims = contextAccessor.HttpContext?.User;
        var sub = claims?.Claims.FirstOrDefault(x => x.Type.Equals("sub"))?.Value;
        var name = claims?.Claims.FirstOrDefault(x => x.Type.Equals("name"))?.Value;
        var role = claims?.Claims.FirstOrDefault(x => x.Type.Equals("role"))?.Value;
        if (sub != null || name != null || role != null)
        {
            Role = role;
            CurrentAccountId =
                !string.IsNullOrEmpty(sub)
                    ? Guid.Parse(sub)
                    : Guid.Empty;
            CurrentAccountName = !string.IsNullOrEmpty(name)
                ? name
                : string.Empty;
            CurrentAccountEmail = claims?.FindFirst(OpenIdConnectConstants.Claims.Email)?.Value;
        }
        else
        {
            Role = "";
            CurrentAccountId = Guid.Empty;
            CurrentAccountName = "Anonymous";
            CurrentAccountEmail = string.Empty;
        }
    }

    public Guid CurrentAccountId { get; }
    public string CurrentAccountName { get; }
    public string CurrentAccountEmail { get; }
    public string Role { get; }
}