using Learnova.Application.Extensions;
using Learnova.Application.Settings;
using Learnova.Domain.Identity;
using Learnova.Infrastructure.Extensions;
using Learnova.Infrastructure.Persistence;
using Learnova.Infrastructure.Seeders;
using Microsoft.AspNetCore.Identity;
using TadaWy.Infrastructure.Seeders;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// Infrastructure (DbContext + Identity + Repos + AuthService)
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// JwtSettings binding (تأكد من اسم السكشن مطابق للـ appsettings)
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JWT"));
// لو عندك فعليًا "JWT" في appsettings، إما تغيره لـ "Jwt" أو تثبت هنا على "JWT".

var app = builder.Build();

// Seeding Roles & Admin
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    await RoleSeeder.SeedRolesAsync(roleManager);
    await AdminSeeder.SeedAdminAsync(userManager);
}

// Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();   // مهم لو عندك JWT
app.UseAuthorization();

app.MapControllers();

app.Run();