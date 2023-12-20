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
        private readonly LocalMailingService _mailingService;

        public PointsOfInterestController(
            IValidator<PointOfInterestCreationDto> validator, 
            IValidator<PointOfInterestUpdatingDTO> updatingValidator,
            ILogger<PointsOfInterestController> logger,
            LocalMailingService mailingService)
        {
            this._creationValidator = validator ?? throw new ArgumentNullException(nameof(validator));
            this._updatingValidator = updatingValidator ?? throw new ArgumentNullException(nameof(updatingValidator));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._mailingService = mailingService ?? throw new ArgumentNullException(nameof(mailingService));
        }

        [HttpGet]
        public ActionResult<IEnumerable<PointOfInterestDto>> GetPointsOfInterests(int cityId)
        {

                var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

                if (city == null)
                {
                    _logger.LogInformation($"City with id {cityId} was not found when requesting POIs.");
                    return NotFound();
                }

                return Ok(city.PointsOfInterest);
            }

        [HttpGet("{poiId}", Name = "GetPointOfInterest")]
        public ActionResult<PointOfInterestDto> GetPointOfInterest(int cityId, int poiId)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }

            var poi = city.PointsOfInterest.FirstOrDefault(poi => poi.Id == poiId);
            if (poi == null)
            {
                return NotFound();
            }

            return Ok(poi);
        }

        [HttpPost]
        public ActionResult<PointOfInterestDto> CreatePointOfInterest(int cityId,
            [FromBody] PointOfInterestCreationDto pointOfInterestCreationDto)
        {

            ValidationResult validationResult = _creationValidator.Validate(pointOfInterestCreationDto);

            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(this.ModelState);
                return BadRequest(validationResult);
            }

            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city is null)
            {
                return NotFound();
            }


            var maxId = CitiesDataStore.Current.Cities.SelectMany(c => c.PointsOfInterest).Max(poi => poi.Id);

            var newPoi = new PointOfInterestDto()
            {
                Id = ++maxId,
                Name = pointOfInterestCreationDto.Name,
                Description = pointOfInterestCreationDto.Description
            };

            city.PointsOfInterest.Add(newPoi);

            return CreatedAtRoute("GetPointOfInterest", new { cityId = cityId, poiId = newPoi.Id }, newPoi);
        }

        [HttpPut("{poiToUpdateId}")]
        public ActionResult<PointOfInterestDto> UpdatePointOfInterest(int cityId, int poiToUpdateId, PointOfInterestUpdatingDTO updateDTO)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }

            var poi = city.PointsOfInterest.FirstOrDefault(poi => poi.Id == poiToUpdateId);
            if (poi == null)
            {
                return NotFound();
            }

            poi.Name = updateDTO.Name;
            poi.Description = updateDTO.Description; //?? poi.Description; commennted out for JsonPatch implementation

            return NoContent();
        }
        [HttpPatch("{poiId}")]
        public ActionResult PartiallyUpdatePOI(int cityId, int poiId,
            JsonPatchDocument<PointOfInterestUpdatingDTO> patchDocument)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }

            var poiFromStore = city.PointsOfInterest.FirstOrDefault(poi => poi.Id == poiId);
            if (poiFromStore == null)
            {
                return NotFound();
            }

            var poiToPatch = new PointOfInterestUpdatingDTO()
            {
                Name = poiFromStore.Name,
                Description = poiFromStore.Description,
            };

            patchDocument.ApplyTo(poiToPatch, this.ModelState);

            var validationRes = _updatingValidator.Validate(poiToPatch);
            if (!validationRes.IsValid)
            {
                validationRes.AddToModelState(this.ModelState);
            }

            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }

            poiFromStore.Name = poiToPatch.Name;
            poiFromStore.Description = poiToPatch.Description;

            return NoContent();

        }

        [HttpDelete("{poiId}")]
        public ActionResult DeletePointOfInterest(int cityId, int poiId)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }

            var poiFromStore = city.PointsOfInterest.FirstOrDefault(poi => poi.Id == poiId);
            if (poiFromStore == null)
            {
                return NotFound();
            }

            city.PointsOfInterest.Remove(poiFromStore);
            _mailingService.sendMail("POI Deleted", $"Deleted POI with id {poiId}, from City with id {cityId}");

            return NoContent();
        }
    }
}
