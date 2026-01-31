using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Services
{
    public class MapperProfile : Profile
    {
        private readonly string imagesPath;
        public MapperProfile()
        {
            imagesPath = Path.Combine(Directory.GetCurrentDirectory(), "images");

            //לכתוב בפועל למי צריך המרה 
            //CreateMap<SourceType, DestinationType>().ForMember(dest => dest.DestProperty, opt => opt.MapFrom(src => src.SourceProperty));
        }


        // --- פונקציות עזר ---

        private byte[] ConvertImageToBytes(string fileName)
        {
            string fullPath = Path.Combine(imagesPath, fileName);
            return File.Exists(fullPath) ? File.ReadAllBytes(fullPath) : Array.Empty<byte>();
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
