using MediatR;
using Users.Application.Common.Results;

namespace Users.Application.Features.UserFeatures.Get.GetById
{
    public sealed record GetUserByIdQuery(Guid Id) : IRequest<Result<GetUserByIdResponse>>;
}