using MediatR;
using Users.Application.Common.Results;
using Users.Domain.Entities;
using Users.Domain.Exceptions.UserExceptions;
using Users.Domain.Interfaces.Repositories;
using Users.Persistence.Database;

namespace Users.Application.Features.UserFeatures.Update
{
    public class UpdateUserHandler(
        IUserRepository userRepository,
        AppDbContext databaseContext
    ) : IRequestHandler<UpdateUserCommand, Result<UpdateUserResponse>>
    {
        public async Task<Result<UpdateUserResponse>> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
        {
            User? user = await userRepository.GetUserByIdAsync(command.Id, cancellationToken);

            if(user is null) return Result<UpdateUserResponse>.Failure("User not found!");

            user.Name = command.Name;
            user.Surname = command.Surname;
            user.Birthdate = command.Birthdate;
            user.Email = command.Email;
            user.Phone = command.Phone;
            user.IsActive = command.IsActive;

            userRepository.Update(user);
            await databaseContext.SaveChangesAsync(cancellationToken);

            UpdateUserResponse response = new(
                user.Id,
                user.Name,
                user.Surname,
                user.Birthdate,
                user.Email,
                user.Phone,
                user.IsActive
            );

            return Result<UpdateUserResponse>.Success(response);
        }
    }
}