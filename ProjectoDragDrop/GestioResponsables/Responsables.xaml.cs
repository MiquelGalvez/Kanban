using ProjectoDragDrop.EditarResponsables;
using ProjectoDragDrop.FormulariCrearResponsable;
using ProjectoDragDrop.Objectes;
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

namespace ProjectoDragDrop.GestioResponsables
{
    /// <summary>
    /// Lógica de interacción para Responsables.xaml
    /// </summary>
    public partial class Responsables : Window
    {
        SqlConnection LaMevaConnexioSQL;
        private MainWindow mainWindow;
        private string laMevaConnexio;

        private Usuaris usuarioSeleccionado;

        private bool mostrarContraseñaReal = false;

        // Crear una lista para almacenar objetos de tipo Tasca
        List<Usuaris> usuaris = new List<Usuaris>();
        public Responsables(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            laMevaConnexio = ConfigurationManager.ConnectionStrings["ProjectoDragDrop.Properties.Settings.kanbanConnectionString"].ConnectionString;
            LaMevaConnexioSQL = new SqlConnection(laMevaConnexio);

            // Call the function
            MostrarTasquesPerEstat(responsables, mostrarContraseñaReal);
        }

        private void registrarUsuari_Click(object sender, RoutedEventArgs e)
        {
            AfegirRespon afegirRespon = new AfegirRespon(false);
            afegirRespon.Closed += AfegirRespon_Closed;
            afegirRespon.Show();
        }

        public void MostrarTasquesPerEstat(ListBox listBox, bool mostrarContraseñaReal)
        {
            string columnaContraseña = mostrarContraseñaReal ? "password" : "LEFT(REPLICATE('*', 8) + password, 8) AS passmask";
            // Incluir el nuevo campo en la consulta SQL
            string consulta = $"SELECT *, {columnaContraseña}, password AS PasswordPlainText FROM Usuaris";

            usuaris = new List<Usuaris>();

            using (SqlConnection conexion = new SqlConnection(laMevaConnexio))
            {
                SqlCommand comando = new SqlCommand(consulta, conexion);

                try
                {
                    conexion.Open();
                    SqlDataReader lector = comando.ExecuteReader();

                    while (lector.Read())
                    {
                        Usuaris usuari = new Usuaris
                        {
                            Id = Convert.ToInt32(lector["Id"]),
                            Nom = lector["nom"].ToString(),
                            Usuari = lector["usuari"].ToString(),
                            Email = lector["email"].ToString(),
                            Password = lector["passmask"].ToString(),
                            Id_rol = ObtenirIdRol(Convert.ToInt32(lector["id_rol"])),
                            PasswordPlainText = lector["PasswordPlainText"].ToString()  // Nuevo campo
                        };
                        usuaris.Add(usuari);
                    }
                    lector.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al obtener los usuarios desde la base de datos: " + ex.Message);
                }
            }
            listBox.ItemsSource = usuaris;
        }

        private string ObtenirIdRol(int id_rol)
        {
            string consulta = "SELECT rol FROM Rol WHERE Id = @id_rol";

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ProjectoDragDrop.Properties.Settings.kanbanConnectionString"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(consulta, conn))
                {
                    cmd.Parameters.AddWithValue("id_rol", id_rol);
                    conn.Open();
                    return (string)cmd.ExecuteScalar();
                }
            }
        }

        private void VerPassword_Click(object sender, RoutedEventArgs e)
        {
            Button verPasswordButton = (Button)sender;
            Usuaris selectedUser = (Usuaris)verPasswordButton.DataContext;

            if (selectedUser != null)
            {
                // Alternar entre mostrar y ocultar la contraseña al hacer clic
                if (mostrarContraseñaReal)
                {
                    // Si ya se están mostrando las contraseñas reales, ocultarlas
                    selectedUser.Password = "********";
                }
                else
                {
                    // Si se están enmascarando las contraseñas, mostrar la real
                    selectedUser.Password = selectedUser.PasswordPlainText;
                }

                // Alternar el estado de mostrarContraseñaReal
                mostrarContraseñaReal = !mostrarContraseñaReal;

                // Actualizar el ListBox para reflejar el cambio en la contraseña
                responsables.Items.Refresh();
            }
        }

        private void AfegirRespon_Closed(object sender, EventArgs e)
        {
            // Actualizar la lista de usuarios en el ListBox después de que se cierra el formulario
            MostrarTasquesPerEstat(responsables, mostrarContraseñaReal);
        }

        private void Eliminar_Click(object sender, RoutedEventArgs e)
        {
            // Obtener el botón "Eliminar" que disparó el evento
            Button eliminarButton = (Button)sender;

            // Obtener el usuario asociado al botón "Eliminar"
            Usuaris usuarioAEliminar = (Usuaris)eliminarButton.DataContext;

            // Verificar si el usuario tiene el rol de superadmin (id_rol = 1)
            if (usuarioAEliminar != null && usuarioAEliminar.Id_rol != "SuperAdmin")
            {
                // Eliminar el usuario de la lista
                usuaris.Remove(usuarioAEliminar);

                // Actualizar el ListBox
                responsables.ItemsSource = null;
                responsables.ItemsSource = usuaris;

                // Eliminar el usuario de la base de datos
                EliminarUsuarioDeBaseDeDatos(usuarioAEliminar);
            }
            else
            {
                MessageBox.Show("No pots eliminar un usuari amb Rol de SuperAdmin.");
            }
        }

        private void EliminarUsuarioDeBaseDeDatos(Usuaris usuario)
        {
            string consulta = "DELETE FROM Usuaris WHERE Id = @Id";

            using (SqlConnection conexion = new SqlConnection(laMevaConnexio))
            {
                SqlCommand comando = new SqlCommand(consulta, conexion);
                comando.Parameters.AddWithValue("@Id", usuario.Id);

                try
                {
                    conexion.Open();
                    comando.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al eliminar el usuario de la base de datos: " + ex.Message);
                }
            }
        }

        private void Editar_Click(object sender, RoutedEventArgs e)
        {
            // Obtener el botón "Editar" que disparó el evento
            Button editarButton = (Button)sender;

            // Obtener el usuario asociado al botón "Editar"
            usuarioSeleccionado = (Usuaris)editarButton.DataContext;

            if (usuarioSeleccionado != null)
            {
                EditarRespon editarRespon = new EditarRespon(usuarioSeleccionado, true);
                editarRespon.Closed += AfegirRespon_Closed;
                editarRespon.Show();
            }
        }
    }
}
