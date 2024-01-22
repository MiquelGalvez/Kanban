using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ProjectoDragDrop.MesInfoTasca
{
    /// <summary>
    /// Lógica de interacción para VeureTasca.xaml
    /// </summary>
    public partial class VeureTasca : Window
    {
        SqlConnection LaMevaConnexioSQL;
        public string laMevaConexioString = ConfigurationManager.ConnectionStrings["ProjectoDragDrop.Properties.Settings.kanbanConnectionString"].ConnectionString;
        public VeureTasca(DataRowView task)
        {
            InitializeComponent();
            LaMevaConnexioSQL = new SqlConnection(laMevaConexioString);

            // Emplenem els TextBlock amb la informacion de cada tasca
            EmplenarInfo(task);
        }


        // Aquesta funció ens emplena tota l'informació de la tasca als TextBlock del xaml
        private void EmplenarInfo(DataRowView task)
        {
            titol.Content = task["titol"].ToString();
            int idPrioritat = Convert.ToInt32(task["id_prioritat"]);
            prioritat.Content = GetPrioritatNameById(idPrioritat); // Funcio que ens dona el nom de la prioritat en base al seu Id
            datafinalitzacio.Content = task["datafinalitzacio"].ToString();
            datacreacio.Content = task["datacreacio"].ToString();
            int idResponsable = Convert.ToInt32(task["id_responsable"]);
            responsable.Content = GetResponsableNameById(idResponsable); // Funcio que ens dona el nom del responsable en base al seu Id
            descripcio.Text = task["descricpio"].ToString();
            int idEstat = Convert.ToInt32(task["id_estat"]);
            estat.Content = GetEstatNameById(idEstat); // Funcio que ens dona el nom del estat en base al seu Id
        }


        // Aquesta funció retorna el string del nom de la prioritat quan li passem el Id
        private string GetPrioritatNameById(int idPrioritat)
        {
            using (SqlConnection connection = new SqlConnection(laMevaConexioString))
            {
                connection.Open();

                string query = "SELECT prioritat FROM Prioritat WHERE Id = @IdPrioritat";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdPrioritat", idPrioritat);

                    return command.ExecuteScalar()?.ToString();
                }
            }
        }

        // Aquesta funció retorna el string del nom del responsable quan li passem el Id
        private string GetResponsableNameById(int idResponsable)
        {
            using (SqlConnection connection = new SqlConnection(laMevaConexioString))
            {
                connection.Open();

                string query = "SELECT nom FROM Usuaris WHERE Id = @IdResponsable";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdResponsable", idResponsable);

                    return command.ExecuteScalar()?.ToString();
                }
            }
        }

        // Aquesta funció retorna el string del nom del estat quan li passem el Id
        private string GetEstatNameById(int idEstat)
        {
            using (SqlConnection connection = new SqlConnection(laMevaConexioString))
            {
                connection.Open();

                string query = "SELECT estat FROM Estat WHERE Id = @IdEstat";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdEstat", idEstat);

                    return command.ExecuteScalar()?.ToString();
                }
            }
        }
    }
}
