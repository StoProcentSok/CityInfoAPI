using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using FluentValidation.Results;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [Route("api/cities/{cityId}/pointsofinterest")]
    [ApiController]
    public class PointsOfInterestController : ControllerBase
    {
        private IValidator<PointOfInterestCreationDto> _creationValidator;
        private IValidator<PointOfInterestUpdatingDTO> _updatingValidator;
        private readonly ILogger<PointsOfInterestController> _logger;
        private readonly IMailingService _mailingService;
        private readonly ICityInfoRepository _cityInfoRepository;
        private readonly IMapper _mapper;

        public PointsOfInterestController(
            IValidator<PointOfInterestCreationDto> validator, 
            IValidator<PointOfInterestUpdatingDTO> updatingValidator,
            ILogger<PointsOfInterestController> logger,
            IMailingService mailingService,
            ICityInfoRepository cItyInfoRepository,
            IMapper mapper)
        {
            this._creationValidator = validator ?? throw new ArgumentNullException(nameof(validator));
            this._updatingValidator = updatingValidator ?? throw new ArgumentNullException(nameof(updatingValidator));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._mailingService = mailingService ?? throw new ArgumentNullException(nameof(mailingService));
            this._cityInfoRepository = cItyInfoRepository ?? throw new ArgumentNullException(nameof(cItyInfoRepository));
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PointOfInterestDto>>> GetPointsOfInterests(int cityId)
        {

            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                _logger.LogInformation($"City with id {cityId} was not found when requesting POIs.");
                return NotFound();
            }

            var poisForCity = await this._cityInfoRepository.GetPointsOfInterestsForCityAsync(cityId);

            return Ok(_mapper.Map<IEnumerable<PointOfInterestDto>>(poisForCity));


        }

        [HttpGet("{poiId}", Name = "GetPointOfInterest")]
        public async Task<ActionResult<PointOfInterestDto>> GetPointOfInterest(int cityId, int poiId)
        {
            
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }

            var poi = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, poiId);

            if (poi is null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<PointOfInterestDto>(poi));
        }

        [HttpPost]
        public async Task<ActionResult<PointOfInterestDto>> CreatePointOfInterest(int cityId,
            [FromBody] PointOfInterestCreationDto pointOfInterestCreationDto)
        {

            ValidationResult validationResult = _creationValidator.Validate(pointOfInterestCreationDto);

            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(this.ModelState);
                return BadRequest(validationResult);
            }

            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }

            var newPoi = _mapper.Map<Entities.PointOfInterest>(pointOfInterestCreationDto);

            await _cityInfoRepository.AddPOIForCityAsync(cityId, newPoi);

            await _cityInfoRepository.SaveChangesASync();

            var createdAndSavedPOI = _mapper.Map<Models.PointOfInterestDto>(newPoi);

            return CreatedAtRoute("GetPointOfInterest", 
                new 
                { 
                    cityId = cityId, 
                    poiId = createdAndSavedPOI.Id 
                }, createdAndSavedPOI);
        }

        [HttpPut("{poiToUpdateId}")]
        public async Task<ActionResult> UpdatePointOfInterest(int cityId, int poiToUpdateId, PointOfInterestUpdatingDTO updateDTO)
        {
            if(!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }

            var POIToBeUpdated = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, poiToUpdateId);

            if (POIToBeUpdated is null) 
            {
                return NotFound();
            }

            _mapper.Map(updateDTO, POIToBeUpdated);

            await _cityInfoRepository.SaveChangesASync();

            return NoContent();
        }

        //[HttpPatch("{poiId}")]
        //public ActionResult PartiallyUpdatePOI(int cityId, int poiId,
        //    JsonPatchDocument<PointOfInterestUpdatingDTO> patchDocument)
        //{
        //    var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
        //    if (city == null)
        //    {
        //        return NotFound();
        //    }

        //    var poiFromStore = city.PointsOfInterest.FirstOrDefault(poi => poi.Id == poiId);
        //    if (poiFromStore == null)
        //    {
        //        return NotFound();
        //    }

        //    var poiToPatch = new PointOfInterestUpdatingDTO()
        //    {
        //        Name = poiFromStore.Name,
        //        Description = poiFromStore.Description,
        //    };

        //    patchDocument.ApplyTo(poiToPatch, this.ModelState);

        //    var validationRes = _updatingValidator.Validate(poiToPatch);
        //    if (!validationRes.IsValid)
        //    {
        //        validationRes.AddToModelState(this.ModelState);
        //    }

        //    if (!this.ModelState.IsValid)
        //    {
        //        return BadRequest(this.ModelState);
        //    }

        //    poiFromStore.Name = poiToPatch.Name;
        //    poiFromStore.Description = poiToPatch.Description;

        //    return NoContent();

        //}

        //[HttpDelete("{poiId}")]
        //public ActionResult DeletePointOfInterest(int cityId, int poiId)
        //{
        //    var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
        //    if (city == null)
        //    {
        //        return NotFound();
        //    }

        //    var poiFromStore = city.PointsOfInterest.FirstOrDefault(poi => poi.Id == poiId);
        //    if (poiFromStore == null)
        //    {
        //        return NotFound();
        //    }

        //    city.PointsOfInterest.Remove(poiFromStore);
        //    _mailingService.sendMail("POI Deleted", $"Deleted POI with id {poiId}, from City with id {cityId}");

        //    return NoContent();
        //}
    }
}
