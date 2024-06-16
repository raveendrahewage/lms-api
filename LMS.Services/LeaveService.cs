using AutoMapper;
using Azure;
using LMS.Data;
using LMS.Data.Enum;
using LMS.Data.Models;
using LMS.Services.Interfaces;
using LMS.Services.Responses;
using LMS.Services.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Services
{
    public class LeaveService : ILeaveService
    {
        private readonly ApplicationDbContext _appDbContext;
        private readonly IMapper _mapper;
        public LeaveService(
            ApplicationDbContext appDbContext,
            IMapper mapper
        )
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
        }
        public async Task<LeaveViewModel> CreateLeave(LeaveViewModel model)
        {
            try
            {
                var leaveToBeCreated = _mapper.Map<Leave>(model);
                var result = await _appDbContext.Leaves.AddAsync(leaveToBeCreated);
                _appDbContext.SaveChanges();
                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<LeaveViewModel> UpdateLeave(LeaveViewModel model)
        {
            try
            {
                var leave = await _appDbContext.Leaves
                    .FirstOrDefaultAsync(x => x.Id == model.Id);
                if(leave is not null )
                {
                    var leaveToBeUpdated = _mapper.Map<LeaveViewModel, Leave>(model, leave);
                    _appDbContext.Leaves.Update(leaveToBeUpdated);
                    await _appDbContext.SaveChangesAsync();
                    return _mapper.Map<LeaveViewModel>(leaveToBeUpdated);
                }
                throw new Exception("Leave not found!");
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<List<LeaveViewModel>> GetAllLeavesByEmployeeId(int id)
        {
            try
            {
                var leaves = await _appDbContext.Leaves
                    .Where(x => x.EmployeeId == id)
                    .ToListAsync();
                return _mapper.Map<List<LeaveViewModel>>(leaves);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<LeaveTypeViewModel> GetLeaveById(int id)
        {
            try
            {
                var leave = await _appDbContext.Leaves
                    .FirstOrDefaultAsync(x => x.Id == id);
                if (leave is not null)
                {
                    return _mapper.Map<LeaveTypeViewModel>(leave);
                }
                throw new Exception("Leave not found!");
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<List<LeaveViewModel>> GetAllLeaves(int? page, int? size)
        {
            try
            {
                List<Leave> leaves = new List<Leave>();
                if (page.HasValue && size.HasValue && page > 0 && size > 0)
                {
                    leaves = await _appDbContext.Leaves
                    .Skip((page.Value - 1) * size.Value)
                    .Take(size.Value)
                        .ToListAsync();
                }
                else
                {
                    leaves = await _appDbContext.Leaves.ToListAsync();
                }
                return _mapper.Map<List<LeaveViewModel>>(leaves);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<List<LeaveViewModel>> GetLeavesBetween(DateTime startDate, DateTime endDate)
        {
            try
            {
                var leaves = await _appDbContext.Leaves
                    .Where(x => x.FromDate >= startDate && x.FromDate <= endDate)
                    .ToListAsync();
                return _mapper.Map<List<LeaveViewModel>>(leaves);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<LeaveViewModel> DeleteLeaveById(int id)
        {
            try
            {
                var leave = await _appDbContext.Leaves
                    .FirstOrDefaultAsync(x => x.Id == id);
                if (leave is not null)
                {
                    leave.Status = DataRecordStatus.Deleted;
                    _appDbContext.Leaves.Update(leave);
                    await _appDbContext.SaveChangesAsync();
                    return _mapper.Map<LeaveViewModel>(leave);
                }
                throw new Exception("Leave not found!");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
