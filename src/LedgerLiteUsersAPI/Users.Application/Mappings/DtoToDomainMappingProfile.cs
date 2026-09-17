using AutoMapper;
using Users.Application.DTO;
using Users.Domain.Entities;

namespace Users.Application.Mappings
{
    public class DtoToDomainMappingProfile : Profile
    {
        public DtoToDomainMappingProfile()
        {
            CreateMap<BankAccountDto, BankAccount>()
                .ConstructUsing(
                    (dto) => new BankAccount(
                        dto.BankName,
                        dto.Holder,
                        dto.AccountNumber,
                        dto.Agency,
                        dto.BankAccountType,
                        Guid.Empty
                    )
                )
                .ForMember(
                    (destination) => destination.Id,
                    (options) => options.MapFrom((src) => src.Id)
                )
                .ForMember(
                    (destination) => destination.UserId,
                    (options) => options.Ignore()
                )
                .ForMember(
                    (destination) => destination.User,
                    (options) => options.Ignore()
                )
                .ForMember(
                    (destination) => destination.CreatedBy,
                    (options) => options.Ignore()
                )
                .ForMember(
                    (destination) => destination.CreatedOnUtc,
                    (options) => options.Ignore()
                )
                .ForMember(
                    (destination) => destination.ModifiedBy,
                    (options) => options.Ignore()
                )
                .ForMember(
                    (destination) => destination.ModifiedOnUtc,
                    (options) => options.Ignore()
                );

            CreateMap<UserDto, User>()
                .ConstructUsing((dto) => new User(
                    dto.Name,
                    dto.Surname,
                    dto.Birthdate,
                    dto.Email,
                    dto.Phone,
                    dto.IsActive
                ))
                .ForMember(
                    (destination) => destination.Id,
                    (options) => options.MapFrom((src) => src.Id))
                .ForMember(
                    (destination) => destination.CreatedBy,
                    (options) => options.Ignore()
                )
                .ForMember(
                    (destination) => destination.CreatedOnUtc,
                    (options) => options.Ignore()
                )
                .ForMember(
                    (destination) => destination.ModifiedBy,
                    (options) => options.Ignore()
                )
                .ForMember(
                    (destination) => destination.ModifiedOnUtc,
                    (options) => options.Ignore()
                );
        }
    }
}