using System;

namespace RACEDAY_API.Models
{
    public class Routes
    {
        public int RouteId { get; set; }

        public string RouteName { get; set; } = string.Empty;

        public string RouteDescription { get; set; } = string.Empty;

        public string RouteUrl { get; set; } = string.Empty;

        public string RouteLocation { get; set; } = string.Empty;

        public int EventId { get; set; }
    }
}