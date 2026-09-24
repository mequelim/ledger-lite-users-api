using MediatR;
using Users.Application.Common.Results;

namespace Users.Application.Features.UserFeatures.Update
{
    public sealed record UpdateUserCommand(
        Guid Id,
        string Name,
        string Surname,
        DateOnly Birthdate,
        string Email,
        string Phone,
        bool IsActive
    ) : IRequest<Result<UpdateUserResponse>>;
}