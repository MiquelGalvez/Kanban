using ProjectoDragDrop.FormulariCrearResponsable;
using System;
using System.Collections.Generic;
using System.Configuration;
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

namespace ProjectoDragDrop.Login
{
    /// <summary>
    /// Lógica de interacción para Login.xaml
    /// </summary>
    public partial class Login : Window
    {

        SqlConnection LaMevaConnexioSQL;
        public Login()
        {
            InitializeComponent();
            string laMevaConnexio = ConfigurationManager.ConnectionStrings["ProjectoDragDrop.Properties.Settings.kanbanConnectionString"].ConnectionString;
            LaMevaConnexioSQL = new SqlConnection(laMevaConnexio);
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            // Quan el TextBox rep el focus, el seu contingut es canvia a una cadena buida
            if (sender is TextBox textBox)
            {
                textBox.Text = string.Empty;
            }
            if (sender is PasswordBox password)
            {
                password.Password = string.Empty;
            }
        }

        // Funcio que comprova si la infromació de inici de sessió es correcte i si es aixi mostra la finestra del kanban
        private void ferLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Retrieve username and password from TextBoxes
                string username = UsernameTextBox.Text;
                string password = PasswordBox.Password.ToString();

                // Query to check if the user with provided credentials exists
                string selectQuery = $"SELECT COUNT(*) FROM Usuaris WHERE usuari = '{username}' AND password = '{password}'";

                using (SqlCommand selectCommand = new SqlCommand(selectQuery, LaMevaConnexioSQL))
                {
                    LaMevaConnexioSQL.Open();
                    int userCount = (int)selectCommand.ExecuteScalar();

                    if (userCount > 0)
                    {
                        MainWindow mainwindwo = new MainWindow(UsernameTextBox.Text.ToString());
                        mainwindwo.Show();
                        this.Close();
                    }
                    else
                    {
                        // User with provided credentials not found
                        MessageBox.Show("Usuari o Contrasenya incorrecte.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
            finally
            {
                LaMevaConnexioSQL.Close();
            }
        }


        // Funcio que ens permet crea un nou usuari si no hens hem registrat abans a l'aplicació
        private void registrarUsuari_Click(object sender, RoutedEventArgs e)
        {
            AfegirRespon afegirRespon = new AfegirRespon(true);
            afegirRespon.Show();
        }
    }
}
