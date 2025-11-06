// Modelo simple para los datos del usuario después del login
public class Usuario
{
    public int id_usuario { get; set; }
    public string nombre_usuario { get; set; }
    public string correo_electronico_usuario { get; set; }
    public string usuario { get; set; }
    // No incluimos la contraseña/hash aquí por seguridad
}

// Modelo para los datos de registro (DTO)
public class UsuarioRegistroDTO
{
    public string nombre { get; set; }
    public string apellido { get; set; }
    public string dui { get; set; }
    public string correo { get; set; }
    public DateTime fecha { get; set; }
    public string nombreUsuario { get; set; }
    public string password { get; set; }
    public string telefono { get; set; } // Asumo que el teléfono se registra aquí
}
