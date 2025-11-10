using System.Data;
using Microsoft.Data.SqlClient;
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

        // 1. Método para Iniciar Sesión (Login) y obtener información
        public Usuario? Login(string nombreUsuario, string password)
        {
            Usuario? usuario = null;
            
            string spName = "sp_validar_usuario_login"; // no existe <-

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

        // 2. Método para Registrar un nuevo Usuario
        public void RegistrarUsuario(UsuarioRegistroDTO nuevoUsuario)
        {
            string spName = "sp_registrar_usuario";            


            using (SqlConnection cnx = _connector.CreateConnection())
            {
                using (SqlCommand cmd = new SqlCommand(spName, cnx))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    
                    // Asegúrate de que los nombres de los parámetros coincidan con tu SP
                    cmd.Parameters.AddWithValue("@nombre", nuevoUsuario.nombre);
                    cmd.Parameters.AddWithValue("@apellido", nuevoUsuario.apellido);
                    cmd.Parameters.AddWithValue("@dui", nuevoUsuario.dui);
                    cmd.Parameters.AddWithValue("@correo", nuevoUsuario.correo);
                    cmd.Parameters.AddWithValue("@fecha", nuevoUsuario.fecha);
                    cmd.Parameters.AddWithValue("@usuario", nuevoUsuario.nombreUsuario);
                    cmd.Parameters.AddWithValue("@password", nuevoUsuario.password);
                    cmd.Parameters.AddWithValue("@idrol", 1);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
