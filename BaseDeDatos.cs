using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace ServicioAgro_Jutiapa.Conexiones
{
    class BaseDeDatos
    {
        private readonly string connectionString;
        /// Inicializa la cadena de conexión obteniéndola del archivo de configuración.
        public BaseDeDatos()
        {
            connectionString = ConfigurationManager.ConnectionStrings["CadenaConexion"].ConnectionString;
        }

        public void Guardar(string prompt, string resultado)
        {
            try
            {
                // Crea y abre la conexión a la base de datos.
                var conn = new SqlConnection(connectionString);
                conn.Open();
                // Prepara el comando SQL para insertar los datos en la tabla Investigaciones
                var cmd = new SqlCommand("INSERT INTO historial (Prompt, Resultado) VALUES (@p, @r)", conn);
                cmd.Parameters.AddWithValue("@p", prompt);
                cmd.Parameters.AddWithValue("@r", resultado);
                ///comando para guardar los datos.
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ///excepción personalizada si ocurre un error al guardar.
                throw new Exception("Error al guardar en la base de datos: " + ex.Message);
            }
        }
        public List<(string Prompt, string Resultado, DateTime Fecha)> ObtenerHistorial()
        {
            var historial = new List<(string, string, DateTime)>();
            string cadenaConexion = System.Configuration.ConfigurationManager.ConnectionStrings["CadenaConexion"].ConnectionString;
            using (SqlConnection conexion = new SqlConnection(cadenaConexion))
            {
                string query = "SELECT Prompt, Resultado, Fecha FROM historial ORDER BY Fecha DESC";
                using (SqlCommand cmd = new SqlCommand(query, conexion))
                {
                    conexion.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            historial.Add((
                                reader.GetString(0),
                                reader.GetString(1),
                                reader.GetDateTime(2)
                            ));
                        }
                    }
                }
            }
            return historial;
        }

    }
}
