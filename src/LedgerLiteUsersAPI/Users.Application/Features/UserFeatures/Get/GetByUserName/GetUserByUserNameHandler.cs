using MediatR;
using Users.Application.Common.Results;
using Users.Domain.Entities;
using Users.Domain.Interfaces.Repositories;

namespace Users.Application.Features.UserFeatures.Get.GetByUserName
{
    public class GetUserByUserNameHandler(IUserRepository userRepository) : IRequestHandler<GetUserByUserNameQuery, Result<GetUserByUserNameResponse>>
    {
        public async Task<Result<GetUserByUserNameResponse>> Handle(GetUserByUserNameQuery query, CancellationToken cancellationToken)
        {
            IEnumerable<User> user = await userRepository.GetUserByNameAsync(query.UserName, cancellationToken);
            GetUserByUserNameResponse response = new(user);

            return Result<GetUserByUserNameResponse>.Success(response);
        }
    }
}