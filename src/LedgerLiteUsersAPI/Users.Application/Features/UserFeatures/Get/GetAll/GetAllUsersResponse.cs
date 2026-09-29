using Users.Application.DTO;

namespace Users.Application.Features.UserFeatures.Get.GetAll
{
    public sealed record GetAllUsersResponse(IEnumerable<UserDto> Users);
}