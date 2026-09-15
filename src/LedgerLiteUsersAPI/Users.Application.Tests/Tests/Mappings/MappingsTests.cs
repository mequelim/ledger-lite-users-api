using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Users.Application.DTOs;
using Users.Application.Mappings;
using Users.Application.Tests.Mocks;
using Users.Domain.Entities;
using Users.Domain.Entities.Enums;

namespace Users.Application.Tests.Tests.Mappings
{
    public class MappingsTests
    {
        private readonly IMapper _mapper;
        private readonly MapperConfiguration _mapperConfiguration;

        // Constructor:
        public MappingsTests()
        {
            _mapperConfiguration = new MapperConfiguration(
                configure: (config) =>
                {
                    config.AddProfile<DomainToDtoMappingProfile>();
                    config.AddProfile<DtoToDomainMappingProfile>();
                },
                loggerFactory: NullLoggerFactory.Instance
            );
            _mapper = _mapperConfiguration.CreateMapper();
        }

        // Tests:
        // SUCCESS CASES:
        // Mapper configuration:
        [Fact]
        public void MapperConfiguration_ShouldBeValid()
        {
            // Arrange, Act & Assert:
            _mapperConfiguration
                .Invoking((config) => config.AssertConfigurationIsValid())
                .Should()
                .NotThrow();
        }

        // User:
        [Fact]
        public void Map_UserToUserDto_ShouldMapCorrectly_WhenUserIsValidAndUserIsActive()
        {
            // Arrange:
            User user = new UserBuilder().Build();

            // Act:
            UserDto userDto = _mapper.Map<UserDto>(user);

            // Assert:
            userDto.Should().NotBeNull();
            userDto.Name.Should().Be("Pedro");
            userDto.Surname.Should().Be("Mequelim");
            userDto.Birthdate.Should().Be(new DateOnly(2002, 02, 15));
            userDto.Email.Should().Be("pedro@email.com");
            userDto.Phone.Should().Be("+55 (41) 9 1234-4567");
            userDto.IsActive.Should().BeTrue();
        }

        [Fact]
        public void Map_UserToUserDto_ShouldMapCorrectly_WhenUserIsValidAndUserIsInactive()
        {
            // Arrange:
            User user = new UserBuilder().Build();
            user.IsActive = false;

            // Act:
            UserDto userDto = _mapper.Map<UserDto>(user);

            // Assert:
            userDto.IsActive.Should().BeFalse();
        }

        [Fact]
        public void Map_UserToUserDto_ShouldMapBankAccountCorrectly_WhenUserHasBankAccount()
        {
            // Arrange:
            User user = new UserBuilder().Build();
            BankAccount bankAccount = user.BankAccounts.First();

            // Act:
            UserDto userDto = _mapper.Map<UserDto>(user);
            BankAccountDto bankAccountDto = userDto.BankAccounts.First();

            // Assert:
            bankAccountDto.Should().NotBeNull();
            bankAccountDto.Id.Should().Be(bankAccount.Id);
            bankAccountDto.BankName.Should().Be(bankAccount.BankName);
            bankAccountDto.Holder.Should().Be(bankAccount.Holder);
            bankAccountDto.AccountNumber.Should().Be(bankAccount.AccountNumber);
            bankAccountDto.Agency.Should().Be(bankAccount.Agency);
            bankAccountDto.BankAccountType.Should().Be(bankAccount.BankAccountType);
        }

        [Fact]
        public void Map_UserToUserDto_ShouldMapCorrectly_WhenBankAccountHolderIsNull()
        {
            // Arrange:
            User user = new UserBuilder()
                .WithBankAccount(BankAccountFactory.CreateWithoutHolder())
                .Build();

            // Act:
            UserDto userDto = _mapper.Map<UserDto>(user);

            // Assert:
            userDto.BankAccounts.Should().ContainSingle();
            userDto.BankAccounts.First().Holder.Should().BeNull();
        }

        [Fact]
        public void Map_BankAccountToBankAccountDto_ShouldMapCorrectly_WhenBankAccountIsValid()
        {
            // Arrange:
            BankAccount bankAccount = BankAccountFactory.CreateDefault();

            // Act:
            BankAccountDto bankAccountDto = _mapper.Map<BankAccountDto>(bankAccount);

            // Assert:
            bankAccountDto.Should().NotBeNull();
            bankAccountDto.Id.Should().Be(bankAccount.Id);
            bankAccountDto.BankName.Should().Be(bankAccount.BankName);
            bankAccountDto.Holder.Should().Be(bankAccount.Holder);
            bankAccountDto.AccountNumber.Should().Be(bankAccount.AccountNumber);
            bankAccountDto.Agency.Should().Be(bankAccount.Agency);
            bankAccountDto.BankAccountType.Should().Be(bankAccount.BankAccountType);
        }

        [Fact]
        public void Map_BankAccountToBankAccountDto_ShouldMapCorrectly_WhenHolderIsNull()
        {
            // Arrange:
            BankAccount bankAccount = BankAccountFactory.CreateWithoutHolder();

            // Act:
            BankAccountDto bankAccountDto = _mapper.Map<BankAccountDto>(bankAccount);

            // Assert:
            bankAccountDto.Holder.Should().BeNull();
        }

        [Fact]
        public void Map_UserDtoToUser_ShouldMapCorrectly_WhenUserDtoIsValid()
        {
            // Arrange:
            UserDto userDto = _mapper.Map<UserDto>(new UserBuilder().Build());

            // Act:
            User user = _mapper.Map<User>(userDto);

            // Assert:
            user.Should().NotBeNull();
            user.Id.Should().Be(userDto.Id);
            user.Name.Should().Be(userDto.Name);
            user.Surname.Should().Be(userDto.Surname);
            user.Birthdate.Should().Be(userDto.Birthdate);
            user.Email.Should().Be(userDto.Email);
            user.Phone.Should().Be(userDto.Phone);
            user.IsActive.Should().Be(userDto.IsActive);
        }

        [Fact]
        public void Map_UserDtoToUser_ShouldMapCorrectly_WhenUserDtoIsInactive()
        {
            // Arrange:
            UserDto userDto = _mapper.Map<UserDto>(new UserBuilder().Build());
            userDto.IsActive = false;

            // Act:
            User user = _mapper.Map<User>(userDto);

            // Assert:
            user.IsActive.Should().BeFalse();
        }

        [Fact]
        public void Map_UserDtoToUser_ShouldMapBankAccountCorrectly_WhenUserDtoHasBankAccount()
        {
            // Arrange:
            UserDto userDto = _mapper.Map<UserDto>(new UserBuilder().Build());
            BankAccountDto bankAccountDto = userDto.BankAccounts.First();

            // Act:
            User user = _mapper.Map<User>(userDto);
            BankAccount bankAccount = user.BankAccounts.First();

            // Assert:
            bankAccount.Should().NotBeNull();
            bankAccount.Id.Should().Be(bankAccountDto.Id);
            bankAccount.BankName.Should().Be(bankAccountDto.BankName);
            bankAccount.Holder.Should().Be(bankAccountDto.Holder);
            bankAccount.AccountNumber.Should().Be(bankAccountDto.AccountNumber);
            bankAccount.Agency.Should().Be(bankAccountDto.Agency);
            bankAccount.BankAccountType.Should().Be(bankAccountDto.BankAccountType);
        }

        // Bank Account:
        [Fact]
        public void Map_BankAccountDtoToBankAccount_ShouldMapCorrectly_WhenBankAccountDtoIsValid()
        {
            // Arrange:
            BankAccountDto bankAccountDto = _mapper.Map<BankAccountDto>(
                BankAccountFactory.CreateDefault()
            );

            // Act:
            BankAccount bankAccount = _mapper.Map<BankAccount>(bankAccountDto);

            // Assert:
            bankAccount.Should().NotBeNull();
            bankAccount.Id.Should().Be(bankAccountDto.Id);
            bankAccount.BankName.Should().Be(bankAccountDto.BankName);
            bankAccount.Holder.Should().Be(bankAccountDto.Holder);
            bankAccount.AccountNumber.Should().Be(bankAccountDto.AccountNumber);
            bankAccount.Agency.Should().Be(bankAccountDto.Agency);
            bankAccount.BankAccountType.Should().Be(bankAccountDto.BankAccountType);
        }

        [Fact]
        public void Map_BankAccountDtoToBankAccount_ShouldMapCorrectly_WhenHolderIsNull()
        {
            // Arrange:
            BankAccountDto bankAccountDto = _mapper.Map<BankAccountDto>(
                BankAccountFactory.CreateWithoutHolder()
            );

            // Act:
            BankAccount bankAccount = _mapper.Map<BankAccount>(bankAccountDto);

            // Assert:
            bankAccount.Holder.Should().BeNull();
        }

        [Fact]
        public void Map_BankAccountDtoToBankAccount_ShouldMapCorrectly_WhenBankAccountTypeIsCorrente()
        {
            // Arrange:
            BankAccountDto bankAccountDto = _mapper.Map<BankAccountDto>(
                BankAccountFactory.CreateDefault()
            );

            // Act:
            BankAccount bankAccount = _mapper.Map<BankAccount>(bankAccountDto);

            // Assert:
            bankAccount.BankAccountType.Should().Be(BankAccountType.Corrente);
        }

        [Fact]
        public void Map_UserListToUserDtoList_ShouldMapCorrectly_WhenUserListContainsItems()
        {
            // Arrange:
            List<User> users =
            [
                new UserBuilder().Build(),
                new UserBuilder().Build()
            ];

            // Act:
            List<UserDto> usersDto = _mapper.Map<List<UserDto>>(users);

            // Assert:
            usersDto.Should().NotBeNull();
            usersDto.Should().HaveCount(2);

            usersDto[0].Id.Should().Be(users[0].Id);
            usersDto[1].Id.Should().Be(users[1].Id);

            usersDto[0].Email.Should().Be(users[0].Email);
            usersDto[1].Email.Should().Be(users[1].Email);
        }

        [Fact]
        public void Map_EmptyUserListToUserDtoList_ShouldReturnEmptyCollection()
        {
            // Arrange:
            List<User> users = [];

            // Act:
            List<UserDto> usersDto = _mapper.Map<List<UserDto>>(users);

            // Assert:
            usersDto.Should().NotBeNull();
            usersDto.Should().BeEmpty();
        }

        // FAILED CASES:
        // Mapper configuration:
        [Fact]
        public void Map_NullUserToUserDto_ShouldReturnNull()
        {
            // Arrange:
            User? user = null;

            // Act:
            UserDto? userDto = _mapper.Map<UserDto?>(user);

            // Assert:
            userDto.Should().BeNull();
        }

        // User:
        [Fact]
        public void Map_NullUserDtoToUser_ShouldReturnNull()
        {
            // Arrange:
            UserDto? userDto = null;

            // Act:
            User? user = _mapper.Map<User?>(userDto);

            // Assert:
            user.Should().BeNull();
        }

        // Bank Account:
        [Fact]
        public void Map_NullBankAccountToBankAccountDto_ShouldReturnNull()
        {
            // Arrange:
            BankAccount? bankAccount = null;

            // Act:
            BankAccountDto? bankAccountDto = _mapper.Map<BankAccountDto?>(bankAccount);

            // Assert:
            bankAccountDto.Should().BeNull();
        }

        [Fact]
        public void Map_NullBankAccountDtoToBankAccount_ShouldReturnNull()
        {
            // Arrange:
            BankAccountDto? bankAccountDto = null;

            // Act:
            BankAccount? bankAccount = _mapper.Map<BankAccount?>(bankAccountDto);

            // Assert:
            bankAccount.Should().BeNull();
        }

        [Fact]
        public void Map_UserDtoToUser_ShouldMapEmptyBankAccounts_WhenBankAccountsDtoIsEmpty()
        {
            // Arrange:
            UserDto userDto = _mapper.Map<UserDto>(new UserBuilder().Build());
            userDto.BankAccounts = [];

            // Act:
            User user = _mapper.Map<User>(userDto);

            // Assert:
            user.Should().NotBeNull();
            user.BankAccounts.Should().BeEmpty();
        }
    }
}