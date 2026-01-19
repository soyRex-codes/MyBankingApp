namespace MyBankingApp.Domain.Common;

/// <summary>
/// Base class for auditable entities with creation and modification tracking.
/// </summary>
public abstract class AuditableEntity : BaseEntity
{
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}
