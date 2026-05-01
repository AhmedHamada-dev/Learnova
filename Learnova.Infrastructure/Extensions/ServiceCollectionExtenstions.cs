using Learnova.Application.Repositories;
using Learnova.Infrastructure.Persistence;
using Learnova.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace TadaWy.Infrastructure.Extensions
{
    public static class ServiceCollectionExtenstions
    {
        public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<LearnovaDbContext>(obtions =>
            {
                obtions.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });
            services.AddHttpClient();
            services.AddScoped<IUserRepository, UserRepository>();
            
        }

        
    }
}