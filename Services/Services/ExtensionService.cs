using Common.Dto.Question;
using Common.Dto.Sessions;
using Common.Dto.User;
using Common.Dto.UserProgress;
using Microsoft.Extensions.DependencyInjection;
using Repository.Repositories;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Services
{
    public static class ExtensionService
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddRepository();
            //services.AddScoped<IService<UserDto>, UserService>();
            services.AddScoped<IsExist<UserDto>, UserService>();

            services.AddScoped<IService<QuestionOptionDto>, QuestionOptionService>();
            services.AddScoped<IService<QuestionDto>, QuestionService>();
            services.AddScoped<IService<SessionDto>, SessionService>();
            services.AddScoped<IService<UserAnswerDto>, UserAnswerService>();
            services.AddScoped<IService<UserSkillProgressDto>, UserSkillProgressService>();
            services.AddScoped<IOpenAi, Chat>();

            return services;
        }
    }
}
