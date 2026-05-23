using System;
using System.Collections.Generic;
using System.Text;

namespace PruebaAppMovil01Rincon
{
    public class Producto
    {
        public int pro_id { get; set; }
        public string pro_nombre { get; set; }
        public string pro_marca { get; set; }
        public double pro_precio { get; set; }

        // Movimientos asociados (maestro-detalle)
        public List<Movimiento> Movimientos { get; set; }
    }
}
