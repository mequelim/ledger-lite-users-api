using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Users.Application.Common.Results;
using Users.Application.DTO;
using Users.Application.Features.UserFeatures.Get.GetAllActiveUsers;
using Users.Domain.Entities;
using Users.Domain.Interfaces.Repositories;

namespace Users.Application.Tests.Tests.Features.UserFeatures.Get.GetAllActiveUsers
{
    public class GetAllActiveUsersHandlerTests
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly GetAllActiveUsersHandler _userHandler;

        public GetAllActiveUsersHandlerTests()
        {
            _userRepository = Substitute.For<IUserRepository>();
            _mapper = Substitute.For<IMapper>();

            _userHandler = new GetAllActiveUsersHandler(
                _userRepository,
                _mapper
            );
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenActiveUsersExist()
        {
            // Arrange:
            GetAllActiveUsersQuery query = new();

            List<User> users =
            [
                new User(
                    "Pedro",
                    "Silva",
                    DateOnly.FromDateTime(DateTime.Today.AddYears(-30)),
                    "pedro@email.com",
                    "11999999999",
                    true
                ),
                new User(
                    "Joao",
                    "Santos",
                    DateOnly.FromDateTime(DateTime.Today.AddYears(-25)),
                    "joao@email.com",
                    "11999999999",
                    true
                )
            ];

            List<UserDto> usersDto =
            [
                UserDto.FromEntity(users[0]),
                UserDto.FromEntity(users[1])
            ];

            _userRepository
                .GetAllActiveUsersAsync(Arg.Any<CancellationToken>())
                .Returns(users);

            _mapper
                .Map<IEnumerable<UserDto>>(users)
                .Returns(usersDto);

            // Act:
            Result<GetAllActiveUsersResponse> result = await _userHandler.Handle(
                query,
                CancellationToken.None
            );

            // Assert:
            result.IsSuccess.Should().BeTrue();
            result.ErrorMessage.Should().BeNullOrEmpty();

            result.Value.Should().NotBeNull();
            result.Value.Users.Should().HaveCount(2);
            result.Value.Users.Should().BeEquivalentTo(usersDto);

            await _userRepository
                .Received(1)
                .GetAllActiveUsersAsync(Arg.Any<CancellationToken>());

            _mapper
                .Received(1)
                .Map<IEnumerable<UserDto>>(users);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenNoActiveUsersExist()
        {
            // Arrange:
            GetAllActiveUsersQuery query = new();

            List<User> users = [];
            List<UserDto> usersDto = [];

            _userRepository
                .GetAllActiveUsersAsync(Arg.Any<CancellationToken>())
                .Returns(users);

            _mapper
                .Map<IEnumerable<UserDto>>(users)
                .Returns(usersDto);

            // Act:
            Result<GetAllActiveUsersResponse> result = await _userHandler.Handle(
                query,
                CancellationToken.None
            );

            // Assert:
            result.IsSuccess.Should().BeTrue();
            result.ErrorMessage.Should().BeNullOrEmpty();

            result.Value.Should().NotBeNull();
            result.Value.Users.Should().BeEmpty();

            await _userRepository
                .Received(1)
                .GetAllActiveUsersAsync(Arg.Any<CancellationToken>());

            _mapper
                .Received(1)
                .Map<IEnumerable<UserDto>>(users);
        }
    }
}