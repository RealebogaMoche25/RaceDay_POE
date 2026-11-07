namespace RaceDay.Web.ViewModels
{
    public class OrganiserDashboardViewModel
    {
        public int TotalEvents { get; set; }

        public int TotalEnrolments { get; set; }

        public int UpcomingEvents { get; set; }

        public string NextEventName { get; set; } = "No upcoming events";

        public DateTime? NextEventDate { get; set; }
    }
}