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

namespace LMS.Services
{
    public class LeaveTypeService: ILeaveTypeService
    {
        private readonly ApplicationDbContext _appDbContext;
        private readonly IMapper _mapper;

        public LeaveTypeService(
            ApplicationDbContext appDbContext,
            IMapper mapper
        )
        {
            _appDbContext = appDbContext;
            _mapper = mapper;;
        }
        
        public async Task<LeaveTypeViewModel> CreateLeaveType(LeaveTypeViewModel model)
        {
            try
            {
                var leaveTypeToBeCreated = _mapper.Map<LeaveType>(model);
                var result = await _appDbContext.LeaveTypes.AddAsync(leaveTypeToBeCreated);
                _appDbContext.SaveChanges();
                return model;
            } catch ( Exception ex )
            {
                throw;
            }
        }
        public async Task<LeaveTypeViewModel> UpdateLeaveType(LeaveTypeViewModel model)
        {
            try
            {
                var leaveTypeToBeUpdated = await _appDbContext.LeaveTypes
                    .FirstOrDefaultAsync(x => x.Id == model.Id);
                if(leaveTypeToBeUpdated is not null)
                {
                    _appDbContext.LeaveTypes.Update(leaveTypeToBeUpdated);
                    await _appDbContext.SaveChangesAsync();
                    return _mapper.Map<LeaveTypeViewModel>(leaveTypeToBeUpdated);
                }
                throw new Exception("Leave type not found!");
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<List<LeaveTypeViewModel>> GetAllLeaveTypes()
        {
            try
            {
                var leaveTypes = await _appDbContext.LeaveTypes
                    .ToListAsync();
                return _mapper.Map<List<LeaveTypeViewModel>>(leaveTypes);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<List<LeaveTypeViewModel>> GetLeavesTypeByName(string typeName)
        {
            try
            {
                var leaveTypes = await _appDbContext.LeaveTypes
                    .Where(x => x.Name.Equals(typeName))
                    .ToListAsync();
                return _mapper.Map<List<LeaveTypeViewModel>>(leaveTypes);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<LeaveTypeViewModel> GetLeaveTypeById(int id)
        {
            try
            {
                var leaveType = await _appDbContext.LeaveTypes
                    .FirstOrDefaultAsync(x => x.Id == id);
                if(leaveType is not null)
                {
                    return _mapper.Map<LeaveTypeViewModel>(leaveType);
                }
                throw new Exception("Leave type not found!");
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<LeaveTypeViewModel> DeleteLeaveTypeById(int id)
        {
            try
            {
                var leaveType = await _appDbContext.LeaveTypes
                    .FirstOrDefaultAsync(x => x.Id == id);
                if(leaveType is not null)
                {
                    leaveType.Status = DataRecordStatus.Deleted;
                    _appDbContext.LeaveTypes.Update(leaveType);
                    await _appDbContext.SaveChangesAsync();
                    return _mapper.Map<LeaveTypeViewModel>(leaveType);
                }
                throw new Exception("Leave type not found!");
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
