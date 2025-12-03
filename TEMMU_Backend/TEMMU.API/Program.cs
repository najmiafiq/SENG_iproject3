using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;
using TEMMU.API.Services;
using TEMMU.Core.Interfaces;
using TEMMU.Infrastructure.Data;
using TEMMU.Infrastructure.Repositories;


namespace TEMMU.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // 1. Add Identity services
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                    .AddEntityFrameworkStores<GameDBContext>()
                    .AddDefaultTokenProviders();
            builder.Services.AddEndpointsApiExplorer();
            // 2. Add JWT Authentication services
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                    };
                });

            // Add Swagger Generator: This generates the OpenAPI specification file (the JSON describing your API).
            builder.Services.AddSwaggerGen(c =>
            {
                // --- Configuration for JWT/Security ---
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token.",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                // Defines which endpoints require the security scheme
                c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference("Bearer", document)] = []
                });
            });

            // 3. Register required services
            builder.Services.AddScoped<IFighterRepository, FighterRepository>();
            builder.Services.AddScoped<TokenService>();
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies()); // Use AutoMapper for DTO mapping

            // ... other services ...

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                // Exposes the OpenAPI specification (swagger.json file)
                app.UseSwagger();

                // Exposes the interactive documentation UI (accessible at /swagger)
                app.UseSwaggerUI(c =>
                {
                    // Optional: Custom path for the UI
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Game API v1");
                });
            }

            // 4. Use the middleware (Crucial order)
            app.UseHttpsRedirection();
            app.UseAuthentication(); // Must be before UseAuthorization
            app.UseAuthorization();
            app.MapControllers();
        }
    }
}
