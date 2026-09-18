using System;

namespace RACEDAY_API.Models
{
    public class Event
    {
        public int EventId { get; set; }

        public string EventName { get; set; } = string.Empty;

        public string EventDescription { get; set; } = string.Empty;

        public DateTime EventDate { get; set; }

        public string EventLocation { get; set; } = string.Empty;

        public decimal EventDistance { get; set; }

        public string EventType { get; set; } = string.Empty;

        public string BannerImageUrl { get; set; } = string.Empty;

        public int OrganiserId { get; set; }
    }
}