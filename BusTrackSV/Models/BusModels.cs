namespace BusTrackSV.Models
{
    public class Bus
    {
        public int id_bus { get; set; }
        public string numero_placa { get; set; } = "";
        public bool estado_bus { get; set; }
        public int id_ruta { get; set; }
        public int id_usuario { get; set; }
    }

    public class BusRegistroDTO
    {
        public string numero_placa { get; set; } = "";
        public bool estado_bus { get; set; }
        public int id_ruta { get; set; }
        public int id_usuario { get; set; }
    }
    public class BusLocation
    {
        public int IdBus { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
        public double Acc { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

}
