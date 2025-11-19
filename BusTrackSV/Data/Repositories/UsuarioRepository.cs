using System.Data;
using Microsoft.Data.SqlClient;
using ConexionBD;
using BusTrackSV.Models;

namespace Data.Repositories
{
    public class UsuarioRepository
    {
        private readonly DbConnector _connector;
        
        public UsuarioRepository(DbConnector connector)
        {
            _connector = connector;
        }

        // 1. Método para Registrar un nuevo Usuario
        public void RegistrarUsuario(UsuarioRegistroDTO nuevoUsuario)
        {
            string spName = "sp_registrar_usuario";            
            using (SqlConnection cnx = _connector.CreateConnection())
            {
                using (SqlCommand cmd = new SqlCommand(spName, cnx))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    
                    cmd.Parameters.AddWithValue("@nombre_completo", nuevoUsuario.nombre_completo);
                    cmd.Parameters.AddWithValue("@correo", nuevoUsuario.correo);
                    cmd.Parameters.AddWithValue("@usuario", nuevoUsuario.usuario);
                    cmd.Parameters.AddWithValue("@password", nuevoUsuario.password);
                    
                    cmd.ExecuteNonQuery();
                }
            }                            
        }

        // 2. Método para Eliminar Usuario
        public void EliminarUsuario(int idUsuario)
        {
            string spName = "sp_eliminar_usuario";
            
            using (SqlConnection cnx = _connector.CreateConnection())
            {
                using (SqlCommand cmd = new SqlCommand(spName, cnx))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_usuario", idUsuario);
                    cmd.ExecuteNonQuery();
                }
            }                            
        }

        // 3. Método para Actualizar información de usuario
        public void ActualizarUsuario(Usuario usuarioAActualizar)
        {
            string spName = "sp_actualizar_usuario";

            using (SqlConnection cnx = _connector.CreateConnection())
            {
                using (SqlCommand cmd = new SqlCommand(spName, cnx))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    
                    cmd.Parameters.AddWithValue("@id_usuario", usuarioAActualizar.id_usuario);
                    cmd.Parameters.AddWithValue("@nombre_completo", usuarioAActualizar.nombre_completo);
                    cmd.Parameters.AddWithValue("@correo", usuarioAActualizar.correo);
                    cmd.Parameters.AddWithValue("@usuario", usuarioAActualizar.usuario);

                    cmd.Parameters.AddWithValue("@password", usuarioAActualizar.password);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        // 4. Método para Iniciar Sesión (Login) y obtener información
        public UsuarioValidado? Login(string nombreUsuario, string password)
        {
            UsuarioValidado? usuario = null;       

            string spName = "sp_validar_usuario_login";            
            using (SqlConnection cnx = _connector.CreateConnection())
            {
                using (SqlCommand cmd = new SqlCommand(spName, cnx))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@usuario", nombreUsuario);
                    cmd.Parameters.AddWithValue("@password", password);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            usuario = new UsuarioValidado
                            {
                                id_usuario = reader.GetInt32(0),
                                nombre_completo = reader.GetString(1),
                                correo = reader.GetString(2),
                                usuario = reader.GetString(3)
                            };
                        }
                        else if (!reader.Read())
                        {
                            throw new Exception("Usuario no registrado");
                        }                           
                    }
                }
            }
            return usuario;           
        }
    }
}
