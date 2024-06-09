using AutoMapper;
using LMS.Data;
using LMS.Data.Models;
using LMS.Services.Helpers.Interfaces;
using LMS.Services.Responses;
using LMS.Services.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Services
{
    public class LeaveService
    {
        private readonly ApplicationDbContext _appDbContext;
        private readonly IMapper _mapper;
        private readonly IApiResponseHelper _apiResponseHelper;
        public LeaveService(
            ApplicationDbContext appDbContext,
            IMapper mapper,
            IApiResponseHelper apiResponseHelper
        )
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
            _apiResponseHelper = apiResponseHelper;
        }
        public async Task<ApiResponse<LeaveViewModel>> CreateLeave(LeaveViewModel model)
        {
            try
            {
                var leaveToBeCreated = _mapper.Map<Leave>(model);
                var result = await _appDbContext.Leaves.AddAsync(leaveToBeCreated);
                _appDbContext.SaveChanges();
                return _apiResponseHelper.GenerateApiResponse<LeaveViewModel>(true, "Leave created successfully!", model);
            }
            catch (Exception ex)
            {
                return _apiResponseHelper.GenerateApiResponse<LeaveViewModel>(false, ex.Message);
            }
        }
        public async Task<ApiResponse<LeaveViewModel>> UpdateLeave(LeaveViewModel model)
        {
            try
            {
                var leaveToBeUpdated = _mapper.Map<Leave>(model);
                var result = _appDbContext.Leaves.Update(leaveToBeUpdated);
                await _appDbContext.SaveChangesAsync();
                return _apiResponseHelper.GenerateApiResponse<LeaveViewModel>(true, "Leave updated successfully!", model);
            }
            catch (Exception ex)
            {
                return _apiResponseHelper.GenerateApiResponse<LeaveViewModel>(false, ex.Message);
            }
        }
        public async Task<ApiResponse<List<LeaveViewModel>>> GetAllLeavesByEmployeeId(int id)
        {
            try
            {
                var leaves = await _appDbContext.Leaves
                    .Where(x => x.EmployeeId == id)
                    .ToListAsync();
                var leavesViewModel = _mapper.Map<List<LeaveViewModel>>(leaves);
                return _apiResponseHelper.GenerateApiResponse<List<LeaveViewModel>>(true, leavesViewModel);
            }
            catch (Exception ex)
            {
                return _apiResponseHelper.GenerateApiResponse<List<LeaveViewModel>>(false, ex.Message);
            }
        }
        public async Task<ApiResponse<LeaveTypeViewModel>> GetLeaveById(int id)
        {
            try
            {
                var leave = await _appDbContext.Leaves
                    .FirstOrDefaultAsync(x => x.Id == id);
                if (leave is not null)
                {
                    var leaveViewModel = _mapper.Map<LeaveTypeViewModel>(leave);
                    _appDbContext.SaveChanges();
                    return _apiResponseHelper.GenerateApiResponse<LeaveTypeViewModel>(true, leaveViewModel);
                }
                return _apiResponseHelper.GenerateApiResponse<LeaveTypeViewModel>(false, "Leave not found!");
            }
            catch (Exception ex)
            {
                return _apiResponseHelper.GenerateApiResponse<LeaveTypeViewModel>(false, ex.Message);
            }
        }
    }
}
