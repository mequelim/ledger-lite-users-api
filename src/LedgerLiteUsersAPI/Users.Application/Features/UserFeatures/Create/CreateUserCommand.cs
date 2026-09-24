using MediatR;
using Users.Application.Common.Results;

namespace Users.Application.Features.UserFeatures.Create
{
    public sealed record CreateUserCommand(
        string Name,
        string Surname,
        DateOnly Birthdate,
        string Email,
        string Phone,
        bool IsActive
    ) : IRequest<Result<CreateUserResponse>>;
}