using BusTrackSV.API;
using ConexionBD;
using Data.Repositories;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

//builder.Services.AddSingleton(new DbConnector(connectionString));

builder.Services.AddScoped<DbConnector>(_ => new DbConnector(connectionString));

builder.Services.AddScoped<BusRepository>(); 
builder.Services.AddScoped<UsuarioRepository>();
builder.Services.AddScoped<ChoferRepository>();
builder.Services.AddScoped<RutasRepository>();
builder.Services.AddScoped<CoordenadasRepository>();
builder.Services.AddScoped<PuntosRutaRepository>();

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

app.MapAuth();

app.Run();
