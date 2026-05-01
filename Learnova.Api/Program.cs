using Learnova.Application.Settings;
using Learnova.Domain.Identity;
using Learnova.Infrastructure.Persistence;
using Learnova.Infrastructure.Seeders;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TadaWy.Infrastructure.Seeders;


 
      
             var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JWT"));

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
             .AddEntityFrameworkStores<LearnovaDbContext>()
             .AddDefaultTokenProviders();

           
            var app = builder.Build();
            using (var scope = app.Services.CreateScope())
            {

                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                await RoleSeeder.SeedRolesAsync(roleManager);
                await AdminSeeder.SeedAdminAsync(userManager);
            }
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
     

