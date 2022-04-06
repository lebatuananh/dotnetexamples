using Microsoft.EntityFrameworkCore;

namespace Shared.Logging.LogError;

public class LogDbContext : DbContext
{
    public LogDbContext(DbContextOptions<LogDbContext> options)
        : base(options)
    {
    }

    public static string SchemaName => "error_log";
    public DbSet<Log> Logs { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        ConfigureLogContext(builder);
    }

    private void ConfigureLogContext(ModelBuilder builder)
    {
        builder.Entity<Log>(log =>
        {
            log.ToTable("log", SchemaName);
            log.HasKey(x => x.Id);
            log.Property(x => x.LogEvent).HasColumnType("jsonb");
            log.Property(x => x.Properties).HasColumnType("jsonb");
        });
    }
}