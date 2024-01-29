using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectoDragDrop.Objectes
{
    internal class Usuaris
    {
        private int _id;
        private string _nom;
        private string _email;
        private string _password;
        private int _id_rol;

        public int Id { get => _id; set => _id = value; }
        public string Nom { get => _nom; set => _nom = value; }
        public string Email { get => _email; set => _email = value; }
        public string Password { get => _password; set => _password = value; }
        public int Id_rol { get => _id_rol; set => _id_rol = value; }
    }
}
