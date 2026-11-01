
namespace RaceDay.Web.ViewModels
{
    public class EnrolmentViewModel
    {
        public int EnrolmentId { get; set; }

        public DateTime EnrolmentDate { get; set; }

        public string EnrolmentStatus { get; set; } = string.Empty;

        public int ParticipantId { get; set; }

        public int EventId { get; set; }

        public int CategoryId { get; set; }
    }
}

