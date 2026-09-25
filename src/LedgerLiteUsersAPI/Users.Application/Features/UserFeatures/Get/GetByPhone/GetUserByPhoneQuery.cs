using MediatR;
using Users.Application.Common.Results;

namespace Users.Application.Features.UserFeatures.Get.GetByPhone
{
    public sealed record GetUserByPhoneQuery(string Phone) : IRequest<Result<GetUserByPhoneResponse>>;
}