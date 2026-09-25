using Users.Domain.Entities;

namespace Users.Application.Features.UserFeatures.Get.GetByUserName
{
    public sealed record GetUserByUserNameResponse(IEnumerable<User> Users);
}