var builder = WebApplication.CreateBuilder(args);


builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirTodo", policy =>
    {
        policy
            .AllowAnyOrigin()
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



app.MapGet("/dir1", () =>
{

    string rutaArchivo = @"C:\Users\works\OneDrive\Escritorio\sensorlog_2.csv";    
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
    /*var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;*/
})
.WithName("GetCoords");

app.Run();

public class Ruta
{
    public List<float> Lat { get; set; } = new List<float>();
    public List<float> Lon { get; set; } = new List<float>();
}
