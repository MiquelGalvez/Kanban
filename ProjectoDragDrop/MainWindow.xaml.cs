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
using ProjectoDragDrop.MesInfoTasca;

namespace ProjectoDragDrop
{
    public partial class MainWindow : Window
    {
        // Variable per fer el ID unic de la tasca
        private int counterID = 1;
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
                novaTasca.ID = "ID: " + counterID.ToString();
                // Afegeix la tasca al listbox anomenat llistattodo

                afeigrEstat(novaTasca);
                counterID++;

            }
        }

        private void EliminarTasca_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            // Obté el DataContext del botó, que hauria de ser l'objecte de dades associat a l'element de la llista.
            Tasca tasca = (Tasca)button.DataContext;
            comporvarEstat(tasca);
        }

        private void EditarTasca_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            Tasca tasca = (Tasca)button.DataContext;

            CrearTasca creartasca = new CrearTasca();

            // Set the fields of CrearTasca window with the values of the selected task for editing
            creartasca.Titol.Text = tasca.Titol;
            creartasca.prioritats.Text = tasca.Prioritat;
            creartasca.responsables.Text = tasca.Responsable;
            creartasca.DescripcioTasca.Text = tasca.Descripcio;
            creartasca.Datafinalització.Text = tasca.DataFinalitzacio;
            creartasca.estats.Text = tasca.Estat;

            comporvarEstat(tasca);

            // Show the CrearTasca window to allow the user to edit the task
            creartasca.ShowDialog();

            // After closing the dialog, get the task from the dialog
            if (creartasca.DialogResult == true)
            {
                // Update the task with the edited values
                tasca.Titol = creartasca.Titol.Text;
                tasca.Prioritat = creartasca.prioritats.Text;
                tasca.Responsable = creartasca.responsables.Text;
                tasca.Descripcio = creartasca.DescripcioTasca.Text;
                tasca.DataFinalitzacio = creartasca.Datafinalització.Text;
                tasca.Estat = creartasca.estats.Text;

                if (tasca.Estat == "TO DO")
                {
                    // Update the ListBox to reflect the changes
                    int index_todo = llistattodo.Items.IndexOf(tasca);
                    llistattodo.Items.Add(tasca);
                }
                else if (tasca.Estat == "DOING")
                {
                    // Update the ListBox to reflect the changes
                    int index_doing = llistattodo.Items.IndexOf(tasca);
                    llistatdoing.Items.Add(tasca);
                }
                else if (tasca.Estat == "IN REVIEW")
                {
                    // Update the ListBox to reflect the changes
                    int index_inreview = llistattodo.Items.IndexOf(tasca);
                    llistatinreview.Items.Add(tasca);
                }
                else if (tasca.Estat == "COMPLETED")
                {
                    // Update the ListBox to reflect the changes
                    int index_completed = llistattodo.Items.IndexOf(tasca);
                    llistatcompleted.Items.Add(tasca);
                }
            }
        }

        private void VeureTasca_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            Tasca tasca = (Tasca)button.DataContext;

            VeureTasca creartasca = new VeureTasca();

            // Set the fields of CrearTasca window with the values of the selected task for editing
            creartasca.titol.Content = tasca.Titol;
            creartasca.prioritat.Content = tasca.Prioritat;
            creartasca.responsable.Content = tasca.Responsable;
            creartasca.descripcio.Content = tasca.Descripcio;
            creartasca.datafinalitzacio.Content = tasca.DataFinalitzacio;
            creartasca.estat.Content = tasca.Estat;
            creartasca.datacreacio.Content = tasca.DataCreacio;

            // Show the CrearTasca window to allow the user to edit the task
            creartasca.ShowDialog();

            if (creartasca.DialogResult == true)
            {
            }
        }

        private void comporvarEstat(Tasca tasca)
        {
            if (tasca.Estat == "TO DO")
            {
                llistattodo.Items.Remove(tasca);
            }
            else if (tasca.Estat == "DOING")
            {
                llistatdoing.Items.Remove(tasca);
            }
            else if (tasca.Estat == "IN REVIEW")
            {
                llistatinreview.Items.Remove(tasca);
            }
            else if (tasca.Estat == "COMPLETED")
            {
                llistatcompleted.Items.Remove(tasca);
            }
        }
        private void afeigrEstat(Tasca tasca)
        {
            if (tasca.Estat == "TO DO")
            {
                llistattodo.Items.Add(tasca);
            }
            else if (tasca.Estat == "DOING")
            {
                llistatdoing.Items.Add(tasca);
            }
            else if (tasca.Estat == "IN REVIEW")
            {
                llistatinreview.Items.Add(tasca);
            }
            else if (tasca.Estat == "COMPLETED")
            {
                llistatcompleted.Items.Add(tasca);
            }
        }

    }
}
