namespace BusTrackSV.Models
{
    

public class Ruta
{
    public int id_ruta { get; set; }
    public string nombre_ruta { get; set; }
    public string descripcion_ruta { get; set; }
}

public class PuntoRuta
{
    public int id_punto_ruta { get; set; }
    public int id_ruta { get; set; }
    public int id_coordenada { get; set; }
    public int orden { get; set; }
}
}