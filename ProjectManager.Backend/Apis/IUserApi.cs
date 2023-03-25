using System.Diagnostics.CodeAnalysis;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Options;
using ProjectManager.Backend.Entities;
using ProjectManager.Backend.Options;

namespace ProjectManager.Backend.Apis; 

public interface IUserApi {
    public User Login(User login);
    public User Register(User register);
    public User GetUser(string userId);
    public IEnumerable<User> GetUsers();
    public bool UpdateUser(User update);
    public void DeleteUser(string userId);
    public bool CanCreateProject(string userId);
}

public sealed class UserApi : IUserApi {
    private readonly DatabaseContext _context;
    private readonly IProjectApi _projects;
    private readonly GeneralOptions _options;

    public UserApi(DatabaseContext context, IProjectApi projects, IOptions<GeneralOptions> options) {
        _context = context;
        _projects = projects;
        _options = options.Value;
    }
    
    public User Login(User login) {
        if (string.IsNullOrEmpty(login.Email) || string.IsNullOrEmpty(login.Password)) return null;
        var user = _context.Users.SingleOrDefault(user => user.Email == login.Email);
        if (user == null) return null;
        var hash = Hash128(login.Password, user.UserId);
        if (user.Password != hash) return null;
        return user;
    }

    public User Register(User register) {
        if (string.IsNullOrEmpty(register.Username) ||
            string.IsNullOrEmpty(register.Email) ||
            string.IsNullOrEmpty(register.Password)) return null;

        if (_context.Users.Any(user => user.Email == register.Email || user.Username == register.Username))
            return null;

        if (register.Email.Length > 255 || register.Username.Length > 255 || register.Password.Length > 255)
            return null;

        if (!register.Email.Contains('@') || !register.Email.Contains('.')) return null;
        if (!register.Password.Any(char.IsLetter) || !register.Password.Any(char.IsDigit) || register.Password.Length < 8) return null;

        User user = new() {
            UserId = Guid.NewGuid().ToString(),
            Email = register.Email,
            Username = register.Username,
            MaxProjects = _options.MaxProjects
        };
        user.Password = Hash128(register.Password, user.UserId);

        _context.Users.Add(user);
        _context.SaveChanges();
        return user;
    }

    public User GetUser(string userId) {
        return _context.Users.SingleOrDefault(user => user.UserId == userId);
    }

    public IEnumerable<User> GetUsers() {
        return _context.Users;
    }

    public bool UpdateUser(User update) {
        if (_context.Users.Any(user => user.UserId != update.UserId && (user.Email == update.Email || user.Username == update.Username)))
            return false;

        if (update.Email.Length > 255 || update.Username.Length > 255 || update.Password.Length > 255)
            return false;

        if (!string.IsNullOrEmpty(update.Email) && (!update.Email.Contains('@') || !update.Email.Contains('.'))) return false;
        if (!string.IsNullOrEmpty(update.Password) && (!update.Password.Any(char.IsLetter) || !update.Password.Any(char.IsDigit) || update.Password.Length < 8)) return false;
        
        var user = _context.Users.SingleOrDefault(user => user.UserId == update.UserId);
        if (user == null) return false;
        if (!string.IsNullOrEmpty(update.Email)) user.Email = update.Email;
        if (!string.IsNullOrEmpty(update.Username)) user.Username = update.Username;
        if (!string.IsNullOrEmpty(update.Password)) user.Password = Hash128(update.Password, user.UserId);
        _context.Users.Update(user);
        _context.SaveChanges();
        return true;
    }

    public void DeleteUser(string userId) {
        if (!_context.Users.Any(user => user.UserId == userId)) return;
        var user = _context.Users.Single(user => user.UserId == userId);

        var projects = _projects.GetProjects(userId);
        foreach (var project in projects) {
            _projects.DeleteProject(project.ProjectId);
        }
        
        _context.Users.Remove(user);
        _context.SaveChanges();
    }

    public bool CanCreateProject(string userId) {
        return GetUser(userId).MaxProjects > _projects.GetProjects(userId).Length;
    }

    private static string Hash128(string plainText, string salt) {
        try {
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: plainText,
                salt: Encoding.Default.GetBytes(salt),
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8
            ));

            return hashed;
        } catch (Exception) { return ""; }
    }
}