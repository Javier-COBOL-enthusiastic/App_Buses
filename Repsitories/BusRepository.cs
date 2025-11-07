using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using ConexionBD;
using BusTrackSV.Models;

namespace Data.Repositories
{
    public class BusRepository
    {
        private readonly DbConnector _connector;

        public BusRepository(DbConnector connector)
        {
            _connector = connector;
        }

        // 1. Método para obtener Buses por ID de Usuario
        public List<Bus> GetBusesByUsuarioId(int idUsuario)
        {
            var buses = new List<Bus>();
            
            string sql = "SELECT id_bus, numero_placa, capacidad, id_ruta, id_usuario FROM buses WHERE id_usuario = @id_usuario";

            using (SqlConnection cnx = _connector.CreateConnection())
            {
                using (SqlCommand cmd = new SqlCommand(sql, cnx))
                {
                    cmd.Parameters.AddWithValue("@id_usuario", idUsuario);
                    
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            buses.Add(new Bus
                            {
                                id_bus = reader.GetInt32(0),
                                numero_placa = reader.GetString(1),
                                capacidad = reader.GetInt32(2),
                                id_ruta = reader.GetInt32(3),
                                id_usuario = reader.GetInt32(4)
                            });
                        }
                    }
                }
            }
            return buses;
        }

        // 2. Método para Registrar un nuevo Bus
        public void RegistrarBus(Bus nuevoBus)
        {
            string spName = "sp_registrar_buses";

            using (SqlConnection cnx = _connector.CreateConnection())
            {
                using (SqlCommand cmd = new SqlCommand(spName, cnx))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    
                    cmd.Parameters.AddWithValue("@numero_placa", nuevoBus.numero_placa);
                    cmd.Parameters.AddWithValue("@capacidad", nuevoBus.capacidad);
                    cmd.Parameters.AddWithValue("@id_ruta", nuevoBus.id_ruta);
                    cmd.Parameters.AddWithValue("@id_usuario", nuevoBus.id_usuario);
                    
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // 3. Método para Eliminar un Bus
        public void EliminarBus(int idBus)
        {
            string sql = "DELETE FROM buses WHERE id_bus = @id_bus"; 

            using (SqlConnection cnx = _connector.CreateConnection())
            {
                using (SqlCommand cmd = new SqlCommand(sql, cnx))
                {
                    cmd.Parameters.AddWithValue("@id_bus", idBus);
                    
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
