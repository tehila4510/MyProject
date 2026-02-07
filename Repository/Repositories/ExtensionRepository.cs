using Repository.Entities;
using Microsoft.Extensions.DependencyInjection;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repository.Repositories
{
    public static class ExtensionRepository
    {
        public static IServiceCollection AddRepository(this IServiceCollection services)
        {
            services.AddScoped<IRepository<User>, UserRepository>();
            services.AddScoped<IRepository<UserAnswer>, UserAnswerRepository>();
            services.AddScoped<IRepository<UserSkillProgress>, UserSkillProgressRepository>();
            services.AddScoped<IRepository<Session>, SessionRepository>();
            services.AddScoped<IRepository<Question>, QestionRepository>();
            services.AddScoped<IRepository<QuestionOption>, QuestionOptionRepository>();
            return services;
        }
    }
}
