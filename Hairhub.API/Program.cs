using BirthdayParty.WebApi.Constants;
using Hairhub.API.Hubs;
using Hairhub.Common.ThirdParties.Implementation;
using Hairhub.Infrastructure;
using Hairhub.Infrastructure.Configuration;
using Hairhub.Service.Helpers;
using Hairhub.Service.Services.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.ComponentModel;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json.Serialization;
using Hairhub.Domain.JsonConverter;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// Config swagger
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

// ******************* Add services to the container  **************************

// Dependency Injection
builder.Services.AddUnitOfWork();
builder.Services.AddDIServices();
builder.Services.AddDIRepositories();
builder.Services.AddDIAccessor();

// Auto Mapping
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

// Add services to the container.
builder.Services.AddHttpClient<GeminiAIService>(client =>
{
    client.BaseAddress = new Uri("https://generativelanguage.googleapis.com/v1beta/");
});
builder.Services.AddHttpContextAccessor();

// Add Background Service 
builder.Services.AddHostedService<BackgroundWorkerService>();
builder.Services.AddDbContext<HaiHubDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.EnableSensitiveDataLogging();
});

// Config SignalR RealTime
builder.Services.AddSignalR();

// Setting CORS for all sources
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy(name: CorsConstant.PolicyName,
//        policy =>
//        {
//            policy.WithOrigins("http://localhost:5173") // Chỉ định origin cụ thể
//                  .AllowAnyHeader()
//                  .AllowAnyMethod()
//                  .AllowCredentials(); // Phải có AllowCredentials khi gửi credentials
//        });
//});
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: CorsConstant.PolicyName,
        policy => {
            policy.WithOrigins("http://localhost:5173", "https://www.hairhub.com.vn", "https://hairhub.id.vn", "http://localhost:3010")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();  // Allow credentials for CORS
        });
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new TimeOnlyJsonConverter());
    });

// Jwt configuration starts here
var jwtIssuer = builder.Configuration.GetSection("JwtSettings:Issuer").Get<string>();
var jwtAudience = builder.Configuration.GetSection("JwtSettings:Audience").Get<string>();
var jwtKey = builder.Configuration.GetSection("JwtSettings:Key").Get<string>();

// Config JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
 .AddJwtBearer(options =>
 {
     options.TokenValidationParameters = new TokenValidationParameters
     {
         ValidateIssuer = true,
         ValidateAudience = true,
         ValidateLifetime = true,
         ValidateIssuerSigningKey = true,
         ValidIssuer = jwtIssuer,
         ValidAudience = jwtAudience,
         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
     };
 });
 //.AddGoogle(googleOption =>
 //{
 //    googleOption.ClientId = builder.Configuration["Authentication:Google:ClientId"];
 //    googleOption.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
 //});

builder.Services.AddAuthorization();

// **** BUILD
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(CorsConstant.PolicyName);

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// MapHub and define route of hub 
app.MapHub<BookAppointmentHub>("book-appointment-hub");

app.Run();


