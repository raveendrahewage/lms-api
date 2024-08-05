using AutoMapper;
using LMS.Data.Models;
using LMS.Data;
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
using Azure;
using System.Drawing;
using LMS.Services.Common;
using LMS.Services.Constants;
using LMS.Services.Helpers;
using System.Collections;

namespace LMS.Services
{
    public class EventService(
        ApplicationDbContext appDbContext,
        IMapper mapper,
        IAccountService accountService
        ) : IEventService
    {
        private readonly ApplicationDbContext _appDbContext = appDbContext;
        private readonly IMapper _mapper = mapper;
        private readonly IAccountService _accountService = accountService;

        public async Task<EventViewModel> CreateEvent(EventViewModel model)
        {
            try
            {
                var eventToBeCreated = _mapper.Map<EventViewModel, Event>(model);
                eventToBeCreated.CreatedBy = _accountService.GetCurrentLoggedInUserId();
                var result = await _appDbContext.Events.AddAsync(eventToBeCreated);
                await _appDbContext.SaveChangesAsync();
                return model;
            } catch (Exception)
            {
                throw;
            }
        }
        public async Task<EventViewModel> UpdateEvent(EventViewModel model)
        {
            try
            {
                var eventToBeUpdated = await _appDbContext.Events
                    .FirstOrDefaultAsync(x => x.Id == model.Id);
                if(eventToBeUpdated is not null)
                {
                    eventToBeUpdated.Title = model.Title;
                    eventToBeUpdated.Description = model.Description;
                    eventToBeUpdated.StartDate = model.StartDate;
                    eventToBeUpdated.EndDate = model.EndDate;
                    eventToBeUpdated.EventStatus = model.EventStatus;
                    eventToBeUpdated.EventMode = model.EventMode;
                    eventToBeUpdated.ModifiedBy = _accountService.GetCurrentLoggedInUserId();
                    eventToBeUpdated.ModifiedDate = DateTime.UtcNow;
                    var result = _appDbContext.Events.Update(eventToBeUpdated);
                    await _appDbContext.SaveChangesAsync();
                    return model;
                }
                throw new Exception("Event not found!");
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<List<EventViewModel>> GetAllEvents()
        {
            try
            {
                var events = await _appDbContext.Events
                    .ToListAsync();
                return _mapper.Map<List<Event>, List<EventViewModel>>(events);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<DataTableResult<EventViewModel>> GetAllEventsSsr(DataTableConfiguration dataTableConfiguration)
        {
            try
            {
                var events = await _appDbContext.Events
                    .ToListAsync();
                var eventViewModels = _mapper.Map<List<Event>, List<EventViewModel>>(events);
                return DataTableResultHandler<EventViewModel>.ResultToSsr(eventViewModels, dataTableConfiguration, DataTableConfigurationOptions.All);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<List<CalendarEventViewModel>> GetLeaveAndEventsBetween(DateOnly startDate, DateOnly endDate)
        {
            try
            {
                var loggedInUserId = _accountService.GetCurrentLoggedInUserId();
                var calendarEvents = new List<CalendarEventViewModel>();
                var leaves = await _appDbContext.Leaves
                    .Include(x => x.LeaveType)
                    .Include(x => x.DateWiseLeaves)
                    .Include(x => x.Employee)
                    .Where(x =>
                        x.LeaveStatus == LeaveStatus.Approved
                        && (x.FromDate >= startDate || x.ToDate <= endDate)
                        || (x.FromDate <= startDate && x.ToDate >= endDate))
                    .ToListAsync();
                foreach (var leave in leaves)
                {
                    foreach (var dateWiseLeave in leave.DateWiseLeaves)
                    {
                        var calendarEvent = new CalendarEventViewModel
                        {
                            CalendarEventId = leave.Id,
                            CalendarEventType = CalendarEventType.Leave,
                            StartDate = dateWiseLeave.Date,
                            EndDate = dateWiseLeave.Date,
                            Title = $"{leave.Employee.FullName}: {dateWiseLeave.Title}"
                        };
                        calendarEvents.Add(calendarEvent);
                    }
                }
                var events = await _appDbContext.Events
                    .Where(x =>
                        x.EventStatus == EventStatus.Active
                        && (x.CreatedBy == loggedInUserId || x.EventMode == EventMode.Public)
                        && (x.StartDate >= startDate || x.EndDate <= endDate)
                        || (x.StartDate <= startDate && x.EndDate >= endDate))
                    .ToListAsync();
                var eventCalendarEvents = _mapper.Map<List<Event>, List<CalendarEventViewModel>>(events);
                return [.. calendarEvents, .. eventCalendarEvents];
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<EventViewModel> GetEventById(int id)
        {
            try
            {
                var dbEvent = await _appDbContext.Events
                    .FirstOrDefaultAsync(x => x.Id == id);
                if(dbEvent is not null)
                {
                    return _mapper.Map<Event, EventViewModel>(dbEvent);
                }
                throw new Exception("Event not found!");
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<EventViewModel> DeleteEventById(int id)
        {
            try
            {
                var dbEvent = await _appDbContext.Events
                    .FirstOrDefaultAsync(x => x.Id == id);
                if(dbEvent is not null)
                {
                    dbEvent.Status = DataRecordStatus.Deleted;
                    dbEvent.DeletedBy = _accountService.GetCurrentLoggedInUserId();
                    dbEvent.DeletedDate = DateTime.UtcNow;
                    _appDbContext.Events.Update(dbEvent);
                    await _appDbContext.SaveChangesAsync();
                    return _mapper.Map<Event, EventViewModel>(dbEvent);
                }
                throw new Exception("Event not found!");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
