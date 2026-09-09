using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Portifolio.Server.Models;

namespace Portifolio.Server.Database
{
    public class DB : DbContext
    {
        public DB(DbContextOptions<DB> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<TechnologieInformation> Technologies { get; set; }
        public DbSet<Project> Projects { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TechnologieInformation>(entity =>
            {
                entity.HasIndex(x => x.Position).IsUnique();
            });

            modelBuilder.Entity<Project>()
                .Property(x => x.IconsTech)
                .HasConversion(
                   v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions)null),
                    v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions)null) ?? new List<string>());


            var dateOnlyConverter = new ValueConverter<DateOnly, DateTime>(
                d => d.ToDateTime(TimeOnly.MinValue),
                d => DateOnly.FromDateTime(d)
            );

            var nullableDateOnlyConverter = new ValueConverter<DateOnly?, DateTime?>(
                d => d.HasValue
                    ? d.Value.ToDateTime(TimeOnly.MinValue)
                    : null,
                d => d.HasValue
                    ? DateOnly.FromDateTime(d.Value)
                    : null
            );

            modelBuilder.Entity<Project>()
                .Property(p => p.Start)
                .HasConversion(dateOnlyConverter)
                .HasColumnType("date");

            modelBuilder.Entity<Project>()
                .Property(p => p.End)
                .HasConversion(nullableDateOnlyConverter)
                .HasColumnType("date");


        }
    }
}
