
namespace RaceDay.Web.ViewModels
{
    public class ManageCategoriesViewModel
    {
        public int EventId { get; set; }

        public string EventName { get; set; } = string.Empty;

        public List<CategoryViewModel> Categories { get; set; } = new();
    }
}
