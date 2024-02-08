using ProjectoDragDrop;
using ProjectoDragDrop.Objectes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ProjectoDragDrop.FormulariEditarTasca
{
    public partial class EditarTasca : Window
    {
        SqlConnection LaMevaConnexioSQL;
        private int TascaId;
        private string ResponsableNom;
        private string PrioritatNom;
        private DadesBBDD dadesBBDD;
        public string laMevaConexioString = ConfigurationManager.ConnectionStrings["ProjectoDragDrop.Properties.Settings.kanbanConnectionString"].ConnectionString;
        public EditarTasca(Tasca tasca)
        {
            dadesBBDD = new DadesBBDD();
            InitializeComponent();
            TascaId = tasca.Id;
            LaMevaConnexioSQL = new SqlConnection(laMevaConexioString);

            ResponsableNom = tasca.Id_responsable;
            PrioritatNom = tasca.Id_Prioritat;

            MostrarRespnsables(dadesBBDD.ObtenerUsuarios());
            MostrarPrioritats(dadesBBDD.ObtenirPrioritats());

            EmplenarInfo(tasca);
        }

        // Funció que sereveix per poder emplenar als TextBox i Date Picker amb la infromació de la tasca
        private void EmplenarInfo(Tasca tarea)
        {
            Titol.Text = tarea.Titol;
            dp1.SelectedDate = tarea.Datafinalitzacio;
            DescripcioTasca.Text = tarea.Descripcio;

            responsables.SelectedItem = ResponsableNom;
            prioridades.SelectedItem = PrioritatNom;
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            // Quan el TextBox rep el focus, el seu contingut es canvia a una cadena buida
            if (sender is TextBox textBox)
            {
                textBox.Text = string.Empty;
            }
        }

        // Funció utilitzada per obtenir l'id del responsable
        private int ObtenirIdResponsable(string responsableNombre)
        {
            Usuaris usuari = dadesBBDD.usuaris.Find(u => u.Usuari == responsableNombre);

            // I finalment si es troba el rol a la llista retornem el seu Id
            if (usuari != null)
            {
                return usuari.Id;
            }

            return -1;
        }

        // Funció utilitzada per obtenir l'id de la prioritat
        private int ObtenirIdPrioritat(string prioritatNombre)
        {
            Prioritats prioritat = dadesBBDD.prioritats.Find(p => p.Prioritat == prioritatNombre);

            // I finalment si es troba el rol a la llista retornem el seu Id
            if (prioritat != null)
            {
                return prioritat.Id;
            }

            return -1;
        }

        // Funcio qeu executa el boto i qeu serevix per guarda la informació a les variables per despres poder executar la comanda sql
        private void GuardarButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Obtenir els valors dels Textbox on s'escriu l'infromació nova
                string titol = Titol.Text;
                DateTime datafinalitzacio = dp1.SelectedDate ?? DateTime.Now.AddDays(7);
                string descripcio = DescripcioTasca.Text;

                string prioritatNombre = prioridades.SelectedItem?.ToString();
                int idPrioritat = ObtenirIdPrioritat(prioritatNombre);

                string responsableNombre = responsables.SelectedItem?.ToString();
                int idResponsable = ObtenirIdResponsable(responsableNombre);

                ActualitzarTasca(TascaId, titol, datafinalitzacio, descripcio, idPrioritat, idResponsable);

                MessageBox.Show("Cambis guardats correctament");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar els canvis: {ex.Message}");
            }
        }

        // Funció qeu cuan es crida el que fa es executar la comanda per modificar l'infromació de la Tasca, amb els valors qeu li pasem al constructor
        private void ActualitzarTasca(int taskId, string titol, DateTime datafinalitzacio, string descripcio, int idPrioritat, int idResponsable)
        {
            string updateQuery = "UPDATE tasca SET titol = @Titol, datafinalitzacio = @DataFinalitzacio, descricpio = @Descripcio, id_prioritat = @IdPrioritat, id_responsable = @IdResponsable WHERE Id = @TaskId";

            using (SqlConnection conn = new SqlConnection(laMevaConexioString))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@TaskId", taskId);
                    cmd.Parameters.AddWithValue("@Titol", titol);
                    cmd.Parameters.AddWithValue("@DataFinalitzacio", datafinalitzacio);
                    cmd.Parameters.AddWithValue("@Descripcio", descripcio);
                    cmd.Parameters.AddWithValue("@IdPrioritat", idPrioritat);
                    cmd.Parameters.AddWithValue("@IdResponsable", idResponsable);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Funcion para mostrar los responsables en el combobox
        private void MostrarRespnsables(List<Usuaris> usuaris)
        {
            foreach (Usuaris usuari in usuaris)
            {
                responsables.Items.Add(usuari.Usuari);
            }
        }

        // Funcion para mostrar las prioridades en el combobox
        private void MostrarPrioritats(List<Prioritats> prioritats)
        {
            foreach (Prioritats prioritat in prioritats)
            {
                prioridades.Items.Add(prioritat.Prioritat);
            }
        }
    }
}
