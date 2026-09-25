using MediatR;
using Users.Application.Common.Results;

namespace Users.Application.Features.UserFeatures.Get.GetAllInactiveUsers
{
    public sealed record GetAllInactiveUsersQuery() : IRequest<Result<GetAllInactiveUsersResponse>>;
}