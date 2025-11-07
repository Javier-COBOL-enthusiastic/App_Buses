// Modelo para los datos del Bus
namespace BusTrackSV.Models;
public class Bus
{
    public int id_bus { get; set; }
    public string numero_placa { get; set; }
    public int capacidad { get; set; }
    public int id_ruta { get; set; }
    public int id_usuario { get; set; }
}
