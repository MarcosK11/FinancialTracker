using FinancialTracker.Domain.Enums;

namespace FinancialTracker.Domain.Entities;

public class Transaction : BaseEntity
{
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public TransactionType Type { get; set; }
    public Guid CategoryId { get; set; }
    public Guid UserId { get; set; }
    public virtual Category Category { get; set; } = null!;
    public virtual User User { get; set; } = null!;
}