namespace BusTrackSV.Models
{
    public class Chofer
    {
        public int id_chofer { get; set; }
        public string nombre_completo { get; set; } = "";
        public string telefono_chofer { get; set; } = "";
        public int id_bus { get; set; }
    }
        public class ChoferRegistroDTO
    {
        public string nombre_completo { get; set; } = "";
        public string telefono_chofer { get; set; } = "";
        public int id_bus { get; set; }
    }
}