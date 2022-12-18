using Microsoft.Extensions.Options;
using ProjectManager.Backend.Entities;
using ProjectManager.Backend.Options;

namespace ProjectManager.Backend.Apis; 

public interface ITokenApi {
    public string GetValidToken(string userId, string clientIp);
    public bool ValidateToken(string tokenId, string clientIp);
    public User GetUserFromToken(string tokenId);
}

public sealed class TokenApi : ITokenApi {
    private readonly DatabaseContext _context;
    private readonly GeneralOptions _options;
    private readonly IUserApi _users;

    public TokenApi(DatabaseContext context, IOptions<GeneralOptions> options, IUserApi users) {
        _context = context;
        _options = options.Value;
        _users = users;
    }
    
    public string GetValidToken(string userId, string clientIp) {
        var token = _context.Tokens.SingleOrDefault(token => token.UserId == userId && token.ClientIp == clientIp);

        if (token == null) {
            token = new() {
                TokenId = Guid.NewGuid().ToString(),
                UserId = userId,
                ClientIp = clientIp,
                Created = DateTime.Now
            };
            _context.Tokens.Add(token);
            _context.SaveChanges();
            return token.TokenId;
        }

        if (!ValidateToken(token.TokenId, clientIp)) {
            _context.Tokens.Remove(token);
            token = new() {
                TokenId = Guid.NewGuid().ToString(),
                UserId = userId,
                ClientIp = clientIp,
                Created = DateTime.Now
            };
            _context.Tokens.Add(token);
            _context.SaveChanges();
            return token.TokenId;
        }

        return token.TokenId;
    }

    public bool ValidateToken(string tokenId, string clientIp) {
        if (tokenId is null || clientIp is null) return false;
        var token = _context.Tokens.SingleOrDefault(token => token.TokenId == tokenId && token.ClientIp == clientIp);
        if (token is null) return false;
        if (_context.Users.SingleOrDefault(user => user.UserId == token.UserId) == null) return false;
        return DateTime.Now - token.Created < TimeSpan.FromDays(_options.TokenTimeInDays);
    }

    public User GetUserFromToken(string tokenId) {
        return _users.GetUser(_context.Tokens.SingleOrDefault(token => token.TokenId == tokenId)?.UserId);
    }
}