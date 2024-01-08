using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Carpo.Core.Web.Api.Extensions
{
    public static class CarpoServiceColletionJwtExtention
    {
        public static IServiceCollection AddJwtBearerConfig(this IServiceCollection services,
            IConfiguration configuration)
        {            
            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })            
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    
                    ValidIssuer = configuration["CarpoConfig:Jwt:Issuer"],
                    ValidAudience = configuration["CarpoConfig:Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["CarpoConfig:Jwt:SigningKey"])),
                    ClockSkew = TimeSpan.Zero
                };
            });
            return services;
        }
    }
}
