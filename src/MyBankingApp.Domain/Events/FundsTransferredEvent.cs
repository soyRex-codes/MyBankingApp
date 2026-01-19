using MyBankingApp.Domain.Common;
using MyBankingApp.Domain.ValueObjects;

namespace MyBankingApp.Domain.Events;

/// <summary>
/// Event raised when funds are transferred between accounts.
/// </summary>
public sealed class FundsTransferredEvent : IDomainEvent
{
    public Guid SourceAccountId { get; }
    public Guid DestinationAccountId { get; }
    public Money Amount { get; }
    public DateTime OccurredOn { get; }

    public FundsTransferredEvent(Guid sourceAccountId, Guid destinationAccountId, Money amount)
    {
        SourceAccountId = sourceAccountId;
        DestinationAccountId = destinationAccountId;
        Amount = amount;
        OccurredOn = DateTime.UtcNow;
    }
}
