using System.Data;
using Microsoft.Data.SqlClient;
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

        // 1. Método para Registrar un nuevo Bus
        public void RegistrarBus(BusRegistroDTO nuevoBus)
        {
            string spName = "sp_registrar_buses";

            using (SqlConnection cnx = _connector.CreateConnection())
            {
                using (SqlCommand cmd = new SqlCommand(spName, cnx))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    
                    cmd.Parameters.AddWithValue("@numero_placa", nuevoBus.numero_placa);
                    cmd.Parameters.AddWithValue("@estado_bus", nuevoBus.estado_bus);
                    cmd.Parameters.AddWithValue("@id_ruta", nuevoBus.id_ruta);
                    cmd.Parameters.AddWithValue("@id_usuario", nuevoBus.id_usuario);
                    
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // 2. Método para Eliminar un Bus
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

        // 3. Método para actualizar un bus
        public void ActualizarBus(Bus busAActualizar)
        {
            string spName = "sp_actualizar_buses";

            using (SqlConnection cnx = _connector.CreateConnection())
            {
                using (SqlCommand cmd = new SqlCommand(spName, cnx))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    
                    cmd.Parameters.AddWithValue("@id_bus", busAActualizar.id_bus);
                    
                    cmd.Parameters.AddWithValue("@numero_placa", busAActualizar.numero_placa);
                    cmd.Parameters.AddWithValue("@estado_bus", busAActualizar.estado_bus);
                    cmd.Parameters.AddWithValue("@id_ruta", busAActualizar.id_ruta);
                    cmd.Parameters.AddWithValue("@id_usuario", busAActualizar.id_usuario);
                    
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // 4. Método para obtener la información de un bus por id
        public Bus? GetBusById(int idBus)
        {
            Bus? bus = null;

            string sql = "SELECT id_bus, numero_placa, estado_bus, id_ruta, id_usuario FROM buses WHERE id_bus = @id_bus";
            try
            {
               using (SqlConnection cnx = _connector.CreateConnection())
                {
                    using (SqlCommand cmd = new SqlCommand(sql, cnx))
                    {
                        cmd.Parameters.AddWithValue("@id_bus", idBus);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                bus = new Bus
                                {
                                    id_bus = reader.GetInt32(0),
                                    numero_placa = reader.GetString(1),
                                    estado_bus = reader.GetBoolean(2),
                                    id_ruta = reader.GetInt32(3),
                                    id_usuario = reader.GetInt32(4)
                                };
                            }
                        }
                    }
                } 
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
            return bus;
        }

        // 5. Método para obtener IDs de Rutas Distintas por ID de Usuario
        public List<int> GetBusesIDByUserID(int idUsuario)
        {
            var BusesID = new List<int>();
            
            string sql = "SELECT id_bus FROM buses WHERE id_usuario = @id_usuario";

            using (SqlConnection cnx = _connector.CreateConnection())
            {
                using (SqlCommand cmd = new SqlCommand(sql, cnx))
                {
                    cmd.Parameters.AddWithValue("@id_usuario", idUsuario);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            BusesID.Add(reader.GetInt32(0));
                        }
                    }
                }
            }
            return BusesID;
        }        
        // 6. Método para obtener una lista de ID de Buses por ID de Ruta y ID de Usuario
        public List<int> GetBusIdsByRutaAndUsuarioId(int idRuta, int idUsuario)
        {
            var busIds = new List<int>();
            
            string sql = "SELECT id_bus FROM buses WHERE id_ruta = @id_ruta AND id_usuario = @id_usuario";

            using (SqlConnection cnx = _connector.CreateConnection())
            {
                using (SqlCommand cmd = new SqlCommand(sql, cnx))
                {
                    cmd.Parameters.AddWithValue("@id_ruta", idRuta);
                    cmd.Parameters.AddWithValue("@id_usuario", idUsuario);
                    
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            busIds.Add(reader.GetInt32(0));
                        }
                    }
                }
            }
            return busIds;
        }

        public bool RutaPertenceUsuario(int idRuta, int idUsuario)
        {
            string sql = "SELECT 1 FROM user_rutas WHERE id_usuario=@id_usuario AND id_ruta=@id_ruta";
            
            using (SqlConnection cnx = _connector.CreateConnection())
            {
                using (SqlCommand cmd = new SqlCommand(sql, cnx))
                {
                    cmd.Parameters.AddWithValue("@id_ruta", idRuta);
                    cmd.Parameters.AddWithValue("@id_usuario", idUsuario);
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
            }             

        }
    }
}
