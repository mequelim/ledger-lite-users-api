using AutoMapper;
using MediatR;
using Users.Application.Common.Results;
using Users.Application.DTO;
using Users.Domain.Entities;
using Users.Domain.Interfaces.Repositories;

namespace Users.Application.Features.UserFeatures.Get.GetByEmail
{
    public class GetUserByEmailHandler(IUserRepository userRepository, IMapper mapper) : IRequestHandler<GetUserByEmailQuery, Result<GetUserByEmailResponse>>
    {
        public async Task<Result<GetUserByEmailResponse>> Handle(GetUserByEmailQuery query, CancellationToken cancellationToken)
        {
            User? user = await userRepository.GetUserByEmailAsync(query.Email, cancellationToken);

            if(user is null) return Result<GetUserByEmailResponse>.Failure("User not found!");

            UserDto userDto = mapper.Map<UserDto>(user);
            GetUserByEmailResponse response = new(userDto);

            return Result<GetUserByEmailResponse>.Success(response);
        }
    }
}