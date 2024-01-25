using ProjectoDragDrop.FormulariCrearTasca;
using ProjectoDragDrop.FormulariEditarTasca;
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
using static ProjectoDragDrop.kanbanDataSet;
using System.Windows.Media;

namespace ProjectoDragDrop
{
    public partial class MainWindow : Window
    {
        SqlConnection LaMevaConnexioSQL;
        private string usauriLogin;
        private int idRol;

        public MainWindow(string usauriLogin)
        {
            InitializeComponent();
            string laMevaConnexio = ConfigurationManager.ConnectionStrings["ProjectoDragDrop.Properties.Settings.kanbanConnectionString"].ConnectionString;
            LaMevaConnexioSQL = new SqlConnection(laMevaConnexio);
            this.usauriLogin = usauriLogin;
            usuari.Content = usauriLogin;

            idRol = ObtenirIdRol(usauriLogin);

            Loaded += MainWindow_Loaded;
            


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
                // Open the VeureTasca window and pass the selected task information
                EditarTasca veureTascaWindow = new EditarTasca(selectedTask);
                veureTascaWindow.ShowDialog();
            }
            ActulitzarTasquesPerEstat();
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

                MessageBoxResult result = MessageBox.Show("¿Segur qeu vols eliminar la tasca?", "Confirmación", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // Create a SqlCommand with parameters
                    using (SqlCommand cmd = new SqlCommand(deleteQuery, LaMevaConnexioSQL))
                    {
                        cmd.Parameters.AddWithValue("@TaskId", taskId);

                        // Execute the DELETE query
                        cmd.ExecuteNonQuery();
                    }
                }
                else if (result == MessageBoxResult.No)
                {
                    
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
            // Agafa la tasca seleccionada del listbox
            if (sender is Button button && button.DataContext is DataRowView selectedTask)
            {
                // Obre la finestra VeureTasca i li passa la informació de la tasca seleccionada
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

        // Funcio que el que fa es cambiar l'estat de la tasca per poder mourela en dependencia a la columna en la que esta actualment
        private void MoureDreta_Click(object sender, RoutedEventArgs e)
        {
            // Obté la tasca seleccionada de la ListBox
            if (sender is Button button && button.DataContext is DataRowView selectedTask)
            {
                // Pots accedir a les propietats de la tasca com selectedTask["Id"], selectedTask["id_estat"], etc.
                int taskId = Convert.ToInt32(selectedTask["Id"]);
                int currentEstatId = Convert.ToInt32(selectedTask["id_estat"]);

                // Defineix el nou id_estat basant-te en la lògica del teu negoci
                int newEstatId = DeterminarNouEstat(currentEstatId);

                // Actualitza el id_estat de la tasca a la base de dades
                ActualitzarTascaEstat(taskId, newEstatId);

                // Després d'actualitzar, refresca la llista de tasques
                ActulitzarTasquesPerEstat();
            }
        }

        // Funcio que el que fa es cambiar l'estat de la tasca per poder mourela en dependencia a la columna en la que esta actualment
        private void MoureEsquerra_Click(object sender, RoutedEventArgs e)
        {
            // Obté la tasca seleccionada de la ListBox
            if (sender is Button button && button.DataContext is DataRowView selectedTask)
            {
                // Pots accedir a les propietats de la tasca com selectedTask["Id"], selectedTask["id_estat"], etc.
                int taskId = Convert.ToInt32(selectedTask["Id"]);
                int currentEstatId = Convert.ToInt32(selectedTask["id_estat"]);

                // Defineix el nou id_estat basant-te en la lògica del teu negoci
                int newEstatId = DeterminarEstatAnterior(currentEstatId);

                // Actualitza el id_estat de la tasca a la base de dades
                ActualitzarTascaEstat(taskId, newEstatId);

                // Després d'actualitzar, refresca la llista de tasques
                ActulitzarTasquesPerEstat();
            }
        }

        // Aquesta funcio el que fa es retoirna el Id del nou estat en dependencia al que tenia abans per poder moure cap a l'esquerra una tasca
        private int DeterminarEstatAnterior(int currentEstatId)
        {
            // Lògica de substitució del estat cap enrere
            switch (currentEstatId)
            {
                case 2: // DOING
                    return 1; // Suposant que 1 és el id_estat per "TO DO"
                case 3: // IN REVIEW
                    return 2; // Suposant que 2 és el id_estat per "DOING"
                case 4: // COMPLETED
                    return 3; // Suposant que 3 és el id_estat per "IN REVIEW"

                default:
                    return currentEstatId; // No hi ha canvis si no es gestiona
            }
        }

        // Aquesta funcio el que fa es retoirna el Id del nou estat en dependencia al que tenia abans per poder moure cap a la dreta una tasca
        private int DeterminarNouEstat(int currentEstatId)
        {

            // Lògica de substitució del estat
            switch (currentEstatId)
            {
                case 1: // TO DO
                    return 2; // Suposant que 2 és el id_estat per "DOING"
                case 2: // DOING
                    return 3; // Suposant que 3 és el id_estat per "IN REVIEW"
                case 3: // IN REVIEW
                    return 4; // Suposant que 4 és el id_estat per "COMPLETED"

                default:
                    return currentEstatId; // No hi ha canvis si no es gestiona
            }
        }

        // Aquesta funcio el que fa es canviar el estat de la tasca a el nou estat en dependencia del qeu tenia abans
        private void ActualitzarTascaEstat(int taskId, int newEstatId)
        {
            try
            {
                // Obre la connexió
                LaMevaConnexioSQL.Open();

                // Defineix la consulta SQL d'ACTUALITZACIÓ
                string updateQuery = "UPDATE tasca SET id_estat = @NewEstatId WHERE Id = @TaskId";

                // Crea un SqlCommand amb paràmetres
                using (SqlCommand cmd = new SqlCommand(updateQuery, LaMevaConnexioSQL))
                {
                    cmd.Parameters.AddWithValue("@TaskId", taskId);
                    cmd.Parameters.AddWithValue("@NewEstatId", newEstatId);

                    // Executa la consulta d'ACTUALITZACIÓ
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                // Gestiona l'excepció (per exemple, registra o mostra un missatge d'error)
                MessageBox.Show($"Error actualitzant l'estat de la tasca: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                // Tanca la connexió
                LaMevaConnexioSQL.Close();
            }
        }

        // Aquesta funcio el que fa es que quan nosaltres fem click a tencar sessio obra una nova finestra de login i tenca la de MainWindow
        private void sortir_Click(object sender, RoutedEventArgs e)
        {
            // Create a new instance of the login window
            ProjectoDragDrop.Login.Login loginwindow = new ProjectoDragDrop.Login.Login();

            // Close the current MainWindow
            this.Close();

            // Show the login window
            loginwindow.Show();
        }

        // Función para obtener el ID del rol según el nombre de usuario, per poder crear les restricciond segons el rol
        private int ObtenirIdRol(string nomusuari)
        {
            try
            {
                // Obre la connexió
                LaMevaConnexioSQL.Open();

                // Defineix la consulta SQL per obtenir l'Id del Rol
                string consulta = "SELECT id_rol FROM Usuaris WHERE usuari = @NombreUsuario";

                // Crea un SqlCommand amb paràmetres
                using (SqlCommand cmd = new SqlCommand(consulta, LaMevaConnexioSQL))
                {
                    // Afegeix el paràmetre amb el valor del nom d'usuari
                    cmd.Parameters.AddWithValue("@NombreUsuario", nomusuari);

                    // Executa la consulta i obté el resultat
                    object resultat = cmd.ExecuteScalar();

                    if (resultat != null)
                    {
                        // Converteix el resultat a enter (assumint que l'Id del Rol és de tipus enter)
                        return Convert.ToInt32(resultat);
                    }
                    else
                    {
                        // Usuari no trobat o Id del Rol és NULL
                        return -1; // Pots usar un altre valor per indicar un estat no vàlid
                    }
                }
            }
            catch (Exception ex)
            {
                // Gestionar l'excepció (per exemple, registrar-la o mostrar un missatge d'error)
                MessageBox.Show($"Error en obtenir l'Id del Rol: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return -1; // Pots usar un altre valor per indicar un estat no vàlid
            }
            finally
            {
                // Tancar la connexió
                LaMevaConnexioSQL.Close();
            }
        }

        // Permissos que es posaran a cada usuari en dependencia al seu rol
        private void Permissos(int idRol)
        {
            // Segons el ID de cada rol que te un usuari al fer login ocultem uns botons o uns altres
            switch (idRol)
            {
                case 1:
                    break;
                case 2:
                    eliminarResponsable.Visibility = Visibility.Collapsed;
                    break;
                case 4:
                    afegirResponsable.Visibility = Visibility.Collapsed;
                    eliminarResponsable.Visibility = Visibility.Collapsed;
                    editarResponsable.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        // Funcio per cridar a la funcio Permissos despres de que el main window hagi estat inicialitzat
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Llama a la función Permissos después de que la ventana se ha cargado completamente
            Permissos(idRol);
        }

    }
}