using BusTrackSV.API;
using BusTrackSV.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ConexionBD;
using Data.Repositories;

var builder = WebApplication.CreateBuilder(args);

var key = "equipovicturboequipovicturboequipovicturboequipovicturbo";

// ================= JWT =================
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
        };        
    });

// ============ Base de Datos ============
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (connectionString == null)
    throw new Exception("Connection string 'DefaultConnection' not found.");

builder.Services.AddScoped<DbConnector>(_ => new DbConnector(connectionString));

// ============ Repositorios ============
builder.Services.AddScoped<BusRepository>();
builder.Services.AddScoped<UsuarioRepository>();
builder.Services.AddScoped<ChoferRepository>();
builder.Services.AddScoped<RutasRepository>();
builder.Services.AddScoped<CoordenadasRepository>();
builder.Services.AddScoped<PuntosRutaRepository>();

// ============ Servicios ============
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<BusService>();
builder.Services.AddScoped<ChoferService>();
builder.Services.AddScoped<RutaService>();
builder.Services.AddSingleton<SnapToRoadService>();
builder.Services.AddSingleton<BusTrackingService>();

// ============ CORS (Arreglado) ============
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirTodo", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5500",
                "https://survival-publicly-sbjct-cooling.trycloudflare.com",
                "https://invite-terrorist-destination-madrid.trycloudflare.com",
                "http://10.74.10.247:5500"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials() // ← IMPORTANTE PARA QUE SE ENVÍE Authorization
            .WithExposedHeaders("Authorization");
    });
});

// ============ MVC ============
builder.Services.AddControllers();

var app = builder.Build();

app.UseCors("PermitirTodo");
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// ============ Controladores ============
app.MapAuthController();
app.MapBusController();
app.MapChoferController();
app.MapRutaController();

app.Run();
