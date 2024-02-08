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
using static ProjectoDragDrop.kanbanDataSet;
using System.Windows.Media;
using ProjectoDragDrop.Objectes;
using System.Threading;

namespace ProjectoDragDrop
{
    public partial class MainWindow : Window
    {
        SqlConnection LaMevaConnexioSQL;
        private string laMevaConnexio;
        private string usauriLogin;
        private int idRol;
        private DadesBBDD dadesBBDD;
        private List<Usuaris> usuaris;
        private List<Estats> estats;
        private List<Rols> rols;
        public MainWindow(string usauriLogin)
        {
            InitializeComponent();
            laMevaConnexio = ConfigurationManager.ConnectionStrings["ProjectoDragDrop.Properties.Settings.kanbanConnectionString"].ConnectionString;
            LaMevaConnexioSQL = new SqlConnection(laMevaConnexio);
            this.usauriLogin = usauriLogin;
            usuari.Content = usauriLogin;
            // Crear una instancia de la clase DadesBBDD
            dadesBBDD = new DadesBBDD();


            usuaris = dadesBBDD.ObtenerUsuarios();
            rols = dadesBBDD.ObtenirRols();
            estats = dadesBBDD.ObtenirEstats();

            idRol = ObtenirIdRol(usauriLogin);

            // Llama a la función Permissos después de que la ventana se ha cargado completamente
            Permissos(idRol);
            // Llama a la función para actualizar las tareas después de mostrar el MessageBox
            ActulitzarTasquesPerEstat();
        }

        // Métode per actualitzar les tasqeus de cada columna
        public void ActulitzarTasquesPerEstat()
        {
            // Limpiar los ListBox antes de agregar las tareas
            llistattodo.Items.Clear();
            llistatdoing.Items.Clear();
            llistatinreview.Items.Clear();
            llistatcompleted.Items.Clear();

            // Agregar las tareas a cada ListBox
            dadesBBDD.MostrarTasquesPerEstat("TO DO", llistattodo);
            dadesBBDD.MostrarTasquesPerEstat("DOING", llistatdoing);
            dadesBBDD.MostrarTasquesPerEstat("IN REVIEW", llistatinreview);
            dadesBBDD.MostrarTasquesPerEstat("COMPLETED", llistatcompleted);

        }

        public int ObtenerIdEstat(string nomEstat)
        {
            // Buscar el estado por su nombre en la lista
            Estats estat = dadesBBDD.estats.Find(e => e.Estat == nomEstat);

            // Si se encuentra el estado, devolver su ID
            if (estat != null)
            {
                return estat.Id;
            }
            else
            {
                // Si no se encuentra el estado, podrías manejarlo de diferentes maneras,
                // como lanzar una excepción, devolver un valor predeterminado o imprimir un mensaje de error.
                // En este ejemplo, devolveremos -1 para indicar un estado no válido.
                return -1;
            }
        }

        // Funció que es crida en el moment de fer click al boto de crear tasca, ens permet crear una tasca nova
        private void AfegirTasca_Click(object sender, RoutedEventArgs e)
        {
            // Obrir la finestra de creació de tasques
            CrearTasca creartasca = new CrearTasca(this, usauriLogin);
            creartasca.ShowDialog(); // Utilitza ShowDialog per esperar l'entrada de l'usuari
        }

        // Funcion que quan es crida ens permet editar la tasca
        private void EditarTasca_Click(object sender, RoutedEventArgs e)
        {
            // Obtén la tarea seleccionada del ListBox
            if (sender is Button button && button.DataContext is Tasca selectedTask)
            {
                // Abre la ventana de edición y pasa la tarea seleccionada como parámetro
                EditarTasca veureTascaWindow = new EditarTasca(selectedTask);
                veureTascaWindow.ShowDialog();
            }
            ActulitzarTasquesPerEstat();
        }

        // Funció que quan es crida, crida a la funcio que elimina la tasca de la base de dades
        private void EliminarTasca_Click(object sender, RoutedEventArgs e)
        {
            // Obtener la tarea seleccionada del ListBox
            if (sender is Button button && button.DataContext is Tasca selectedTask)
            {
                if (idRol == 1 || idRol == 2)
                {
                    // Obtener el ID de la tarea seleccionada
                    int taskId = selectedTask.Id;

                    // Implementar la lógica para eliminar la tarea de la base de datos
                    EliminarTasca(taskId);

                    // Llama a la función para actualizar las tareas después de mostrar el MessageBox
                    ActulitzarTasquesPerEstat();
                }
                else
                {
                    MessageBox.Show("Només el SuperAdmin i l'Admin poden eliminar la tasca", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
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

                MessageBoxResult result = MessageBox.Show("¿Segur que vols eliminar la tasca?", "Confirmación", MessageBoxButton.YesNo, MessageBoxImage.Question);

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
            if (sender is Button button && button.DataContext is Tasca selectedTask)
            {
                // Obre la finestra VeureTasca i li passa la informació de la tasca seleccionada
                VeureTasca veureTascaWindow = new VeureTasca(selectedTask);
                veureTascaWindow.ShowDialog();
            }
        }

        // Funcio que el que fa es cambiar l'estat de la tasca per poder mourela en dependencia a la columna en la que esta actualment
        private void MoureTasca_Click(object sender, RoutedEventArgs e)
        {
            // Obtenir la tasca seleccionada del listbox
            if (sender is Button button && button.DataContext is Tasca selectedTask)
            {
                // Comprovar si l'usuari té permisos per moure la tasca al darrer estat
                if (idRol == 1 || idRol == 2) // Comprova si l'usuari és SuperAdmin (canvia aquesta condició si el codi d'identificació de SuperAdmin és diferent)
                {
                    int taskId = selectedTask.Id;
                    string currentEstat = selectedTask.Id_estat;
                    int currentEstatId = ObtenirIdEstat(currentEstat);

                    // Determina la direcció del moviment segons el botó que s'hagi premut
                    bool moureHaciaDreta = (button.Name == "MoureDreta");

                    // Determina l'ID del nou estat
                    int nouEstatId = ObtenerNuevoEstatId(currentEstatId, moureHaciaDreta);

                    string nouEstat = ObtenirNomEstat(nouEstatId);

                    // Actualitza l'estat de la tasca a la llista de tasques
                    selectedTask.Id_estat = nouEstat.ToString();

                    // Actualitza la base de dades amb el nou estat de la tasca
                    ActualitzarABBDD(nouEstatId, taskId);

                    // Actualitza les tasques en tots els ListBox
                    ActulitzarTasquesPerEstat();
                }
                else
                {
                    MessageBox.Show("Només el SuperAdmin i l'Admin poden moure la tasca", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // Funció per determinar el nou estat de la tasca
        private int ObtenerNuevoEstatId(int currentEstatId, bool moureHaciaDreta)
        {
            // Determina el nou estat de la tasca segons la direcció del moviment a partir del nom del boto de la tasca
            return moureHaciaDreta ? DeterminarNouEstat(currentEstatId) : DeterminarEstatAnterior(currentEstatId);
        }

        // Aquesta funcio el que fa es retorna el Id del nou estat en dependencia al que tenia abans per poder moure cap a la dreta una tasca
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

        // Funcio qeu el qeu fa es un cop actualitzat correctament l'objecte tasca el que fa es edita a la base de dades el estat d'aquesta tasca a partir del Id
        private void ActualitzarABBDD(int newEstatId, int taskId)
        {
            try
            {
                // Abrir la conexión a la base de datos
                LaMevaConnexioSQL.Open();

                // Definir la consulta SQL para actualizar el estado de la tarea
                string updateQuery = "UPDATE Tasca SET id_estat = @NewEstatId WHERE Id = @TaskId";

                // Crear un comando SQL con los parámetros
                using (SqlCommand cmd = new SqlCommand(updateQuery, LaMevaConnexioSQL))
                {
                    cmd.Parameters.AddWithValue("@NewEstatId", newEstatId);
                    cmd.Parameters.AddWithValue("@TaskId", taskId);

                    // Ejecutar la consulta de actualización
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                // Manejar la excepción
                MessageBox.Show($"Error al mover la tarea: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                // Cerrar la conexión a la base de datos
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

        // Funcio que al fer click ens obra la finestra per gestionar els responsables/usuaris
        private void Responsables_Click(object sender, RoutedEventArgs e)
        {
            GestioResponsables.Responsables responsables = new GestioResponsables.Responsables(this);
            responsables.ShowDialog();
        }

        // Permissos que es posaran a cada usuari en dependencia al seu rol
        private void Permissos(int idRol)
        {
            // Segons el ID de cada rol que te un usuari al fer login ocultem uns botons o uns altres
            switch (idRol)
            {
                case 1:
                    break;
                case 4:
                    ResponsableButton.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        public int ObtenirIdRol(string nomUsuari)
        {
            // Busca el rol pel nom d'usuari a la partir de la llista d'usuaris que emplenem nomes en executar la app
            Usuaris usuari = dadesBBDD.usuaris.Find(u => u.Usuari.Trim() == nomUsuari.Trim());

            // Si aquest usuari s'ha trobat, busca el rol a la llista d'usuaris
            if (usuari != null)
            {
                Rols rol = dadesBBDD.rols.Find(r => r.Id.ToString() == usuari.Id_rol);

                // I finalment si es troba el rol a la llista retornem el seu Id
                if (rol != null)
                {
                    return rol.Id;
                }
            }

            // Si no se encuentra el usuario o el rol, devolver -1 para indicar un error
            return -1;
        }
        public int ObtenirIdEstat(string nomEstat)
        {

            Estats estat = dadesBBDD.estats.Find(e => e.Estat == nomEstat);

            
            if (estat != null)
            {
                return estat.Id;
            }
            
            return -1;
        }
        public string ObtenirNomEstat(int IdEstat)
        {
            Estats estat = dadesBBDD.estats.Find(e => e.Id == IdEstat);
            if (estat != null)
            {
                return estat.Estat;
            }
            return "null";
        }
    }
}