using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
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
using Users.WebAPI.Controllers;

namespace Users.WebAPI.Tests.Tests.Controllers
{
    public class UsersControllerTests
    {
        private readonly IMediator _mediator;
        private readonly UsersController _controller;

        public UsersControllerTests()
        {
            _mediator = Substitute.For<IMediator>();
            _controller = new UsersController(_mediator);
        }

        [Fact]
        public async Task GetAllUsers_ShouldReturnOk()
        {
            // Arrange:
            Result<GetAllUsersResponse> response = Result<GetAllUsersResponse>.Failure("Test");

            _mediator
                .Send(Arg.Any<GetAllUsersQuery>(), Arg.Any<CancellationToken>())
                .Returns(response);

            // Act:
            IActionResult result = await _controller.GetAllUsers(CancellationToken.None);

            // Assert:
            result.Should().BeOfType<OkObjectResult>();

            OkObjectResult okResult = (OkObjectResult)result;
            okResult.Value.Should().Be(response);

            await _mediator
                .Received(1)
                .Send(
                    Arg.Any<GetAllUsersQuery>(),
                    Arg.Any<CancellationToken>()
                );
        }

        [Fact]
        public async Task GetAllActiveUsers_ShouldReturnOk()
        {
            // Arrange:
            Result<GetAllActiveUsersResponse> response = Result<GetAllActiveUsersResponse>.Failure("Test");

            _mediator
                .Send(Arg.Any<GetAllActiveUsersQuery>(), Arg.Any<CancellationToken>())
                .Returns(response);

            // Act:
            IActionResult result = await _controller.GetAllActiveUsers(CancellationToken.None);

            // Assert:
            result.Should().BeOfType<OkObjectResult>();

            OkObjectResult okResult = (OkObjectResult)result;
            okResult.Value.Should().Be(response);

            await _mediator
                .Received(1)
                .Send(
                    Arg.Any<GetAllActiveUsersQuery>(),
                    Arg.Any<CancellationToken>()
                );
        }

        [Fact]
        public async Task GetAllInactiveUsers_ShouldReturnOk()
        {
            // Arrange:
            Result<GetAllInactiveUsersResponse> response = Result<GetAllInactiveUsersResponse>.Failure("Test");

            _mediator
                .Send(Arg.Any<GetAllInactiveUsersQuery>(), Arg.Any<CancellationToken>())
                .Returns(response);

            // Act:
            IActionResult result = await _controller.GetAllInactiveUsers(CancellationToken.None);

            // Assert:
            result.Should().BeOfType<OkObjectResult>();

            OkObjectResult okResult = (OkObjectResult)result;
            okResult.Value.Should().Be(response);

            await _mediator
                .Received(1)
                .Send(
                    Arg.Any<GetAllInactiveUsersQuery>(),
                    Arg.Any<CancellationToken>()
                );
        }

        [Fact]
        public async Task GetUserById_ShouldSendCorrectQuery()
        {
            // Arrange:
            Guid userId = Guid.NewGuid();
            Result<GetUserByIdResponse> response = Result<GetUserByIdResponse>.Failure("Test");

            _mediator
                .Send(Arg.Any<GetUserByIdQuery>(), Arg.Any<CancellationToken>())
                .Returns(response);

            // Act:
            IActionResult result = await _controller.GetUserById(userId, CancellationToken.None);

            // Assert:
            result.Should().BeOfType<OkObjectResult>();

            OkObjectResult okResult = (OkObjectResult)result;
            okResult.Value.Should().Be(response);

            await _mediator
                .Received(1)
                .Send(
                    Arg.Is<GetUserByIdQuery>((query) => query.Id == userId),
                    Arg.Any<CancellationToken>()
                );
        }

        [Fact]
        public async Task GetUserByUserName_ShouldSendCorrectQuery()
        {
            // Arrange:
            string userName = "Pedro";
            Result<GetUserByUserNameResponse> response = Result<GetUserByUserNameResponse>.Failure("Test");

            _mediator
                .Send(Arg.Any<GetUserByUserNameQuery>(), Arg.Any<CancellationToken>())
                .Returns(response);

            // Act:
            IActionResult result = await _controller.GetUserByUserName(userName, CancellationToken.None);

            // Assert:
            result.Should().BeOfType<OkObjectResult>();

            OkObjectResult okResult = (OkObjectResult)result;
            okResult.Value.Should().Be(response);

            await _mediator
                .Received(1)
                .Send(
                    Arg.Is<GetUserByUserNameQuery>((query) => query.UserName == userName),
                    Arg.Any<CancellationToken>()
                );
        }

        [Fact]
        public async Task GetUserByEmail_ShouldSendCorrectQuery()
        {
            // Arrange:
            string email = "pedro@email.com";
            Result<GetUserByEmailResponse> response = Result<GetUserByEmailResponse>.Failure("Test");

            _mediator
                .Send(Arg.Any<GetUserByEmailQuery>(), Arg.Any<CancellationToken>())
                .Returns(response);

            // Act:
            IActionResult result = await _controller.GetUserByEmail(email, CancellationToken.None);

            // Assert:
            result.Should().BeOfType<OkObjectResult>();

            OkObjectResult okResult = (OkObjectResult)result;
            okResult.Value.Should().Be(response);

            await _mediator
                .Received(1)
                .Send(
                    Arg.Is<GetUserByEmailQuery>((query) => query.Email == email),
                    Arg.Any<CancellationToken>()
                );
        }

        [Fact]
        public async Task GetUserByPhone_ShouldSendCorrectQuery()
        {
            // Arrange:
            string phone = "11999999999";
            Result<GetUserByPhoneResponse> response = Result<GetUserByPhoneResponse>.Failure("Test");

            _mediator
                .Send(Arg.Any<GetUserByPhoneQuery>(), Arg.Any<CancellationToken>())
                .Returns(response);

            // Act:
            IActionResult result = await _controller.GetUserByPhone(phone, CancellationToken.None);

            // Assert:
            result.Should().BeOfType<OkObjectResult>();

            OkObjectResult okResult = (OkObjectResult)result;
            okResult.Value.Should().Be(response);

            await _mediator
                .Received(1)
                .Send(
                    Arg.Is<GetUserByPhoneQuery>((query) => query.Phone == phone),
                    Arg.Any<CancellationToken>()
                );
        }

        [Fact]
        public async Task CreateUser_ShouldSendCommand()
        {
            // Arrange:
            CreateUserCommand command = default!;
            Result<CreateUserResponse> response = Result<CreateUserResponse>.Failure("Test");

            _mediator
                .Send(Arg.Any<CreateUserCommand>(), Arg.Any<CancellationToken>())
                .Returns(response);

            // Act:
            IActionResult result = await _controller.CreateUser(command, CancellationToken.None);

            // Assert:
            result.Should().BeOfType<OkObjectResult>();

            OkObjectResult okResult = (OkObjectResult)result;
            okResult.Value.Should().Be(response);

            await _mediator
                .Received(1)
                .Send(command, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task UpdateUser_ShouldUseUserIdFromRoute()
        {
            // Arrange:
            Guid userId = Guid.NewGuid();

            UpdateUserCommand command = new(
                Guid.NewGuid(),
                "Pedro",
                "Silva",
                DateOnly.FromDateTime(DateTime.Today.AddYears(-25)),
                "pedro@email.com",
                "11999999999",
                true
            );

            Result<UpdateUserResponse> response = Result<UpdateUserResponse>.Failure("Test");

            _mediator
                .Send(Arg.Any<UpdateUserCommand>(), Arg.Any<CancellationToken>())
                .Returns(response);

            // Act:
            IActionResult result = await _controller.UpdateUser(
                userId,
                command,
                CancellationToken.None
            );

            // Assert:
            result.Should().BeOfType<OkObjectResult>();

            OkObjectResult okResult = (OkObjectResult)result;
            okResult.Value.Should().Be(response);

            await _mediator
                .Received(1)
                .Send(
                    Arg.Is<UpdateUserCommand>((request) => request.Id == userId),
                    Arg.Any<CancellationToken>()
                );
        }

        [Fact]
        public async Task DeleteUser_ShouldSendCommandWithUserId()
        {
            // Arrange:
            Guid userId = Guid.NewGuid();
            Result<DeleteUserResponse> response = Result<DeleteUserResponse>.Failure("Test");

            _mediator
                .Send(Arg.Any<DeleteUserCommand>(), Arg.Any<CancellationToken>())
                .Returns(response);

            // Act:
            IActionResult result = await _controller.DeleteUser(userId, CancellationToken.None);

            // Assert:
            result.Should().BeOfType<OkObjectResult>();

            OkObjectResult okResult = (OkObjectResult)result;
            okResult.Value.Should().Be(response);

            await _mediator
                .Received(1)
                .Send(
                    Arg.Is<DeleteUserCommand>((command) => command.Id == userId),
                    Arg.Any<CancellationToken>()
                );
        }
    }
}