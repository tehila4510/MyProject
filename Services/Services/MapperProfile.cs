using AutoMapper;
using Common.Dto.Question;
using Common.Dto.Sessions;
using Common.Dto.User;
using Common.Dto.UserProgress;
using Repository.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Services.Services
{
    public class MapperProfile : Profile
    {
        private readonly string imagesPath;
        public MapperProfile()
        {
            imagesPath = Path.Combine(Directory.GetCurrentDirectory(), "ProfileImages");

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
                .ForMember(d => d.AnsweredAt, o => o.MapFrom(_ => DateTime.UtcNow));

            //Session

            CreateMap<Session, SessionDto>()
                .ForMember(d => d.DurationInMinutes, 
                o => o.MapFrom(s => s.StartedAt.HasValue && s.EndedAt.HasValue? (s.EndedAt.Value - s.StartedAt.Value).TotalMinutes : 0));

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

            CreateMap<Question, QuestionDto>().ReverseMap();
            CreateMap<QuestionOption, QuestionOptionDto>().ReverseMap();
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
