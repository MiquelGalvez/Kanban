using ProjectoDragDrop.FormulariCrearTasca;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Configuration;
using System.Windows;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Data;
using ProjectoDragDrop.MesInfoTasca;
using ProjectoDragDrop.FormulariCrearResponsable;

namespace ProjectoDragDrop
{
    public partial class MainWindow : Window
    {
        SqlConnection LaMevaConnexioSQL;

        public MainWindow()
        {
            InitializeComponent();
            string laMevaConnexio = ConfigurationManager.ConnectionStrings["ProjectoDragDrop.Properties.Settings.kanbanConnectionString"].ConnectionString;
            LaMevaConnexioSQL = new SqlConnection(laMevaConnexio);

            // Funció que es crida al principi per poder mostrar les tasques ja guardades en la base de dades
            ActulitzarTasquesPerEstat();
        }

        // Mostrar les asques per estat en la seva columna especifica
        private void MostrarTasquesPerEstat(string estado, ListBox listBox)
        {
            string consulta = "SELECT * FROM tasca t INNER JOIN Prioritat p ON t.id_prioritat = p.Id INNER JOIN Estat e ON t.id_estat = e.Id" + " WHERE e.estat = @Estado";

            SqlDataAdapter elMeuAdaptador = new SqlDataAdapter(consulta, LaMevaConnexioSQL);

            using (elMeuAdaptador)
            {
                elMeuAdaptador.SelectCommand.Parameters.AddWithValue("@Estado", estado);

                DataTable dt = new DataTable();
                elMeuAdaptador.Fill(dt);

                listBox.SelectedValuePath = "Id";
                listBox.ItemsSource = dt.DefaultView;
            }
        }

        // Actualitzar les tasques de cada columna
        public void ActulitzarTasquesPerEstat()
        {
            MostrarTasquesPerEstat("TO DO", llistattodo);
            MostrarTasquesPerEstat("DOING", llistatdoing);
            MostrarTasquesPerEstat("IN REVIEW", llistatinreview);
            MostrarTasquesPerEstat("COMPLETED", llistatcompleted);
        }

        // Funció que es crida en el moment de fer click al boto de crear tasca, ens permet crear una tasca nova
        private void AfegirTasca_Click(object sender, RoutedEventArgs e)
        {
            // Obrir la finestra de creació de tasques
            CrearTasca creartasca = new CrearTasca(this);
            creartasca.ShowDialog(); // Utilitza ShowDialog per esperar l'entrada de l'usuari

            // Després de tancar el diàleg, obté la tasca del diàleg
            if (creartasca.DialogResult == true)
            {

            }
        }

        // Funcion que quan es crida ens permet editar la tasca
        private void EditarTasca_Click(object sender, RoutedEventArgs e)
        {
            // Get the selected task from the ListBox
            if (sender is Button button && button.DataContext is DataRowView selectedTask)
            {
                // Logica per modificar la informació de la tasca

                // After editing, refresh the task list
                ActulitzarTasquesPerEstat();
            }
        }

        // Funció que quan es crida, crida a la funcio que elimina la tasca de la base de dades
        private void EliminarTasca_Click(object sender, RoutedEventArgs e)
        {
            // Get the selected task from the ListBox
            if (sender is Button button && button.DataContext is DataRowView selectedTask)
            {
                // You can access task properties like selectedTask["Id"], selectedTask["titol"], etc.
                int taskId = Convert.ToInt32(selectedTask["Id"]);

                // Implement the logic to delete the task from the database
                EliminarTasca(taskId);

                // After deleting, refresh the task list
                ActulitzarTasquesPerEstat();
            }
        }

        // Funció que eliminar la tasca de la base de dades
        private void EliminarTasca(int taskId)
        {
            try
            {
                // Open the connection
                LaMevaConnexioSQL.Open();

                // Define the DELETE SQL query
                string deleteQuery = "DELETE FROM tasca WHERE Id = @TaskId";

                // Create a SqlCommand with parameters
                using (SqlCommand cmd = new SqlCommand(deleteQuery, LaMevaConnexioSQL))
                {
                    cmd.Parameters.AddWithValue("@TaskId", taskId);

                    // Execute the DELETE query
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                // Handle the exception (e.g., log or display an error message)
                MessageBox.Show($"Error deleting task: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                // Close the connection
                LaMevaConnexioSQL.Close();
            }
        }

        // Funcío que ens permet veure tota l'informació d'una tasca en especific
        private void VeureInformacióTasca_Click(object sender, RoutedEventArgs e)
        {
            // Get the selected task from the ListBox
            if (sender is Button button && button.DataContext is DataRowView selectedTask)
            {
                // Open the VeureTasca window and pass the selected task information
                VeureTasca veureTascaWindow = new VeureTasca(selectedTask);
                veureTascaWindow.ShowDialog();
            }
        }

        // Funció que quan es crida obra una nova finestra per poder crear un respnosable
        private void AfegirResponsable_Click(object sender, RoutedEventArgs e)
        {
            // Obrir la finestra de creació de tasques
            AfegirRespon afegirresponsable = new AfegirRespon();
            afegirresponsable.ShowDialog(); // Utilitza ShowDialog per esperar l'entrada de l'usuari

            // Després de tancar el diàleg, obté la tasca del diàleg
            if (afegirresponsable.DialogResult == true)
            {

            }
        }

        // Funció que quan es crida ens permet eliminar un usuari de la base de dades, si posem les seves credencials correctament
        private void EliminarUsuari_Click(object sender, RoutedEventArgs e)
        {
            // Obrir la finestra de eliminació de usuaris
            EliminarUsuari.EliminarUsuari deleteuser = new EliminarUsuari.EliminarUsuari();
            deleteuser.ShowDialog();

            if (deleteuser.DialogResult == true)
            {

            }
        }
    }
}