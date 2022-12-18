using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ProjectManager.Backend.Security; 

public sealed class AuthorizedAttribute : TypeFilterAttribute {
    public AuthorizedAttribute(params string[] permission) : base(typeof(AuthorizationHandler)) {
        Arguments = new object[] { permission };
    }
}

public sealed class AuthorizationHandler : IAuthorizationFilter {
    
    private readonly string[] _permissions;

    public AuthorizationHandler(params string[] permissions) {
        _permissions = permissions;
    }

    
    public void OnAuthorization(AuthorizationFilterContext context) {
        if (context.Filters.Any(item => item is IAllowAnonymousFilter)) return;
        if (context.HttpContext.User.Identity?.IsAuthenticated != true) {
            context.Result = new UnauthorizedResult();
            return;
        }
        
        //TODO: Handle Permissions
    }
}