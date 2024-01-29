using ProjectoDragDrop;
using ProjectoDragDrop.FormulariCrearTasca;
using ProjectoDragDrop.Login;
using System;
using System.Data.SqlClient;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Configuration;

namespace ProjectoDragDrop.FormulariCrearResponsable
{
    public partial class AfegirRespon : Window
    {
        SqlConnection LaMevaConnexioSQL;
        private bool openedFromLogin;
        private string nomrol;
        public AfegirRespon(bool openedFromLogin = false)
        {

            InitializeComponent();
            this.openedFromLogin = openedFromLogin;
            string laMevaConnexio = ConfigurationManager.ConnectionStrings["ProjectoDragDrop.Properties.Settings.kanbanConnectionString"].ConnectionString;
            LaMevaConnexioSQL = new SqlConnection(laMevaConnexio);
            if (openedFromLogin)
            {
                Roles.Visibility = Visibility.Collapsed;
            }
            // Quan s'executa la finestra es crida a aquesta funció
            MostrarRols();
        }


        // Funcio que el que fa es posar al TextBlock on estigui assignat el text a blanc quan es fa click dins del TextBlock
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            // Quan el TextBox rep el focus, el seu contingut es canvia a una cadena buida
            if (sender is TextBox textBox)
            {
                textBox.Text = string.Empty;
            }
        }

        // Funció que ens mostra el nom dels rols al combobox a l'hora de crear un usuari
        private void MostrarRols()
        {
            string consulta = "SELECT rol FROM Rol";

            SqlDataAdapter elMeuAdaptador = new SqlDataAdapter(consulta, LaMevaConnexioSQL);
            using (elMeuAdaptador)
            {

                DataTable dt = new DataTable();
                elMeuAdaptador.Fill(dt);

                //POR CADA USER AÑADIR AL COMBOBOX
                foreach (DataRow row in dt.Rows)
                {
                    rols.Items.Add(row["rol"].ToString());
                }
            }
        }

        // Funció que s'executa al fer click al boto de crear, on agafa tot els valors dels TextBlock per emplenar l'informació de la base de dades
        private void CrearUsuari(object sender, RoutedEventArgs e)
        {
            try
            {
                // Obtener valores de los controles
                string nom = Nom.Text;
                string email = Email.Text;
                string usuari = User.Text; 
                string pwd = Password.Text;

                // Si hi ha algun usuari a la base de dades els usuaris nous que es registrin tindran per defecte el Rol de User, pero si no hi ha cap usuari encara a la base de dades el primer usuari qeu es registri sera admin
                ProjectoDragDrop.Login.Login loginInstance = new ProjectoDragDrop.Login.Login();
                int countusers = loginInstance.ComprobarUsers();

                if (countusers <= 0)
                {
                    nomrol = "SuperAdmin";
                }
                else {
                    // Obtener identificador del responsable seleccionado
                    nomrol = rols.SelectedItem?.ToString() ?? "User";
                }

                
                
                int idRol = ObtenirIdRol(nomrol);


                // Realizar la inserción en la base de datos
                string inserirTascaQuery = "INSERT INTO Usuaris (nom, email, usuari, password, id_rol) " +
                                           "VALUES (@Nom, @Email, @User, @Password, @idRol)";

                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ProjectoDragDrop.Properties.Settings.kanbanConnectionString"].ConnectionString))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(inserirTascaQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@Nom", nom);
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@User", usuari);
                        cmd.Parameters.AddWithValue("@Password", pwd);
                        cmd.Parameters.AddWithValue("@idRol", idRol);

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Usuari creat exitosament.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al crear l'usuari: {ex.Message}");
            }
        }
        
        //Metode per obtenir el id del Rol per aixi poder crear l'usuari correctament a la taula Usuaris
        private int ObtenirIdRol(string responsableNombre)
        {
            string consulta = "SELECT Id FROM Rol WHERE rol = @nomrol";

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ProjectoDragDrop.Properties.Settings.kanbanConnectionString"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(consulta, conn))
                {
                    cmd.Parameters.AddWithValue("@nomrol", responsableNombre);
                    conn.Open();
                    return (int)cmd.ExecuteScalar();
                }
            }
        }
    }
    

}
