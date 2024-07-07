using AutoMapper;
using LMS.Data.Enum;
using LMS.Data.Models;
using LMS.Services.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Services.Mappings
{
    public class AutoMapperProfile: Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<SystemUserViewModel, SystemUser>()
                .ForMember(d => d.UserName, opt => opt.MapFrom(s => s.Email))
                .ForMember(d => d.NormalizedUserName, opt => opt.MapFrom(s => s.Email))
                .ForMember(d => d.Email, opt => opt.MapFrom(s => s.Email))
                .ForMember(d => d.NormalizedEmail, opt => opt.MapFrom(s => s.Email))
                .ForMember(d => d.NormalizedUserName, opt => opt.MapFrom(s => s.Email));

            CreateMap<SystemUser, SystemUserViewModel>();

            CreateMap<SystemUser, SystemUserListItemViewModel>()
                .ForMember(d => d.RoleName, opt => opt.MapFrom(s =>s.Role.Name))
                .ForMember(d => d.SupervisorName, opt => opt.MapFrom(s =>s.Supervisor.FullName));

            CreateMap<SystemRole, SystemRoleViewModel>().ReverseMap();

            CreateMap<SignUpViewModel, SystemUser>()
                .ForMember(d => d.UserName, opt => opt.MapFrom(s => s.Email))
                .ForMember(d => d.NormalizedUserName, opt => opt.MapFrom(s => s.Email))
                .ForMember(d => d.Email, opt => opt.MapFrom(s => s.Email))
                .ForMember(d => d.NormalizedEmail, opt => opt.MapFrom(s => s.Email))
                .ForMember(d => d.NormalizedUserName, opt => opt.MapFrom(s => s.Email));

            CreateMap<LeaveType, LeaveTypeViewModel>().ReverseMap();

            CreateMap<Leave, LeaveViewModel>().ReverseMap();

            CreateMap<Leave, LeaveListItemViewModel>()
                .ForMember(d => d.SupervisorId, opt => opt.MapFrom(s => s.Employee.SupervisorId))
                .ForMember(d => d.SupervisorName, opt => opt.MapFrom(s => s.Employee.Supervisor.FullName))
                .ForMember(d => d.EmployeeName, opt => opt.MapFrom(s => s.Employee.FullName))
                .ForMember(d => d.LeaveTypeName, opt => opt.MapFrom(s => s.LeaveType.Name));

            CreateMap<Event, EventViewModel>().ReverseMap();

            CreateMap<DateWiseLeave, DateWiseLeaveViewModel>().ReverseMap();

            CreateMap<Event, CalendarEventViewModel>()
                .ForMember(d => d.CalendarEventType, opt => opt.MapFrom(s => CalendarEventType.Event))
                .ForMember(d => d.CalendarEventId, opt => opt.MapFrom(s => s.Id));
        }
    }
}
