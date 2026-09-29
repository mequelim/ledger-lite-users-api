using AutoMapper;
using MediatR;
using Users.Application.Common.Results;
using Users.Application.DTO;
using Users.Domain.Entities;
using Users.Domain.Interfaces.Repositories;

namespace Users.Application.Features.UserFeatures.Get.GetAllInactiveUsers
{
    public class GetAllInactiveUsersHandler(IUserRepository userRepository, IMapper mapper) : IRequestHandler<GetAllInactiveUsersQuery, Result<GetAllInactiveUsersResponse>>
    {
        public async Task<Result<GetAllInactiveUsersResponse>> Handle(GetAllInactiveUsersQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<User> users = await userRepository.GetAllInactiveUsersAsync(cancellationToken);
            IEnumerable<UserDto> usersDto = mapper.Map<IEnumerable<UserDto>>(users);

            GetAllInactiveUsersResponse response = new(usersDto);

            return Result<GetAllInactiveUsersResponse>.Success(response);
        }
    }
}