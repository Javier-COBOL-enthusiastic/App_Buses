namespace BusTrackSV.Models
{
    public class Usuario
    {
        public int id_usuario { get; set; }
        public string nombre_completo { get; set; }
        public string correo { get; set; }
        public string usuario { get; set; }
        public string password {get; set; }
    }
    public class Usuario_validado
    {
        public int id_usuario { get; set; }
        public string nombre_completo { get; set; }
        public string correo { get; set; }
        public string usuario { get; set; }
    }
}
