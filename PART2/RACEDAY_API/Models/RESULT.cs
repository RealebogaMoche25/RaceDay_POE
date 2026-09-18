using System;

namespace RACEDAY_API.Models
{
    public class Result
    {
        public int ResultId { get; set; }

        public TimeSpan FinishTime { get; set; }

        public int FinishingPosition { get; set; }

        public int EnrolmentId { get; set; }
    }
}
