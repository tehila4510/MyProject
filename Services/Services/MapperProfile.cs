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
            imagesPath = Path.Combine(Directory.GetCurrentDirectory(), "IMG");

            // Entity -> DTO  (שליפה מהמערכת)
            // Entity -> DTO
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.AvatarUrl,
                           opt => opt.MapFrom(src => Encoding.UTF8.GetBytes(src.AvatarUrl)))
                .ForMember(dest => dest.file, opt => opt.Ignore());

            // DTO -> Entity
            CreateMap<UserDto, User>()
                .ForMember(dest => dest.AvatarUrl,
                           opt => opt.MapFrom(src => src.AvatarUrl != null ? Encoding.UTF8.GetString(src.AvatarUrl) : null));

            CreateMap<Session, SessionDto>().ReverseMap();
            CreateMap<UserAnswer, UserAnswerDto>().ReverseMap();
            CreateMap<UserSkillProgress, UserSkillProgressDto>().ReverseMap();
            CreateMap<Question, QuestionDto>().ReverseMap();
            CreateMap<QuestionOption, QuestionOptionDto>().ReverseMap();
        }


        // --- פונקציות עזר ---

        private byte[] ConvertImageToBytes(string base64String)
        {
            return !string.IsNullOrEmpty(base64String) ? Convert.FromBase64String(base64String) : Array.Empty<byte>();
        }

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
