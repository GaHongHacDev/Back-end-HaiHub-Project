﻿using BirthdayParty.WebApi.Constants;
using Hairhub.API.Hubs;
using Hairhub.Infrastructure;
using Hairhub.Infrastructure.Configuration;
using Hairhub.Service.Helpers;
using Hairhub.Service.Services.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Org.BouncyCastle.Asn1.IsisMtt.Ocsp;
using System.ComponentModel;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Lấy đường dẫn chứng chỉ và private key từ appsettings.json
var certPath = builder.Configuration["Kestrel:Endpoints:Https:Certificate:Path"];
var certKeyPath = builder.Configuration["Kestrel:Endpoints:Https:Certificate:KeyPath"];

// Cấu hình Kestrel để lắng nghe HTTPS
builder.WebHost.UseKestrel(options =>
{
    if (!string.IsNullOrEmpty(certPath) && !string.IsNullOrEmpty(certKeyPath))
    {
        options.ListenAnyIP(443, listenOptions =>
        {
            listenOptions.UseHttps(certPath, certKeyPath);
        });
    }
});

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//Config swagger
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

//******************* Add services to the container  **************************

//Dependecy Injection
builder.Services.AddUnitOfWork();
builder.Services.AddDIServices();
builder.Services.AddDIRepositories();
builder.Services.AddDIAccessor();

//Auto Mapping
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

// Add services to the container.
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();


//Add Background Service 
builder.Services.AddHostedService<BackgroundWorkerService>();
builder.Services.AddDbContext<HaiHubDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.EnableSensitiveDataLogging(); // Thêm dòng này
});

//Config SignalR RealTime
builder.Services.AddSignalR();

//Setting Cors for all source
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: CorsConstant.PolicyName,
        policy => { policy.WithOrigins("*").AllowAnyHeader().AllowAnyMethod(); });
});

//Jwt configuration starts here
var jwtIssuer = builder.Configuration.GetSection("JwtSettings:Issuer").Get<string>();
var jwtAudience = builder.Configuration.GetSection("JwtSettings:Audience").Get<string>();
var jwtKey = builder.Configuration.GetSection("JwtSettings:Key").Get<string>();
//Config JWT
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
builder.Services.AddAuthorization();

builder.Services.AddHttpsRedirection(options =>
{
    options.HttpsPort = 443; // Cổng HTTPS mặc định là 443
});

//****BUILD
var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(CorsConstant.PolicyName);

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Map Hub and define route of hub
app.MapHub<BookHub>("book-hub");

app.Run();