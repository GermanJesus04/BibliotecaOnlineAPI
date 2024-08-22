using BibliotecaOnlineApi.Infraestructura.Data;
using BibliotecaOnlineApi.Infraestructura.HelpierConfiguracion;
using BibliotecaOnlineApi.Infraestructura.Servicios.AutenticacionServicio;
using BibliotecaOnlineApi.Infraestructura.Servicios.AutenticacionServicio.Interfaces;
using BibliotecaOnlineApi.Infraestructura.Servicios.LibroServicio;
using BibliotecaOnlineApi.Infraestructura.Servicios.LibroServicio.Interfaces;
using BibliotecaOnlineApi.Infraestructura.Servicios.PrestamoServicio;
using BibliotecaOnlineApi.Infraestructura.Servicios.PrestamoServicio.Interfaces;
using BibliotecaOnlineApi.Infraestructura.Servicios.UsersServicio;
using BibliotecaOnlineApi.Infraestructura.Servicios.UsersServicio.Interfaces;
using BibliotecaOnlineApi.Model.Configuracion;
using BibliotecaOnlineApi.Model.Modelo;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


#region ***Configuracion de autenticacion con JWT***
builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("JwtConfig"));

var key = Encoding.ASCII.GetBytes(builder.Configuration.GetSection("JwtConfig:Secret").Value);

///para verifica los parametros del token sean correctos
var ParametrosValidacionToken = new TokenValidationParameters()
{
    ValidateIssuerSigningKey = true, ///por cada peticion, verifica la clave de firma del emisor
    IssuerSigningKey = new SymmetricSecurityKey(key), ///compara nuestra clave con la que el token envia, y deben ser iguales (simetria)
    ValidateIssuer = false, ///debe ser true, pero en proyecto local al generarlo causa un problema, para fines de prueba se pone false
    ValidateAudience = false, ///solo para fines de prueba es falso
    ValidateLifetime = true,
    RequireExpirationTime = false, ///solo para practica, ya que los token jwt son de corta duracion, solo viven 30 segundos, y no tener que generar otros
    ClockSkew = TimeSpan.Zero
};

builder.Services.AddSingleton(ParametrosValidacionToken);

///actualizamos el middelware para que sepa que hay una autenticacion que debe verificar
builder.Services.AddAuthentication(options =>
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

#endregion


#region **Configuracion DbContext
const string nombreConexion = "NombreConexion";
var ConfigConexion = builder.Configuration.GetConnectionString(nombreConexion);

builder.Services.AddDbContext<AppDbContext>(opcion =>
{
    opcion.UseSqlServer(
        ConfigConexion ?? throw new InvalidOperationException("Cadena de conexion no encontrada")
        );
});
#endregion


#region ***Configuracion de Identity
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = false;
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedEmail = false;
    options.Lockout.AllowedForNewUsers = false;
    options.Lockout.MaxFailedAccessAttempts = 5;
    //options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(4);
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders()
.AddPasswordValidator<PasswordValidator<User>>();

#endregion


const string misReglasCors = "ReglasCors";
builder.Services.AddCors(opc => 
{
    opc.AddPolicy(name: misReglasCors, builder =>
    {
        builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

///inyeccion servicios  
builder.Services.AddScoped<IAutenticacionServicios, AutenticacionServicios>();
builder.Services.AddScoped<ILibroServicios, LibroServicios>();
builder.Services.AddScoped<IPrestamoServicios, PrestamoServicios>();
builder.Services.AddScoped<IUsuarioServicios, UsuarioServicios>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.Configure<AdminSettings>(builder.Configuration.GetSection("AdminSettings"));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(misReglasCors);

app.UseHttpsRedirection();

//usamos la autenticacion que acabammos de configurar, en el midelware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Inicializar los roles y usuarios por defecto
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        var adminSettings = services.GetRequiredService<IOptions<AdminSettings>>().Value;
        var userManager = services.GetRequiredService<UserManager<User>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        await DatosSemilla.Inicializar(services, userManager, roleManager, adminSettings);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}


app.Run();
