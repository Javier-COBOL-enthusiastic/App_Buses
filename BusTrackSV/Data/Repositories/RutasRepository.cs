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

        // 1. Método para Registrar una nueva Ruta (ahora devuelve el id generado)
        public int RegistrarRuta(RutaDTO nuevaRuta)
        {
            string spName = "sp_registrar_rutas";

            using (SqlConnection cnx = _connector.CreateConnection())
            {
                using (SqlCommand cmd = new SqlCommand(spName, cnx))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@nombre_ruta", nuevaRuta.nombre_ruta);
                    cmd.Parameters.AddWithValue("@descripcion_ruta", nuevaRuta.descripcion_ruta);

                    // se espera que el stored procedure haga: SELECT CAST(SCOPE_IDENTITY() AS INT) AS id_ruta;
                    var result = cmd.ExecuteScalar();
                    return result == null ? 0 : Convert.ToInt32(result);
                }
            }
        }

        // 2. Método para Eliminar una Ruta
        public void EliminarRuta(int idRuta)
        {
            string spName = "sp_eliminar_coord_rutas";
            string sp2Name = "sp_eliminar_coordenadas";
            string sp3Name = "sp_eliminar_rutas";

            using (SqlConnection cnx = _connector.CreateConnection())
            {
                using (SqlTransaction tran = cnx.BeginTransaction())
                {
                    try
                    {
                        var deletedCoords = new List<int>();
                        
                        using (SqlCommand cmd = new SqlCommand(spName, cnx, tran))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@id_ruta", idRuta);

                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {                                    
                                    deletedCoords.Add(reader.GetInt32(0));
                                }
                            }
                        }
                        
                        if (deletedCoords.Count > 0)
                        {
                            using (SqlCommand cmdDelCoord = new SqlCommand(sp2Name, cnx, tran))
                            {
                                cmdDelCoord.CommandType = CommandType.StoredProcedure;
                                var p = cmdDelCoord.Parameters.Add("@id_coordenada", SqlDbType.Int);

                                foreach (var idCoord in deletedCoords)
                                {
                                    p.Value = idCoord;
                                    cmdDelCoord.ExecuteNonQuery();
                                }
                            }
                        }

                        using (SqlCommand cmdDelRuta = new SqlCommand(sp3Name, cnx, tran))
                        {
                            cmdDelRuta.CommandType = CommandType.StoredProcedure;
                            cmdDelRuta.Parameters.AddWithValue("@id_ruta", idRuta);
                            cmdDelRuta.ExecuteNonQuery();
                        }

                        tran.Commit();
                    }
                    catch(Exception ex)
                    {
                        tran.Rollback();
                        throw new Exception("Error al eliminar la ruta y sus coordenadas asociadas: " + ex.Message);
                    }
                }
            }
        }

        // 3. Método para Actualizar la información de una Ruta
        // Descontinuado por problemas
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
        public List<int> GetRutasIDByUsuarioID(int idUsuario)
        {
            List<int> rutasID = new List<int>();

            string sql = "SELECT id_ruta FROM user_rutas where id_usuario = @id_usuario";

            using (SqlConnection cnx = _connector.CreateConnection())
            {
                using (SqlCommand cmd = new SqlCommand(sql, cnx))
                {
                    cmd.Parameters.AddWithValue("@id_usuario", idUsuario);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            rutasID.Add(reader.GetInt32(0));
                        }
                    }
                }
            }

            return rutasID;
        }
        
        public List<int> GetUsuariosIDporRuta(int idRuta)
        {
            List<int> usuariosID = new List<int>();

            string sql = "SELECT id_usuario FROM user_rutas where id_ruta = @id_ruta";

            using (SqlConnection cnx = _connector.CreateConnection())
            {
                using (SqlCommand cmd = new SqlCommand(sql, cnx))
                {
                    cmd.Parameters.AddWithValue("@id_ruta", idRuta);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            usuariosID.Add(reader.GetInt32(0));
                        }
                    }
                }
            }

            return usuariosID;
        }        
        
        public void VincularRutaUsuario(int idRuta, int idUsuario)
        {
            string sql = "sp_registrar_usuarios_rutas";
            using (SqlConnection cnx = _connector.CreateConnection())
            {
                using (SqlCommand cmd = new SqlCommand(sql, cnx))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_ruta", idRuta);
                    cmd.Parameters.AddWithValue("@id_usuario", idUsuario);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void DesvincularRutaUsuario(int idRuta, int idUsuario)
        {
            string sql = "DELETE FROM user_rutas WHERE id_ruta = @id_ruta AND id_usuario = @id_usuario";
            using (SqlConnection cnx = _connector.CreateConnection())
            {
                using (SqlCommand cmd = new SqlCommand(sql, cnx))
                {
                    cmd.Parameters.AddWithValue("@id_ruta", idRuta);
                    cmd.Parameters.AddWithValue("@id_usuario", idUsuario);

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
