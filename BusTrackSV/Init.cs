using BusTrackSV.API;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;


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
/*var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

//builder.Services.AddSingleton(new DbConnector(connectionString));

builder.Services.AddScoped<DbConnector>(_ => new DbConnector(connectionString));

builder.Services.AddScoped<BusRepository>(); 
builder.Services.AddScoped<UsuarioRepository>();
builder.Services.AddScoped<RutasRepository>();*/


builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirTodo", policy =>
    {
        policy
            .WithOrigins("http://localhost:8080")
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

app.UseHttpsRedirection();

app.UseCors("PermitirTodo");

app.UseAuthorization();
app.UseAuthorization();

app.MapGet("/secure", [Authorize]() =>
{
    return Results.Json(new { msg = "Ruta protegida con JWT" });   
});
app.MapAuth();
app.MapBus();

app.Run();
