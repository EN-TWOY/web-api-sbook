using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProyectoSBooks.Controllers;
using ProyectoSBooks.Models;
using ProyectoSBooks.Web.Models;

namespace ProyectoSBooks.Web.Controllers
{
    public class MenuController : Controller
    {

        public async Task<IActionResult> Index()
        {
            // Categorias
            int cantidadCategorias = await CategoriaHelper.ContarCategorias();
            ViewBag.CantidadCategorias = cantidadCategorias;

            // Clientes
            int cantidadClientes = await ClienteHelper.ContarClientes();
            ViewBag.CantidadClientes = cantidadClientes;

            // Libros
            int cantidadLibros = await LibroHelper.ContarLibros();
            ViewBag.CantidadLibros = cantidadLibros;

            // Empleados
            int cantidadEmpleados = await EmpleadoHelper.ContarEmpleados();
            ViewBag.CantidadEmpleados = cantidadEmpleados;

            // Proveedores
            int cantidadProveedores = await ProveedorHelper.ContarProveedores();
            ViewBag.CantidadProveedores = cantidadProveedores;

            // Venta

            // Detalle Venta

            return View();
        }

        public static class CategoriaHelper
        {
            public static async Task<int> ContarCategorias()
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:7194/api/apiCategoria/");
                    HttpResponseMessage mensaje = await client.GetAsync("categorias");
                    string cadena = await mensaje.Content.ReadAsStringAsync();

                    List<Categoria> categorias = JsonConvert.DeserializeObject<List<Categoria>>(cadena);
                    int cantidadCategorias = categorias.Count;
                    return cantidadCategorias;
                }
            }
        }

        public static class ClienteHelper
        {
            public static async Task<int> ContarClientes()
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:7194/api/apiCliente/");
                    HttpResponseMessage mensaje = await client.GetAsync("clientes");
                    string cadena = await mensaje.Content.ReadAsStringAsync();

                    List<Cliente> clientes = JsonConvert.DeserializeObject<List<Cliente>>(cadena);
                    int cantidadClientes = clientes.Count;
                    return cantidadClientes;
                }
            }
        }

        public static class LibroHelper
        {
            public static async Task<int> ContarLibros()
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:7194/api/apiProducto/");
                    HttpResponseMessage mensaje = await client.GetAsync("productos");
                    string cadena = await mensaje.Content.ReadAsStringAsync();

                    List<ProductoDTO> libros = JsonConvert.DeserializeObject<List<ProductoDTO>>(cadena);
                    int cantidadLibros = libros.Count;
                    return cantidadLibros;
                }
            }
        }

        public static class EmpleadoHelper
        {
            public static async Task<int> ContarEmpleados()
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:7194/api/apiEmpleado/");
                    HttpResponseMessage mensaje = await client.GetAsync("empleados");
                    string cadena = await mensaje.Content.ReadAsStringAsync();

                    List<Empleado> empleados = JsonConvert.DeserializeObject<List<Empleado>>(cadena);
                    int cantidadEmpleados = empleados.Count;
                    return cantidadEmpleados;
                }
            }
        }

        public static class ProveedorHelper
        {
            public static async Task<int> ContarProveedores()
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:7194/api/apiProveedor/");
                    HttpResponseMessage mensaje = await client.GetAsync("proveedores");
                    string cadena = await mensaje.Content.ReadAsStringAsync();

                    List<Proveedor> proveedores = JsonConvert.DeserializeObject<List<Proveedor>>(cadena);
                    int cantidadProveedores = proveedores.Count;
                    return cantidadProveedores;
                }
            }
        }
    }
}
