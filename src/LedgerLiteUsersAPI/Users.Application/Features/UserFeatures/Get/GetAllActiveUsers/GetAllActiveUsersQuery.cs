using MediatR;
using Users.Application.Common.Results;

namespace Users.Application.Features.UserFeatures.Get.GetAllActiveUsers
{
    public sealed record GetAllActiveUsersQuery() : IRequest<Result<GetAllActiveUsersResponse>>;
}