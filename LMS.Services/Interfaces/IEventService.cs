using LMS.Services.Common;
using LMS.Services.Responses;
using LMS.Services.ViewModels;

namespace LMS.Services.Interfaces
{
    public interface IEventService
    {
        Task<EventViewModel> CreateEvent(EventViewModel model);
        Task<EventViewModel> DeleteEventById(int id);
        Task<List<EventViewModel>> GetAllEvents();
        Task<DataTableResult<EventViewModel>> GetAllEventsSsr(DataTableConfiguration dataTableConfiguration);
        Task<List<CalendarEventViewModel>> GetLeaveAndEventsBetween(DateOnly startDate, DateOnly endDate);
        Task<EventViewModel> GetEventById(int id);
        Task<EventViewModel> UpdateEvent(EventViewModel model);
    }
}