using BusTrackSV.API;
using BusTrackSV.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ConexionBD;
using Data.Repositories;

/*using ConexionBD;
using Data.Repositories;*/

var builder = WebApplication.CreateBuilder(args);

var key = "equipovicturboequipovicturboequipovicturboequipovicturbo"; //te odio Ivan;

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
    }
);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if(connectionString == null)
{
    throw new Exception("Connection string 'DefaultConnection' not found.");
}

builder.Services.AddScoped<DbConnector>(_ => new DbConnector(connectionString));

builder.Services.AddScoped<BusRepository>(); 
builder.Services.AddScoped<UsuarioRepository>();
builder.Services.AddScoped<ChoferRepository>();
builder.Services.AddScoped<RutasRepository>();
builder.Services.AddScoped<CoordenadasRepository>();
builder.Services.AddScoped<PuntosRutaRepository>();


builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<BusService>();
builder.Services.AddScoped<ChoferService>();
builder.Services.AddScoped<RutaService>();
builder.Services.AddSingleton<SnapToRoadService>();


builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirTodo", policy =>
    {
        policy
            .WithOrigins("http://localhost:5500", "http://127.0.0.1:5500") // Reemplaza con los orígenes permitidos
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddControllers();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}


//Comentado porque no existe certificado SSL en desarrollo
//app.UseHttpsRedirection();

app.UseCors("PermitirTodo");

app.UseAuthorization();

app.MapAuthController();
app.MapBusController();
app.MapChoferController();
app.MapRutaController();

app.Run();
