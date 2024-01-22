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

namespace ProjectoDragDrop.FormulariCrearTasca
{
    public partial class CrearTasca : Window
    {
        SqlConnection LaMevaConnexioSQL;
        private MainWindow mainWindow;
        public CrearTasca(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            string laMevaConnexio = ConfigurationManager.ConnectionStrings["ProjectoDragDrop.Properties.Settings.kanbanConnectionString"].ConnectionString;
            LaMevaConnexioSQL = new SqlConnection(laMevaConnexio);

            MostrarRespnsables();
            MostrarPrioritats();
            // Agregar el evento Click al botón CrearButton
            CrearButton.Click += CrearButton_Click;
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


        // Aquesta funcio crea la tasca a la base de dades
        private void CrearButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Obtindre els valors dels controladors
                string titol = Titol.Text;
                DateTime datacreacio = DateTime.Now;  // Obtenim la data del dia d'avui
                DateTime datafinalitzacio = dp1.SelectedDate ?? DateTime.Now; //  Obtenim la data seleccionada y si no seleccionem res obtenim la data del dia d'avui
                string descripcio = DescripcioTasca.Text;

                // Obtenir l'id del responsable selecionat
                string responsableNombre = responsables.SelectedItem?.ToString();
                int idResponsable = ObtenerIdResponsable(responsableNombre);

                // Obtenir l'id de la prioritat selecionat
                string prioritatNombre = prioritats.SelectedItem?.ToString();
                int idPrioritat = ObtenerIdPrioritat(prioritatNombre);

                // Obtenir l'id del estat per defecte, no el podem seleccionar sempre ens posara sempre l'estat com a TO DO
                int idEstat = ObtenerIdEstat("TO DO");

                // Inserir aquesta tasca a la Taula de la base de dades
                string inserirTascaQuery = "INSERT INTO Tasca (titol, descricpio, datacreacio, datafinalitzacio, id_responsable, id_prioritat, id_estat) " +
                                           "VALUES (@Titol, @Descripcio, @DataCreacio, @DataFinalitzacio, @IdResponsable, @IdPrioritat, @IdEstat)";

                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ProjectoDragDrop.Properties.Settings.kanbanConnectionString"].ConnectionString))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(inserirTascaQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@Titol", titol);
                        cmd.Parameters.AddWithValue("@Descripcio", descripcio);
                        cmd.Parameters.AddWithValue("@DataCreacio", datacreacio);
                        cmd.Parameters.AddWithValue("@DataFinalitzacio", datafinalitzacio);
                        cmd.Parameters.AddWithValue("@IdResponsable", idResponsable);
                        cmd.Parameters.AddWithValue("@IdPrioritat", idPrioritat);
                        cmd.Parameters.AddWithValue("@IdEstat", idEstat);

                        cmd.ExecuteNonQuery();
                    }
                }

                mainWindow.ActulitzarTasquesPerEstat();
                MessageBox.Show("Tasca creada exitosamente.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al crear la tasca: {ex.Message}");
            }
        }


        // Funció utilitzada per obtenir l'id del responsable
        private int ObtenerIdResponsable(string responsableNombre)
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
        private int ObtenerIdPrioritat(string prioritatNombre)
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

        // Funció utilitzada per obtenir l'id del estat
        private int ObtenerIdEstat(string estatNombre)
        {
            string consulta = "SELECT Id FROM Estat WHERE estat = @Estat";

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ProjectoDragDrop.Properties.Settings.kanbanConnectionString"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(consulta, conn))
                {
                    cmd.Parameters.AddWithValue("@Estat", estatNombre);
                    conn.Open();
                    return (int)cmd.ExecuteScalar();
                }
            }
        }
    }
}
