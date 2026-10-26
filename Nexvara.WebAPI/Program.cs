using AutoMapper;
using Core.Common;
using Core.Configuration;
using Core.Interface.Repository;
using Core.Interface.Services;
using Core.Mapping;
using Domain.Entities;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Nexvara.WebAPI.Middleware;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

#region Mail Configuration

var emailSettings = builder.Configuration.GetSection("EmailSettings");
var smtpHost = emailSettings["Host"];
var smtpPort = emailSettings.GetValue<int>("Port");
var username = emailSettings["Username"];
var password = emailSettings["Password"];
var fromName = emailSettings["FromName"];
var toEmail = emailSettings["ToEmail"];
builder.Services.Configure<EmailSetting>(emailSettings);
#endregion

// Add services to the container.

builder.Services.AddControllers();

#region Cloudinary
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("Cloudinary"));
#endregion

#region Global Exception
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
#endregion

#region Connection string
builder.Services.AddDbContext<ApplicationDbContext>(Options => Options.UseSqlServer(builder.Configuration.GetConnectionString("con")));
#endregion

#region Cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactPolicy", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173", "http://localhost:3000", "https://nexvara-technology.onrender.com/")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
#endregion

#region AutoMapper
builder.Services.AddAutoMapper(
    cfg => { },
    typeof(MapppingData).Assembly
);
#endregion


#region DI

builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        // Password policy
        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = true;

        // Account lockout policy
        options.Lockout.AllowedForNewUsers = true;
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan =
            TimeSpan.FromMinutes(15);

        // Email configuration
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();
builder.Services.AddScoped<IUIRepository, UIRepository>();
builder.Services.AddScoped<IUIServices, UIServices>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IFileUploadServices, FileUploadServices>();
builder.Services.AddScoped<IAccountServices, AccountServices>();
builder.Services.AddScoped<IEmailServices, EmailServices>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<ICoursesServices, CoursesServices>();
builder.Services.AddScoped<ICoursesRepository, CoursesRepository>();
builder.Services.AddScoped<IBatchRepository, BatchRepository>();
builder.Services.AddScoped<IBatchServices, BatchServices>();
builder.Services.AddScoped<ITeacherRepository, TeacherRepository>();
builder.Services.AddScoped<ITeacherServices, TeacherServices>();
#endregion

#region JwtConfiguration
var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is missing.");
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT Issuer is missing.");
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? throw new InvalidOperationException("JWT Audience is missing.");
builder.Services.AddAuthentication(options => {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(options =>{
        options.TokenValidationParameters = new TokenValidationParameters {
                ValidateIssuer = true,
                ValidIssuer = jwtIssuer,
                ValidateAudience = true,
                ValidAudience = jwtAudience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtKey)),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
        };
    });

// Swagger + JWT Bearer Authentication
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Nexvara Web API",
        Version = "v1",
        Description = "Nexvara API with JWT Authentication"
    });

    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme,
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = JwtBearerDefaults.AuthenticationScheme,
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter your JWT token here. Do not type 'Bearer '."
        });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme
                }
            },
            Array.Empty<string>()
        }
    });
});
#endregion
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi


var app = builder.Build();
await SeederData.SeedAsync(app.Services);
// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Nexvara Web API v1");
    options.RoutePrefix = "swagger";
});
//}
app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseCors("ReactPolicy");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
