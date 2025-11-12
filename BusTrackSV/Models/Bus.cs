namespace BusTrackSV.Models
{
    public class Bus
    {
        public int Id { get; set; }
        public string Placa { get; set; } = "";
        public int Capacidad { get; set; }
    }

    public class Chofer
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = "";
        public Bus? Bus { get; set; }
    }
}
