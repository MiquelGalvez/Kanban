using ProjectoDragDrop.Objectes;
using System;
using System.Windows;

namespace ProjectoDragDrop.MesInfoTasca
{
    /// <summary>
    /// Lógica de interacción para VeureTasca.xaml
    /// </summary>
    public partial class VeureTasca : Window
    {
        public VeureTasca(Tasca task)
        {
            InitializeComponent();

            // Emplenem els TextBlock amb la informacion de cada tasca
            EmplenarInfo(task);
        }

        // Aquesta funció ens emplena tota l'informació de la tasca als TextBlock del xaml
        private void EmplenarInfo(Tasca task)
        {
            titol.Content = task.Titol;
            prioritat.Content = task.Id_Prioritat;
            datafinalitzacio.Content = task.Datafinalitzacio.ToString();
            datacreacio.Content = task.Datacreacio.ToString();
            responsable.Content = task.Id_responsable;
            descripcio.Text = task.Descripcio;
            estat.Content = task.Id_estat;
        }
    }
}