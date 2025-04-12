using AutoMapper;
using PSK2025.Models.DTOs;
using PSK2025.Models.Entities;

namespace PSK2025.ApiService.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>();
            
            CreateMap<RegisterDto, User>();
                
            CreateMap<UpdateUserDto, User>();
        }
    }
}