using System.ComponentModel.DataAnnotations;

namespace RaceDay.Web.ViewModels
{
    public class CreateResultViewModel
    {
        [Required(ErrorMessage = "Please select a participant enrolment.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid enrolment.")]
        public int EnrolmentId { get; set; }

        [Required(ErrorMessage = "Finish time is required.")]
        public TimeSpan FinishTime { get; set; }

        [Required(ErrorMessage = "Finishing position is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Finishing position must be at least 1.")]
        public int FinishingPosition { get; set; }
    }
}