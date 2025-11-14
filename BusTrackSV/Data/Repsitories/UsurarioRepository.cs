using System.Data;
using System.Data.SqlClient;
using ConexionBD;
using BusTrackSV.Models;
using System.Collections.Generic;

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
                    
                    cmd.Parameters.AddWithValue("@nombre", nuevoUsuario.nombre);
                    cmd.Parameters.AddWithValue("@correo", nuevoUsuario.correo);
                    cmd.Parameters.AddWithValue("@usuario", nuevoUsuario.nombreUsuario);
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
        public Usuario Login(string nombreUsuario, string password)
        {
            Usuario usuario = null;
            
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
                            usuario = new Usuario
                            {
                                id_usuario = reader.GetInt32(0),
                                nombre_usuario = reader.GetString(1),
                                correo_electronico_usuario = reader.GetString(2),
                                usuario = reader.GetString(3)
                            };
                        }
                    }
                }
            }
            return usuario;
        }
    }
}
