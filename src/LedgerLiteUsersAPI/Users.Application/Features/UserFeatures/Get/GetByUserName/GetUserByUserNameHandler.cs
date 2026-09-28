using AutoMapper;
using MediatR;
using Users.Application.Common.Results;
using Users.Application.DTO;
using Users.Domain.Entities;
using Users.Domain.Interfaces.Repositories;

namespace Users.Application.Features.UserFeatures.Get.GetByUserName
{
    public class GetUserByUserNameHandler(
        IUserRepository userRepository,
        IMapper mapper)
        : IRequestHandler<GetUserByUserNameQuery, Result<GetUserByUserNameResponse>>
    {
        public async Task<Result<GetUserByUserNameResponse>> Handle(
            GetUserByUserNameQuery query,
            CancellationToken cancellationToken)
        {
            IEnumerable<User> users = await userRepository.GetUserByNameAsync(
                query.UserName,
                cancellationToken);

            IEnumerable<UserDto> userDtos = mapper.Map<IEnumerable<UserDto>>(users);

            GetUserByUserNameResponse response = new(userDtos);

            return Result<GetUserByUserNameResponse>.Success(response);
        }
    }
}