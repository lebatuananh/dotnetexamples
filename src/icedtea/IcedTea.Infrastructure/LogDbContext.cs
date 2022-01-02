using IcedTea.Domain.AggregateModel.LogErrorAggregate;
using Microsoft.EntityFrameworkCore;

namespace IcedTea.Infrastructure;

public class LogDbContext : DbContext
{
    public LogDbContext(DbContextOptions<LogDbContext> options)
        : base(options)
    {
    }

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
            log.ToTable("Log");
            log.HasKey(x => x.Id);
            log.Property(x => x.Level).HasMaxLength(128);
        });
    }
}