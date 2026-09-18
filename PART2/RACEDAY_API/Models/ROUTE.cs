using System;

namespace RACEDAY_API.Models
{
    public class ROUTE
    {

        public int RouteId { get; set; }

        public string RouteName { get; set; }

        public string RouteDescription { get; set; }

        public string RouteUrl { get; set; }

        public string RouteLocation { get; set; }

        public int EventId { get; set; }

    }
}