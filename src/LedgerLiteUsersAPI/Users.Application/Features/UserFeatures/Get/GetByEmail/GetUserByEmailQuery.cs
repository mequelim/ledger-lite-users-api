using MediatR;
using Users.Application.Common.Results;

namespace Users.Application.Features.UserFeatures.Get.GetByEmail
{
    public sealed record GetUserByEmailQuery(string Email) : IRequest<Result<GetUserByEmailResponse>>;
}