using BibliotecaOnlineApi.Infraestructura.Configuracion.ConfiguracionJwt;
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
using BibliotecaOnlineApi.Model.Modelo;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

const string nombreConexion = "NombreConexion";
var ConfigConexion = builder.Configuration.GetConnectionString(nombreConexion);
builder.Services.AddDbContext<AppDbContext>(opcion =>
{
    opcion.UseSqlServer(
        ConfigConexion ?? throw new InvalidOperationException("Cadena de conexion no encontrada")
        );
}); 


builder.Services.ConfigurarJwt(builder.Configuration);

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


builder.Services.AddSwaggerGen(
    opciones =>
    {
        opciones.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header, //donde va a ir al bearer token
            Description = "JWT Autorizacion header"
        });

        opciones.AddSecurityRequirement(new OpenApiSecurityRequirement()
        {{
            new OpenApiSecurityScheme ()
            {
                Reference =  new OpenApiReference()
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string []{ }
        }});
    }
);

//configuracion de Cors
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
app.UseSwagger();
app.UseSwaggerUI();

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
