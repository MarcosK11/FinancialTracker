using FinancialTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinancialTracker.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<Budget> Budgets => Set<Budget>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configurações do User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Name).IsRequired().HasMaxLength(100);
            entity.Property(u => u.Email).IsRequired().HasMaxLength(255);
            entity.HasIndex(u => u.Email).IsUnique();
        });

        // Configurações do Category
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Name).IsRequired().HasMaxLength(100);
            entity.Property(c => c.Color).IsRequired().HasMaxLength(7); // Ex: #FFFFFF
            entity.Property(c => c.Type).IsRequired();

            // Relacionamento com User
            entity.HasOne(c => c.User)
                  .WithMany(u => u.Categories)
                  .HasForeignKey(c => c.UserId)
                  .IsRequired(false); // Categorias do sistema têm UserId null

            // Índice para categorias do usuário
            entity.HasIndex(c => new { c.UserId, c.Name }).IsUnique();
        });

        // Configurações do Transaction
        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Amount).HasPrecision(18, 2).IsRequired();
            entity.Property(t => t.Description).IsRequired().HasMaxLength(255);
            entity.Property(t => t.Date).IsRequired();
            entity.Property(t => t.Type).IsRequired();

            // Relacionamento com Category
            entity.HasOne(t => t.Category)
                  .WithMany(c => c.Transactions)
                  .HasForeignKey(t => t.CategoryId)
                  .OnDelete(DeleteBehavior.Restrict);

            // Relacionamento com User
            entity.HasOne(t => t.User)
                  .WithMany(u => u.Transactions)
                  .HasForeignKey(t => t.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configurações do Budget
        modelBuilder.Entity<Budget>(entity =>
        {
            entity.HasKey(b => b.Id);
            entity.Property(b => b.Amount).HasPrecision(18, 2).IsRequired();
            entity.Property(b => b.StartDate).IsRequired();
            entity.Property(b => b.EndDate).IsRequired();

            // Relacionamento com Category
            entity.HasOne(b => b.Category)
                  .WithMany()
                  .HasForeignKey(b => b.CategoryId)
                  .OnDelete(DeleteBehavior.Restrict);

            // Relacionamento com User
            entity.HasOne(b => b.User)
                  .WithMany(u => u.Budgets)
                  .HasForeignKey(b => b.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Índice para evitar orçamentos duplicados para a mesma categoria e período
            entity.HasIndex(b => new { b.UserId, b.CategoryId, b.StartDate, b.EndDate }).IsUnique();
        });

        // Dados iniciais para categorias do sistema
        modelBuilder.Entity<Category>().HasData(
            new Category
            {
                Id = new Guid("6b1e2cf0-6cb4-4f6c-b2a7-7b8c6ca0fd33"),
                Name = "Salário",
                Color = "#4BC0C0",
                Type = Domain.Enums.CategoryType.Income,
                UserId = null
            },
            new Category
            {
                Id = new Guid("a3c4c07a-2d42-4f17-a52b-5c05d379f9ca"),
                Name = "Alimentação",
                Color = "#FF6384",
                Type = Domain.Enums.CategoryType.Expense,
                UserId = null
            },
            new Category
            {
                Id = new Guid("f1bb3b4d-779b-4d7a-a0d2-92e950b602d6"),
                Name = "Transporte",
                Color = "#36A2EB",
                Type = Domain.Enums.CategoryType.Expense,
                UserId = null
            },
            new Category
            {
                Id = new Guid("d0d68ad2-890f-4cb0-9c2c-2cbe2d7b2799"),
                Name = "Lazer",
                Color = "#FFCD56",
                Type = Domain.Enums.CategoryType.Expense,
                UserId = null
            },
            new Category
            {
                Id = new Guid("9de70c73-8398-47af-9bf6-6fdff4cd39a5"),
                Name = "Moradia",
                Color = "#9966FF",
                Type = Domain.Enums.CategoryType.Expense,
                UserId = null
            }
        );
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Automaticamente setar CreatedAt e UpdatedAt
        var entries = ChangeTracker
            .Entries()
            .Where(e => e.Entity is BaseEntity && (
                e.State == EntityState.Added
                || e.State == EntityState.Modified));

        foreach (var entityEntry in entries)
        {
            ((BaseEntity)entityEntry.Entity).UpdatedAt = DateTime.UtcNow;

            if (entityEntry.State == EntityState.Added)
            {
                ((BaseEntity)entityEntry.Entity).CreatedAt = DateTime.UtcNow;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}