using LMS.Services;
using LMS.Services.Common;
using LMS.Services.Helpers.Interfaces;
using LMS.Services.Interfaces;
using LMS.Services.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using System.Security.Policy;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LMS.Api.Controllers
{
    [Authorize]
    [Route("api/event")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly IApiResponseHelper _apiResponseHelper;
        public EventController(IEventService eventService, IApiResponseHelper apiResponseHelper)
        {
            _eventService = eventService;
            _apiResponseHelper = apiResponseHelper;
        }
        [HttpPost]
        public async Task<IActionResult> CreateNewEvent(EventViewModel model)
        {
            try
            {
                var result = await _eventService.CreateEvent(model);
                return Ok(_apiResponseHelper.GenerateApiResponse(true, "Event created successfully!", result));
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPatch]
        public async Task<IActionResult> UpdateEvent(EventViewModel model)
        {
            try
            {
                var result = await _eventService.UpdateEvent(model);
                return Ok(_apiResponseHelper.GenerateApiResponse(true, "Event updated successfully!", result));
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteEventById(int id)
        {
            try
            {
                var result = await _eventService.DeleteEventById(id);
                return Ok(_apiResponseHelper.GenerateApiResponse(true, "Event deleted successfully!", result));
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet]
        [Route("get-events")]
        public async Task<IActionResult> GetAllEvents()
        {
            try
            {
                var result = await _eventService.GetAllEvents();
                return Ok(_apiResponseHelper.GenerateApiResponse(true, result));
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost]
        [Route("get-events/ssr")]
        public async Task<IActionResult> GetAllEventsSsr(DataTableConfiguration dataTableConfiguration)
        {
            try
            {
                var result = await _eventService.GetAllEventsSsr(dataTableConfiguration);
                return Ok(_apiResponseHelper.GenerateApiResponse(true, result));
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetEventById(int id)
        {
            try
            {
                var result = await _eventService.GetEventById(id);
                return Ok(_apiResponseHelper.GenerateApiResponse(true, result));
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet]
        [Route("get-events-between")]
        public async Task<IActionResult> GetEventsBetweenDate(DateTime startDate, DateTime endDate)
        {
            try
            {
                var result = await _eventService.GetLeaveAndEventsBetween(startDate, endDate);
                return Ok(_apiResponseHelper.GenerateApiResponse(true, result));
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
