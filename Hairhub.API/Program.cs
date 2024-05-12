using BirthdayParty.WebApi.Constants;
using Hairhub.API.Helpers;
using Hairhub.Infrastructure.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//******************* Add services to the container  ****************************

//Dependecy Injection
builder.Services.RegisterServices();
//Auto Mapping
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

// Add services to the container.
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: CorsConstant.PolicyName,
        policy => { policy.WithOrigins("*").AllowAnyHeader().AllowAnyMethod(); });
});

//Jwt configuration starts here
var jwtIssuer = builder.Configuration.GetSection("Jwt:JWT_ISSUER").Get<string>();
var jwtKey = builder.Configuration.GetSection("Jwt:JWT_SECRET_KEY").Get<string>();
//Config JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
 .AddJwtBearer(options =>
 {
     options.TokenValidationParameters = new TokenValidationParameters
     {
         ValidateIssuer = true,
         ValidateAudience = false,
         /*             ValidateLifetime = true,
         */
         ValidateIssuerSigningKey = true,
         ValidIssuer = jwtIssuer,
         //ValidAudience = jwtIssuer,
         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
     };
 });


//****BUILD
var app = builder.Build();


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
