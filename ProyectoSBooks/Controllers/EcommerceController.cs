using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using System.Data;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Session;
using ProyectoSBooks.Models;

namespace ProyectoSBooks.Controllers {
    public class EcommerceController : Controller {

        // Recuperar la cadena de conexion
        public readonly IConfiguration _iconfig; 

        public EcommerceController(IConfiguration iconfig) {
            _iconfig = iconfig;
        }

        IEnumerable<Producto> catalogo() {
            //ejecutar sql: idproducto,nombreproducto,nombrecategoria,
            //precioUnidad,unidadesEnExistencia de tb_productos y tb_categorias
            List<Producto> temporal = new List<Producto>();
            using (SqlConnection cn = new SqlConnection(_iconfig["ConnectionStrings:cadena"])) {
                cn.Open();
                SqlCommand cmd = new SqlCommand("Select IdProducto,NombreProducto,NombreCategoria,PrecioUnidad," +
                    "UnidadesEnExistencia, umedida from tb_productos p join tb_categorias c on p.IdCategoria = c.IdCategoria", cn);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read()) {
                    temporal.Add(new Producto() {
                        idproducto = dr.GetInt32(0),
                        descripcion = dr.GetString(1),
                        categoria = dr.GetString(2),
                        precio = dr.GetDecimal(3),
                        stock = dr.GetInt16(4),
                        umedida = dr.GetString(5)
                    });
                }
                cn.Close();
            }
            return temporal;
        }

        // Paginacion
        // p es el número de la fila
        public async Task<IActionResult> Paginacion(int p = 0) {

            /*14.2. Definir f (número de filas por página), 
             *              c (cantidad de registros), 
             *              npags (número de páginas) */
            int f = 15;
            int c = catalogo().Count();
            int npags = c % f == 0 ? c / f : c / f + 1; //es una condicional
            //condiciona que si no sale exacta la división cree una página más

            //14.3 Almaceno p y npags en ViewBag
            ViewBag.p = p;
            ViewBag.npags = npags;

            //14.4 Envío a la vista la parte de los registros contados desde una página
            return View(await Task.Run(() => catalogo().Skip(p * f).Take(f)));
        }

        public IActionResult Portal() {
            // 1.CONTINUAR: S10: Compras
            // espacio para definir el Session canasta, no no existe definir
            if (HttpContext.Session.GetString("canasta") == null) {
                HttpContext.Session.SetString("canasta",
                JsonConvert.SerializeObject(new List<Registro>()));
            }
            // enviar el catalogo a la vista
            ViewBag.mensaje = TempData["mensaje"];
            return View(catalogo());
        }

        public IActionResult Seleccionar(int id = 0) {
            //buscar por idproducto
            Producto reg = catalogo().FirstOrDefault(p => p.idproducto == id);
            if (reg == null)
                return RedirectToAction("Portal");
            else
                return View(reg);
        }

        // 3. Completar los datos de seleccionar
        [HttpPost]
        public IActionResult Seleccionar(int idproducto, int cantidad) {
            //buscar al registro de producto por idproducto
            Producto reg = catalogo().FirstOrDefault(p => p.idproducto == idproducto);

            //instanciar Registro y pasar sus datos
            Registro it = new Registro() {
                idproducto = idproducto,
                descripcion = reg.descripcion,
                categoria = reg.categoria,
                precio = reg.precio,
                cantidad = cantidad
            };

            // Deserializar el Sesion canasta y lo almaceno en temporal
            List<Registro> temporal = JsonConvert.DeserializeObject<List<Registro>>(HttpContext.Session.GetString("canasta"));
            temporal.Add(it);
            // Volver a serializar almacenando el Session
            HttpContext.Session.SetString("canasta", JsonConvert.SerializeObject(temporal));
            TempData["mensaje"] = "Has agregado el libro " + it.descripcion.ToUpper() + " a tu carrito de compras";
            // return View(reg);
            // TempData["mensaje"] = mensaje;
            return RedirectToAction("Portal");
        }

        // 4. Crear nuestro ISAR Resumen
        // 5. Crear su vista de razor: List
        public IActionResult Resumen() {
            //enviar el contenido del Sesion canasta a la vista
            return View(JsonConvert.DeserializeObject<List<Registro>>(HttpContext.Session.GetString("canasta")));
        }

        // 9. Eliminar el registro de la sesion
        public IActionResult Delete(int id, int q) {
            // Eliminar el registro del Session canasta por idproducto y cantidad
            // Seserializa
            List<Registro> registros = JsonConvert.DeserializeObject<List<Registro>>(HttpContext.Session.GetString("canasta"));

            registros.Remove(registros.Find(p => p.idproducto == id && p.cantidad == q));

            HttpContext.Session.SetString("canasta", JsonConvert.SerializeObject(registros));
            // Redireccionar hacia el Resumen
            return RedirectToAction("Resumen");
        }

        // 10. Agregar IACRESULT comprar()
        // 11. crear la vista de Comprar: Create / ir a la vista
        public IActionResult Comprar() {
            //para comprar necesito enviar un formulario para ingresar datos
            return View(new Cliente());
        }

        // 15. Parte final de comprar con http
        [HttpPost]
        public IActionResult Comprar(Cliente reg) {
            string mensaje = "";
            using (SqlConnection cn = new SqlConnection(_iconfig["ConnectionStrings:cadena"])) {
                //como vas a guardar, cabecera,detalle y actualizar stock, definir transaccion
                cn.Open();
                SqlTransaction tr = cn.BeginTransaction(IsolationLevel.Serializable);
                try {
                    //tb_pedidos: 1RA PARTE
                    SqlCommand cmd = new SqlCommand("usp_agrega_pedido", cn, tr);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@idpedido", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@dni", reg.dni);
                    cmd.Parameters.AddWithValue("@nombre", reg.nombre);
                    cmd.Parameters.AddWithValue("@email", reg.email);
                    cmd.ExecuteNonQuery();
                    int idpedido = (int)cmd.Parameters["@idpedido"].Value; //recupero el valor de @idpedido
                    //tb_pedidos_deta: 2DA PARTE
                    List<Registro> temporal = JsonConvert.DeserializeObject<List<Registro>>(
                        HttpContext.Session.GetString("canasta"));
                    // Leer cada registro de temporal
                    foreach (Registro it in temporal) {
                        cmd = new SqlCommand(
                            "exec usp_agrega_detalle @idpedido,@idproducto,@cantidad,@precio", cn, tr);
                        cmd.Parameters.AddWithValue("@idpedido", idpedido);
                        cmd.Parameters.AddWithValue("@idproducto", it.idproducto);
                        cmd.Parameters.AddWithValue("@cantidad", it.cantidad);
                        cmd.Parameters.AddWithValue("@precio", it.precio);
                        cmd.ExecuteNonQuery(); //ejecutar
                    }
                    // actualizar stock: 3RA PARTE (FINAL)
                    foreach (Registro it in temporal) {
                        cmd = new SqlCommand("exec usp_actualiza_stock @idproducto,@cant", cn, tr);
                        cmd.Parameters.AddWithValue("@idproducto", it.idproducto);
                        cmd.Parameters.AddWithValue("@cant", it.cantidad);
                        cmd.ExecuteNonQuery();
                    }
                    tr.Commit(); 
                    mensaje = $"Se ha registrado su compra correctamente Nro: {idpedido}";
                }
                catch (SqlException ex) {
                    mensaje = ex.Message;
                    tr.Rollback();
                }
                finally { cn.Close(); }
            }
            // ViewBag.mensaje = mensaje;
            // return View(reg);

            HttpContext.Session.Clear();
            HttpContext.Session.SetString("canasta", "");
            HttpContext.Session.Remove("canasta");

            TempData["mensaje"] = mensaje;
            return RedirectToAction("Portal");
        }

    }
}
