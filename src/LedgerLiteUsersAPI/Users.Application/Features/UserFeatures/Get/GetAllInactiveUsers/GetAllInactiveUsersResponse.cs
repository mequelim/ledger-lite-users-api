using Users.Domain.Entities;

namespace Users.Application.Features.UserFeatures.Get.GetAllInactiveUsers
{
    public sealed record GetAllInactiveUsersResponse(IEnumerable<User> Users);
}