using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Users.Application.Common.Results;
using Users.Application.DTO;
using Users.Application.Features.UserFeatures.Get.GetByPhone;
using Users.Domain.Entities;
using Users.Domain.Interfaces.Repositories;

namespace Users.Application.Tests.Tests.Features.UserFeatures.Get.GetByPhone
{
    public class GetUserByPhoneHandlerTests
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly GetUserByPhoneHandler _userHandler;

        public GetUserByPhoneHandlerTests()
        {
            _userRepository = Substitute.For<IUserRepository>();
            _mapper = Substitute.For<IMapper>();

            _userHandler = new GetUserByPhoneHandler(
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

            GetUserByPhoneQuery query = new(user.Phone);
            UserDto userDto = UserDto.FromEntity(user);

            _userRepository
                .GetUserByPhoneAsync(query.Phone, Arg.Any<CancellationToken>())
                .Returns(user);

            _mapper
                .Map<UserDto>(user)
                .Returns(userDto);

            // Act:
            Result<GetUserByPhoneResponse> result = await _userHandler.Handle(
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
                .GetUserByPhoneAsync(query.Phone, Arg.Any<CancellationToken>());

            _mapper
                .Received(1)
                .Map<UserDto>(user);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenUserDoesNotExist()
        {
            // Arrange:
            GetUserByPhoneQuery query = new("11999999999");

            _userRepository
                .GetUserByPhoneAsync(query.Phone, Arg.Any<CancellationToken>())
                .Returns((User?)null);

            // Act:
            Result<GetUserByPhoneResponse> result = await _userHandler.Handle(
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
    }
}