using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Users.Application.Common.Results;
using Users.Application.DTO;
using Users.Application.Features.UserFeatures.Get.GetById;
using Users.Domain.Entities;
using Users.Domain.Interfaces.Repositories;

namespace Users.Application.Tests.Tests.Features.UserFeatures.Get.GetById
{
    public class GetUserByIdHandlerTests
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly GetUserByIdHandler _userHandler;

        public GetUserByIdHandlerTests()
        {
            _userRepository = Substitute.For<IUserRepository>();
            _mapper = Substitute.For<IMapper>();

            _userHandler = new GetUserByIdHandler(
                _userRepository,
                _mapper
            );
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenUserExists()
        {
            // Arrange:
            User user = new(
                "Pedro",
                "Silva",
                DateOnly.FromDateTime(DateTime.Today.AddYears(-30)),
                "pedro@email.com",
                "11999999999",
                true
            );

            GetUserByIdQuery query = new(user.Id);
            UserDto userDto = UserDto.FromEntity(user);

            _userRepository
                .GetUserByIdAsync(query.Id, Arg.Any<CancellationToken>())
                .Returns(user);

            _mapper
                .Map<UserDto>(user)
                .Returns(userDto);

            // Act:
            Result<GetUserByIdResponse> result = await _userHandler.Handle(
                query,
                CancellationToken.None
            );

            // Assert:
            result.IsSuccess.Should().BeTrue();
            result.ErrorMessage.Should().BeNullOrEmpty();

            result.Value.Should().NotBeNull();
            result.Value.User.Should().BeEquivalentTo(userDto);

            await _userRepository
                .Received(1)
                .GetUserByIdAsync(query.Id, Arg.Any<CancellationToken>());

            _mapper
                .Received(1)
                .Map<UserDto>(user);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenUserDoesNotExist()
        {
            // Arrange:
            Guid userId = Guid.NewGuid();

            GetUserByIdQuery query = new(userId);

            _userRepository
                .GetUserByIdAsync(query.Id, Arg.Any<CancellationToken>())
                .Returns((User?)null);

            // Act:
            Result<GetUserByIdResponse> result = await _userHandler.Handle(
                query,
                CancellationToken.None
            );

            // Assert:
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("User not found!");

            _mapper
                .DidNotReceive()
                .Map<UserDto>(Arg.Any<User>());
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenUserIdIsEmpty()
        {
            // Arrange:
            GetUserByIdQuery query = new(Guid.Empty);

            // Act:
            Result<GetUserByIdResponse> result = await _userHandler.Handle(
                query,
                CancellationToken.None
            );

            // Assert:
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("The user id cannot be null!");

            await _userRepository
                .DidNotReceive()
                .GetUserByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
        }
    }
}