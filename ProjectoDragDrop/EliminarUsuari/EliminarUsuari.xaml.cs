﻿using ProjectoDragDrop;
using ProjectoDragDrop.EliminarUsuari;
using System;
using System.Data.SqlClient;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Configuration;

namespace ProjectoDragDrop.EliminarUsuari
{
    /// <summary>
    /// Lógica de interacción para EliminarUsuari.xaml
    /// </summary>
    public partial class EliminarUsuari : Window
    {
        SqlConnection LaMevaConnexioSQL;
        public EliminarUsuari()
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
        }

        private void EliminarUsuari_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Retrieve username and password from TextBoxes
                string username = User.Text;
                string password = Password.Text;

                // Query to check if the user with provided credentials exists
                string selectQuery = $"SELECT COUNT(*) FROM Usuaris WHERE usuari = '{username}' AND password = '{password}'";

                using (SqlCommand selectCommand = new SqlCommand(selectQuery, LaMevaConnexioSQL))
                {
                    LaMevaConnexioSQL.Open();
                    int userCount = (int)selectCommand.ExecuteScalar();

                    if (userCount > 0)
                    {
                        // User with correct credentials exists, proceed with deletion
                        string deleteQuery = $"DELETE FROM Usuaris WHERE usuari = '{username}' AND password = '{password}'";

                        using (SqlCommand deleteCommand = new SqlCommand(deleteQuery, LaMevaConnexioSQL))
                        {
                            int rowsAffected = deleteCommand.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Usuari eliminat correctament.");
                            }
                            else
                            {
                                MessageBox.Show("Error al eliminar l'usuari.");
                            }
                        }
                    }
                    else
                    {
                        // User with provided credentials not found
                        MessageBox.Show("Usuari o Contrasenya incorrecte, usuari no trobat.");
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
    }
}