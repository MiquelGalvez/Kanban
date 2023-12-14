using ProjectoDragDrop.FormulariCrearTasca;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Configuration;
using ProjecteDragDrop;

namespace ProjectoDragDrop
{
    public partial class MainWindow : Window
    {
        // Variable per fer el ID unic de la tasca
        private int counterID = 0;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void CrearTasca_Click(object sender, RoutedEventArgs e)
        {
            // Obrir la finestra de creació de tasques
            CrearTasca creartasca = new CrearTasca();
            creartasca.ShowDialog(); // Utilitza ShowDialog per esperar l'entrada de l'usuari

            // Després de tancar el diàleg, obté la tasca del diàleg
            if (creartasca.DialogResult == true)
            {
                Tasca novaTasca = creartasca.CreatedTask;
                novaTasca.ID = counterID.ToString();
                // Afegeix la tasca al listbox anomenat llistattodo
                Llistattodo.Items.Add(novaTasca);
                counterID++;
            }
        }

        private void EliminarTasca_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            // Obtén el DataContext del botó, que hauria de ser l'objecte de dades associat a l'element de la llista.
            Tasca tasca = (Tasca)button.DataContext;
            Llistattodo.Items.Remove(tasca);
        }

        private void EditarTasca_Click(object sender, RoutedEventArgs e)
        {
            // Encara no s'ha implementat la lògica per a l'edició de tasques
        }
    }
}
