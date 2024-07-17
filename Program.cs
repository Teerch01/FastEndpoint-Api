using FastEndpoints;
using FastEndpoints.Swagger;
using FastEndpoints.Security;
using WebApi.Data;
using Microsoft.EntityFrameworkCore;
using WebApi.Service;

var bld = WebApplication.CreateBuilder();

var conn = bld.Configuration.GetConnectionString("DefaultConnection");
bld.Services
    .AddDbContext<UserContext>(option => option.UseMySql(conn, ServerVersion.AutoDetect(conn)))
    .AddScoped<UserService>()
    .AddAuthenticationJwtBearer(s => s.SigningKey = "admin")
    .AddAuthorization()
    .AddFastEndpoints()
    .SwaggerDocument(); //define a swagger document

var app = bld.Build();
app.UseAuthentication()
    .UseAuthorization()
    .UseFastEndpoints()
   .UseSwaggerGen(); //add this

app.MapGet("/", () => @"UserApi. Navigate to /Swagger to open test UI");
app.Run();


