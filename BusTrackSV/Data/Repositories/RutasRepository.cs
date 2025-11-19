using System.Data;
using Microsoft.Data.SqlClient;
using ConexionBD;
using BusTrackSV.Models;

namespace Data.Repositories
{
    public class RutasRepository
    {
        private readonly DbConnector _connector;

        public RutasRepository(DbConnector connector)
        {
            _connector = connector;
        }

        // 1. Método para Registrar una nueva Ruta
        public void RegistrarRuta(Ruta nuevaRuta)
        {
            string spName = "sp_registrar_rutas";

            using (SqlConnection cnx = _connector.CreateConnection())
            {
                using (SqlCommand cmd = new SqlCommand(spName, cnx))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    
                    cmd.Parameters.AddWithValue("@nombre_ruta", nuevaRuta.nombre_ruta);
                    cmd.Parameters.AddWithValue("@descripcion_ruta", nuevaRuta.descripcion_ruta);
                    
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // 2. Método para Eliminar una Ruta
        public void EliminarRuta(int idRuta)
        {
            string spName = "sp_eliminar_rutas";

            using (SqlConnection cnx = _connector.CreateConnection())
            {
                using (SqlCommand cmd = new SqlCommand(spName, cnx))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    
                    cmd.Parameters.AddWithValue("@id_ruta", idRuta);
                    
                    cmd.ExecuteNonQuery();
                }
            }
        }
        
        // 3. Método para Actualizar la información de una Ruta
        public void ActualizarRuta(Ruta rutaAActualizar)
        {
            string spName = "sp_actualizar_ruta";

            using (SqlConnection cnx = _connector.CreateConnection())
            {
                using (SqlCommand cmd = new SqlCommand(spName, cnx))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    
                    cmd.Parameters.AddWithValue("@id_ruta", rutaAActualizar.id_ruta);
                    cmd.Parameters.AddWithValue("@nombre_ruta", rutaAActualizar.nombre_ruta);
                    cmd.Parameters.AddWithValue("@descripcion_ruta", rutaAActualizar.descripcion_ruta);
                    
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // 4. Método para obtener la información de una ruta por su id
        public Ruta? GetRutaById(int idRuta)
        {
            Ruta? ruta=null;
            
            string sql="SELECT id_ruta, nombre_ruta, descripcion_ruta FROM rutas WHERE id_ruta=@id_ruta";

            using (SqlConnection cnx = _connector.CreateConnection())
            {
                using (SqlCommand cmd = new SqlCommand(sql, cnx))
                {
                    // Añadir el parámetro para el ID de la Ruta
                    cmd.Parameters.AddWithValue("@id_ruta", idRuta);
                    
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            ruta = new Ruta
                            {
                                id_ruta = reader.GetInt32(0),
                                nombre_ruta = reader.GetString(1),
                                descripcion_ruta = reader.GetString(2)
                            };
                        }
                    }
                }
            }
            return ruta;
        }
    }
}
