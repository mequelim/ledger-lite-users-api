using MediatR;
using Users.Application.Common.Results;
using Users.Domain.Entities;
using Users.Domain.Interfaces.Repositories;

namespace Users.Application.Features.UserFeatures.Get.GetAllActiveUsers
{
    public class GetAllActiveUsersHandler(IUserRepository userRepository) : IRequestHandler<GetAllActiveUsersQuery, Result<GetAllActiveUsersResponse>>
    {
        public async Task<Result<GetAllActiveUsersResponse>> Handle(GetAllActiveUsersQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<User> users = await userRepository.GetAllActiveUsersAsync(cancellationToken);
            GetAllActiveUsersResponse response = new(users);

            return Result<GetAllActiveUsersResponse>.Success(response);
        }
    }
}