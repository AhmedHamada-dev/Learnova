using Learnova.Application.IRepository;
using Learnova.Application.IServices;
using Learnova.Application.Services;
using Learnova.Domain.Identity;
using Learnova.Infrastructure.Persistence;
using Learnova.Infrastructure.Repositories;
using Learnova.Infrastructure.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace Learnova.Infrastructure.Extensions
{
    public static class ServiceCollectionExtenstions
    {
        public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<LearnovaDbContext>(obtions =>
            {
                obtions.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

            services.Configure<IdentityOptions>(options =>
            {
                options.Lockout.MaxFailedAccessAttempts = 5;     
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15); 
                options.Lockout.AllowedForNewUsers = true;
            });
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireNonAlphanumeric = true; 
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;

                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<LearnovaDbContext>()
            .AddDefaultTokenProviders();
            services.AddHttpClient();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IEmailService,EmailService>();
            services.AddScoped<IEmailVerificationRepository,EmailVerificationRepository>();

        }

        
    }
}