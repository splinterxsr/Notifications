namespace Fcg.Contracts
{
    public record UserCreatedEvent
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = null!;
        public string UserEmail { get; set; } = null!;
        public int UserRole { get; set; }
    }
}
