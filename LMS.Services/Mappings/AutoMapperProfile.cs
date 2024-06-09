using AutoMapper;
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
            CreateMap<SystemRole, SystemRoleViewModel>().ReverseMap();
            CreateMap<SignUpViewModel, SystemUser>()
                .ForMember(d => d.UserName, opt => opt.MapFrom(s => s.Email))
                .ForMember(d => d.NormalizedUserName, opt => opt.MapFrom(s => s.Email))
                .ForMember(d => d.Email, opt => opt.MapFrom(s => s.Email))
                .ForMember(d => d.NormalizedEmail, opt => opt.MapFrom(s => s.Email))
                .ForMember(d => d.NormalizedUserName, opt => opt.MapFrom(s => s.Email));
            CreateMap<LeaveType, LeaveTypeViewModel>().ReverseMap();
            CreateMap<Leave, LeaveViewModel>().ReverseMap();
            CreateMap<Event, EventViewModel>().ReverseMap();
        }
    }
}
