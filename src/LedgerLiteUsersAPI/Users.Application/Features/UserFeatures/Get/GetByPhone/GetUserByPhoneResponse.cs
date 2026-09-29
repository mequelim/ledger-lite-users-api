using Users.Application.DTO;

namespace Users.Application.Features.UserFeatures.Get.GetByPhone
{
    public sealed record GetUserByPhoneResponse(UserDto User);
}