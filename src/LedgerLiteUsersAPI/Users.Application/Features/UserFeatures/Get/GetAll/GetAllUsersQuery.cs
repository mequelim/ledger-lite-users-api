using MediatR;
using Users.Application.Common.Results;

namespace Users.Application.Features.UserFeatures.Get.GetAll
{
    public sealed record GetAllUsersQuery() : IRequest<Result<GetAllUsersResponse>>;
}