using System;

namespace RACEDAY_API.Models
{
    public class RESULT
    {

        public int ResultId { get; set; }

        public TimeSpan FinishTime { get; set; }

        public int FinishingPosition { get; set; }

        public int EnrollmentId { get; set; }

    }
}
