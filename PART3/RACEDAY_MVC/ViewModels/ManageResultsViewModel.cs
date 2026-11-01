namespace RaceDay.Web.ViewModels
{
    public class ManageResultsViewModel
    {
        public int EventId { get; set; }

        public string EventName { get; set; } = string.Empty;

        public List<OrganiserEnrolmentViewModel> Enrolments { get; set; } = new();

        public List<ResultViewModel> Results { get; set; } = new();
    }
}