using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ProjectoDragDrop.Objectes
{
    internal class DadesBBDD
    {
        private string connectionString;
        public List<Estats> estats;
        public List<Usuaris> usuaris;
        public List<Rols> rols;
        public List<Prioritats> prioritats;

        public DadesBBDD()
        {
            // Inicializar la cadena de conexión desde la configuración
            connectionString = ConfigurationManager.ConnectionStrings["ProjectoDragDrop.Properties.Settings.kanbanConnectionString"].ConnectionString;
        }

        // Funcio per obtenir tots els usuaris de la base de dades i guardaro a una llista
        public List<Usuaris> ObtenerUsuarios()
        {
            usuaris = new List<Usuaris>();

            string query = "SELECT * FROM Usuaris";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    try
                    {
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Usuaris usuario = new Usuaris();
                                usuario.Id = Convert.ToInt32(reader["Id"]);
                                usuario.Nom = reader["nom"].ToString();
                                usuario.Usuari = reader["usuari"].ToString();
                                usuario.Email = reader["email"].ToString();
                                usuario.Password = reader["password"].ToString();
                                usuario.Id_rol = reader["id_rol"].ToString();

                                usuaris.Add(usuario);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        
                        Console.WriteLine("Error al obtenir usuaris de la BBDD: " + ex.Message);
                    }
                }
            }

            return usuaris;
        }

        // Funcio per obtenir tots els rols de la base de dades i guardaro a una llista
        public List<Rols> ObtenirRols()
        {
            rols = new List<Rols>();

            string query = "SELECT * FROM Rol";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    try
                    {
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Rols rol = new Rols();
                                rol.Id = Convert.ToInt32(reader["Id"]);
                                rol.Rol = reader["rol"].ToString();


                                rols.Add(rol);
                            }
                        }
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine("Error al obtenir rols de la BBDD: " + ex.Message);
                    }
                }
            }

            return rols;
        }

        // Funcio per obtenir tots els estats de la base de dades i guardaro a una llista
        public List<Estats> ObtenirEstats()
        {
            estats = new List<Estats>();

            string query = "SELECT * FROM Estat";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    try
                    {
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Estats estat = new Estats();
                                estat.Id = Convert.ToInt32(reader["Id"]);
                                estat.Estat = reader["estat"].ToString();


                                estats.Add(estat);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error al obtenir estats de la BBDD: " + ex.Message);
                    }
                }
            }

            return estats;
        }

        // Funcio per obtenir tots els estats de la base de dades i guardaro a una llista
        public List<Prioritats> ObtenirPrioritats()
        {
            prioritats = new List<Prioritats>();

            string query = "SELECT * FROM Prioritat";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    try
                    {
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Prioritats prioritat = new Prioritats();
                                prioritat.Id = Convert.ToInt32(reader["Id"]);
                                prioritat.Prioritat = reader["prioritat"].ToString();


                                prioritats.Add(prioritat);
                            }
                        }
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine("Error al obtenir prioritats de la BBDD: " + ex.Message);
                    }

                }
            }

            return prioritats;
        }

        // Funcio que executa la comanda SQL per obtenir tota la imformació de cada tasca i posa la tasca a cada un dels listbox en dependencia al estat de la tasca
        public void MostrarTasquesPerEstat(string estado, ListBox listBox)
        {
            // Consulta SQL para obtener las tareas según el estado
            string consulta = "SELECT * FROM tasca t INNER JOIN Prioritat p ON t.id_prioritat = p.Id INNER JOIN Estat e ON t.id_estat = e.Id" + " WHERE e.estat = @Estado";


            // Utilizar un objeto SqlConnection y un objeto SqlCommand para ejecutar la consulta
            using (SqlConnection conexion = new SqlConnection(connectionString))
            {
                SqlCommand comando = new SqlCommand(consulta, conexion);
                comando.Parameters.AddWithValue("@Estado", estado); // Método para obtener el ID de estado

                try
                {
                    conexion.Open();
                    SqlDataReader lector = comando.ExecuteReader();

                    // Leer los datos y crear objetos Tasca
                    while (lector.Read())
                    {
                        Tasca tasca = new Tasca()
                        {
                            Id = Convert.ToInt32(lector["Id"]),
                            Titol = lector["titol"].ToString(),
                            Descripcio = lector["descricpio"].ToString(),
                            Datacreacio = Convert.ToDateTime(lector["datacreacio"]),
                            Datafinalitzacio = lector["datafinalitzacio"] != DBNull.Value ? Convert.ToDateTime(lector["datafinalitzacio"]) : DateTime.MinValue,
                            Id_responsable = ObtenirNomResponsable(Convert.ToInt32(lector["id_responsable"])),
                            Id_Prioritat = ObtenirNomPriorioritat(Convert.ToInt32(lector["id_prioritat"])),
                            Id_estat = ObtenirNomEstat(Convert.ToInt32(lector["id_estat"])),
                            PrioritatBackground = BackgroundPriority(Convert.ToInt32(lector["id_prioritat"]))

                        };
                        listBox.Items.Add(tasca);
                    }
                    lector.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al obtenir les tasques des de la base de dades: " + ex.Message);
                }
            }
        }


        /* AQUESTES FUNCIONS NOMES ES CRIDEN EN EXECUTAR EL PROGRAMA PER OBTENIR ELS VALORS DELS NOMS EN COMPTES DELS ID'S*/

        // Funcio per establir el backgorund del textblock on esta posada la prioritat segis el Id de la mateixa
        public Brush BackgroundPriority(int prioritat)
        {
            Brush color;
            switch (prioritat)
            {
                case 1:
                    color = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#e62e1b"));
                    break;
                case 2:
                    color = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#fafbfd"));
                    break;
                case 3:
                    color = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF00"));
                    break;
                default:
                    color = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00FF0000"));
                    break;
            }
            return color;
        }

        // Obtenir nom de la Prioritat
        public string ObtenirNomPriorioritat(int id_prioritat)
        {
            string consulta = "SELECT prioritat FROM Prioritat WHERE Id = @idprioritat";

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ProjectoDragDrop.Properties.Settings.kanbanConnectionString"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(consulta, conn))
                {
                    cmd.Parameters.AddWithValue("@idprioritat", id_prioritat);
                    conn.Open();
                    return (string)cmd.ExecuteScalar();
                }
            }
        }

        // Obtenir el nom del Responsable
        public string ObtenirNomResponsable(int id_usuari)
        {
            string consulta = "SELECT usuari FROM Usuaris WHERE Id = @idusuari";

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ProjectoDragDrop.Properties.Settings.kanbanConnectionString"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(consulta, conn))
                {
                    cmd.Parameters.AddWithValue("@idusuari", id_usuari);
                    conn.Open();
                    return (string)cmd.ExecuteScalar();
                }
            }
        }

        // Obtenir el nom del estat de la tasca
        public string ObtenirNomEstat(int id_estat)
        {
            string consulta = "SELECT estat FROM Estat WHERE Id = @idestat";

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ProjectoDragDrop.Properties.Settings.kanbanConnectionString"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(consulta, conn))
                {
                    cmd.Parameters.AddWithValue("@idestat", id_estat);
                    conn.Open();
                    return (string)cmd.ExecuteScalar();
                }
            }
        }
    }
}
