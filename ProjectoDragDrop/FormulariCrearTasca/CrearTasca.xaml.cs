using ProjecteDragDrop;
using System;
using System.Collections.Generic;
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

namespace ProjectoDragDrop.FormulariCrearTasca
{
    public partial class CrearTasca : Window
    {
        
        // Propietat interna per emmagatzemar la Tasca creada
        internal Tasca CreatedTask { get; private set; }

        // Constructor de la classe
        public CrearTasca()
        {
            InitializeComponent();
            CrearLlistats();
        }

        // Gestor d'esdeveniments per al clic al botó Crear
        private void CrearButton_Click(object sender, RoutedEventArgs e)
        {
            // Suposant que dp1.Text és la representació de cadena de la data
            if (DateTime.TryParse(dp1.Text, out DateTime dataFinalitzacio))
            {
                // Obtenir la data i hora actual
                DateTime dataCreacio = DateTime.Now;

                // Permetre que la data de finalització sigui el mateix dia que la data de creació
                if (dataFinalitzacio >= dataCreacio.Date)
                {
                    // Creació d'una nova Tasca amb les dades proporcionades
                    Tasca newTask = new Tasca
                    {
                        Titol = Titol.Text,
                        Descripcio = DescripcioTasca.Text,
                        // Format de la data de creació en el format desitjat
                        DataCreacio = dataCreacio.ToString("dd/MM/yyyy"),
                        DataFinalitzacio = dataFinalitzacio.ToString("dd/MM/yyyy"),
                        // Obtenir el responsable seleccionat o posar "Sense Assignar" per defecte
                        Responsable = (responsables.SelectedItem as Tasca)?.Responsable ?? "Sense Asignar",
                        // Obtenir la prioritat seleccionada o posar "Baixa" per defecte
                        Prioritat = (prioritats.SelectedItem as Tasca)?.Prioritat ?? "Baixa",
                        // Obtenir l'estat per defecte "TO DO"
                        Estat = "TO DO",
                    };

                    // Assignació de la Tasca creada a la propietat CreatedTask
                    CreatedTask = newTask;

                    // Establiment de DialogResult a true per indicar l'èxit
                    DialogResult = true;

                    // Tancament de la finestra
                    Close();
                }
                else
                {
                    // Mostrar missatge d'error si la data de finalització no és igual o posterior a la data de creació
                    MessageBox.Show("La data de finalització ha de ser igual o posterior a la data de creació");
                }
            }
            else
            {
                // Gestió del cas en què la cadena no es pugui convertir a DateTime
                MessageBox.Show("Format de data no vàlid per a DataFinalitzacio");
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

        private void CrearLlistats()
        {
            // Inicialització d'una llista de prioritats per posar al combobox de proprietats del formulari
            List<Tasca> llistatprioritats = new List<Tasca>();
            llistatprioritats.Add(new Tasca { Prioritat = "Alta" });
            llistatprioritats.Add(new Tasca { Prioritat = "Mitja" });
            llistatprioritats.Add(new Tasca { Prioritat = "Baixa" });

            // Assignació de la llista com a origen de dades per a l'element prioritats
            prioritats.ItemsSource = llistatprioritats;

            // Inicialització d'una llista de responsables per posar al combobox de responsables del formulari
            List<Tasca> llistatreponsables = new List<Tasca>();
            llistatreponsables.Add(new Tasca { Responsable = "Miquel" });
            llistatreponsables.Add(new Tasca { Responsable = "Juanes" });
            llistatreponsables.Add(new Tasca { Responsable = "Sense Asignar" });

            // Assignació de la llista com a origen de dades per a l'element responsables
            responsables.ItemsSource = llistatreponsables;

        }

    }
}
