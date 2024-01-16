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

        public async Task<IEnumerable<City>> GetCitiesAsync(string? name, string? searchQuery, int pageNumber, int pageSize)
        {
            var collection = context.Cities as IQueryable<City>;

            if (!string.IsNullOrEmpty(name))
            {
                name = name.Trim();
                collection = collection.Where(c => c.Name == name);
            }

            if (!string.IsNullOrEmpty(searchQuery))
            {
                searchQuery = searchQuery.Trim().ToLower(); ;
                collection = collection.Where(c => c.Name.ToLower().Contains(searchQuery)
                || (c.Description != null && c.Description.ToLower().Contains(searchQuery)));
            }

            return await collection.OrderBy(c => c.Name)
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<City?> GetCityAsync(int cityId, bool includePOIs = false)
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


        public async Task AddPOIForCityAsync(int cityId, PointOfInterest POI)
        {
            var city = await GetCityAsync(cityId);

            if (city != null)
            {
                city.PointsOfInterest.Add(POI);
            }
        }

        public void DeletePOI(PointOfInterest poi)
        {
            context.PointsOfInterests.Remove(poi);
        }

        public async Task<bool> SaveChangesASync()
        {
             return (await context.SaveChangesAsync() >= 0);
        }
    }
}
