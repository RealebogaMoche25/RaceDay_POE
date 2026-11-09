using System.ComponentModel.DataAnnotations;

namespace RaceDay.Web.ViewModels
{
    public class CreateEventViewModel
    {
        [Required(ErrorMessage = "Event name is required.")]
        [StringLength(100)]
        public string EventName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Event description is required.")]
        public string EventDescription { get; set; } = string.Empty;

        [Required(ErrorMessage = "Event date is required.")]
        public DateTime EventDate { get; set; }

        [Required(ErrorMessage = "Event location is required.")]
        public string EventLocation { get; set; } = string.Empty;

  
        [Required(ErrorMessage = "Event distance is required.")]
        [Range(0.01, 1000, ErrorMessage = "Enter a valid event distance.")]
        public decimal EventDistance { get; set; }

        [Required(ErrorMessage = "Event type is required.")]
        public string EventType { get; set; } = string.Empty;

        public string? BannerImageUrl { get; set; }

        public string? BannerImageViewUrl { get; set; }
    }
}
