using AutoMapper;
using LMS.Data.Models;
using LMS.Data;
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
using Azure;
using System.Drawing;

namespace LMS.Services
{
    public class EventService: IEventService
    {
        private readonly ApplicationDbContext _appDbContext;
        private readonly IMapper _mapper;
        private readonly IAccountService _accountService;

        public EventService(
            ApplicationDbContext appDbContext,
            IMapper mapper,
            IAccountService accountService
        )
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
            _accountService = accountService;
        }
        
        public async Task<EventViewModel> CreateEvent(EventViewModel model)
        {
            try
            {
                var eventToBeCreated = _mapper.Map<Event>(model);
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
                var dbEvent = await _appDbContext.Events
                    .FirstOrDefaultAsync(x => x.Id == model.Id);
                if(dbEvent is not null)
                {
                    var eventToBeUpdated = _mapper.Map<EventViewModel, Event>(model, dbEvent);
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
        public async Task<List<EventViewModel>> GetAllEvents(int? page, int? size)
        {
            try
            {
                List<Event> events = new List<Event>();
                if (page.HasValue && size.HasValue && page > 0 && size > 0)
                {
                    events = await _appDbContext.Events
                    .Skip((page.Value - 1) * size.Value)
                    .Take(size.Value)
                        .ToListAsync();
                }
                else
                {
                    events = await _appDbContext.Events.ToListAsync();
                }
                return _mapper.Map<List<EventViewModel>>(events);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<List<EventViewModel>> GetLeaveAndEventsBetween(DateTime startDate, DateTime endDate)
        {
            try
            {
                var events = await _appDbContext.Events
                    .Where(x => x.StartDate >= startDate && x.EndDate <= endDate)
                    .ToListAsync();
                return _mapper.Map<List<EventViewModel>>(events);
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
                    return _mapper.Map<EventViewModel>(dbEvent);
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
                    dbEvent.DeletedDate = DateTime.Now;
                    _appDbContext.Events.Update(dbEvent);
                    await _appDbContext.SaveChangesAsync();
                    return _mapper.Map<EventViewModel>(dbEvent);
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
