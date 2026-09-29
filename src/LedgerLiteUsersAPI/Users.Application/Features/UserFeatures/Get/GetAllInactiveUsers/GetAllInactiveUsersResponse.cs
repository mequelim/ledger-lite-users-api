using Users.Application.DTO;

namespace Users.Application.Features.UserFeatures.Get.GetAllInactiveUsers
{
    public sealed record GetAllInactiveUsersResponse(IEnumerable<UserDto> Users);
}