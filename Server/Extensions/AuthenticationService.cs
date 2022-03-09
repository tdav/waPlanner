using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace waPlanner.Extensions
{
    public static class AuthenticationService
    {
        public static void AddMyAuthentication(this IServiceCollection services, IConfiguration confi)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            var str = confi["SystemVars:PrivateKeyString"];
            var key = Encoding.ASCII.GetBytes(str);

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
             {
                 x.SaveToken = true;
                 x.RequireHttpsMetadata = false;
                 x.TokenValidationParameters = new TokenValidationParameters
                 {
                     ValidateIssuerSigningKey = true,
                     IssuerSigningKey = new SymmetricSecurityKey(key),
                     ValidateIssuer = false,
                     ValidateAudience = false
                 };
             });
        }
    }
}
