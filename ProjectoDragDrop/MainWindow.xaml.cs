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
using ProjectoDragDrop.FormulariCrearResponsable;

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

            // Posem els valors ja introduits dins de la tasca pero despres poder canviarlos respecte als qeu ja tenim en una primera isntancia
            creartasca.Titol.Text = tasca.Titol;
            creartasca.prioritats.Text = tasca.Prioritat;
            creartasca.responsables.Text = tasca.Responsable;
            creartasca.DescripcioTasca.Text = tasca.Descripcio;
            creartasca.dp1.Text = tasca.DataFinalitzacio;

            // Mostrem la finestra de crear Tasca
            creartasca.ShowDialog();

            
            if (creartasca.DialogResult == true)
            {
                // Canviem els valor de la tasca per els valors escrits dins de la finestra d'editar tasca
                tasca.Titol = creartasca.Titol.Text;
                tasca.Prioritat = creartasca.prioritats.Text;
                tasca.Responsable = creartasca.responsables.Text;
                tasca.Descripcio = creartasca.DescripcioTasca.Text;
                tasca.DataFinalitzacio = creartasca.dp1.Text;
                

                if (tasca.Estat == "TO DO")
                {
                    // Recarregem els items dels listbox per poder veure el canvis
                    llistattodo.Items.Refresh();
                }
                else if (tasca.Estat == "DOING")
                {
                    // Recarregem els items dels listbox per poder veure el canvis
                    llistatdoing.Items.Refresh();
                }
                else if (tasca.Estat == "IN REVIEW")
                {
                    // Recarregem els items dels listbox per poder veure el canvis
                    llistatinreview.Items.Refresh();
                }
                else if (tasca.Estat == "COMPLETED")
                {
                    // Recarregem els items dels listbox per poder veure el canvis
                    llistatcompleted.Items.Refresh();
                }
            }
        }

        private void VeureTasca_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            Tasca tasca = (Tasca)button.DataContext;

            VeureTasca creartasca = new VeureTasca();

            // El que fa es posar als tetxbox, l'informació de la tasca seleccionada.
            creartasca.titol.Content = tasca.Titol;
            creartasca.prioritat.Content = tasca.Prioritat;
            creartasca.responsable.Content = tasca.Responsable;
            creartasca.descripcio.Text = tasca.Descripcio;
            creartasca.datafinalitzacio.Content = tasca.DataFinalitzacio;
            creartasca.estat.Content = tasca.Estat;
            creartasca.datacreacio.Content = tasca.DataCreacio;
            

            // Mostrem la finestra per verue l'informació de la tasca 
            creartasca.ShowDialog();

            if (creartasca.DialogResult == true)
            {
            }
        }

        //Funció que es crida al fer clic al botó per moure cap a la dreta la tasca i el qeu fa es actualitzar l'estat al seguent estat disponible
        private void comporvarEstat(Tasca tasca)
        {
            if (tasca.Estat == "TO DO")
            {
                llistattodo.Items.Remove(tasca);
                tasca.Estat = "DOING";
            }
            else if (tasca.Estat == "DOING")
            {
                llistatdoing.Items.Remove(tasca);
                tasca.Estat = "IN REVIEW";
            }
            else if (tasca.Estat == "IN REVIEW")
            {
                llistatinreview.Items.Remove(tasca);
                tasca.Estat = "COMPLETED";
            }
            else if (tasca.Estat == "COMPLETED")
            {
                llistatcompleted.Items.Remove(tasca);
            }
        }
 
        //Funció que es crida al fer clic al botó per moure cap a l'esquerra la tasca i el qeu fa es actualitzar l'estat al seguent estat disponible
        private void comporvarEstatEs(Tasca tasca)
        {
            if (tasca.Estat == "TO DO")
            {
                llistattodo.Items.Remove(tasca);
            }
            else if (tasca.Estat == "DOING")
            {
                llistatdoing.Items.Remove(tasca);
                tasca.Estat = "TO DO";
            }
            else if (tasca.Estat == "IN REVIEW")
            {
                llistatinreview.Items.Remove(tasca);
                tasca.Estat = "DOING";
            }
            else if (tasca.Estat == "COMPLETED")
            {
                llistatcompleted.Items.Remove(tasca);
                tasca.Estat = "IN REVIEW";
            }
        }


        //Funcio per un cop actualitzat l'estat de la tasca, afegeixi a la nova columna la tasca
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


        //Funcio per moure a la dreta i canviar d'estat les tasques
        private void MoureDreta_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            Tasca tasca = (Tasca)button.DataContext;
            comporvarEstat(tasca);
            afeigrEstat(tasca);
        }


        //Funcio per moure a l'esquerra i canviar d'estat les tasques
        private void MoureEsquerra_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            Tasca tasca = (Tasca)button.DataContext;
            comporvarEstatEs(tasca);
            afeigrEstat(tasca);
        }

        private void AfegirRespon_Click(object sender, RoutedEventArgs e)
        {
            // Crear una nueva instancia de AfegirRespon
            AfegirRespon crearResponsable = new AfegirRespon();

            // Mostrar la ventana utilizando ShowDialog para esperar la entrada del usuario
            crearResponsable.ShowDialog();
        }

    }
}
