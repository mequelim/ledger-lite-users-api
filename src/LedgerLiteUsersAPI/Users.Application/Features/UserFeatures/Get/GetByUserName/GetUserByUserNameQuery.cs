using MediatR;
using Users.Application.Common.Results;
using Users.Domain.Entities;

namespace Users.Application.Features.UserFeatures.Get.GetByUserName
{
    public sealed record GetUserByUserNameQuery(string UserName) : IRequest<Result<GetUserByUserNameResponse>>;
}