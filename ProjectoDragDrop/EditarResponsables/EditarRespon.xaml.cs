using ProjectoDragDrop.Objectes;
using System;
using System.Collections.Generic;
using System.Configuration;
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
using System.Windows.Shapes;

namespace ProjectoDragDrop.EditarResponsables
{
    /// <summary>
    /// Lógica de interacción para EditarRespon.xaml
    /// </summary>
    public partial class EditarRespon : Window
    {
        SqlConnection LaMevaConnexioSQL;
        private MainWindow mainWindow;
        private string laMevaConnexio;
        private bool openedFromLogin;
        private Usuaris usuario;


        public EditarRespon(Usuaris usuario, bool openedFromLogin = false)
        {
            InitializeComponent();
            this.usuario = usuario;
            this.openedFromLogin = openedFromLogin;
            laMevaConnexio = ConfigurationManager.ConnectionStrings["ProjectoDragDrop.Properties.Settings.kanbanConnectionString"].ConnectionString;
            LaMevaConnexioSQL = new SqlConnection(laMevaConnexio);
            MostrarDatosUsuario();
        }

        private void MostrarDatosUsuario()
        {
            // Llenar los campos del formulario con los datos del usuario
            Nom.Text = usuario.Nom;
            User.Text = usuario.Usuari;
            Password.Text = usuario.PasswordPlainText;
            Email.Text = usuario.Email;

            // Llenar el ComboBox con los roles disponibles
            MostrarRols();

            // Establecer el rol seleccionado en el ComboBox
            rols.SelectedItem = usuario.Id_rol;

            // Bloquear el ComboBox si el usuario es SuperAdmin
            if (EsAdministrador(usuario.Id_rol))
            {
                rols.IsEnabled = false;
            }

            // También puedes ajustar otros campos según sea necesario
        }

        private void MostrarRols()
        {
            string consulta = "SELECT rol FROM Rol WHERE Id > 0;";

            SqlDataAdapter elMeuAdaptador = new SqlDataAdapter(consulta, LaMevaConnexioSQL);
            using (elMeuAdaptador)
            {
                DataTable dt = new DataTable();
                elMeuAdaptador.Fill(dt);

                // Limpiar el ComboBox antes de agregar elementos
                rols.Items.Clear();

                // Por cada rol, añadir al ComboBox (excepto SuperAdmin si ya hay un SuperAdmin)
                foreach (DataRow row in dt.Rows)
                {
                    string rol = row["rol"].ToString();

                    // Deshabilitar SuperAdmin para todos excepto para el usuario existente con ese rol
                    if (rol == "SuperAdmin" && !EsSuperAdministradorExistente())
                    {
                        rols.Items.Add(rol);
                    }
                    else if (rol != "SuperAdmin")
                    {
                        rols.Items.Add(rol);
                    }
                }
            }
        }

        private bool EsSuperAdministradorExistente()
        {
            // Lógica para determinar si ya existe un superadministrador en la base de datos
            // Puedes consultar la base de datos o utilizar cualquier otro criterio
            // En este ejemplo, asumimos que ya existe un superadministrador
            return true;  // Cambia esta lógica según tu implementación real
        }

        private bool EsAdministrador(string idRol)
        {
            // Lógica para determinar si el rol es un administrador
            // Puedes utilizar la información que tengas en la base de datos o cualquier otro criterio
            // Aquí asumimos que "Admin" es el rol de administrador
            return idRol == "SuperAdmin";
        }

        private void GuardarEdicion_Click(object sender, RoutedEventArgs e)
        {
            // Recopilar los datos editados del formulario
            string nuevoNom = Nom.Text;
            string nuevoUser = User.Text;
            string nuevaPassword = Password.Text; // Asegúrate de manejar las contraseñas de manera segura en una aplicación real
            string nuevoEmail = Email.Text;
            string nuevoRol = rols.SelectedItem.ToString();

            // Actualizar los registros en la base de datos con los nuevos datos
            ActualizarDatosEnBaseDeDatos(nuevoNom, nuevoUser, nuevaPassword, nuevoEmail, nuevoRol);

            // Puedes mostrar un mensaje o realizar otras acciones después de guardar los cambios
            MessageBox.Show("Dades guardades correctament.");
            this.Close();
        }
        private void ActualizarDatosEnBaseDeDatos(string nuevoNom, string nuevoUser, string nuevaPassword, string nuevoEmail, string nuevoRol)
        {
            // Realizar la lógica para actualizar los datos en la base de datos
            // Utiliza parámetros y consultas SQL seguras para evitar inyecciones de SQL
            string consulta = "UPDATE Usuaris SET nom = @nom, usuari = @usuari, password = @password, email = @email, id_rol = @id_rol WHERE Id = @Id";

            using (SqlConnection conexion = new SqlConnection(laMevaConnexio))
            {
                SqlCommand comando = new SqlCommand(consulta, conexion);
                comando.Parameters.AddWithValue("@nom", nuevoNom);
                comando.Parameters.AddWithValue("@usuari", nuevoUser);
                comando.Parameters.AddWithValue("@password", nuevaPassword);
                comando.Parameters.AddWithValue("@email", nuevoEmail);
                comando.Parameters.AddWithValue("@id_rol", ObtenerIdRolPorNombre(nuevoRol));
                comando.Parameters.AddWithValue("@Id", usuario.Id);

                try
                {
                    conexion.Open();
                    comando.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al actualizar los datos en la base de datos: " + ex.Message);
                }
            }
        }
        private int ObtenerIdRolPorNombre(string nombreRol)
        {
            // Lógica para obtener el Id del rol basado en su nombre
            // Puedes usar una consulta SQL u otro método según la estructura de tu base de datos
            string consulta = "SELECT Id FROM Rol WHERE rol = @rol";

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ProjectoDragDrop.Properties.Settings.kanbanConnectionString"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(consulta, conn))
                {
                    cmd.Parameters.AddWithValue("@rol", nombreRol);
                    conn.Open();
                    return (int)cmd.ExecuteScalar();
                }
            }
        }
    }
}
