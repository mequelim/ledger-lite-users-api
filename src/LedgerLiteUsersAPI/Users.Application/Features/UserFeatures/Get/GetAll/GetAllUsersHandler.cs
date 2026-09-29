using AutoMapper;
using MediatR;
using Users.Application.Common.Results;
using Users.Application.DTO;
using Users.Domain.Entities;
using Users.Domain.Interfaces.Repositories;

namespace Users.Application.Features.UserFeatures.Get.GetAll
{
    public class GetAllUsersHandler(IUserRepository userRepository, IMapper mapper) : IRequestHandler<GetAllUsersQuery, Result<GetAllUsersResponse>>
    {
        public async Task<Result<GetAllUsersResponse>> Handle(GetAllUsersQuery query, CancellationToken cancellationToken)
        {
            IEnumerable<User> users = await userRepository.GetAllAsync(cancellationToken);
            IEnumerable<UserDto> usersDto = mapper.Map<IEnumerable<UserDto>>(users);

            GetAllUsersResponse response = new(usersDto);

            return Result<GetAllUsersResponse>.Success(response);
        }
    }
}