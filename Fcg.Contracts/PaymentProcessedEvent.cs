namespace Fcg.Contracts
{
    public record PaymentProcessedEvent
    (
        Guid TransactionId,
        int UserId,
        string? UserEmail,
        string GameId,
        PaymentStatus Status
    );

    public enum PaymentStatus
    {
        Approved,
        Rejected
    }
}