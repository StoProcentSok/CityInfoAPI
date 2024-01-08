using AutoMapper;

namespace CityInfo.API.MapProfiles
{
    public class POIProfile : Profile
    {
        public POIProfile()
        {
            CreateMap<Entities.PointOfInterest, Models.PointOfInterestDto>();
            CreateMap<Models.PointOfInterestCreationDto, Entities.PointOfInterest>();
            CreateMap<Models.PointOfInterestUpdatingDTO, Entities.PointOfInterest>();
        }
    }
}
