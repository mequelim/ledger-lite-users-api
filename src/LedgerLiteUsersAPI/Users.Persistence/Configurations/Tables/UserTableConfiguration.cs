using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Users.Domain.Entities;

namespace Users.Persistence.Configurations.Tables
{
    public class UserTableConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            // Table's name:
            builder.ToTable("users");

            // ID (Primary Key [FK]):
            builder.HasKey((user) => user.Id);
            builder
                .Property((user) => user.Id)
                .HasColumnName("user_id");

            // Other field:
            builder
                .Property((user) => user.Name)
                .HasMaxLength(100)
                .IsRequired();

            builder
                .Property((user) => user.Birthdate)
                .IsRequired();

            builder
                .Property((user) => user.Surname)
                .HasMaxLength(100)
                .IsRequired();

            builder
                .Property((user) => user.Email)
                .HasMaxLength(150)
                .IsRequired();

            builder
                .Property((user) => user.Phone)
                .HasMaxLength(20)
                .IsRequired();

            builder
                .Property((user) => user.IsActive)
                .IsRequired();

            // Relationships:
            // 1:N - BankAccount (the FK is in the bank_account table):
            builder
                .HasMany((user) => user.BankAccounts)
                .WithOne((bankAccount) => bankAccount.User)
                .HasForeignKey((bankAccount) => bankAccount.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Shadow Properties (Audit Dates):
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