using CityInfo.API.DbContexts;
using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.Services
{
    public class CityInfoRepository : ICityInfoRepository
    {
        private readonly CityInfoContext context;

        public CityInfoRepository(CityInfoContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<bool> CityExistsAsync(int cityId)
        {
            return await this.context.Cities.AnyAsync(c => c.Id == cityId);
        }

        public async Task<IEnumerable<City>> GetCitiesAsync()
        {
            return await context.Cities.OrderBy(c => c.Name).ToListAsync();
        }

        public async Task<City?> GetCityAsync(int cityId, bool includePOIs)
        {
            if (includePOIs)
            {
                return await context.Cities.Include(c => c.PointsOfInterest).Where(c => c.Id == cityId).FirstOrDefaultAsync();
            }

            return await context.Cities.Where(c => c.Id == cityId).FirstOrDefaultAsync();
        }

        public async Task<PointOfInterest?> GetPointOfInterestForCityAsync(int cityId, int poiId)
        {
            return await context.PointsOfInterests.Where(poi => poi.Id == poiId && poi.CityId == cityId).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<PointOfInterest>> GetPointsOfInterestsForCityAsync(int cityId)
        {
            return await context.PointsOfInterests.Where(poi => poi.CityId == cityId).ToListAsync();
        }
    }
}
