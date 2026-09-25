using MediatR;
using Users.Application.Common.Results;
using Users.Domain.Exceptions.UserExceptions;
using Users.Domain.Interfaces.Repositories;
using Users.Persistence.Database;

namespace Users.Application.Features.UserFeatures.Delete
{
    public sealed class DeleteUserHandler(
        IUserRepository userRepository,
        AppDbContext databaseContext
    ) : IRequestHandler<DeleteUserCommand, Result<DeleteUserResponse>>
    {
        public async Task<Result<DeleteUserResponse>> Handle(DeleteUserCommand command, CancellationToken cancellationToken)
        {
            try
            {
                await userRepository.DeleteAsync(command.Id, cancellationToken);
                await databaseContext.SaveChangesAsync(cancellationToken);
            }
            catch(UserNotFoundException exception)
            {
                return Result<DeleteUserResponse>.Failure(exception.Message);
            }

            DeleteUserResponse response = new(command.Id);

            return Result<DeleteUserResponse>.Success(response);
        }
    }
}