
namespace RaceDay.Web.ViewModels
{
    public class CategoryViewModel
    {
        public int CategoryId { get; set; }

        public string CategoryName { get; set; } = string.Empty;

        public string CategoryDescription { get; set; } = string.Empty;

        public int EventId { get; set; }
    }
}

