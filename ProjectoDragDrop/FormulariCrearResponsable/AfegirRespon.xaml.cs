using ProjecteDragDrop;
using ProjectoDragDrop.Classe;
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
                    // Set other properties as needed based on your Responsable class
                };

                // You may want to perform additional validation or processing here

                // Set the CreatedTask property to the new Responsable object
                CreatedTask = newResponsable;

                // Optionally, you can close or navigate away from this page
                // For example, you can navigate back to the previous page:
                // NavigationService?.GoBack();
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

        private void Id_Respon_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Verifica si la entrada es un número
            if (!IsNumeric(e.Text))
            {
                // Si no es un número, establece la propiedad Handled en true para evitar que se agregue al TextBox
                e.Handled = true;
            }
        }

        // Función para verificar si una cadena es numérica
        private bool IsNumeric(string text)
        {
            return int.TryParse(text, out _);
        }
    }
}
