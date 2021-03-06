using API.Data;
using API.Data.Repositories;
using API.Helpers;
using API.Interfaces;
using API.Interfaces.Repositories;
using API.Services;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            // Add Token Service
            services.AddScoped<ITokenService, TokenService>();

            // Add Repository
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ILikesRepository, LikesRepository>();

            // Add AutoMapper
            services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);


            // Add Database Context
            services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlite(config.GetConnectionString("DefaultConnection"));
            });

            return services;
        }
    }
}