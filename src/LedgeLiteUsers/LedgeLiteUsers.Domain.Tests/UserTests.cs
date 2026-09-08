using LedgeLiteUsers.Domain.Entities;
using LedgeLiteUsers.Domain.Errors.UserExceptions;
using LedgeLiteUsers.Domain.Tests.Mocks;

namespace LedgeLiteUsers.Domain.Tests
{
    public class UserTests
    {
        // Success cases:
        [Fact]
        public void Create_WhenUserObjectWithAllFilledData_UserShouldBeCreate()
        {
            // Arrange & Act:
            User user = new UserBuilder().Build();

            // Assert:
            Assert.NotNull(user);
        }

        // Failed cases:
        [Theory]
        [InlineData("")]
        [InlineData("email-without-the-at-sign")]
        public void Create_WhenEmailIsInvalid_ShouldThrowInvalidUserEmailException(string invalidEmail)
        {
            // Arrange & Act:
            InvalidUserEmailException exception = Assert.Throws<InvalidUserEmailException>(
                () => new User(
                    name: "Pedro",
                    surname: "Mequelim",
                    birthdate: new DateOnly(2002, 02, 15),
                    email: invalidEmail,
                    phone: "+55 (41) 9 1234-4567",
                    isActive: true
                )
            );

            // Assert:
            Assert.Equal(invalidEmail, exception.Email);
        }
    }
}