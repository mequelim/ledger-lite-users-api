using Users.Application.DTO;

namespace Users.Application.Features.UserFeatures.Get.GetAllActiveUsers
{
    public sealed record GetAllActiveUsersResponse(IEnumerable<UserDto> Users);
}