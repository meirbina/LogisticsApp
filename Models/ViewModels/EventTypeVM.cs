using Microsoft.AspNetCore.Mvc.Rendering;

namespace SMS.Models.ViewModels
{
    public class EventTypeVM
    {
        public EventType EventType { get; set; }
        public IEnumerable<SelectListItem> BranchList { get; set; }
        public IEnumerable<SelectListItem> EventList { get; set; }
        public IEnumerable<SelectListItem> AudienceList { get; set; }
        public IEnumerable<SelectListItem> ControlClassList { get; set; }
        public IEnumerable<SelectListItem> SectionList { get; set; }
    }
}
