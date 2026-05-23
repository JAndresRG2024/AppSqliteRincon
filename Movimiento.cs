using System;
using System.Collections.Generic;
using System.Text;

namespace PruebaAppMovil01Rincon
{
    public class Movimiento
    {
        public int mov_id { get; set; }
        public int pro_id { get; set; }
        public string mov_tipo { get; set; } // "entrada" o "salida"
        public int mov_cantidad { get; set; }
        public string mov_nota { get; set; }
        public DateTime mov_fecha { get; set; }
    }
}
