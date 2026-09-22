using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Users.Application.Common.Results;
using Users.Application.Features.BankAccountFeatures.Delete;
using Users.Domain.Exceptions.BankAccount;
using Users.Domain.Interfaces.Repositories;
using Users.Persistence.Database;

namespace Users.Application.Tests.Tests.Features.BankAccountFeatures.Delete
{
    public class DeleteBankAccountHandlerTests : IDisposable
    {
        private readonly IBankAccountRepository _bankAccountRepository;
        private readonly AppDbContext _databaseContext;
        private readonly DeleteBankAccountHandler _bankAccountHandler;

        // Constructor:
        public DeleteBankAccountHandlerTests()
        {
            _bankAccountRepository = Substitute.For<IBankAccountRepository>();

            DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _databaseContext = new AppDbContext(options);
            _bankAccountHandler = new DeleteBankAccountHandler(_bankAccountRepository, _databaseContext);
        }

        // Methods:
        private static DeleteBankAccountCommand CreateValidCommand(Guid accountId) => new(accountId);

        public void Dispose()
        {
            _databaseContext.Database.EnsureDeleted();
            _databaseContext.Dispose();

            GC.SuppressFinalize(this);
        }

        // Tests:
        [Fact]
        public async Task Handle_ShouldDeleteBankAccount_WhenCommandIsValid()
        {
            // Arrange:
            Guid accountId = Guid.NewGuid();
            DeleteBankAccountCommand command = CreateValidCommand(accountId);

            // Act:
            Result<DeleteBankAccountResponse> result = await _bankAccountHandler.Handle(command, CancellationToken.None);

            // Assert:
            result.IsSuccess.Should().BeTrue();
            result.ErrorMessage.Should().BeNullOrEmpty();

            result.Value.Should().NotBeNull();
            result.Value.Id.Should().Be(accountId);

            await _bankAccountRepository
                .Received(1)
                .DeleteAsync(
                    accountId,
                    Arg.Any<CancellationToken>()
                );
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenBankAccountDoesNotExist()
        {
            // Arrange:
            Guid accountId = Guid.NewGuid();

            DeleteBankAccountCommand command = CreateValidCommand(accountId);

            _bankAccountRepository
                .When(
                    (repository) => repository.DeleteAsync(command.Id, Arg.Any<CancellationToken>())
                )
                .Do(
                    (_) => throw new BankAccountNotFoundException("Id", command.Id)
                );

            // Act:
            Result<DeleteBankAccountResponse> result = await _bankAccountHandler.Handle(command, CancellationToken.None);

            // Assert:
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().NotBeNullOrWhiteSpace();

            await _bankAccountRepository
                .Received(1)
                .DeleteAsync(accountId, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_ShouldCallRepositoryDeleteOnlyOnce_WhenCommandIsValid()
        {
            // Arrange:
            Guid accountId = Guid.NewGuid();
            DeleteBankAccountCommand command = CreateValidCommand(accountId);

            // Act:
            await _bankAccountHandler.Handle(command, CancellationToken.None);

            // Assert:
            await _bankAccountRepository
                .Received(1)
                .DeleteAsync(accountId, Arg.Any<CancellationToken>());

            await _bankAccountRepository
                .DidNotReceive()
                .DeleteAsync(
                    Arg.Is<Guid>(id => id != accountId),
                    Arg.Any<CancellationToken>()
                );
        }

        [Fact]
        public async Task Handle_ShouldReturnDeletedBankAccountId_WhenDeletionSucceeds()
        {
            // Arrange:
            Guid accountId = Guid.NewGuid();
            DeleteBankAccountCommand command = CreateValidCommand(accountId);

            // Act:
            Result<DeleteBankAccountResponse> result = await _bankAccountHandler.Handle(command, CancellationToken.None);

            // Assert:
            result.IsSuccess.Should().BeTrue();
            result.Value.Id.Should().Be(command.Id);
        }
    }
}