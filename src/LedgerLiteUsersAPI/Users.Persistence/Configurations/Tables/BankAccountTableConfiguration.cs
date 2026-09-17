using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Users.Domain.Entities;

namespace Users.Persistence.Configurations.Tables
{
    public class BankAccountTableConfiguration : IEntityTypeConfiguration<BankAccount>
    {
        /// <summary>
        /// Configures the entity framework settings for the BankAccount entity.
        /// </summary>
        /// <param name="builder">The builder being used to configure the BankAccount entity.</param>
        public void Configure(EntityTypeBuilder<BankAccount> builder)
        {
            // Table's name:
            builder.ToTable("bank_accounts");

            // ID (Primary Key [FK]):
            builder.HasKey((bankAccount) => bankAccount.Id);

            builder
                .Property((bankAccount) => bankAccount.Id)
                .HasColumnName("bank_account_id")
                .IsRequired();

            // Other fields:
            builder
                .Property((bankAccount) => bankAccount.AccountNumber)
                .HasMaxLength(20)
                .IsRequired();

            builder
                .Property((bankAccount) => bankAccount.Agency)
                .HasMaxLength(12)
                .IsRequired();

            builder
                .Property((bankAccount) => bankAccount.BankAccountType)
                .IsRequired();

            builder
                .Property((bankAccount) => bankAccount.BankName)
                .HasMaxLength(100)
                .IsRequired();

            builder
                .Property((bankAccount) => bankAccount.Holder)
                .HasMaxLength(200);

            // Foreign Key (FK):
            builder
                .Property((bankAccount) => bankAccount.UserId)
                .IsRequired();

            // Dates:
            builder
                .Property<DateTime>("created_at")
                .HasDefaultValueSql("NOW()")
                .IsRequired();

            builder
                .Property<DateTime>("updated_at")
                .HasDefaultValueSql("NOW()")
                .IsRequired();
        }
    }
}