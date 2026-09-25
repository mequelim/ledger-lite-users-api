using MediatR;
using Users.Application.Common.Results;
using Users.Domain.Entities;
using Users.Domain.Interfaces.Repositories;

namespace Users.Application.Features.UserFeatures.Get.GetAllInactiveUsers
{
    public class GetAllInactiveUsersHandler(IUserRepository userRepository) : IRequestHandler<GetAllInactiveUsersQuery, Result<GetAllInactiveUsersResponse>>
    {
        public async Task<Result<GetAllInactiveUsersResponse>> Handle(GetAllInactiveUsersQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<User> users = await userRepository.GetAllInactiveUsersAsync(cancellationToken);
            GetAllInactiveUsersResponse response = new(users);

            return Result<GetAllInactiveUsersResponse>.Success(response);
        }
    }
}