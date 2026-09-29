using AutoMapper;
using MediatR;
using Users.Application.Common.Results;
using Users.Application.DTO;
using Users.Domain.Entities;
using Users.Domain.Interfaces.Repositories;

namespace Users.Application.Features.UserFeatures.Get.GetByPhone
{
    public class GetUserByPhoneHandler(IUserRepository userRepository, IMapper mapper) : IRequestHandler<GetUserByPhoneQuery, Result<GetUserByPhoneResponse>>
    {
        public async Task<Result<GetUserByPhoneResponse>> Handle(GetUserByPhoneQuery command, CancellationToken cancellationToken)
        {
            User? user = await userRepository.GetUserByPhoneAsync(command.Phone, cancellationToken);

            if(user is null) return Result<GetUserByPhoneResponse>.Failure("User not found!");

            UserDto userDto = mapper.Map<UserDto>(user);
            GetUserByPhoneResponse response = new(userDto);

            return Result<GetUserByPhoneResponse>.Success(response);
        }
    }
}