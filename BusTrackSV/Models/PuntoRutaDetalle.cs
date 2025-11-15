namespace BusTrackSV.Models
{
    public class PuntoRutaDetalle
    {
        public int id_punto_ruta { get; set; }
        public decimal latitud { get; set; }
        public decimal longitud { get; set; }  
        public string nombre_ruta { get; set; }
    }
}
