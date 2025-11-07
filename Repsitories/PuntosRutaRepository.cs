using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using ConexionBD;
using BusTrackSV.Models;

namespace Data.Repositories
{
    public class PuntosRutaRepository
    {
        private readonly DbConnector _connector;

        public PuntosRutaRepository(DbConnector connector)
        {
            _connector = connector;
        }

        // Método para obtener Puntos de Ruta por ID de Ruta
        public List<PuntoRuta> GetPuntosByRutaId(int idRuta)
        {
            var puntos = new List<PuntoRuta>();
            string sql = "SELECT id_punto_ruta, id_ruta, id_coordenada, orden FROM puntos_ruta WHERE id_ruta = @id_ruta ORDER BY orden ASC";

            using (SqlConnection cnx = _connector.CreateConnection())
            {
                using (SqlCommand cmd = new SqlCommand(sql, cnx))
                {
                    cmd.Parameters.AddWithValue("@id_ruta", idRuta);
                    
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            puntos.Add(new PuntoRuta
                            {
                                id_punto_ruta = reader.GetInt32(0),
                                id_ruta = reader.GetInt32(1),
                                id_coordenada = reader.GetInt32(2),
                                orden = reader.GetInt32(3)
                            });
                        }
                    }
                }
            }
            return puntos;
        }
    }
}
