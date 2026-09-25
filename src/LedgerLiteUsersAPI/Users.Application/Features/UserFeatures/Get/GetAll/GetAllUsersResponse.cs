using Users.Domain.Entities;

namespace Users.Application.Features.UserFeatures.Get.GetAll
{
    public sealed record GetAllUsersResponse(IEnumerable<User> Users);
}