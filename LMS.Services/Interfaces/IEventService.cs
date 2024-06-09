using LMS.Services.Responses;
using LMS.Services.ViewModels;

namespace LMS.Services.Interfaces
{
    public interface IEventService
    {
        Task<ApiResponse<EventViewModel>> CreateEvent(EventViewModel model);
        Task<ApiResponse<EventViewModel>> DeleteEventById(int id);
        Task<ApiResponse<List<EventViewModel>>> GetAllEvents();
        Task<ApiResponse<List<EventViewModel>>> GetLeaveAndEventsBetweenDate(DateTime startDate, DateTime endDate);
        Task<ApiResponse<EventViewModel>> GetEventById(int id);
        Task<ApiResponse<EventViewModel>> UpdateEvent(EventViewModel model);
    }
}