using Users.Application.DTO;

namespace Users.Application.Features.UserFeatures.Get.GetByUserName
{
    public sealed record GetUserByUserNameResponse(IEnumerable<UserDto> Users);
}