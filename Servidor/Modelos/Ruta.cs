namespace BusTrackSV.Models
{
    public record Coordenada(float lat, float lon);

    public class Ruta
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = "";        
        public string Descripcion { get; set; } = "";
        public int Id_ruta { get; set; }
        public List<Coordenada> Coordenadas { get; set; } = new();        
    }
}
