namespace BusTrackSV.Models
{
    public class Bus
    {
        public int id_bus { get; set; }
        public string numero_placa { get; set; }
        public bool estado_bus { get; set; }
        public int id_ruta { get; set; }
        public int id_usuario { get; set; }
    }
}
