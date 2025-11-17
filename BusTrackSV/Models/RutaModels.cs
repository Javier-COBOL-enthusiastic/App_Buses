namespace BusTrackSV.Models
{
    public record Coordenada(float latitud, float longitud);

     public class Ruta
    {
        public int id_ruta { get; set; }
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
}
