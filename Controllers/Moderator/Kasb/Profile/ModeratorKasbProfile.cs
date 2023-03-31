using AutoMapper;
using VitcAuth.Models;

namespace VitcAuth.Controllers.Moderator.Kasb
{
    public class ModeratorKasbProfile : Profile
    {
        public ModeratorKasbProfile()
        {
            CreateMap<StudentKasbCreate, StudentJob>()
            .ForMember(e => e.Avatar, src => src.MapFrom(e => e.OutputPath));

            CreateMap<StudentJob, StudentKasbResponse>()
            .ForMember(e => e.JobName, src => src.MapFrom(e => e.Job.Name))
            .ForMember(e => e.RegionName, src => src.MapFrom(e => e.Region.Name))
            .ForMember(e => e.Name, src => src.MapFrom(e => e.Lastname + " " + e.Firstname))
            .ForMember(e => e.Passport, src =>
            src.MapFrom(e => e.StudentJobPassport.PassportSerial +
            e.StudentJobPassport.PassportNumber));
        }
    }
}