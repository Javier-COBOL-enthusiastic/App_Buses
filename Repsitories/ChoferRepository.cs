using System.Collections.Generic;
using System.Data.SqlClient;
using ConexionBD; // Para acceder a DbConnector
using System.Data;
// using [TuProyecto].Models; // Asume que tienes un modelo Bus

namespace Data.Repositories
{
    public class BusRepository // Implementa IBusRepository si la definiste
    {
        private readonly DbConnector _connector;

        // 1. Inyección de Dependencias: Recibir el DbConnector
        public BusRepository(DbConnector connector)
        {
            _connector = connector;
        }

        public List<Bus> BusDeUsuario()
        {
            var buses = new List<Bus>();
            string sql = "SELECT id_bus, numero_placa, capacidad, id_rutas, id_usuario FROM Buses"; // Reemplaza con tu tabla real

            // 2. Usar 'using' para asegurar que la conexión se cierra y libera
            using (SqlConnection cnx = _connector.CreateConnection())
            {
                using (SqlCommand cmd = new SqlCommand(sql, cnx))
                {
                    // 3. Ejecutar el comando y leer los resultados
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            buses.Add(new Bus
                            {
                                Id = reader.GetInt32(0),
                                Placa = reader.GetString(1),
                                Modelo = reader.GetString(2)
                                // Mapear el resto de las propiedades
                            });
                        }
                    }
                }
            } // La conexión se cierra aquí automáticamente
            return buses;
        }
    }
}
