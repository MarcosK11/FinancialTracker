namespace FinancialTracker.Domain.Entities;

public class Budget : BaseEntity
{
    public decimal Amount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Guid CategoryId { get; set; }
    public Guid UserId { get; set; }
    public virtual Category Category { get; set; } = null!;
    public virtual User User { get; set; } = null!;
}