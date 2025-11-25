<<<<<<< HEAD
using System.Collections.Generic;
=======
>>>>>>> Javier
using System.Data;
using Microsoft.Data.SqlClient;
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

<<<<<<< HEAD
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
                    
=======
        // 1. Método para Agregar los puntos de la Ruta
        public void RegistrarPuntosRuta(int idRuta, List<int> idsCoordenadas)
        {
            // --- 1. Crear el DataTable con las Relaciones (id_ruta, id_coordenada, orden) ---
            DataTable dtPuntosRuta = new DataTable();
            
            // El orden de estas columnas es importante
            dtPuntosRuta.Columns.Add("id_ruta", typeof(int));
            dtPuntosRuta.Columns.Add("id_coordenada", typeof(int));
            dtPuntosRuta.Columns.Add("orden", typeof(int)); 

            // Variable para controlar el orden secuencial (inicia en 1)
            int orden = 1;

            // Llenar el DataTable usando los IDs de coordenadas y asignando el idRuta y el orden
            foreach (int idCoordenada in idsCoordenadas)
            {
                dtPuntosRuta.Rows.Add(
                    idRuta,       // Valor constante para id_ruta
                    idCoordenada, // ID de la coordenada de la lista
                    orden         // Valor secuencial (1, 2, 3, ...)
                );
                orden++;
            }
            
            // --- 2. Realizar la Inserción Masiva con SqlBulkCopy ---
            
            using (SqlConnection cnx = _connector.CreateConnection())
            {
                using (SqlTransaction transaction = cnx.BeginTransaction())
                {
                    try
                    {
                        using (SqlBulkCopy bulkCopy = new SqlBulkCopy(cnx, SqlBulkCopyOptions.Default, transaction))
                        {
                            // La tabla de destino es 'puntos_ruta'
                            bulkCopy.DestinationTableName = "puntos_ruta";
                            
                            // Mapear las columnas del DataTable (origen) a la tabla de SQL (destino)
                            // Nota: No incluimos id_punto_ruta ya que es auto-incremental.
                            bulkCopy.ColumnMappings.Add("id_ruta", "id_ruta");
                            bulkCopy.ColumnMappings.Add("id_coordenada", "id_coordenada");
                            bulkCopy.ColumnMappings.Add("orden", "orden");
                            
                            bulkCopy.WriteToServer(dtPuntosRuta);
                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        // Revertir si hay algún error
                        transaction.Rollback();
                        throw new Exception("Error al registrar puntos de ruta masivamente: " + ex.Message);
                    }
                }
            }
        }

        // 2. Método para enlistar los puntos de la Ruta que se eliminan desde CoordenadasRepository
        public List<int> ObtenerIdsCoordenadasPorRuta(int idRuta)
        {
            List<int> idsCoordenadas = new List<int>();

            string sqlQuery = @"
                SELECT 
                    id_coordenada 
                FROM puntos_ruta 
                WHERE id_ruta = @id_ruta
                ORDER BY orden ASC;";

            using (SqlConnection cnx = _connector.CreateConnection())
            {
                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnx))
                {
                    cmd.CommandType = CommandType.Text;
                    
                    cmd.Parameters.AddWithValue("@id_ruta", idRuta);

>>>>>>> Javier
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
<<<<<<< HEAD
                            puntos.Add(new PuntoRuta
                            {
                                id_punto_ruta = reader.GetInt32(0),
                                id_ruta = reader.GetInt32(1),
                                id_coordenada = reader.GetInt32(2),
                                orden = reader.GetInt32(3)
                            });
=======
                            idsCoordenadas.Add(reader.GetInt32(0)); 
>>>>>>> Javier
                        }
                    }
                }
            }
<<<<<<< HEAD
            return puntos;
=======

            return idsCoordenadas;
        }

        // 3. Método para Actualizar los puntos de la Ruta
        
        // 4. Método para obtener las coordenadas ordenadas de una ruta por su id
        public List<PuntoRutaDetalle> ObtenerCoordenadasDeRuta(int idRuta)
        {
            string sqlQuery = @"
                SELECT 
                    pr.id_punto_ruta, 
                    c.longitud, 
                    c.latitud, 
                    r.nombre_ruta 
                FROM puntos_ruta pr
                INNER JOIN rutas r ON pr.id_ruta = r.id_ruta
                INNER JOIN coordenadas c ON pr.id_coordenada = c.id_coordenada
                WHERE pr.id_ruta = @id_ruta
                ORDER BY pr.orden ASC";

            List<PuntoRutaDetalle> puntosRuta = new List<PuntoRutaDetalle>();

            using (SqlConnection cnx = _connector.CreateConnection())
            {
                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnx))
                {
                    cmd.CommandType = CommandType.Text;
                    
                    cmd.Parameters.AddWithValue("@id_ruta", idRuta);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            PuntoRutaDetalle punto = new PuntoRutaDetalle
                            {
                                id_punto_ruta = reader.GetInt32(reader.GetOrdinal("id_punto_ruta")),
                                latitud = reader.GetDecimal(reader.GetOrdinal("latitud")),
                                longitud = reader.GetDecimal(reader.GetOrdinal("longitud")),
                                nombre_ruta = reader["nombre_ruta"].ToString()
                            };
                            puntosRuta.Add(punto);
                        }
                    }
                }
            }
            
            return puntosRuta;
>>>>>>> Javier
        }
    }
}
