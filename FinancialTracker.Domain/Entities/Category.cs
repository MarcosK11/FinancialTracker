using FinancialTracker.Domain.Enums;

namespace FinancialTracker.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public CategoryType Type { get; set; }
    public Guid? UserId { get; set; }
    public virtual User? User { get; set; }
    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}