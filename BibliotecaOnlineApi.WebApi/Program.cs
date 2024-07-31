using BibliotecaOnlineApi.Infraestructura.Data;
using BibliotecaOnlineApi.Infraestructura.Servicios.AutenticacionServicio;
using BibliotecaOnlineApi.Infraestructura.Servicios.AutenticacionServicio.Interfaces;
using BibliotecaOnlineApi.Infraestructura.Servicios.LibroServicio;
using BibliotecaOnlineApi.Infraestructura.Servicios.LibroServicio.Interfaces;
using BibliotecaOnlineApi.Model.Configuracion;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Configurar Coonexion a BBDD
const string nombreConexion = "NombreConexion";
var ConfigConexion = builder.Configuration.GetConnectionString(nombreConexion);

builder.Services.AddDbContext<AppDbContext>(opcion =>
{
    opcion.UseSqlServer(
        ConfigConexion ?? throw new InvalidOperationException("Cadena de conexion no encontrada")
        );
});


//Configuracion de Autenticacion con JWT
///Nos permitira usar la clave en la clase
builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("JwtConfig"));


//actualizamos el middelware para que sepa que hay una autenticacion que debe verificar

var key = Encoding.ASCII.GetBytes(builder.Configuration.GetSection("JwtConfig:Secret").Value);

//para verifica los parametros del token, valida que el toke sea de nuestra app y no uno random de internet
var ParametrosValidacionToken = new TokenValidationParameters()
{
    ValidateIssuerSigningKey = true, ///por cada peticion, verifica la clave de firma del emisor
    IssuerSigningKey = new SymmetricSecurityKey(key), //compara nuestra clave con la que el token envia, y deben ser iguales (simetria)
    ValidateIssuer = false, ///debe ser true, pero en proyecto local al generarlo causa un problema, para fines de prueba se pone false
    ValidateAudience = false, ///solo para fines de prueba es falso
    ValidateLifetime = true,
    RequireExpirationTime = false, ///solo para practica, ya que los token jwt son de corta duracion, solo viven 30 segundos, y no tener que generar otros
    ClockSkew = TimeSpan.Zero
};


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer( jwt =>
    {
        jwt.SaveToken = true;
        jwt.TokenValidationParameters = ParametrosValidacionToken;
    });

builder.Services.AddSingleton(ParametrosValidacionToken);


//agregar el administrador de identidad predeterminado
builder.Services.AddDefaultIdentity<IdentityUser>(options => 
options.SignIn.RequireConfirmedEmail =false)
    .AddEntityFrameworkStores<AppDbContext>();


//inyeccion servicios

builder.Services.AddScoped<IAutenticacionServicios, AutenticacionServicios>();
builder.Services.AddScoped<ILibroServicios, LibroServicios>();


builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//usamos la autenticacion que acabammos de configurar, en el midelware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
