using AutoMapper;
using VitcAuth.Models;

namespace VitcAuth.Controllers.Moderator.Auto
{
    public class ModeratorAutoProfile : Profile
    {
        public ModeratorAutoProfile()
        {
            CreateMap<User, UserCredResponse>()
            .ForMember(e => e.Name, src => src.MapFrom(e => e.Profile.Firstname + " " + e.Profile.Lastname));
            CreateMap<TeacherType, TeacherTypeCred>();
            CreateMap<Group, GroupEditResponse>()
            .ForMember(e => e.MainTeacher,
            src => src.MapFrom(e => e.MainTeacherEntity))
            .ForMember(e => e.MedTeacher,
            src => src.MapFrom(e => e.MedTeacherEntity))
            .ForMember(e => e.MainAssistant, src => src.MapFrom(e => e.MainAssistantTeacherEntity))
            .Include<Group, GroupShowResponse>();

            CreateMap<Group, GroupEditResponseWithUsers>()
            .ForMember(e => e.PracticeTeacher,
             src => src.MapFrom(e => e.PracticeTeacherEntity))
            .ForMember(e => e.PracticeAssistantTeacher,
            src => src.MapFrom(e => e.PracticeAssistantTeacherEntity));

            CreateMap<Group, GroupShowResponse>()
            .ForMember(e => e.CurriculumCount, src => src.MapFrom(e => e.GroupCurriculum.Count()));

            CreateMap<GroupCurriculum, ScheduleResponse>()
            .ForMember(e => e.LessonName, src => src.MapFrom(e => e.Lesson == null ? e.AdditionalLesson : e.Lesson.Name))
            .ForMember(e => e.TeacherTypeName, src => src.MapFrom(e => e.Type.DisplayName));

            CreateMap<Group, GroupResponse>()
            .ForMember(e => e.PeriodName, src => src.MapFrom(e => e.Period.Name));

            CreateMap<Group, GroupOnlyResponse>();

            CreateMap<User, StudentResponse>()
            .ForMember(e => e.Name, src => src.MapFrom(e => e.Profile.Lastname + " " + e.Profile.Firstname))
            .ForMember(e => e.GroupName, src => src.MapFrom(e => e.Group.GroupName))
            .ForMember(e => e.CategoryName, src => src.MapFrom(e => e.Category.CategoryName))
            .Include<User, StudentAutoListResponse>()
            .Include<User, StudentFireResponse>();
            
            CreateMap<User, StudentAutoListResponse>()
            .ForMember(e => e.Passport, src => src.MapFrom(e => e.Passport.PassportSerial + e.Passport.PassportNumber));


            CreateMap<UserProfile, CalculatePriceResponse>();
            CreateMap<User, StudentFireResponse>();
        }
    }
}