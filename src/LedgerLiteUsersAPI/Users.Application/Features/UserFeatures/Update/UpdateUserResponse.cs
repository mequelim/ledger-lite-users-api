namespace Users.Application.Features.UserFeatures.Update
{
    public sealed record UpdateUserResponse(
        Guid Id,
        string Name,
        string Surname,
        DateOnly Birthdate,
        string Email,
        string Phone,
        bool IsActive
    );
}