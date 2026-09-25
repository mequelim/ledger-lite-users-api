using Users.Domain.Entities;

namespace Users.Application.Features.UserFeatures.Get.GetAllActiveUsers
{
    public sealed record GetAllActiveUsersResponse(IEnumerable<User> Users);
}