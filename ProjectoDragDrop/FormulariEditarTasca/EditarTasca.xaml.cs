using ProjectoDragDrop;
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

namespace ProjectoDragDrop.FormulariEditarTasca
{
    public partial class EditarTasca : Window
    {
        SqlConnection LaMevaConnexioSQL;
        private string TascaId;
        private string ResponsableNom;
        private string PrioritatNom;
        public string laMevaConexioString = ConfigurationManager.ConnectionStrings["ProjectoDragDrop.Properties.Settings.kanbanConnectionString"].ConnectionString;
        public EditarTasca(DataRowView task)
        {
            InitializeComponent();
            TascaId = task["Id"].ToString(); // Serveix per declarar quina es la Id de la tasca seleci
            LaMevaConnexioSQL = new SqlConnection(laMevaConexioString);

            int idResponsable = Convert.ToInt32(task["id_responsable"]);
            ResponsableNom = GetResponsableNameById(idResponsable);
            int idPrioritat = Convert.ToInt32(task["id_prioritat"]);
            PrioritatNom = GetPrioritatNameById(idPrioritat);

            // Crida les funcions per emplenar els combobox
            MostrarRespnsables();
            MostrarPrioritats();

            // Comproba si la informació de la tasac s'ha pasat correctament si es aixi fa tot l'ho altre
            if (task != null)
            {
                EmplenarInfo(task);
            }
        }

        // Funció que sereveix per poder emplenar als TextBox i Date Picker la infromació de la tasca
        private void EmplenarInfo(DataRowView task)
        {
            Titol.Text = task["titol"].ToString();
            dp1.SelectedDate = Convert.ToDateTime(task["datafinalitzacio"]);
            DescripcioTasca.Text = task["descricpio"].ToString();

            // Establir el valor per defecte dels combobox en dependencia a la informació que ja tenim a la tasca
            responsables.SelectedItem = ResponsableNom;
            prioritats.SelectedItem = PrioritatNom;
        }

        // Funcio per poder afegir els noms dels responsables des de la base de dades al combobox de responsables
        private void MostrarRespnsables()
        {
            string consulta = "SELECT nom FROM Usuaris";

            SqlDataAdapter elMeuAdaptador = new SqlDataAdapter(consulta, LaMevaConnexioSQL);
            using (elMeuAdaptador)
            {

                DataTable dt = new DataTable();
                elMeuAdaptador.Fill(dt);

                //POR CADA USER AÑADIR AL COMBOBOX
                foreach (DataRow row in dt.Rows)
                {
                    responsables.Items.Add(row["nom"].ToString());
                }
            }
        }

        // Funcio per poder afegir els noms de les prioritats des de la base de dades al combobox de prioritats
        private void MostrarPrioritats()
        {
            string consulta = "SELECT prioritat FROM Prioritat";

            SqlDataAdapter elMeuAdaptador = new SqlDataAdapter(consulta, LaMevaConnexioSQL);
            using (elMeuAdaptador)
            {

                DataTable dt = new DataTable();
                elMeuAdaptador.Fill(dt);

                //POR CADA USER AÑADIR AL COMBOBOX
                foreach (DataRow row in dt.Rows)
                {
                    prioritats.Items.Add(row["prioritat"].ToString());
                }
            }
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            // Quan el TextBox rep el focus, el seu contingut es canvia a una cadena buida
            if (sender is TextBox textBox)
            {
                textBox.Text = string.Empty;
            }
        }

        // Funció utilitzada per obtenir l'id del responsable
        private int ObtenirIdResponsable(string responsableNombre)
        {
            string consulta = "SELECT Id FROM Usuaris WHERE nom = @Nom";

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ProjectoDragDrop.Properties.Settings.kanbanConnectionString"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(consulta, conn))
                {
                    cmd.Parameters.AddWithValue("@Nom", responsableNombre);
                    conn.Open();
                    return (int)cmd.ExecuteScalar();
                }
            }
        }

        // Funció utilitzada per obtenir l'id de la prioritat
        private int ObtenirIdPrioritat(string prioritatNombre)
        {
            string consulta = "SELECT Id FROM Prioritat WHERE prioritat = @Prioritat";

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ProjectoDragDrop.Properties.Settings.kanbanConnectionString"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(consulta, conn))
                {
                    cmd.Parameters.AddWithValue("@Prioritat", prioritatNombre);
                    conn.Open();
                    return (int)cmd.ExecuteScalar();
                }
            }
        }

        // Funcio qeu executa el boto i qeu serevix per guarda la informació a les variables per despres poder executar la comanda sql
        private void GuardarButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Obtén los valores de los controles
                string titol = Titol.Text;
                DateTime datafinalitzacio = dp1.SelectedDate ?? DateTime.Now;
                string descripcio = DescripcioTasca.Text;

                string prioritatNombre = prioritats.SelectedItem?.ToString();
                int idPrioritat = ObtenirIdPrioritat(prioritatNombre);

                string responsableNombre = responsables.SelectedItem?.ToString();
                int idResponsable = ObtenirIdResponsable(responsableNombre);

                ActualitzarTasca(int.Parse(TascaId), titol, datafinalitzacio, descripcio, idPrioritat, idResponsable);

                MessageBox.Show("Cambis guardats correctament");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar els canvis: {ex.Message}");
            }
        }

        // Funció qeu cuan es crida el que fa es executar la comanda per modificar l'infromació de la Tasca
        private void ActualitzarTasca(int taskId, string titol, DateTime datafinalitzacio, string descripcio, int idPrioritat, int idResponsable)
        {
            string updateQuery = "UPDATE tasca SET titol = @Titol, datafinalitzacio = @DataFinalitzacio, descricpio = @Descripcio, id_prioritat = @IdPrioritat, id_responsable = @IdResponsable WHERE Id = @TaskId";

            using (SqlConnection conn = new SqlConnection(laMevaConexioString))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@TaskId", taskId);
                    cmd.Parameters.AddWithValue("@Titol", titol);
                    cmd.Parameters.AddWithValue("@DataFinalitzacio", datafinalitzacio);
                    cmd.Parameters.AddWithValue("@Descripcio", descripcio);
                    cmd.Parameters.AddWithValue("@IdPrioritat", idPrioritat);
                    cmd.Parameters.AddWithValue("@IdResponsable", idResponsable);

                    cmd.ExecuteNonQuery();
                }
            }
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
    }
}
