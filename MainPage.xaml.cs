namespace PruebaAppMovil01Rincon
{
    public partial class MainPage : ContentPage
    {
        private Productos productos;

        public MainPage()
        {
            InitializeComponent();
            productos = new Productos();
            CargarProductos();
        }

        private void cmdCrear_Clicked(object sender, EventArgs e)
        {
            var precio = double.Parse(string.IsNullOrWhiteSpace(txtPrecio.Text) ? "0" : txtPrecio.Text);
            var nuevo = productos.CreateProducto(txtNombre.Text, txtMarca.Text, precio);
            DisplayAlertAsync("Producto creado", "Se ha creado el producto", "OK");
            CargarProductos();
        }

        private void cmdLeer_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtId.Text)) return;
            var prod = productos.ReadProductoById(int.Parse(txtId.Text));
            if (prod != null)
            {
                txtNombre.Text = prod.pro_nombre;
                txtMarca.Text = prod.pro_marca;
                txtPrecio.Text = prod.pro_precio.ToString();
            }
            else
            {
                DisplayAlertAsync("Producto no encontrado", "No se encontró el producto con el ID proporcionado.", "OK");
            }
        }
        private void cmdActualizar_Clicked(object sender, EventArgs e)
        {
            try
            {
                productos.UpdateProducto(int.Parse(txtId.Text), txtNombre.Text, txtMarca.Text, double.Parse(txtPrecio.Text));
                DisplayAlertAsync("Producto actualizado", "Se ha actualizado el producto correctamente.", "OK");
                CargarProductos();
            }
            catch (Exception ex)
            {
                DisplayAlertAsync("Error", $"Ocurrió un error al actualizar el producto: {ex.Message}", "OK");
            }
        }

        private void cmdBorrar_Clicked(object sender, EventArgs e)
        {
            try
            {
                productos.DeleteProducto(int.Parse(txtId.Text));
                txtId.Text = "";
                txtNombre.Text = "";
                txtMarca.Text = "";
                txtPrecio.Text = "";

                DisplayAlertAsync("Producto eliminado", "Se ha eliminado el producto correctamente.", "OK");
                CargarProductos();
            }
            catch(Exception ex)
            {
                DisplayAlertAsync("Error", $"Ocurrió un error al eliminar el producto: {ex.Message}", "OK");
            }

        }

        private void CargarProductos()
        {
            var lista = productos.ReadAllProductos();
            lvProductos.ItemsSource = lista;
        }

        private void LvProductos_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null) return;
            var seleccionado = (Producto)e.SelectedItem;
            txtId.Text = seleccionado.pro_id.ToString();
            txtNombre.Text = seleccionado.pro_nombre;
            txtMarca.Text = seleccionado.pro_marca;
            txtPrecio.Text = seleccionado.pro_precio.ToString();

            // Cargar movimientos asociados
            var movimientos = productos.ReadMovimientosByProducto(seleccionado.pro_id);
            lvMovimientos.ItemsSource = movimientos;
            // Limpiar selección y campos de movimiento
            lvMovimientos.SelectedItem = null;
            txtMovCantidad.Text = "";
            txtMovNota.Text = "";
            pickerMovTipo.SelectedIndex = -1;
        }

        private void CmdCrearMov_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtId.Text))
            {
                DisplayAlertAsync("Error", "Seleccione primero un producto para agregar movimientos.", "OK");
                return;
            }

            var proId = int.Parse(txtId.Text);
            var tipo = pickerMovTipo.SelectedItem?.ToString() ?? "entrada";
            var cantidad = int.Parse(string.IsNullOrWhiteSpace(txtMovCantidad.Text) ? "0" : txtMovCantidad.Text);
            var nota = txtMovNota.Text;

            productos.CreateMovimiento(proId, tipo, cantidad, nota);
            txtMovCantidad.Text = "";
            txtMovNota.Text = "";
            pickerMovTipo.SelectedIndex = -1;

            // Refrescar lista de movimientos
            var movimientos = productos.ReadMovimientosByProducto(proId);
            lvMovimientos.ItemsSource = movimientos;
        }

        private void LvMovimientos_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null) return;
            var seleccionado = (Movimiento)e.SelectedItem;
            pickerMovTipo.SelectedItem = seleccionado.mov_tipo;
            txtMovCantidad.Text = seleccionado.mov_cantidad.ToString();
            txtMovNota.Text = seleccionado.mov_nota;
        }

        private void CmdActualizarMov_Clicked(object sender, EventArgs e)
        {
            if (lvMovimientos.SelectedItem == null)
            {
                DisplayAlertAsync("Error", "Seleccione primero un movimiento para actualizar.", "OK");
                return;
            }

            var seleccionado = (Movimiento)lvMovimientos.SelectedItem;
            var tipo = pickerMovTipo.SelectedItem?.ToString() ?? "entrada";
            var cantidad = int.Parse(string.IsNullOrWhiteSpace(txtMovCantidad.Text) ? "0" : txtMovCantidad.Text);
            var nota = txtMovNota.Text;

            try
            {
                productos.UpdateMovimiento(seleccionado.mov_id, tipo, cantidad, nota);
                var movimientos = productos.ReadMovimientosByProducto(int.Parse(txtId.Text));
                lvMovimientos.ItemsSource = movimientos;
                DisplayAlertAsync("Movimiento actualizado", "Se ha actualizado el movimiento.", "OK");
            }
            catch (Exception ex)
            {
                DisplayAlertAsync("Error", $"Ocurrió un error al actualizar el movimiento: {ex.Message}", "OK");
            }
        }

        private void CmdBorrarMov_Clicked(object sender, EventArgs e)
        {
            if (lvMovimientos.SelectedItem == null)
            {
                DisplayAlertAsync("Error", "Seleccione primero un movimiento para eliminar.", "OK");
                return;
            }

            var seleccionado = (Movimiento)lvMovimientos.SelectedItem;
            try
            {
                productos.DeleteMovimiento(seleccionado.mov_id);
                var movimientos = productos.ReadMovimientosByProducto(int.Parse(txtId.Text));
                lvMovimientos.ItemsSource = movimientos;
                txtMovCantidad.Text = "";
                txtMovNota.Text = "";
                pickerMovTipo.SelectedIndex = -1;
                DisplayAlertAsync("Movimiento eliminado", "Se ha eliminado el movimiento.", "OK");
            }
            catch (Exception ex)
            {
                DisplayAlertAsync("Error", $"Ocurrió un error al eliminar el movimiento: {ex.Message}", "OK");
            }
        }
    }
}
