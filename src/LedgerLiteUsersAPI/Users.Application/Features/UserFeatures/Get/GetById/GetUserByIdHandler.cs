using MediatR;
using Users.Application.Common.Results;
using Users.Domain.Entities;
using Users.Domain.Interfaces.Repositories;

namespace Users.Application.Features.UserFeatures.Get.GetById
{
    public class GetUserByIdHandler(IUserRepository userRepository) : IRequestHandler<GetUserByIdQuery, Result<GetUserByIdResponse>>
    {
        public async Task<Result<GetUserByIdResponse>> Handle(GetUserByIdQuery query, CancellationToken cancellationToken)
        {
            if(query.Id == Guid.Empty) return Result<GetUserByIdResponse>.Failure("The user id cannot be null!");

            User? user = await userRepository.GetUserByIdAsync(query.Id, cancellationToken);

            if(user is null) return Result<GetUserByIdResponse>.Failure("User not found!");

            GetUserByIdResponse response = new(user.Id);

            return Result<GetUserByIdResponse>.Success(response);
        }
    }
}