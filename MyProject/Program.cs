
using Common.Dto.Question;
using Common.Dto.Sessions;
using Common.Dto.User;
using Common.Dto.UserProgress;
using Services.Interfaces;
using Services.Services;

namespace MyProject
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddScoped<IService<QuestionOptionDto>, QuestionOptionService>();
            builder.Services.AddScoped<IService<QuestionDto>, QuestionService>();
            builder.Services.AddScoped<IService<SessionDto>, SessionService>();
            builder.Services.AddScoped<IService<UserAnswerDto>, UserAnswerService>();
            builder.Services.AddScoped<IService<UserDto>, UserService>();
            builder.Services.AddScoped<IService<UserSkillProgressDto>, UserSkillProgressService>();

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

            //app.UseAuthentication();
            //app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
