using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorSv.Models
{
    public interface IDbContextFactory<TContext> where TContext : DbContext
    {
        TContext CreateDbContext();
    }

    public class blazordbFactory<TContext>
        : IDbContextFactory<TContext> where TContext : DbContext
    {
        public blazordbFactory(IServiceProvider provider)
        {
            this.provider = provider;
        }
        private readonly IServiceProvider provider;
        public TContext CreateDbContext()
        {
            return ActivatorUtilities.CreateInstance<TContext>(provider);
        }
    }

    public static class FactoryExtensions
    {
        public static IServiceCollection AddDbContextFactory<TContext>(
            this IServiceCollection collection,
            Action<DbContextOptionsBuilder> optionsAction = null,
            ServiceLifetime contextAndOptionsLifetime = ServiceLifetime.Singleton)
            where TContext : DbContext
        {
            collection.Add(new ServiceDescriptor(
                typeof(IDbContextFactory<TContext>),
                sp => new blazordbFactory<TContext>(sp),
                contextAndOptionsLifetime));
            collection.Add(new ServiceDescriptor(
                typeof(DbContextOptions<TContext>),
                sp => GetOptions<TContext>(optionsAction, sp),
                contextAndOptionsLifetime));

            return collection;
        }
        private static DbContextOptions<TContext> GetOptions<TContext>(
            Action<DbContextOptionsBuilder> action,
            IServiceProvider sp = null) where TContext : DbContext
        {
            var optionsBuilder = new DbContextOptionsBuilder<TContext>();
            if (sp != null)
            {
                optionsBuilder.UseApplicationServiceProvider(sp);
            }
            action?.Invoke(optionsBuilder);
            return optionsBuilder.Options;
        }
    }

    public partial class blazordbContext : DbContext
    {
        /*
        public blazordbContext()
        {
        }
        */
        public blazordbContext(DbContextOptions<blazordbContext> options)
            : base(options)
        {
        }
        public virtual DbSet<Books> Books { get; set; }
        /*
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // #warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=.;Database=blazordb;Trusted_connection=True");
            }
        }
        */

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Books>(entity =>
            {
                entity.ToTable("books");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Author)
                    .IsRequired()
                    .HasColumnName("author")
                    .HasMaxLength(50);

                entity.Property(e => e.Price).HasColumnName("price");

                entity.Property(e => e.Publisher)
                    .HasColumnName("publisher")
                    .HasMaxLength(50);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasColumnName("title")
                    .HasMaxLength(50);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
