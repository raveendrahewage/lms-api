using LMS.Services.Responses;
using LMS.Services.ViewModels;

namespace LMS.Services.Interfaces
{
    public interface IEventService
    {
        Task<EventViewModel> CreateEvent(EventViewModel model);
        Task<EventViewModel> DeleteEventById(int id);
        Task<List<EventViewModel>> GetAllEvents(int? page, int? size);
        Task<List<EventViewModel>> GetLeaveAndEventsBetween(DateTime startDate, DateTime endDate);
        Task<EventViewModel> GetEventById(int id);
        Task<EventViewModel> UpdateEvent(EventViewModel model);
    }
}