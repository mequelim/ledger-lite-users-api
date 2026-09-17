using AutoMapper;
using Users.Application.DTO;
using Users.Domain.Entities;

namespace Users.Application.Mappings
{
    public class DomainToDtoMappingProfile : Profile
    {
        public DomainToDtoMappingProfile()
        {
            CreateMap<BankAccount, BankAccountDto>()
                .ForMember(
                    (destination) => destination.BankName,
                    (options) => options.MapFrom((src) => src.BankName)
                );
            CreateMap<User, UserDto>();
        }
    }
}