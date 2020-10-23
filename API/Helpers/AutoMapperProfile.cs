using System.Linq;
using API.DTOs;
using API.Entities;
using API.Extensions;
using AutoMapper;

namespace API.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<AppUser, MemberDto>()
            .ForMember(
                dest => dest.PhotoUrl,
                option => option.MapFrom(
                    src => src.Photos.FirstOrDefault(x => x.IsMain).FileName
                )
                )
            .ForMember(
                dest => dest.Age,
                option => option.MapFrom(
                    src => src.DateOfBirth.CalculateAge()
                )
            );
            CreateMap<Photo, PhotoDto>();
        }
    }
}