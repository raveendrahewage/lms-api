using AutoMapper;
using LMS.Data.Models;
using LMS.Data;
using LMS.Services.Helpers.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LMS.Services.ViewModels;
using LMS.Services.Responses;
using Microsoft.EntityFrameworkCore;
using LMS.Data.Enum;
using LMS.Services.Interfaces;

namespace LMS.Services
{
    public class EventService: IEventService
    {
        private readonly ApplicationDbContext _appDbContext;
        private readonly IMapper _mapper;
        private readonly IApiResponseHelper _apiResponseHelper;

        public EventService(
            ApplicationDbContext appDbContext,
            IMapper mapper,
            IApiResponseHelper apiResponseHelper
        )
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
            _apiResponseHelper = apiResponseHelper;
        }
        
        public async Task<ApiResponse<EventViewModel>> CreateEvent(EventViewModel model)
        {
            try
            {
                var eventToBeCreated = _mapper.Map<Event>(model);
                var result = await _appDbContext.Events.AddAsync(eventToBeCreated);
                _appDbContext.SaveChanges();
                return _apiResponseHelper.GenerateApiResponse<EventViewModel>(true, "Event created successfully!",model);
            } catch ( Exception ex )
            {
                return _apiResponseHelper.GenerateApiResponse<EventViewModel>(false, ex.Message);
            }
        }
        public async Task<ApiResponse<EventViewModel>> UpdateEvent(EventViewModel model)
        {
            try
            {
                var eventToBeUpdated = await _appDbContext.Events
                    .FirstOrDefaultAsync(x => x.Id == model.Id);
                if(eventToBeUpdated is not null)
                {
                    var result = _appDbContext.Events.Update(eventToBeUpdated);
                    await _appDbContext.SaveChangesAsync();
                    return _apiResponseHelper.GenerateApiResponse<EventViewModel>(true, "Event updated successfully!", model);
                }
                return _apiResponseHelper.GenerateApiResponse<EventViewModel>(true, "Event updated successfully!", model);
            }
            catch (Exception ex)
            {
                return _apiResponseHelper.GenerateApiResponse<EventViewModel>(false, ex.Message);
            }
        }
        public async Task<ApiResponse<List<EventViewModel>>> GetAllEvents()
        {
            try
            {
                var events = await _appDbContext.Events
                    .ToListAsync();
                var eventsViewModel = _mapper.Map<List<EventViewModel>>(events);
                return _apiResponseHelper.GenerateApiResponse<List<EventViewModel>>(true, eventsViewModel);
            }
            catch (Exception ex)
            {
                return _apiResponseHelper.GenerateApiResponse<List<EventViewModel>>(false, ex.Message);
            }
        }
        public async Task<ApiResponse<List<EventViewModel>>> GetLeaveAndEventsBetweenDate(DateTime startDate, DateTime endDate)
        {
            try
            {
                var events = await _appDbContext.Events
                    .Where(x => x.StartDate >= startDate && x.EndDate <= endDate)
                    .ToListAsync();
                var eventsViewModel = _mapper.Map<List<EventViewModel>>(events);
                _appDbContext.SaveChanges();
                return _apiResponseHelper.GenerateApiResponse<List<EventViewModel>>(true, eventsViewModel);
            }
            catch (Exception ex)
            {
                return _apiResponseHelper.GenerateApiResponse<List<EventViewModel>>(false, ex.Message);
            }
        }
        public async Task<ApiResponse<EventViewModel>> GetEventById(int id)
        {
            try
            {
                var dbEvent = await _appDbContext.Events
                    .FirstOrDefaultAsync(x => x.Id == id);
                if(dbEvent is not null)
                {
                    var leaveTypeViewModel = _mapper.Map<EventViewModel>(dbEvent);
                    _appDbContext.SaveChanges();
                    return _apiResponseHelper.GenerateApiResponse<EventViewModel>(true, leaveTypeViewModel);
                }
                return _apiResponseHelper.GenerateApiResponse<EventViewModel>(false, "Event not found!");
            }
            catch (Exception ex)
            {
                return _apiResponseHelper.GenerateApiResponse<EventViewModel>(false, ex.Message);
            }
        }
        public async Task<ApiResponse<EventViewModel>> DeleteEventById(int id)
        {
            try
            {
                var dbEvent = await _appDbContext.Events
                    .FirstOrDefaultAsync(x => x.Id == id);
                if(dbEvent is not null)
                {
                    dbEvent.Status = DataRecordStatus.Deleted;
                    var leaveTypeViewModel = _mapper.Map<EventViewModel>(dbEvent);
                    _appDbContext.SaveChanges();
                    return _apiResponseHelper.GenerateApiResponse<EventViewModel>(true, "Event deleted successfully!",leaveTypeViewModel);
                }
                return _apiResponseHelper.GenerateApiResponse<EventViewModel>(true, "Event not found!");
            }
            catch (Exception ex)
            {
                return _apiResponseHelper.GenerateApiResponse<EventViewModel>(false, ex.Message);
            }
        }
    }
}
