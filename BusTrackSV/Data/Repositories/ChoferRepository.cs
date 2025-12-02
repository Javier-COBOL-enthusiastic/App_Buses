using System.Data;
using Microsoft.Data.SqlClient;
using ConexionBD;
using BusTrackSV.Models;

namespace Data.Repositories
{
    public class ChoferRepository
    {
        private readonly DbConnector _connector;

        public ChoferRepository(DbConnector connector)
        {
            _connector = connector;
        }
        
        // 1. Método para gregar un nuevo chofer
        public void RegistrarChofer(ChoferRegistroDTO NuevoChofer)
        {
            string spName="sp_registrar_choferes";
            
            using (SqlConnection cnx = _connector.CreateConnection())
            {
                using (SqlCommand cmd = new SqlCommand(spName, cnx))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@nombre_completo", NuevoChofer.nombre_completo);
                    cmd.Parameters.AddWithValue("@telefono_chofer", NuevoChofer.telefono_chofer);
                    cmd.Parameters.AddWithValue("@id_bus", NuevoChofer.id_bus);

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

        public Chofer? GetChoferByID(int idChofer)
        {
            Chofer? chofer = null;            
            string sql = "SELECT id_chofer, nombre_completo_chofer, telefono_chofer, id_bus FROM choferes WHERE id_chofer = @id_chofer";

            using (SqlConnection cnx = _connector.CreateConnection())
            {
                using (SqlCommand cmd = new SqlCommand(sql, cnx))
                {
                    cmd.Parameters.AddWithValue("@id_chofer", idChofer);
                    
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            chofer = new Chofer
                            {
                                id_chofer = reader.GetInt32(0),
                                nombre_completo = reader.GetString(1),
                                telefono_chofer = reader.GetString(2),
                                id_bus = reader.GetInt32(3)
                            };
                        }
                    }
                }
            }
            return chofer;
        }

        // 4. Método para obtener la información de un chofer por id_bus
        public Chofer? GetChoferByIdBus(int idBus)
        {
            Chofer? chofer = null;

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
                                telefono_chofer = reader.GetString(2),
                                id_bus = reader.GetInt32(3)
                            };
                        }
                    }
                }
            }
            return chofer;
        }

        public List<int> GetChoferIdByUserID(int userID)
        {
            List<int> clst = new List<int>();

            string sql = """
            SELECT c.id_chofer, b.id_bus, u.id_usuario FROM choferes c 
            INNER JOIN buses b on c.id_bus = b.id_bus 
            INNER JOIN usuarios u on b.id_usuario = u.id_usuario 
            WHERE u.id_usuario = @id_usuario;
            """;

            using (SqlConnection cnx = _connector.CreateConnection())
            {
                using (SqlCommand cmd =  new SqlCommand(sql, cnx))
                {
                    cmd.Parameters.AddWithValue("@id_usuario", userID);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var id = reader.GetInt32(0);
                            clst.Add(id);
                        }
                    }
                }   
            }
            
            return clst;
        }
        public  BusRutaInfo? GetBusByChoferUserID(int choferUserID)
        {
            BusRutaInfo? bus = null;
            string sql = """
            SELECT       
            b.id_bus,      
            b.numero_placa,
            b.estado_bus,
            r.nombre_ruta,
            r.descripcion_ruta  
            FROM buses b
            INNER JOIN rutas r ON b.id_ruta = r.id_ruta
            INNER JOIN choferes ch ON b.id_bus = ch.id_bus
            INNER JOIN usuarios u ON u.nombre_completo_usuario = ch.nombre_completo_chofer
            WHERE u.id_usuario = @id_usuario;
            """;
            using (SqlConnection cnx = _connector.CreateConnection())
            {
                using (SqlCommand cmd = new SqlCommand(sql, cnx))
                {
                    cmd.Parameters.AddWithValue("@id_usuario", choferUserID);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            bus = new BusRutaInfo
                            {
                                id_bus = reader.GetInt32(0),
                                numero_placa = reader.GetString(1),
                                estado_bus = reader.GetBoolean(2),
                                nombre_ruta = reader.GetString(3),
                                descripcion_ruta = reader.GetString(4)
                            };
                        }
                    }
                }
            }
            return bus;
        }
        public int GetChoferUsuario(int choferID)
        {            
            int user = 0;
            string sql = """
            SELECT            
            u.id_usuario
            FROM usuarios u
            JOIN choferes ch ON u.nombre_completo_usuario = ch.nombre_completo_chofer
            WHERE ch.id_chofer = @id_chofer
            """;

            using (SqlConnection cnx = _connector.CreateConnection())
            {
                using (SqlCommand cmd = new SqlCommand(sql, cnx))
                {
                    cmd.Parameters.AddWithValue("@id_chofer", choferID);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user = reader.GetInt32(0);                            
                        }
                    }
                }
            }        
            return user;
        }
    }
}
