using Microsoft.AspNetCore.Http;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class FileService : IFileService
    {
        public async Task<string> SaveFileAsync(IFormFile file)
        {
            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var path = Path.Combine(Environment.CurrentDirectory, "ProfileImages", fileName);

            using var stream = new FileStream(path, FileMode.Create);
            await file.CopyToAsync(stream);

            return fileName;
        }
    }
}
