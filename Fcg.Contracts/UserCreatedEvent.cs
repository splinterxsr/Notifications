namespace Fcg.Contracts
{
    public record UserCreatedEvent
    (
        int UserId,
        string UserName,
        string UserEmail,
        int UserRole
    );
}