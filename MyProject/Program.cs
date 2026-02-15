
using DataContext;
using Microsoft.Extensions.DependencyInjection;
using Repository.Interfaces;
using Services.Interfaces;
using Services.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using DataContext.model;


namespace MyProject
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);
            var connection = builder.Configuration.GetConnectionString("DefaultConnection");

            builder.Services.AddHttpClient();
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            builder.Services.AddSingleton<IContext>(new GlottieContext(connection));
            builder.Services.AddSingleton<IOpenAi, Chat>(); // Chat עם HttpClient
            builder.Services.AddAutoMapper(typeof(MapperProfile));
            builder.Services.AddServices();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();


            //authorize -אימות 
            //authenticate -הרשאות גישה

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
