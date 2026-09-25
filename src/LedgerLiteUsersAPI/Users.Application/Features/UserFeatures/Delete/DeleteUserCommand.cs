using MediatR;
using Users.Application.Common.Results;

namespace Users.Application.Features.UserFeatures.Delete
{
    public record DeleteUserCommand(Guid Id) : IRequest<Result<DeleteUserResponse>>;
}