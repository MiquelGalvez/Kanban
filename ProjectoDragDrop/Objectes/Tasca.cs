using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ProjectoDragDrop.Objectes
{
    public class Tasca
    {
        private int _id;
        private string _titol;
        private string _descripcio;
        private DateTime _datacreacio;
        private DateTime _datafinalitzacio;
        private string _id_responsable;
        private string _id_prioritat;
        private string _id_estat;
        private Brush prioritatBackground;


        public int Id { get => _id; set => _id = value; }
        public string Titol { get => _titol; set => _titol = value; }
        public string Descripcio { get => _descripcio; set => _descripcio = value; }
        public DateTime Datacreacio { get => _datacreacio; set => _datacreacio = value; }
        public DateTime Datafinalitzacio { get => _datafinalitzacio; set => _datafinalitzacio = value; }
        public string Id_responsable { get => _id_responsable; set => _id_responsable = value; }
        public string Id_Prioritat { get => _id_prioritat; set => _id_prioritat = value; }
        public string Id_estat { get => _id_estat; set => _id_estat = value; }
        public Brush PrioritatBackground { get => prioritatBackground; set => prioritatBackground = value; }
    }
}
