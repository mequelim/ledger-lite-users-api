using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Users.Application.Common.Results;
using Users.Application.DTO;
using Users.Application.Features.UserFeatures.Get.GetAll;
using Users.Domain.Entities;
using Users.Domain.Interfaces.Repositories;

namespace Users.Application.Tests.Tests.Features.UserFeatures.Get.GetAll
{
    public class GetAllUsersHandlerTests
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly GetAllUsersHandler _userHandler;

        public GetAllUsersHandlerTests()
        {
            _userRepository = Substitute.For<IUserRepository>();
            _mapper = Substitute.For<IMapper>();
            _userHandler = new GetAllUsersHandler(
                _userRepository,
                _mapper
            );
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenUsersExist()
        {
            // Arrange:
            GetAllUsersQuery query = new();

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
                    "João",
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
                .GetAllAsync(Arg.Any<CancellationToken>())
                .Returns(users);

            _mapper
                .Map<IEnumerable<UserDto>>(users)
                .Returns(usersDto);

            // Act:
            Result<GetAllUsersResponse> result = await _userHandler.Handle(query, CancellationToken.None);

            // Assert:
            result.IsSuccess.Should().BeTrue();
            result.ErrorMessage.Should().BeNullOrEmpty();

            result.Value.Should().NotBeNull();
            result.Value.Users.Should().HaveCount(2);
            result.Value.Users.Should().BeEquivalentTo(usersDto);

            await _userRepository
                .Received(1)
                .GetAllAsync(Arg.Any<CancellationToken>());

            _mapper
                .Received(1)
                .Map<IEnumerable<UserDto>>(users);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenNoUsersExist()
        {
            // Arrange:
            GetAllUsersQuery query = new();

            List<User> users = [];
            List<UserDto> usersDto = [];

            _userRepository
                .GetAllAsync(Arg.Any<CancellationToken>())
                .Returns(users);

            _mapper
                .Map<IEnumerable<UserDto>>(users)
                .Returns(usersDto);

            // Act:
            Result<GetAllUsersResponse> result = await _userHandler.Handle(query, CancellationToken.None);

            // Assert:
            result.IsSuccess.Should().BeTrue();
            result.ErrorMessage.Should().BeNullOrEmpty();

            result.Value.Should().NotBeNull();
            result.Value.Users.Should().BeEmpty();

            await _userRepository
                .Received(1)
                .GetAllAsync(Arg.Any<CancellationToken>());

            _mapper
                .Received(1)
                .Map<IEnumerable<UserDto>>(users);
        }
    }
}