using LMS.Data.Enum;
using LMS.Data.Models;
using LMS.Services.ViewModels;
using Mapster;
using MapsterMapper;

namespace LMS.Services.Mappings
{
    public class MapsterRegistry : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<SystemUserViewModel, SystemUser>()
                .Map(d => d.UserName, s => s.Email)
                .Map(d => d.NormalizedUserName, s => s.Email)
                .Map(d => d.Email, s => s.Email)
                .Map(d => d.NormalizedEmail, s => s.Email);

            config.NewConfig<SystemUser, SystemUserViewModel>();

            config.NewConfig<SystemUser, SystemUserListItemViewModel>()
                .Map(d => d.RoleName, s => s.Role.Name)
                .Map(d => d.SupervisorName, s => s.Supervisor.FullName);

            config.NewConfig<SystemRole, SystemRoleViewModel>();
            config.NewConfig<SystemRoleViewModel, SystemRole>();

            config.NewConfig<SignUpViewModel, SystemUser>()
                .Map(d => d.UserName, s => s.Email)
                .Map(d => d.NormalizedUserName, s => s.Email)
                .Map(d => d.Email, s => s.Email)
                .Map(d => d.NormalizedEmail, s => s.Email);

            config.NewConfig<LeaveType, LeaveTypeViewModel>();
            config.NewConfig<LeaveTypeViewModel, LeaveType>();

            config.NewConfig<Leave, LeaveViewModel>();
            config.NewConfig<LeaveViewModel, Leave>();

            config.NewConfig<Leave, LeaveListItemViewModel>()
                .Map(d => d.SupervisorId, s => s.Employee.SupervisorId)
                .Map(d => d.SupervisorName, s => s.Employee.Supervisor.FullName)
                .Map(d => d.EmployeeName, s => s.Employee.FullName)
                .Map(d => d.LeaveTypeName, s => s.LeaveType.Name);

            config.NewConfig<Event, EventViewModel>();
            config.NewConfig<EventViewModel, Event>();

            config.NewConfig<DateWiseLeave, DateWiseLeaveViewModel>();
            config.NewConfig<DateWiseLeaveViewModel, DateWiseLeave>();

            config.NewConfig<LeaveAvailability, LeaveAvailabilityViewModel>();
            config.NewConfig<LeaveAvailabilityViewModel, LeaveAvailability>();

            config.NewConfig<Event, CalendarEventViewModel>()
                .Map(d => d.SystemUserId, s => s.CreatedBy)
                .Map(d => d.CalendarEventType, s => CalendarEventType.Event)
                .Map(d => d.CalendarEventId, s => s.Id);
        }
    }
}