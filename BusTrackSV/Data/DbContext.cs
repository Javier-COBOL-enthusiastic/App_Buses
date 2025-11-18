using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace ConexionBD
{
    public class DbConnector
    {
        private readonly string _connectionString;

        public DbConnector(string connectionString)
        {
            _connectionString = connectionString 
                ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public SqlConnection CreateConnection()
        {
            try
            {
                var cnx = new SqlConnection(_connectionString);
                cnx.Open();
                return cnx;
            }
            catch (SqlException ex)
            {
                Console.WriteLine("Error al conectar: " + ex.Message);
                throw;
            }
        }
    }
}
