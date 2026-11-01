using System;

namespace RaceDay.Web.ViewModels
{
    public class ResultViewModel
    {
        public int ResultId { get; set; }

        public TimeSpan FinishTime { get; set; }

        public int FinishingPosition { get; set; }

        public int EnrolmentId { get; set; }
    }
}