using AutoMapper;
using E_Tickets.Models;
using E_Tickets.Models.ViewModels;
using Microsoft.Extensions.Options;

namespace E_Tickets.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<ApplicationUserVM, ApplicationUser>();
            //.ForMember(
            //   dest => dest.Name,
            //   Options => Options.MapFrom(scr => scr.Name)
            //);
        }
    }
}
