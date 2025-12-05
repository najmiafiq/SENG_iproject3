using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;
using TEMMU.API.Services;
using TEMMU.Core.Interfaces;


    public class Program
    {
        
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // --- 1. CONFIGURATION ---
        var configuration = builder.Configuration;

        // --- 2. DATABASE (Entity Framework Core) ---
        builder.Services.AddDbContext<GameDBContext>(options =>
            // will try to get connection string named "DefaultConnection" from appsettings.json
            // or from docker secrets or environment variables
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // --- 3. IDENTITY AND AUTHENTICATION ---
        // a. Add Identity (User Management)
        builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<GameDBContext>() // Link Identity to EF Core context
            .AddDefaultTokenProviders();

        // b. Add JWT Authentication (Token Validation)
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["Jwt:Issuer"],
                ValidAudience = configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
            };
        });

        // --- 4. SERVICES AND DEPENDENCY INJECTION ---
        builder.Services.AddControllers();

        // Register Repositories and Services
        builder.Services.AddScoped<IFighterRepository, FighterRepository>();
        builder.Services.AddScoped<ITokenService, TokenService>(); // The JWT token generation service

        // Add AutoMapper: Scans assemblies for classes inheriting from Profile (e.g., FighterProfile)
        builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            // Configure the UI to allow JWT token input (Bearer scheme)
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("Bearer", document)] = []
            });
        });

        // --- 6. APPLICATION BUILD AND PIPELINE CONFIGURATION ---
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            // Enable Swagger Middleware
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        // Global error handling middleware can go here (app.UseExceptionHandler())

        // Use HTTPS Redirection (recommended for security)
        app.UseHttpsRedirection();

        // --- AUTHENTICATION AND AUTHORIZATION MIDDLEWARE (CRUCIAL ORDER) ---
        // 1. Authentication: Checks if the token is valid (WHO is the user?)
        app.UseAuthentication();
        // 2. Authorization: Checks if the authenticated user can access the resource (CAN the user do this?)
        app.UseAuthorization();

        // Map controllers to handle incoming HTTP requests
        app.MapControllers();

        app.Run();
    }
    }
