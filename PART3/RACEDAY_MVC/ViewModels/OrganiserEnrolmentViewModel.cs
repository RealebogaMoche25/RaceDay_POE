
namespace RaceDay.Web.ViewModels
{
    public class OrganiserEnrolmentViewModel
    {
        public int EnrolmentId { get; set; }

        public int ParticipantId { get; set; }

        public string ParticipantName { get; set; } = string.Empty;

        public int EventId { get; set; }

        public int CategoryId { get; set; }

        public string CategoryName { get; set; } = string.Empty;

        public DateTime EnrolmentDate { get; set; }

        public string EnrolmentStatus { get; set; } = string.Empty;
    }
}
