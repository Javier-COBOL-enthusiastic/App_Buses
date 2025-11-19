using System.Data;
using Microsoft.Data.SqlClient;
using ConexionBD;
using BusTrackSV.Models;

namespace Data.Repositories
{
    public class CoordenadasRepository
    {
        private readonly DbConnector _connector;

        public CoordenadasRepository(DbConnector connector)
        {
            _connector = connector;
        }
        
        // 1. Método para Agregar las coordenadas de una ruta

        // Asume que la clase CoordenadasRepository tiene acceso a _connector.CreateConnection()

        public List<int> RegistrarCoordenadas(List<Coordenada> nuevasCoordenadas)
        {
            List<int> idsGenerados = new List<int>();
            
            // --- 1. Crear el DataTable con las Coordenadas ---
            DataTable dtCoordenadas = new DataTable();
            dtCoordenadas.Columns.Add("latitud", typeof(decimal));
            dtCoordenadas.Columns.Add("longitud", typeof(decimal));

            foreach (var c in nuevasCoordenadas)
            {
                dtCoordenadas.Rows.Add(c.latitud, c.longitud);
            }
            
            // --- 2. Preparar el comando SQL para la inserción y retorno de IDs ---
            // Creamos la tabla temporal y usamos BulkCopy para llenarla eficientemente.
            // Luego, en un solo comando, insertamos desde la temporal a la real y capturamos los IDs.
            string bulkInsertCommand = @"
                -- 1. Crear una tabla temporal para recibir los datos masivos
                CREATE TABLE #CoordenadasTemp (
                    latitud DECIMAL(10,7),
                    longitud DECIMAL(10,7)
                );

                -- NOTA: La tabla temporal #CoordenadasTemp se llena desde C# usando SqlBulkCopy

                -- 2. Insertar de la tabla temporal a la tabla real 'coordenadas'
                --    y usar la cláusula OUTPUT para retornar los IDs generados (id_coordenada)
                INSERT INTO coordenadas(latitud, longitud)
                OUTPUT inserted.id_coordenada -- ¡Clave para devolver los IDs!
                SELECT latitud, longitud FROM #CoordenadasTemp;

                -- 3. La tabla temporal se elimina automáticamente al finalizar la conexión/sesión
            ";

            using (SqlConnection cnx = _connector.CreateConnection())
            {
                // Iniciar una transacción para asegurar que la tabla temporal es segura
                using (SqlTransaction transaction = cnx.BeginTransaction())
                {
                    try
                    {
                        // A. Usar SqlBulkCopy para la inserción de datos a la tabla temporal
                        using (SqlBulkCopy bulkCopy = new SqlBulkCopy(cnx, SqlBulkCopyOptions.Default, transaction))
                        {
                            // Asignar los nombres de las columnas de origen y destino
                            bulkCopy.ColumnMappings.Add("latitud", "latitud");
                            bulkCopy.ColumnMappings.Add("longitud", "longitud");
                            
                            bulkCopy.DestinationTableName = "#CoordenadasTemp";
                            bulkCopy.WriteToServer(dtCoordenadas);
                        }

                        // B. Ejecutar el comando para mover datos a la tabla final y obtener IDs
                        using (SqlCommand cmd = new SqlCommand(bulkInsertCommand, cnx, transaction))
                        {
                            cmd.CommandType = CommandType.Text;
                            
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                // Leer todos los IDs devueltos por la cláusula OUTPUT
                                while (reader.Read())
                                {
                                    // El resultado del OUTPUT es una columna con el ID
                                    idsGenerados.Add(reader.GetInt32(0)); 
                                }
                            }
                        }

                        // C. Confirmar si todo salió bien
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        // Revertir en caso de error
                        transaction.Rollback();
                        // Puedes relanzar la excepción o manejarla
                        throw new Exception("Error al registrar coordenadas masivamente: " + ex.Message);
                    }
                }
            }

            return idsGenerados;
        }
        
        // Se recomienda, justo despues de ejecutar el método 1, guardar la lista que retorna, en una varibale

        // 2. Método para Eliminar las coordenadas de una ruta
        public void EliminarCoordenadas(List<int> idsCoordenadasAEliminar)
        {
            if (idsCoordenadasAEliminar == null || !idsCoordenadasAEliminar.Any())
            {
                return;
            }

            string idsString = string.Join(",", idsCoordenadasAEliminar);

            string sqlQuery = $@"
                DELETE FROM coordenadas
                WHERE id_coordenada IN ({idsString});
            ";

            using (SqlConnection cnx = _connector.CreateConnection())
            {
                using (SqlCommand cmd = new SqlCommand(sqlQuery, cnx))
                {
                    cmd.CommandType = CommandType.Text;

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
