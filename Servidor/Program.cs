var builder = WebApplication.CreateBuilder(args);


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



app.MapGet("/coords/{rutaId}", (int rutaId) =>
{
    string rutaArchivo = rutaId switch
    {
        1 => @"C:\Users\works\OneDrive\Escritorio\sensorlog_pos_20251002_174122.csv",
        2 => @"C:\Users\works\OneDrive\Escritorio\sensorlog_pos_20251008_065710.csv",
        3 => @"C:\Users\works\OneDrive\Escritorio\sensorlog_2.csv",
        _ => throw new ArgumentException("Ruta no encontrada")
    };
    
    var ruta = new Ruta();
    using (StreamReader sr = new StreamReader(rutaArchivo))
    {
        string linea;
        sr.ReadLine();
        while ((linea = sr.ReadLine()) != null)
        {
            var Array = linea.Split(',');
            ruta.Lat.Add(float.Parse(Array[1]));
            ruta.Lon.Add(float.Parse(Array[2]));
        }
    }
    return ruta;    
})
.WithName("GetCoords");

app.Run();

public class Ruta
{
    public List<float> Lat { get; set; } = new List<float>();
    public List<float> Lon { get; set; } = new List<float>();
}
