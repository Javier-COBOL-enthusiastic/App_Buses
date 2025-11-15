using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using ConexionBD;
using BusTrackSV.Models;

namespace Data.Repositories
{
    public class ChoferRepository
    {
        private readonly DbConnector _connector;

        public BusRepository(DbConnector connector)
        {
            _connector = connector;
        }
        
        // 1. Método para gregar un nuevo chofer
        public void RegistrarChofer(ChoferRegistroDTO NuevoChofer)
        {
            string spName="sp_registrar_choferes";
            
            using (SqlConnection cnx = _conector.CreateConection())
            {
                using (SqlCommand cmd = new SqlCommand(spName, cnx))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@nombre_completo", NuevoChofer.nombre_completo);
                    cmd.Parameters.AddWithValue("@telefono_chofer", NuevoChofer.telefono_chofer);
                    cmd.Parameters.AddWithValue("@id_bus", NuevoChofer.id_bus)

                    cmd.ExecuteNonQuery();
                }                
            }
        }

        // 2. Método para eliminar un chofer
        public void EliminarChofer(int idChofer)
        {
            string sql = "DELETE FROM choferes WHERE id_chofer = @id_chofer"; 

            using (SqlConnection cnx = _connector.CreateConnection())
            {
                using (SqlCommand cmd = new SqlCommand(sql, cnx))
                {
                    cmd.Parameters.AddWithValue("@id_chofer", idChofer);
                    
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // 3. Método para actualizar un Chofer
        public void ActualizarChofer(Chofer ChoferAActualizar)
        {
            string spName = "sp_actualizar_chofer";

            using (SqlConnection cnx = _connector.CreateConnection())
            {
                using (SqlCommand cmd = new SqlCommand(spName, cnx))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    
                    cmd.Parameters.AddWithValue("@id_chofer", ChoferAActualizar.id_chofer);
                    
                    cmd.Parameters.AddWithValue("@nombre_completo_chofer", ChoferAActualizar.nombre_completo);
                    cmd.Parameters.AddWithValue("@telefono_chofer", ChoferAActualizar.telefono_chofer);
                    cmd.Parameters.AddWithValue("@id_bus", ChoferAActualizar.id_bus);
                    
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // 4. Método para obtener la información de un chofer por id_bus
        public Chofer GetChoferByIdBus(int idBus)
        {
            Chofer chofer = null;

            string sql = "SELECT id_chofer, nombre_completo_chofer, telefono_chofer, id_bus FROM choferes WHERE id_bus = @id_bus";

            using (SqlConnection cnx = _connector.CreateConnection())
            {
                using (SqlCommand cmd = new SqlCommand(sql, cnx))
                {
                    cmd.Parameters.AddWithValue("@id_bus", idBus);
                    
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            chofer = new Chofer
                            {
                                id_chofer = reader.GetInt32(0),
                                nombre_completo = reader.GetString(1),
                                telefono_chofer = reader.GetBoolean(2),
                                id_bus = reader.GetInt32(3)
                            };
                        }
                    }
                }
            }
            return chofer;
        }
    }
}
