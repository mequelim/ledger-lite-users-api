using MediatR;
using Microsoft.AspNetCore.Mvc;
using Users.Application.Common.Results;
using Users.Application.Features.UserFeatures.Create;
using Users.Application.Features.UserFeatures.Delete;
using Users.Application.Features.UserFeatures.Get.GetAll;
using Users.Application.Features.UserFeatures.Get.GetAllActiveUsers;
using Users.Application.Features.UserFeatures.Get.GetAllInactiveUsers;
using Users.Application.Features.UserFeatures.Get.GetByEmail;
using Users.Application.Features.UserFeatures.Get.GetById;
using Users.Application.Features.UserFeatures.Get.GetByPhone;
using Users.Application.Features.UserFeatures.Get.GetByUserName;
using Users.Application.Features.UserFeatures.Update;

namespace Users.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(IMediator mediator) : ControllerBase
    {
        // GET:
        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers(CancellationToken cancellationToken)
        {
            Result<GetAllUsersResponse> response = await mediator.Send(new GetAllUsersQuery(), cancellationToken);

            return Ok(response);
        }

        [HttpGet("GetAllActiveUsers")]
        public async Task<IActionResult> GetAllActiveUsers(CancellationToken cancellationToken)
        {
            Result<GetAllActiveUsersResponse> response = await mediator.Send(new GetAllActiveUsersQuery(), cancellationToken);

            return Ok(response);
        }

        [HttpGet("GetAllInactiveUsers")]
        public async Task<IActionResult> GetAllInactiveUsers(CancellationToken cancellationToken)
        {
            Result<GetAllInactiveUsersResponse> response = await mediator.Send(new GetAllInactiveUsersQuery(), cancellationToken);

            return Ok(response);
        }

        [HttpGet("GetUserById/{userId:guid}")]
        public async Task<IActionResult> GetUserById(Guid userId, CancellationToken cancellationToken)
        {
            Result<GetUserByIdResponse> response = await mediator.Send(new GetUserByIdQuery(userId), cancellationToken);

            return Ok(response);
        }

        [HttpGet("GetUserByUserName/{userName}")]
        public async Task<IActionResult> GetUserByUserName(string userName, CancellationToken cancellationToken)
        {
            Result<GetUserByUserNameResponse> response = await mediator.Send(new GetUserByUserNameQuery(userName), cancellationToken);

            return Ok(response);
        }

        [HttpGet("GetUserByEmail/{email}")]
        public async Task<IActionResult> GetUserByEmail(string email, CancellationToken cancellationToken)
        {
            Result<GetUserByEmailResponse> response = await mediator.Send(new GetUserByEmailQuery(email), cancellationToken);

            return Ok(response);
        }

        [HttpGet("GetUserByPhone/{phone}")]
        public async Task<IActionResult> GetUserByPhone(string phone, CancellationToken cancellationToken)
        {
            Result<GetUserByPhoneResponse> response = await mediator.Send(new GetUserByPhoneQuery(phone), cancellationToken);

            return Ok(response);
        }

        // CREATE:
        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand command, CancellationToken cancellationToken)
        {
            Result<CreateUserResponse> response = await mediator.Send(command, cancellationToken);

            return Ok(response);
        }

        // UPDATE:
        [HttpPatch("UpdateUser/{userId:guid}")]
        public async Task<IActionResult> UpdateUser(Guid userId, [FromBody] UpdateUserCommand command, CancellationToken cancellationToken)
        {
            UpdateUserCommand request = command with
            {
                Id = userId
            };

            Result<UpdateUserResponse> response = await mediator.Send(request, cancellationToken);

            return Ok(response);
        }

        // DELETE:
        [HttpDelete("DeleteUser/{userId:guid}")]
        public async Task<IActionResult> DeleteUser(Guid userId, CancellationToken cancellationToken)
        {
            DeleteUserCommand request = new(userId);

            Result<DeleteUserResponse> response = await mediator.Send(request, cancellationToken);

            return Ok(response);
        }
    }
}