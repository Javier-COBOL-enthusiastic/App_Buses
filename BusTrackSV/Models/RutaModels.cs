namespace BusTrackSV.Models
{
    public record Coordenada(decimal latitud, decimal longitud);

     public class Ruta
    {
        public int id_ruta { get; set; }
        public string nombre_ruta { get; set; } = "";
        public string descripcion_ruta { get; set; } = "";
    }

    public class RutaDTO
    {
        public string nombre_ruta { get; set; } = "";
        public string descripcion_ruta { get; set; } = "";
    }

    public class PuntoRutaDetalle
    {
        public int id_punto_ruta { get; set; }
        public decimal latitud { get; set; }
        public decimal longitud { get; set; }  
        public string nombre_ruta { get; set; } = "";
    }
    public class RegistrarRutaDTO
    {
        public RutaDTO nuevaRuta { get; set; } = new RutaDTO();
        public List<Coordenada> coordenadas { get; set; } = new List<Coordenada>();
    }
}
