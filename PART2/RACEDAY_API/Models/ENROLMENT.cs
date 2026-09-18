using System;

namespace RACEDAY_API.Models
{
    public class ENROLMENT
    {

        public int EnrollmentId { get; set; }

        public DateTime EnrollmentDate { get; set; }

        public string EnrollmentStatus { get; set; }

        public int ParticipantId { get; set; }

        public int EventId { get; set; }

        public int CategoryId { get; set; }

    }
}
