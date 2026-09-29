using Users.Application.DTO;

namespace Users.Application.Features.UserFeatures.Get.GetByEmail
{
    public sealed record GetUserByEmailResponse(UserDto User);
}