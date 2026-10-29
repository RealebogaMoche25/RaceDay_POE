
namespace RaceDay.Web.ViewModels
{
    public class EventViewModel
    {
        public int EventId { get; set; }

        public string EventName { get; set; } = string.Empty;

        public string? EventDescription { get; set; }

        public DateTime EventDate { get; set; }

        public string? EventLocation { get; set; }

        public decimal EventDistance { get; set; }

        public string? EventType { get; set; }

        public string? BannerImageUrl { get; set; }

        public int OrganiserId { get; set; }
    }
}

