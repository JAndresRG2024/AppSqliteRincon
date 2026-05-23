using Dapper;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace PruebaAppMovil01Rincon
{
    public class Productos
    {
        private string connectionString;
        private SqliteConnection connection;

        public Productos()
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "productos.db");
            connectionString = $"Data Source={dbPath}";
            connection = new SqliteConnection(connectionString);
            connection.Open();

            // Activar claves foráneas y esquema más correcto
            connection.Execute("PRAGMA foreign_keys = ON;");
            connection.Execute("CREATE TABLE IF NOT EXISTS productos (pro_id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                                "pro_nombre TEXT NOT NULL, " +
                                "pro_marca TEXT, " +
                                "pro_precio REAL NOT NULL, " +
                                "created_at TEXT DEFAULT (datetime('now')))");

            // Tabla movimientos con FK hacia productos y borrado en cascada
            connection.Execute("CREATE TABLE IF NOT EXISTS movimientos (mov_id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                                "pro_id INTEGER NOT NULL, " +
                                "mov_tipo TEXT NOT NULL, " +
                                "mov_cantidad INTEGER DEFAULT 0, " +
                                "mov_nota TEXT, " +
                                "mov_fecha TEXT DEFAULT (datetime('now')), " +
                                "FOREIGN KEY(pro_id) REFERENCES productos(pro_id) ON DELETE CASCADE)");
        }

        //Crear un producto
        public Producto CreateProducto(string nombre, string marca, double precio)
        {
            var nuevo = new Producto
            {
                pro_nombre = nombre,
                pro_marca = marca,
                pro_precio = precio
            };
            var cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO productos (pro_nombre, pro_marca, pro_precio) VALUES ($n, $m, $p); SELECT last_insert_rowid();";
            cmd.Parameters.AddWithValue("$n", nuevo.pro_nombre);
            cmd.Parameters.AddWithValue("$m", nuevo.pro_marca);
            cmd.Parameters.AddWithValue("$p", nuevo.pro_precio);
            var id = (long)cmd.ExecuteScalar();
            nuevo.pro_id = (int)id;
            return nuevo;
        }

        //Leer un producto
        public Producto ReadProductoById(int id)
        {
            var data = connection.Query<Producto>("SELECT * FROM productos WHERE pro_id = @pro_id", new { pro_id = id }).ToList();
            if (data.Count == 0) return null; else return data[0];
        }

        //Leer todos los productos
        public List<Producto> ReadAllProductos()
        {
            var data = connection.Query<Producto>("SELECT * FROM productos").ToList();
            return data;
        }
        //Actualizar un producto
        public void UpdateProducto(int id, string nombre, string marca, double precio)
        {
            var recordsAffected = connection.Execute("UPDATE productos SET pro_nombre = @pro_nombre, pro_marca = @pro_marca, pro_precio = @pro_precio WHERE pro_id = @pro_id", new { pro_nombre = nombre, pro_marca = marca, pro_precio = precio, pro_id = id });
            if (recordsAffected == 0) throw new Exception("No se pudo actualizar el producto");
        }

        //Eliminar un producto
        public void DeleteProducto(int id)
        {
            var recordsAffected = connection.Execute("DELETE FROM productos WHERE pro_id = @pro_id", new { pro_id = id });
            if (recordsAffected == 0) throw new Exception("No se pudo eliminar el producto");
        }

        // Movimientos (maestro-detalle) CRUD
        public Movimiento CreateMovimiento(int proId, string tipo, int cantidad, string nota)
        {
            var mov = new Movimiento
            {
                pro_id = proId,
                mov_tipo = tipo,
                mov_cantidad = cantidad,
                mov_nota = nota,
                mov_fecha = DateTime.Now
            };
            var cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO movimientos (pro_id, mov_tipo, mov_cantidad, mov_nota, mov_fecha) VALUES ($p, $t, $c, $n, $f); SELECT last_insert_rowid();";
            cmd.Parameters.AddWithValue("$p", mov.pro_id);
            cmd.Parameters.AddWithValue("$t", mov.mov_tipo);
            cmd.Parameters.AddWithValue("$c", mov.mov_cantidad);
            cmd.Parameters.AddWithValue("$n", mov.mov_nota);
            cmd.Parameters.AddWithValue("$f", mov.mov_fecha.ToString("yyyy-MM-dd HH:mm:ss"));
            var id = (long)cmd.ExecuteScalar();
            mov.mov_id = (int)id;
            return mov;
        }

        public List<Movimiento> ReadMovimientosByProducto(int proId)
        {
            var data = connection.Query<Movimiento>("SELECT * FROM movimientos WHERE pro_id = @pro_id ORDER BY mov_fecha DESC", new { pro_id = proId }).ToList();
            return data;
        }

        public void DeleteMovimiento(int movId)
        {
            var recordsAffected = connection.Execute("DELETE FROM movimientos WHERE mov_id = $id", new { id = movId });
            if (recordsAffected == 0) throw new Exception("No se pudo eliminar el movimiento");
        }

        public void UpdateMovimiento(int movId, string tipo, int cantidad, string nota)
        {
            var recordsAffected = connection.Execute("UPDATE movimientos SET mov_tipo = $t, mov_cantidad = $c, mov_nota = $n WHERE mov_id = $id", new { t = tipo, c = cantidad, n = nota, id = movId });
            if (recordsAffected == 0) throw new Exception("No se pudo actualizar el movimiento");
        }
    }
}
