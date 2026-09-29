using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Users.Application.Common.Results;
using Users.Application.DTO;
using Users.Application.Features.UserFeatures.Get.GetAllInactiveUsers;
using Users.Domain.Entities;
using Users.Domain.Interfaces.Repositories;

namespace Users.Application.Tests.Tests.Features.UserFeatures.Get.GetAllInactiveUsers
{
    public class GetAllInactiveUsersHandlerTests
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly GetAllInactiveUsersHandler _userHandler;

        public GetAllInactiveUsersHandlerTests()
        {
            _userRepository = Substitute.For<IUserRepository>();
            _mapper = Substitute.For<IMapper>();

            _userHandler = new GetAllInactiveUsersHandler(
                _userRepository,
                _mapper
            );
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenInactiveUsersExist()
        {
            // Arrange:
            GetAllInactiveUsersQuery query = new();

            List<User> users =
            [
                new User(
                    "Pedro",
                    "Silva",
                    DateOnly.FromDateTime(DateTime.Today.AddYears(-30)),
                    "pedro@email.com",
                    "11999999999",
                    false
                ),
                new User(
                    "Joao",
                    "Santos",
                    DateOnly.FromDateTime(DateTime.Today.AddYears(-25)),
                    "joao@email.com",
                    "11999999999",
                    false
                )
            ];

            List<UserDto> usersDto =
            [
                UserDto.FromEntity(users[0]),
                UserDto.FromEntity(users[1])
            ];

            _userRepository
                .GetAllInactiveUsersAsync(Arg.Any<CancellationToken>())
                .Returns(users);

            _mapper
                .Map<IEnumerable<UserDto>>(users)
                .Returns(usersDto);

            // Act:
            Result<GetAllInactiveUsersResponse> result = await _userHandler.Handle(
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
                .GetAllInactiveUsersAsync(Arg.Any<CancellationToken>());

            _mapper
                .Received(1)
                .Map<IEnumerable<UserDto>>(users);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenNoInactiveUsersExist()
        {
            // Arrange:
            GetAllInactiveUsersQuery query = new();

            List<User> users = [];
            List<UserDto> usersDto = [];

            _userRepository
                .GetAllInactiveUsersAsync(Arg.Any<CancellationToken>())
                .Returns(users);

            _mapper
                .Map<IEnumerable<UserDto>>(users)
                .Returns(usersDto);

            // Act:
            Result<GetAllInactiveUsersResponse> result = await _userHandler.Handle(
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
                .GetAllInactiveUsersAsync(Arg.Any<CancellationToken>());

            _mapper
                .Received(1)
                .Map<IEnumerable<UserDto>>(users);
        }
    }
}