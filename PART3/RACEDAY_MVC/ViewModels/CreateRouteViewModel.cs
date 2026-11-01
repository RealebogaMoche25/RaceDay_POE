using System.ComponentModel.DataAnnotations;

namespace RaceDay.Web.ViewModels
{
    public class CreateRouteViewModel
    {
        [Required(ErrorMessage = "Route name is required.")]
        public string RouteName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Route description is required.")]
        public string RouteDescription { get; set; } = string.Empty;

        [Required(ErrorMessage = "Route URL is required.")]
        [Url(ErrorMessage = "Please enter a valid route URL.")]
        public string RouteUrl { get; set; } = string.Empty;

        [Required(ErrorMessage = "Route location is required.")]
        public string RouteLocation { get; set; } = string.Empty;
    }
}