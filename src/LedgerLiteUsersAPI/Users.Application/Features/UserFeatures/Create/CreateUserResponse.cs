namespace Users.Application.Features.UserFeatures.Create
{
    public sealed record CreateUserResponse(
        Guid Id,
        string Name,
        string Surname,
        DateOnly Birthdate,
        string Email,
        string Phone,
        bool IsActive
    );
}