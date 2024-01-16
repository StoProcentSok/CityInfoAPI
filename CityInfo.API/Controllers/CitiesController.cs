using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CityInfo.API.Controllers
{
    [ApiController]
    [Route("api/cities")]
    public class CitiesController : ControllerBase
    {
        private readonly ICityInfoRepository _repository;
        private readonly IMapper _mapper;
        private readonly int maxPageSize = 20;

        public CitiesController(
            ICityInfoRepository repository,
            IMapper mapper)
        {
            this._repository = repository ?? throw new ArgumentNullException(nameof(repository));
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<CityWithoutPOIsDTO>>> GetCities(
            [FromQuery(Name = "cityName")] string? name, //[FromQuery] unnecessary when identifier name is same as in query
            string? searchQuery,
            int pageNumber = 1,
            int pageSize = 10)
        {
            pageSize = pageSize > this.maxPageSize ? this.maxPageSize : pageSize;

            var (citiesEntities, paginationMetadata) = await _repository
                .GetCitiesAsync(name, searchQuery, pageNumber, pageSize);

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

            return Ok(_mapper.Map<IEnumerable<CityWithoutPOIsDTO>>(citiesEntities));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetCity(int id, bool includePOIs = false)
        {
            var city = await _repository.GetCityAsync(id, includePOIs);

            if (city == null)
            {
                return NotFound();
            }

            if (includePOIs)
            {
                return Ok(_mapper.Map<CityDto>(city));
            }

            return Ok(_mapper.Map<CityWithoutPOIsDTO>(city));
        }
    }
}
