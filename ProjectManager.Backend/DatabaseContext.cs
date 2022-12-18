using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ProjectManager.Backend.Entities;
using ProjectManager.Backend.Options;

namespace ProjectManager.Backend; 

public class DatabaseContext : DbContext {
    private readonly GeneralOptions _options;
    
    public DbSet<User> Users { get; set; }
    public DbSet<UserToken> Tokens { get; set; }
    public DbSet<Project> Projects { get; set; }

    public DatabaseContext(IOptions<GeneralOptions> options) {
        _options = options.Value;
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        optionsBuilder.UseMySQL(_options.Database);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<User>(entry => {
            entry.HasKey(e => e.UserId);
            entry.Property(e => e.Email);
            entry.Property(e => e.Username);
            entry.Property(e => e.Password);
            entry.Property(e => e.MaxProjects);
        });

        modelBuilder.Entity<UserToken>(entry => {
            entry.HasKey(e => e.TokenId);
            entry.Property(e => e.UserId);
            entry.Property(e => e.ClientIp);
            entry.Property(e => e.Created);
        });

        modelBuilder.Entity<Project>(entry => {
            entry.HasKey(e => e.ProjectId);
            entry.Property(e => e.OwnerId);
            entry.Property(e => e.Name);
            entry.Property(e => e.Port);
            entry.Property(e => e.ContainerName);
        });
    }
}