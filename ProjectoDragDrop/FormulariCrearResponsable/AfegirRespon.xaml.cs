using ProjecteDragDrop;
using ProjectoDragDrop.Classe;
using ProjectoDragDrop.FormulariCrearTasca;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ProjectoDragDrop.FormulariCrearResponsable
{
    public partial class AfegirRespon : Window
    {
        internal Responsable CreatedTask { get; private set; }

        public AfegirRespon()
        {
            InitializeComponent();
        }

        private void CrearButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Responsable newResponsable = new Responsable
                {
                    Nom = Name_new.Text,
                    Cognoms = LastName.Text,
                };
                CreatedTask = newResponsable;
                (Owner as CrearTasca)?.AgregarResponsableALista(newResponsable.Nom);

                // Limpiar los TextBox después de agregar el responsable
                Name_new.Text = string.Empty;
                LastName.Text = string.Empty;

                // Cierra la ventana actual
                Close();
            }

            catch (Exception ex)
            {
                MessageBox.Show($"Error creating Responsable: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
    }
}
