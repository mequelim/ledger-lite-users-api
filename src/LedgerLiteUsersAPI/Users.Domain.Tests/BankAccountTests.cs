using Users.Domain.Entities;
using Users.Domain.Entities.Enums;
using Users.Domain.Exceptions.BankAccount;
using Users.Domain.Tests.Mocks;

namespace Users.Domain.Tests
{
    public class BankAccountTests
    {
        // Success cases:
        [Theory]
        [InlineData("12345-6", "1234")]
        [InlineData("010234678-7", "1234-6")]
        [InlineData("123456789012-X", "1234-x")]
        public void Create_WhenDataIsValid_BankAccountShouldBeCreated(string accountNumber, string agency)
        {
            // Arrange & Act:
            Guid userId = Guid.NewGuid();
            BankAccount bankAccount = new(
                "Itaú",
                "Pedro Mequelim",
                accountNumber,
                agency,
                BankAccountType.Checking,
                userId
            );

            // Assert:
            Assert.NotNull(bankAccount);
            Assert.Equal("Itaú", bankAccount.BankName);
            Assert.Equal("Pedro Mequelim", bankAccount.Holder);
            Assert.Equal(accountNumber, bankAccount.AccountNumber);
            Assert.Equal(agency, bankAccount.Agency);
            Assert.Equal(BankAccountType.Checking, bankAccount.BankAccountType);
            Assert.Equal(userId, bankAccount.UserId);
        }

        [Fact]
        public void Create_WhenHolderIsnull_BankAccountShouldBeCreate()
        {
            // Arrange & Act:
            Guid userId = Guid.NewGuid();
            BankAccount bankAccount = BankAccountFactory.CreateWithoutHolder();

            // Assert:
            Assert.NotNull(bankAccount);
            Assert.Null(bankAccount.Holder);
        }

        // Failed Cases:
        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("1234")]
        [InlineData("1234567890123")]
        [InlineData("12345-")]
        [InlineData("12345-AB")]
        public void Create_WhenAccountNumberIsInvalid_ShouldThrowInvalidBankAccountAccountNumberException(string invalidAccountNumber)
        {
            // Arrange & Act:
            InvalidAccountNumberException exception = Assert.Throws<InvalidAccountNumberException>(() => new BankAccount(
                "Itaú",
                "Titular",
                invalidAccountNumber,
                "1234",
                BankAccountType.Checking,
                Guid.NewGuid()
            ));

            // Assert:
            Assert.Equal(invalidAccountNumber, exception.AccountNumber);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("123")]
        [InlineData("1234567890123")]
        [InlineData("12345-")]
        [InlineData("12345-AB")]
        public void Create_WhenAgencyIsInvalid_ShouldThrowInvalidBankAccountAgencyException(string invalidAgency)
        {
            // Arrange & Act:
            InvalidAgencyException exception = Assert.Throws<InvalidAgencyException>(() => new BankAccount(
                "Bradesco",
                "Titular",
                "12345-6",
                invalidAgency,
                BankAccountType.Business,
                Guid.NewGuid()
            ));

            // Assert:
            Assert.Equal(invalidAgency, exception.Agency);
        }
    }
}