using Users.Domain.Entities;
using Users.Domain.Entities.Enums;

namespace Users.Persistence.Tests.Mocks.Domain
{
    public static class BankAccountFactory
    {
        public static BankAccount CreateDefault(Guid? userId = null)
        {
            return new BankAccount(
                "Santander",
                "Pedro",
                "010234678-7",
                "1234",
                BankAccountType.Corrente,
                userId ?? Guid.NewGuid()
            );
        }

        public static BankAccount CreateWithoutHolder(Guid? userId = null)
        {
            return new BankAccount(
                "Santander",
                null,
                "010234678-7",
                "6789",
                BankAccountType.Corrente,
                userId ?? Guid.NewGuid()
            );
        }
    }
}