using MediatR;
using Users.Application.Common.Results;
using Users.Domain.Entities;
using Users.Domain.Interfaces.Repositories;

namespace Users.Application.Features.UserFeatures.Get.GetAll
{
    public class GetAllUsersHandler(IUserRepository userRepository) : IRequestHandler<GetAllUsersQuery, Result<GetAllUsersResponse>>
    {
        public async Task<Result<GetAllUsersResponse>> Handle(GetAllUsersQuery query, CancellationToken cancellationToken)
        {
            IEnumerable<User> users = await userRepository.GetAllAsync(cancellationToken);
            GetAllUsersResponse response = new GetAllUsersResponse(users);

            return Result<GetAllUsersResponse>.Success(response);
        }
    }
}