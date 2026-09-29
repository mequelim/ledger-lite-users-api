using System.Runtime.CompilerServices;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Users.Application.Common.Results;
using Users.Application.Features.BankAccountFeatures.Create;
using Users.Application.Features.BankAccountFeatures.Delete;
using Users.Application.Features.BankAccountFeatures.Get.GetAll;
using Users.Application.Features.BankAccountFeatures.Get.GetByBankName;
using Users.Application.Features.BankAccountFeatures.Get.GetById;
using Users.Application.Features.BankAccountFeatures.Get.GetByUserId;
using Users.Application.Features.BankAccountFeatures.Get.GetByUserName;
using Users.Application.Features.BankAccountFeatures.Update;
using Users.WebAPI.Controllers;

namespace Users.WebAPI.Tests.Tests.Controllers
{
    public class BankAccountsControllerTests
    {
        private readonly IMediator _mediator;
        private readonly BankAccountsController _controller;

        public BankAccountsControllerTests()
        {
            _mediator = Substitute.For<IMediator>();

            _controller = new BankAccountsController(_mediator);
        }

        [Fact]
        public async Task GetAllBankAccounts_ShouldReturnOk()
        {
            // Arrange:
            Result<GetAllBankAccountsResponse> response =
                Result<GetAllBankAccountsResponse>.Failure("Test");

            _mediator
                .Send(Arg.Any<GetAllBankAccountsQuery>(), Arg.Any<CancellationToken>())
                .Returns(response);

            // Act:
            IActionResult result = await _controller.GetAllBankAccounts(CancellationToken.None);

            // Assert:
            result.Should().BeOfType<OkObjectResult>();

            OkObjectResult okResult = (OkObjectResult)result;
            okResult.Value.Should().Be(response);

            await _mediator
                .Received(1)
                .Send(
                    Arg.Any<GetAllBankAccountsQuery>(),
                    Arg.Any<CancellationToken>()
                );
        }

        [Fact]
        public async Task GetBankAccountById_ShouldSendCorrectQuery()
        {
            // Arrange:
            Guid bankAccountId = Guid.NewGuid();

            Result<GetBankAccountByIdResponse> response =
                Result<GetBankAccountByIdResponse>.Failure("Test");

            _mediator
                .Send(Arg.Any<GetBankAccountByIdQuery>(), Arg.Any<CancellationToken>())
                .Returns(response);

            // Act:
            IActionResult result = await _controller.GetBankAccountById(
                bankAccountId,
                CancellationToken.None
            );

            // Assert:
            result.Should().BeOfType<OkObjectResult>();

            OkObjectResult okResult = (OkObjectResult)result;
            okResult.Value.Should().Be(response);

            await _mediator
                .Received(1)
                .Send(
                    Arg.Is<GetBankAccountByIdQuery>(
                        (query) => query.Id == bankAccountId
                    ),
                    Arg.Any<CancellationToken>()
                );
        }

        [Fact]
        public async Task GetBankAccountByUserId_ShouldSendCorrectQuery()
        {
            // Arrange:
            Guid userId = Guid.NewGuid();

            Result<GetBankAccountByUserIdResponse> response =
                Result<GetBankAccountByUserIdResponse>.Failure("Test");

            _mediator
                .Send(Arg.Any<GetBankAccountByUserIdQuery>(), Arg.Any<CancellationToken>())
                .Returns(response);

            // Act:
            IActionResult result = await _controller.GetBankAccountByUserId(
                userId,
                CancellationToken.None
            );

            // Assert:
            result.Should().BeOfType<OkObjectResult>();

            OkObjectResult okResult = (OkObjectResult)result;
            okResult.Value.Should().Be(response);

            await _mediator
                .Received(1)
                .Send(
                    Arg.Is<GetBankAccountByUserIdQuery>(
                        (query) => query.UserId == userId
                    ),
                    Arg.Any<CancellationToken>()
                );
        }

        [Fact]
        public async Task GetBankAccountByBankName_ShouldSendCorrectQuery()
        {
            // Arrange:
            string bankName = "Nubank";

            Result<GetBankAccountByBankNameResponse> response =
                Result<GetBankAccountByBankNameResponse>.Failure("Test");

            _mediator
                .Send(Arg.Any<GetBankAccountByBankNameQuery>(), Arg.Any<CancellationToken>())
                .Returns(response);

            // Act:
            IActionResult result = await _controller.GetBankAccountByBankName(
                bankName,
                CancellationToken.None
            );

            // Assert:
            result.Should().BeOfType<OkObjectResult>();

            OkObjectResult okResult = (OkObjectResult)result;
            okResult.Value.Should().Be(response);

            await _mediator
                .Received(1)
                .Send(
                    Arg.Is<GetBankAccountByBankNameQuery>(
                        (query) => query.BankName == bankName
                    ),
                    Arg.Any<CancellationToken>()
                );
        }

        [Fact]
        public async Task GetBankAccountByUserName_ShouldSendCorrectQuery()
        {
            // Arrange:
            string userName = "Pedro";

            Result<GetBankAccountByUserNameResponse> response =
                Result<GetBankAccountByUserNameResponse>.Failure("Test");

            _mediator
                .Send(Arg.Any<GetBankAccountByUserNameQuery>(), Arg.Any<CancellationToken>())
                .Returns(response);

            // Act:
            IActionResult result = await _controller.GetBankAccountByUserName(
                userName,
                CancellationToken.None
            );

            // Assert:
            result.Should().BeOfType<OkObjectResult>();

            OkObjectResult okResult = (OkObjectResult)result;
            okResult.Value.Should().Be(response);

            await _mediator
                .Received(1)
                .Send(
                    Arg.Is<GetBankAccountByUserNameQuery>(
                        (query) => query.UserName == userName
                    ),
                    Arg.Any<CancellationToken>()
                );
        }

        [Fact]
        public async Task CreateBankAccount_ShouldSendCommand()
        {
            // Arrange:
            CreateBankAccountCommand command = default!;

            Result<CreateBankAccountResponse> response =
                Result<CreateBankAccountResponse>.Failure("Test");

            _mediator
                .Send(Arg.Any<CreateBankAccountCommand>(), Arg.Any<CancellationToken>())
                .Returns(response);

            // Act:
            IActionResult result = await _controller.CreateBankAccount(
                command,
                CancellationToken.None
            );

            // Assert:
            result.Should().BeOfType<OkObjectResult>();

            OkObjectResult okResult = (OkObjectResult)result;
            okResult.Value.Should().Be(response);

            await _mediator
                .Received(1)
                .Send(command, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task UpdateBankAccount_ShouldUseBankAccountIdFromRoute()
        {
            // Arrange:
            Guid bankAccountId = Guid.NewGuid();

            UpdateBankAccountCommand command =
                (UpdateBankAccountCommand)RuntimeHelpers.GetUninitializedObject(
                    typeof(UpdateBankAccountCommand)
                );

            Result<UpdateBankAccountResponse> response =
                Result<UpdateBankAccountResponse>.Failure("Test");

            _mediator
                .Send(Arg.Any<UpdateBankAccountCommand>(), Arg.Any<CancellationToken>())
                .Returns(response);

            // Act:
            IActionResult result = await _controller.UpdateBankAccount(
                bankAccountId,
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
                    Arg.Is<UpdateBankAccountCommand>(
                        (request) => request.Id == bankAccountId
                    ),
                    Arg.Any<CancellationToken>()
                );
        }

        [Fact]
        public async Task DeleteBankAccount_ShouldSendCommandWithBankAccountId()
        {
            // Arrange:
            Guid bankAccountId = Guid.NewGuid();

            Result<DeleteBankAccountResponse> response =
                Result<DeleteBankAccountResponse>.Failure("Test");

            _mediator
                .Send(Arg.Any<DeleteBankAccountCommand>(), Arg.Any<CancellationToken>())
                .Returns(response);

            // Act:
            IActionResult result = await _controller.DeleteBankAccount(
                bankAccountId,
                CancellationToken.None
            );

            // Assert:
            result.Should().BeOfType<OkObjectResult>();

            OkObjectResult okResult = (OkObjectResult)result;
            okResult.Value.Should().Be(response);

            await _mediator
                .Received(1)
                .Send(
                    Arg.Is<DeleteBankAccountCommand>(
                        (command) => command.Id == bankAccountId
                    ),
                    Arg.Any<CancellationToken>()
                );
        }
    }
}