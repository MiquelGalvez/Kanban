using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectoDragDrop.Objectes
{
    internal class Tasca
    {
        private int _id;
        private string _titol;
        private string _descripcio;
        private DateTime _datacreacio;
        private DateTime _datafinalitzacio;
        private int _id_responsable;
        private int _id_rol;
        private int _id_estat;

        public int Id { get => _id; set => _id = value; }
        public string Titol { get => _titol; set => _titol = value; }
        public string Descripcio { get => _descripcio; set => _descripcio = value; }
        public DateTime Datacreacio { get => _datacreacio; set => _datacreacio = value; }
        public DateTime Datafinalitzacio { get => _datafinalitzacio; set => _datafinalitzacio = value; }
        public int Id_responsable { get => _id_responsable; set => _id_responsable = value; }
        public int Id_rol { get => _id_rol; set => _id_rol = value; }
        public int Id_estat { get => _id_estat; set => _id_estat = value; }
    }
}
