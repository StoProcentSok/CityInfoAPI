﻿using CityInfo.API.Entities;

namespace CityInfo.API.Services
{
    public interface ICityInfoRepository
    {
        Task<IEnumerable<City>> GetCitiesAsync();
        Task<(IEnumerable<City>, PaginationMetadata)> GetCitiesAsync(string? name, string? searchQuery, int pageNumber, int pageSize);
        Task<City?> GetCityAsync(int cityId, bool includePOIs = false);

        Task<IEnumerable<PointOfInterest>> GetPointsOfInterestsForCityAsync(int cityId);

        Task<PointOfInterest?> GetPointOfInterestForCityAsync(int cityId, int poiId);

        Task<bool> CityExistsAsync(int cityId);

        Task AddPOIForCityAsync(int cityId, PointOfInterest POI);

        Task<bool> SaveChangesASync();

        Task<bool> CityNameMatchesCityId(string? cityName, int cityId);

        void DeletePOI(PointOfInterest poi);
    }
}
