namespace RaceDay.Web.ViewModels
{
    public class MyResultViewModel
    {
        public int ResultId { get; set; }

        public int EventId { get; set; }

        public string EventName { get; set; } = string.Empty;

        public DateTime EventDate { get; set; }

        public string EventLocation { get; set; } = string.Empty;

        public TimeSpan FinishTime { get; set; }

        public int FinishingPosition { get; set; }

        public string CategoryName { get; set; } = "Not specified";

        public int TotalFinishers { get; set; }
    }
}