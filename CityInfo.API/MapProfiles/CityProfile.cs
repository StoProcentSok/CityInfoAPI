using AutoMapper;

namespace CityInfo.API.MapProfiles
{
    public class CityProfile : Profile
    {
        public CityProfile()
        {
            CreateMap<Entities.City, Models.CityWithoutPOIsDTO>();
            CreateMap<Entities.City, Models.CityDto>();
            
        }
    }
}
