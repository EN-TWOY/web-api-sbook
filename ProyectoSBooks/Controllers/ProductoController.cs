using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using ProyectoSBooks.Models;
using System.Data;
// imports
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using Azure;
using ProyectoSBooks.Web.Models;

namespace ProyectoSBooks.Web.Controllers
{
    public class ProductoController : Controller
    {
        public readonly IConfiguration iconfig;
        public ProductoController(IConfiguration _iconfig)
        {
            iconfig = _iconfig;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.mensaje = TempData["mensaje"];
            return View(await listadoProductos());
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.categorias = new SelectList(await listadoCategorias(), "IdCategoria", "NombreCategoria");
            ViewBag.proveedores = new SelectList(await listadoProveedores(), "IdProveedor", "NombreCia");
            return View(await Task.Run(() => new ProductoDTO()));
        }

        public async Task<IActionResult> Edit(int IdProducto)
        {
            ProductoDTO productoExistente = await ObtenerLibroPorId(IdProducto);
            if (productoExistente == null)
            {
                return NotFound();
            }
            ViewBag.categorias = new SelectList(await listadoCategorias(), "IdCategoria", "NombreCategoria");
            ViewBag.proveedores = new SelectList(await listadoProveedores(), "IdProveedor", "NombreCia");
            return View(productoExistente);
        }

        async Task<List<ProductoDTO>> listadoProductos()
        {
            List<ProductoDTO> objProducto = new List<ProductoDTO>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7194/api/apiProducto/");
                HttpResponseMessage mensaje = await client.GetAsync("productos");
                string cadena = await mensaje.Content.ReadAsStringAsync();

                objProducto = JsonConvert.DeserializeObject<List<ProductoDTO>>(cadena).Select(
                    c => new ProductoDTO
                    {
                        IdProducto = c.IdProducto,
                        NombreProducto = c.NombreProducto,
                        NombreCia = c.NombreCia,
                        NombreCategoria = c.NombreCategoria,
                        umedida = c.umedida,
                        PrecioUnidad = c.PrecioUnidad,
                        UnidadesEnExistencia = c.UnidadesEnExistencia
                    }).ToList();
            }
            return objProducto;
        }

        async Task<List<Categoria>> listadoCategorias()
        {
            List<Categoria> objCategorias = new List<Categoria>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7194/api/apiCategoria/");
                HttpResponseMessage mensaje = await client.GetAsync("categorias");
                string cadena = await mensaje.Content.ReadAsStringAsync();

                objCategorias = JsonConvert.DeserializeObject<List<Categoria>>(cadena).Select(
                    s => new Categoria
                    {
                        IdCategoria = s.IdCategoria,
                        NombreCategoria = s.NombreCategoria,
                        Descripcion = s.Descripcion
                    }).ToList();
            }
            return objCategorias;
        }

        async Task<List<Proveedor>> listadoProveedores()
        {
            List<Proveedor> objProveedor = new List<Proveedor>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7194/api/apiProveedor/");
                HttpResponseMessage mensaje = await client.GetAsync("proveedores");
                string cadena = await mensaje.Content.ReadAsStringAsync();

                objProveedor = JsonConvert.DeserializeObject<List<Proveedor>>(cadena).Select(
                    c => new Proveedor
                    {
                        IdProveedor = c.IdProveedor,
                        NombreCia = c.NombreCia,
                        NombreContacto = c.NombreContacto,
                        CargoContacto = c.CargoContacto,
                        Direccion = c.Direccion,
                        idpais = c.idpais,
                        Telefono = c.Telefono,
                        Fax = c.Fax
                    }).ToList();
            }
            return objProveedor;
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductoDTO reg)
        {
            string mensaje = "";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7194/api/apiProducto/");
                StringContent contenido = new StringContent(JsonConvert.SerializeObject(reg), System.Text.Encoding.UTF8, "application/json");

                HttpResponseMessage msg = await client.PostAsync("agregar", contenido);
                mensaje = await msg.Content.ReadAsStringAsync();
            }
            TempData["mensaje"] = mensaje;
            return RedirectToAction("Index");
        }

        public async Task<ProductoDTO> ObtenerLibroPorId(int IdProducto)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7194/api/apiProducto/");
                HttpResponseMessage response = await client.GetAsync($"buscar/{IdProducto}");
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    ProductoDTO producto = JsonConvert.DeserializeObject<ProductoDTO>(json);
                    return producto;
                }
                else
                {
                    return null;
                }
            }
        }

        [HttpPost]
        public IActionResult Edit(ProductoDTO reg)
        {
            string mensaje = "";
            using (SqlConnection cn = new SqlConnection(iconfig["ConnectionStrings:cadena"]))
            {
                cn.Open();
                try
                {
                    SqlCommand cmd = new SqlCommand("usp_libro_actualiza", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdProducto", reg.IdProducto);
                    cmd.Parameters.AddWithValue("@NombreProducto", reg.NombreProducto);
                    cmd.Parameters.AddWithValue("@IdProveedor", reg.IdProveedor);
                    cmd.Parameters.AddWithValue("@IdCategoria", reg.IdCategoria);
                    cmd.Parameters.AddWithValue("@umedida", reg.umedida);
                    cmd.Parameters.AddWithValue("@PrecioUnidad", reg.PrecioUnidad);
                    cmd.Parameters.AddWithValue("@UnidadesEnExistencia", reg.UnidadesEnExistencia);
                    cmd.ExecuteNonQuery();
                    mensaje = $"Se actualizó correctamente el libro {reg.NombreProducto.ToUpper()}";
                }
                catch (SqlException ex)
                {
                    mensaje = ex.Message;
                }
                finally
                {
                    cn.Close();
                }
            }
            TempData["mensaje"] = mensaje;
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int IdProducto)
        {
            try
            {
                string mensaje = "";
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:7194/api/");

                    HttpResponseMessage msg = await client.DeleteAsync($"apiProducto/eliminar?IdProducto={IdProducto}");
                    if (msg.IsSuccessStatusCode)
                    {
                        mensaje = $"Se eliminó correctamente el libro con ID {IdProducto}";
                    }
                    else
                    {
                        mensaje = $"Error al eliminar el libro. Detalles: {msg.ReasonPhrase}";
                    }
                }

                TempData["mensaje"] = mensaje;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["mensaje"] = "Error al eliminar el libro. Detalles: " + ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}
