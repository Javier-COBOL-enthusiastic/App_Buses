using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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

        // 2. Método para Obtener la información de todas las Rutas
        public List<Ruta> GetAllRutas()
        {
            var rutas = new List<Ruta>();
            string sql = "SELECT id_ruta, nombre_ruta, descripcion_ruta FROM rutas";

            using (SqlConnection cnx = _connector.CreateConnection())
            {
                using (SqlCommand cmd = new SqlCommand(sql, cnx))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            rutas.Add(new Ruta
                            {
                                id_ruta = reader.GetInt32(0),
                                nombre_ruta = reader.GetString(1),
                                descripcion_ruta = reader.GetString(2)
                            });
                        }
                    }
                }
            }
            return rutas;
        }
    }
}
