//using AutoMapper;
//using Common.Dto;
//using DataContext.Entities;
//using System;
//using System.Collections.Generic;
//using System.IO;

//namespace Service.Services
//{
//    public class MapperProfile : Profile
//    {
//        private readonly string imagesPath;

//        public MapperProfile()
//        {
//            // תיקיית תמונות
//            imagesPath = Path.Combine(Directory.GetCurrentDirectory(), "images");

//            // --- תמונות ---
//            CreateMap<User, UserDto>()
//                .ForMember(dest => dest.ArrImage,
//                           opt => opt.MapFrom(src => ConvertImageToBytes(src.ImageUrl)))
//                .ForMember(dest => dest.Status,
//                           opt => opt.MapFrom(src => ConvertBoolToStatus(src.IsActive)))
//                .ForMember(dest => dest.BirthDateString,
//                           opt => opt.MapFrom(src => ConvertDateToString(src.BirthDate)));

//            CreateMap<BabyDto, Baby>()
//                .ForMember(dest => dest.ImageUrl,
//                           opt => opt.MapFrom(src => src.ImageFile?.FileName ?? string.Empty))
//                .ForMember(dest => dest.IsActive,
//                           opt => opt.MapFrom(src => ConvertStatusToBool(src.Status)))
//                .ForMember(dest => dest.BirthDate,
//                           opt => opt.MapFrom(src => ConvertStringToDate(src.BirthDateString)));

//            // --- רשימות ---
//            CreateMap<List<Baby>, List<BabyDto>>().ReverseMap();

//            // --- Enum / Flags ---
//            CreateMap<User, UserDto>()
//                .ForMember(dest => dest.Role,
//                           opt => opt.MapFrom(src => src.Role.ToString()))
//                .ForMember(dest => dest.IsVerifiedStatus,
//                           opt => opt.MapFrom(src => src.IsVerified ? "Yes" : "No"));

//            CreateMap<UserDto, User>()
//                .ForMember(dest => dest.Role,
//                           opt => opt.MapFrom(src => Enum.Parse(typeof(UserRole), src.Role)))
//                .ForMember(dest => dest.IsVerified,
//                           opt => opt.MapFrom(src => src.IsVerifiedStatus.Equals("Yes", StringComparison.OrdinalIgnoreCase)));

//            // --- תאריכים ---
//            CreateMap<Order, OrderDto>()
//                .ForMember(dest => dest.OrderDateString,
//                           opt => opt.MapFrom(src => ConvertDateToString(src.OrderDate)));
//            CreateMap<OrderDto, Order>()
//                .ForMember(dest => dest.OrderDate,
//                           opt => opt.MapFrom(src => ConvertStringToDate(src.OrderDateString)));
//        }

//        // --- פונקציות עזר ---

//        private byte[] ConvertImageToBytes(string fileName)
//        {
//            string fullPath = Path.Combine(imagesPath, fileName);
//            return File.Exists(fullPath) ? File.ReadAllBytes(fullPath) : Array.Empty<byte>();
//        }

//        private string ConvertBoolToStatus(bool flag)
//        {
//            return flag ? "Active" : "Inactive";
//        }

//        private bool ConvertStatusToBool(string status)
//        {
//            return status.Equals("Active", StringComparison.OrdinalIgnoreCase);
//        }

//        private string ConvertDateToString(DateTime date)
//        {
//            return date.ToString("yyyy-MM-dd");
//        }

//        private DateTime ConvertStringToDate(string str)
//        {
//            return DateTime.TryParse(str, out var date) ? date : DateTime.MinValue;
//        }
//    }
//}
