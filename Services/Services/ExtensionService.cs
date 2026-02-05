using Common.Dto.User;
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
            services.AddScoped<IService<UserDto>, UserService>();
            services.AddScoped<IsExist<UserDto>, UserService>();
            return services;
        }
    }
}
