using FluentValidation;

namespace CityInfo.API.Models
{
    public class PointOfInterestDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    public class PointOfInterestCreationDto
    {
        //[Required(ErrorMessage = "Please provide name for POI")]
        //[MaxLength(50)]
        public string Name { get; set; } = string.Empty;
        
        //[MaxLength(200)]
        public string? Description { get; set; }
    }

    public class PointOfInterestUpdatingDTO
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    public class POIValidator : AbstractValidator<PointOfInterestCreationDto>
    {
        public POIValidator() 
        {
            RuleFor(poi => poi.Name).NotEmpty().Length(1, 50);
            RuleFor(poi => poi.Description).MaximumLength(200);
        }
    }

    public class POIUpdatingValidator : AbstractValidator<PointOfInterestUpdatingDTO>
    {
        public POIUpdatingValidator()
        {
            RuleFor(poi => poi.Name).NotEmpty().Length(1, 50);
            RuleFor(poi => poi.Description).MaximumLength(200);
        }
    }
}
