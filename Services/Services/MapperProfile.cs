using AutoMapper;
using Common.Dto.Question;
using Common.Dto.Sessions;
using Common.Dto.User;
using Common.Dto.UserProgress;
using Repository.Entities;
using System.Text;

namespace Services.Services
{
    public class MapperProfile : Profile
    {
        private readonly string imagesPath;
        private readonly string questionImagePath;
        public MapperProfile()
        {
           imagesPath = Path.Combine(Directory.GetCurrentDirectory(), "ProfileImages");
           questionImagePath= Path.Combine(Directory.GetCurrentDirectory(), "QuestionImages");

            //User

            // Entity -> DTO
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.AvatarUrl,
                           opt => opt.MapFrom(src => Encoding.UTF8.GetBytes(src.AvatarUrl)))
                .ForMember(dest => dest.file, opt => opt.Ignore());
            // DTO -> Entity
            CreateMap<UserDto, User>()
                .ForMember(dest => dest.AvatarUrl,
                           opt => opt.MapFrom(src => src.AvatarUrl != null ? Encoding.UTF8.GetString(src.AvatarUrl) : null))
                                .ForMember(d => d.Role, o => o.Ignore());

            CreateMap<UserUpdateDto, User>()
                .ForMember(dest => dest.AvatarUrl,
                           opt => opt.MapFrom(src => src.AvatarUrl != null ? Encoding.UTF8.GetString(src.AvatarUrl) : null))
                .ForMember(d => d.Role, o => o.Ignore())
                .ForMember(d => d.CurrentLevel, o => o.Ignore())
                .ForMember(d => d.Xp, o => o.Ignore())
                .ForMember(d => d.Streak, o => o.Ignore())
                .ForMember(d => d.Hearts, o => o.Ignore())
                .ForMember(d => d.LastActivity, o => o.Ignore())
                .ForMember(d => d.CreatedAt, o => o.Ignore())
                .ForMember(d => d.ProUntil, o => o.Ignore());
            //UserAnswer

            CreateMap<UserAnswer, UserAnswerDto>();
            CreateMap<UserAnswerDto, UserAnswer>()
                .ForMember(d => d.User, o => o.Ignore())
                .ForMember(d => d.Question, o => o.Ignore())
                .ForMember(d => d.Session, o => o.Ignore())
                .ForMember(d => d.IsCorrect, o => o.Ignore());


            //Session

            CreateMap<Session, SessionDto>()
                .ForMember(d => d.DurationInMinutes,
                o => o.MapFrom(s => s.StartedAt.HasValue && s.EndedAt.HasValue ? (s.EndedAt.Value - s.StartedAt.Value).TotalMinutes : 0));

            CreateMap<SessionDto, Session>()
                .ForMember(d => d.User, o => o.Ignore())
                .ForMember(d => d.UserAnswers, o => o.Ignore())
                .ForMember(d => d.StartedAt, o => o.Ignore())
                .ForMember(d => d.EndedAt, o => o.Ignore());


            //UserSkillProgress

            CreateMap<UserSkillProgress, UserSkillProgressDto>();
            CreateMap<UserSkillProgressDto, UserSkillProgress>()
                .ForMember(d => d.User, o => o.Ignore());

            //question

            CreateMap<Question, QuestionDto>()
            .ForMember(dest => dest.ImageUrl, opt => opt.Ignore())
            .ForMember(dest => dest.file, opt => opt.Ignore())
            .ForMember(dest => dest.AnswerRecordId, opt => opt.Ignore());
            


            CreateMap<QuestionDto, Question>()
                .ForMember(dest => dest.QuestionTypeMask, opt => opt.Ignore())
                .ForMember(dest => dest.AudioType, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.ImageURL, opt => opt.Ignore())
                .ForMember(dest => dest.UserAnswers, opt => opt.Ignore());

            //options
            CreateMap<QuestionOption, QuestionOptionDto>()
                 .ForMember(dest => dest.IsCorrect, opt => opt.Ignore());

            CreateMap<QuestionOptionDto, QuestionOption>()
                .ForMember(dest => dest.Question, opt => opt.Ignore());
        }


        // --- פונקציות עזר ---


        private string ConvertBoolToStatus(bool flag)
        {
            return flag ? "Active" : "Inactive";
        }

        private bool ConvertStatusToBool(string status)
        {
            return status.Equals("Active", StringComparison.OrdinalIgnoreCase);
        }

        private string ConvertDateToString(DateTime date)
        {
            return date.ToString("yyyy-MM-dd");
        }

        private DateTime ConvertStringToDate(string str)
        {
            return DateTime.TryParse(str, out var date) ? date : DateTime.MinValue;
        }
    }


}
