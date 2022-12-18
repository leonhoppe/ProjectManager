namespace ProjectManager.Backend.Security; 

public interface ITokenContext {
    public bool IsAuthenticated { get; }
    public string TokenId { get; }
    public string UserId { get; }
}

public sealed class TokenContext : ITokenContext {
    private readonly IHttpContextAccessor _accessor;

    public bool IsAuthenticated => _accessor.HttpContext?.User.Identity?.IsAuthenticated == true;
    public string TokenId => _accessor.HttpContext?.User.GetTokenId();
    public string UserId => _accessor.HttpContext?.User.GetUserId();
    
    public TokenContext(IHttpContextAccessor accessor) {
        _accessor = accessor;
    }
}