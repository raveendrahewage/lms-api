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
    public class LeaveTypeService: ILeaveTypeService
    {
        private readonly ApplicationDbContext _appDbContext;
        private readonly IMapper _mapper;
        private readonly IApiResponseHelper _apiResponseHelper;

        public LeaveTypeService(
            ApplicationDbContext appDbContext,
            IMapper mapper,
            IApiResponseHelper apiResponseHelper
        )
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
            _apiResponseHelper = apiResponseHelper;
        }
        
        public async Task<ApiResponse<LeaveTypeViewModel>> CreateLeaveType(LeaveTypeViewModel model)
        {
            try
            {
                var leaveTypeToBeCreated = _mapper.Map<LeaveType>(model);
                var result = await _appDbContext.LeaveTypes.AddAsync(leaveTypeToBeCreated);
                _appDbContext.SaveChanges();
                return _apiResponseHelper.GenerateApiResponse<LeaveTypeViewModel>(true, "Leave type created successfully!",model);
            } catch ( Exception ex )
            {
                return _apiResponseHelper.GenerateApiResponse<LeaveTypeViewModel>(false, ex.Message);
            }
        }
        public async Task<ApiResponse<LeaveTypeViewModel>> UpdateLeaveType(LeaveTypeViewModel model)
        {
            try
            {
                var leaveTypeToBeUpdated = await _appDbContext.LeaveTypes
                    .FirstOrDefaultAsync(x => x.Id == model.Id);
                if(leaveTypeToBeUpdated is not null)
                {
                    var result = _appDbContext.LeaveTypes.Update(leaveTypeToBeUpdated);
                    await _appDbContext.SaveChangesAsync();
                    return _apiResponseHelper.GenerateApiResponse<LeaveTypeViewModel>(true, "Leave type updated successfully!", model);
                }
                return _apiResponseHelper.GenerateApiResponse<LeaveTypeViewModel>(true, "Leave type updated successfully!", model);
            }
            catch (Exception ex)
            {
                return _apiResponseHelper.GenerateApiResponse<LeaveTypeViewModel>(false, ex.Message);
            }
        }
        public async Task<ApiResponse<List<LeaveTypeViewModel>>> GetAllLeaveTypes()
        {
            try
            {
                var leaveTypes = await _appDbContext.LeaveTypes
                    .ToListAsync();
                var leaveTypesViewModel = _mapper.Map<List<LeaveTypeViewModel>>(leaveTypes);
                return _apiResponseHelper.GenerateApiResponse<List<LeaveTypeViewModel>>(true, leaveTypesViewModel);
            }
            catch (Exception ex)
            {
                return _apiResponseHelper.GenerateApiResponse<List<LeaveTypeViewModel>>(false, ex.Message);
            }
        }
        public async Task<ApiResponse<List<LeaveTypeViewModel>>> GetLeavesTypeByName(string typeName)
        {
            try
            {
                var leaveTypes = await _appDbContext.LeaveTypes
                    .Where(x => x.Name.Equals(typeName))
                    .ToListAsync();
                var leaveTypesViewModel = _mapper.Map<List<LeaveTypeViewModel>>(leaveTypes);
                _appDbContext.SaveChanges();
                return _apiResponseHelper.GenerateApiResponse<List<LeaveTypeViewModel>>(true, leaveTypesViewModel);
            }
            catch (Exception ex)
            {
                return _apiResponseHelper.GenerateApiResponse<List<LeaveTypeViewModel>>(false, ex.Message);
            }
        }
        public async Task<ApiResponse<LeaveTypeViewModel>> GetLeaveTypeById(int id)
        {
            try
            {
                var leaveType = await _appDbContext.LeaveTypes
                    .FirstOrDefaultAsync(x => x.Id == id);
                if(leaveType is not null)
                {
                    var leaveTypeViewModel = _mapper.Map<LeaveTypeViewModel>(leaveType);
                    _appDbContext.SaveChanges();
                    return _apiResponseHelper.GenerateApiResponse<LeaveTypeViewModel>(true, leaveTypeViewModel);
                }
                return _apiResponseHelper.GenerateApiResponse<LeaveTypeViewModel>(false, "Leave type not found!");
            }
            catch (Exception ex)
            {
                return _apiResponseHelper.GenerateApiResponse<LeaveTypeViewModel>(false, ex.Message);
            }
        }
        public async Task<ApiResponse<LeaveTypeViewModel>> DeleteLeaveTypeById(int id)
        {
            try
            {
                var leaveType = await _appDbContext.LeaveTypes
                    .FirstOrDefaultAsync(x => x.Id == id);
                if(leaveType is not null)
                {
                    leaveType.Status = DataRecordStatus.Deleted;
                    var leaveTypeViewModel = _mapper.Map<LeaveTypeViewModel>(leaveType);
                    _appDbContext.SaveChanges();
                    return _apiResponseHelper.GenerateApiResponse<LeaveTypeViewModel>(true, "Leave type deleted successfully!",leaveTypeViewModel);
                }
                return _apiResponseHelper.GenerateApiResponse<LeaveTypeViewModel>(true, "Leave type not found!");
            }
            catch (Exception ex)
            {
                return _apiResponseHelper.GenerateApiResponse<LeaveTypeViewModel>(false, ex.Message);
            }
        }
    }
}
