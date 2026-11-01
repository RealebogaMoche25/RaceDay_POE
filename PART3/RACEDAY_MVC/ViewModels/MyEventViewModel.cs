namespace RaceDay.Web.ViewModels
{
    public class MyEventViewModel
    {
        public int EnrolmentId { get; set; }
        public int EventId { get; set; }
        public string EventName { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string EventLocation { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string EnrolmentStatus { get; set; } = string.Empty;
        public DateTime EnrolmentDate { get; set; }

    }
}