using System.Linq;
using AutoMapper;
using DatingApp.Api.Dtos;
using DatingApp.Api.Models;

namespace DatingApp.Api.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserForList>().ForMember(dest => dest.PhotoUrl, 
            opt => opt.MapFrom(scr => scr.Photos.FirstOrDefault(p => p.IsMain).Url)).ForMember(dest=>dest.Age,
            opt =>opt.MapFrom(src =>src.DateOfBirth.CalculateAge()));
            
            CreateMap<User, UserForDetailed>().ForMember(dest =>dest.PhotoUrl 
            , opt =>opt.MapFrom(src=>src.Photos.FirstOrDefault(p=>p.IsMain).Url)).ForMember(dest=>dest.Age,
            opt=>opt.MapFrom(src=>src.DateOfBirth.CalculateAge())
            );
            CreateMap<Photo, PhotoForDetailed>();
        }
    }
}