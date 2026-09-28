using AutoMapper;
using MediatR;
using Users.Application.Common.Results;
using Users.Application.DTO;
using Users.Domain.Entities;
using Users.Domain.Interfaces.Repositories;

namespace Users.Application.Features.UserFeatures.Get.GetAllActiveUsers
{
    public class GetAllActiveUsersHandler(IUserRepository userRepository, IMapper mapper) : IRequestHandler<GetAllActiveUsersQuery, Result<GetAllActiveUsersResponse>>
    {
        public async Task<Result<GetAllActiveUsersResponse>> Handle(GetAllActiveUsersQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<User> users = await userRepository.GetAllActiveUsersAsync(cancellationToken);
            IEnumerable<UserDto> usersDto = mapper.Map<IEnumerable<UserDto>>(users);

            GetAllActiveUsersResponse response = new(usersDto);

            return Result<GetAllActiveUsersResponse>.Success(response);
        }
    }
}