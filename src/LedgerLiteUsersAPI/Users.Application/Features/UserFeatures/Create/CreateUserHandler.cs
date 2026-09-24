using MediatR;
using Users.Application.Common.Results;
using Users.Domain.Entities;
using Users.Domain.Exceptions.UserExceptions;
using Users.Domain.Interfaces.Repositories;
using Users.Persistence.Database;

namespace Users.Application.Features.UserFeatures.Create
{
    public sealed class CreateUserHandler(IUserRepository userRepository, AppDbContext databaseContext) : IRequestHandler<CreateUserCommand, Result<CreateUserResponse>>
    {
        public async Task<Result<CreateUserResponse>> Handle(CreateUserCommand command, CancellationToken cancellationToken)
        {
            User? userByEmail = await userRepository.GetUserByEmailAsync(command.Email, cancellationToken);
            User? userByPhone = await userRepository.GetUserByPhoneAsync(command.Phone, cancellationToken);

            if(userByEmail is not null) return Result<CreateUserResponse>.Failure("User e-mail already exists!");
            if(userByPhone is not null) return Result<CreateUserResponse>.Failure("User phone already exists!");

            User user;

            try
            {
                user = new User(
                    command.Name,
                    command.Surname,
                    command.Birthdate,
                    command.Email,
                    command.Phone,
                    command.IsActive
                );
            }
            catch(InvalidUserAgeException exception)
            {
                return Result<CreateUserResponse>.Failure(exception.Message);
            }
            catch(InvalidUserEmailException exception)
            {
                return Result<CreateUserResponse>.Failure(exception.Message);
            }
            catch(InvalidUserPhoneException exception)
            {
                return Result<CreateUserResponse>.Failure(exception.Message);
            }

            userRepository.Create(user);
            await databaseContext.SaveChangesAsync(cancellationToken);

            CreateUserResponse response = new(
                user.Id,
                user.Name,
                user.Surname,
                user.Birthdate,
                user.Email,
                user.Phone,
                user.IsActive
            );

            return Result<CreateUserResponse>.Success(response);
        }
    }
}