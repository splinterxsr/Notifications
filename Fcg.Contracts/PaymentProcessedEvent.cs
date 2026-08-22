namespace Fcg.Contracts
{
    public record PaymentProcessedEvent
    {
        public Guid TransactionId { get; set; }
        public int UserId { get; set; }
        public string? UserEmail { get; set; }
        public string GameId { get; set; } = string.Empty;
        public PaymentStatus Status { get; set; }
    }

    public enum PaymentStatus
    {
        Approved,
        Rejected
    }
}