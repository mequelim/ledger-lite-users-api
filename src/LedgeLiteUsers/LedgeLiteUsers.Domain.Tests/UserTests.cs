using System.Runtime.InteropServices;
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

        [Theory]
        [InlineData("")]
        [InlineData("+55 (41) 9 1234-456")]
        public void Create_WhenPhoneIsInvalid_ShouldThrowInvalidPhoneException(string invalidPhone)
        {
            // Arrange & Act:
            InvalidUserPhoneException exception = Assert.Throws<InvalidUserPhoneException>(
                () => new User(
                    name: "Pedro",
                    surname: "Mequelim",
                    birthdate: new DateOnly(2002, 02, 15),
                    email: "pedro@gmail.com",
                    phone: invalidPhone,
                    isActive: true
                )
            );

            // Assert:
            Assert.Equal(invalidPhone, exception.Phone);
        }

        [Fact]
        public void Create_WhenAgeIsInvalid_ShouldThrowInvalidUserAgeException()
        {
            // Arrange & Act:
            DateOnly birthDate = new DateOnly(2011, 02, 15);
            InvalidUserAgeException exception = Assert.Throws<InvalidUserAgeException>(
                () => new User(
                    name: "Pedro",
                    surname: "Mequelim",
                    birthdate: birthDate,
                    email: "pedro@gmail.com",
                    phone: "+55 (41) 9 1234-4567",
                    isActive: true
                )
            );

            // Assert:
            Assert.Equal(birthDate, exception.Birthdate);
        }
    }
}