using System.Text;
using API.Middlewares;
using Application;
using Application.Contracts;
using Application.Services;
//using Application.Validations;
//using Application.Settings;
using Domain.Entities;
using FluentValidation;
using FluentValidation.AspNetCore;
using Infrastructure.Repositories;
using Infrastructure.Contracts;
using Infrastructure.Data.DbContext;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Application.Validations;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace API.Extensions;

public static class ServiceExtensions
{
    public static void ConfigureCors(this IServiceCollection serviceCollection) =>
        serviceCollection.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", builder =>
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .WithExposedHeaders("X-Pagination"));
        });

    public static void ConfigureIisIntegration(this IServiceCollection serviceCollection) =>
        serviceCollection.Configure<IISOptions>(options => { });

    public static void ConfigureRepositoryManager(this IServiceCollection serviceCollection) =>
        serviceCollection.AddScoped<IRepositoryManager, RepositoryManager>();

    public static void ConfigureServiceManager(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddScoped<IServiceManager, ServiceManager>();
        serviceCollection.AddHttpClient<AdminUserService>();
    }
        
        


    public static void ConfigureSqlContext(this IServiceCollection serviceCollection, IConfiguration configuration) =>
        serviceCollection.AddDbContext<AppDbContext>(
            opts =>
            {
                opts.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

    //public static void ConfigureVersioning(this IServiceCollection services)
    //{
    //    services.AddApiVersioning(opt =>
    //    {
    //        opt.ReportApiVersions = true;
    //        opt.AssumeDefaultVersionWhenUnspecified = true;
    //        opt.DefaultApiVersion = new ApiVersion(1, 0);
    //    });
    //    services.AddVersionedApiExplorer(opt =>
    //    {
    //        opt.GroupNameFormat = "'v'VVV";
    //        opt.SubstituteApiVersionInUrl = true;
    //    });
    //}

    //public static void ConfigureResponseCaching(this IServiceCollection services) =>
    //    services.AddResponseCaching();

    //public static void ConfigureHttpCacheHeaders(this IServiceCollection services) =>
    //    services.AddHttpCacheHeaders(expirationOpt =>
    //    {
    //        expirationOpt.MaxAge = 65;
    //        expirationOpt.CacheLocation = CacheLocation.Private;
    //    }, validationOpt =>
    //    {

    //        validationOpt.MustRevalidate = true;
    //    });

    public static void ConfigureIdentity(this IServiceCollection services)
    {
        var builder = services.AddIdentity<User, Role>(opt =>
        {
            opt.Password.RequireDigit = true;
            opt.Password.RequireLowercase = true;
            opt.Password.RequireUppercase = true;
            opt.Password.RequireNonAlphanumeric = false;
            opt.Password.RequiredLength = 8;
            opt.User.RequireUniqueEmail = true;
        })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();
    }

    public static void ConfigureJWT(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["secret"];

        services.AddAuthentication(opt =>
        {
            opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings["ValidIssuer"],
                ValidAudience = jwtSettings["ValidAudience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]))
            };
        });
    }

    public static void ConfigureSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(s =>
        {
            s.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Better Reads Web API",
                Version = "v1",
                Description = "Add books to your Read List and leave reviews",
                Contact = new OpenApiContact
                {
                    Name = "Ikponwosa",
                    Email = "ikponwosae@outlook.com"
                }
            });

            var xmlFile = $"{typeof(API.AssemblyReference).Assembly.GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            // s.IncludeXmlComments(xmlPath);

            s.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Add JWT with Bearer",
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            s.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Name = "Bearer"
                    },
                    new List<string>()
                }
            });
        });
    }

    public static void ConfigureMvc(this IServiceCollection services)
    {
        services.AddMvc()
            .ConfigureApiBehaviorOptions(o =>
            {
                o.InvalidModelStateResponseFactory = context => new ValidationFailedResult(context.ModelState);
            })
            .AddFluentValidation(x => x.RegisterValidatorsFromAssemblyContaining<UserValidator>());
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
    }

    public static void ConfigureApiVersioning(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddApiVersioning(opt =>
        {
            opt.AssumeDefaultVersionWhenUnspecified = true;
            opt.DefaultApiVersion = new ApiVersion(1, 0);
            opt.ReportApiVersions = true;
        });
        services.AddVersionedApiExplorer(opt =>
        {
            opt.GroupNameFormat = "'v'VVV";
            opt.SubstituteApiVersionInUrl = true;
        });
        //services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
        services.AddMvcCore().AddApiExplorer();
    }
}