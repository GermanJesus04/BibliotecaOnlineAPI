using BibliotecaOnlineApi.Model.DTOs.JwtDTOs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace BibliotecaOnlineApi.Infraestructura.Configuracion.ConfiguracionJwt
{
    public static class JsonWebTokenConfiguracion
    {
        public static void ConfigurarJwt(
            this IServiceCollection services,
            IConfiguration configuracion
            )
        {
            var bindJwtSetting = new JwtParametros();
            configuracion.Bind("JsonWebTokenKeys", bindJwtSetting);
            var ParametrosValidacionToken = new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = bindJwtSetting.KeyFirmaEsValida,
                IssuerSigningKey = new SymmetricSecurityKey(
                            System.Text.Encoding.UTF8.GetBytes(bindJwtSetting.Key)
                            ),
                ValidateIssuer = bindJwtSetting.EmisorEsValido,
                ValidIssuer = bindJwtSetting.Emisor,
                ValidateAudience = bindJwtSetting.AudienciaEsValida,
                ValidAudience = bindJwtSetting.Audiencia,
                RequireExpirationTime = bindJwtSetting.TiempoCaducidadEsValido,
                ValidateLifetime = bindJwtSetting.TiempoVidaEsValido,
                ClockSkew = TimeSpan.Zero
            };

            //add singleton de los jwtsetting
            services.AddSingleton(bindJwtSetting);
            services.AddSingleton(ParametrosValidacionToken);
            
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            })
                .AddJwtBearer(jwt =>
                {
                    jwt.SaveToken = true;
                    jwt.TokenValidationParameters = ParametrosValidacionToken;
                });
        }
    }
}
