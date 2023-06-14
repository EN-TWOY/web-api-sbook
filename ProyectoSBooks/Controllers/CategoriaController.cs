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

namespace ProyectoSBooks.Controllers
{
    public class CategoriaController : Controller {

        public readonly IConfiguration iconfig;
        public CategoriaController(IConfiguration _iconfig) {
            iconfig = _iconfig;
        }

        public async Task<IActionResult> listado()
        {
            ViewBag.mensaje = TempData["mensaje"];
            return View(await listadoCategorias());
        }

        public async Task<IActionResult> Create()
        {
            return View(await Task.Run(() => new Categoria()));
        }

        public async Task<IActionResult> Edit(int IdCategoria)
        {
            Categoria categoriaExistente = await ObtenerCategoriaPorId(IdCategoria);
            if (categoriaExistente == null)
            {
                return NotFound();
            }
            return View(categoriaExistente);
        }

        async Task<List<Categoria>> listadoCategorias()
        {
            List<Categoria> objCategorias = new List<Categoria>();
            using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7194/api/apiCategoria/");
                HttpResponseMessage mensaje = await client.GetAsync("categorias");
                string cadena=await mensaje.Content.ReadAsStringAsync();

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

        [HttpPost]
        public async Task<IActionResult> Create(Categoria reg)
        {
            string mensaje = "";
            using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7194/api/apiCategoria/");
                StringContent contenido = new StringContent(JsonConvert.SerializeObject(reg), System.Text.Encoding.UTF8, "application/json");

                HttpResponseMessage msg = await client.PostAsync("agregar", contenido);
                mensaje = await msg.Content.ReadAsStringAsync();
            }
            TempData["mensaje"] = mensaje;
            return RedirectToAction("listado");
        }

        public async Task<Categoria> ObtenerCategoriaPorId(int IdCategoria)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7194/api/apiCategoria/");
                HttpResponseMessage response = await client.GetAsync($"buscar/{IdCategoria}");
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    Categoria categoria = JsonConvert.DeserializeObject<Categoria>(json);
                    return categoria;
                }
                else
                {
                    return null;
                }
            }
        }

        [HttpPost]
        public IActionResult Edit(Categoria reg)
        {
            string mensaje = "";
            using (SqlConnection cn = new SqlConnection(iconfig["ConnectionStrings:cadena"]))
            {
                cn.Open();
                try
                {
                    SqlCommand cmd = new SqlCommand("usp_categoria_actualiza", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@idcategoria", reg.IdCategoria);
                    cmd.Parameters.AddWithValue("@nombre", reg.NombreCategoria);
                    cmd.Parameters.AddWithValue("@descripcion", reg.Descripcion);
                    cmd.ExecuteNonQuery();
                    mensaje = $"Se actualizó correctamente la categoria {reg.NombreCategoria.ToUpper()}";
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
            return RedirectToAction("Listado");
        }

        public async Task<IActionResult> Delete(int IdCategoria)
        {
            try
            {
                string mensaje = "";
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:7194/api/");

                    HttpResponseMessage msg = await client.DeleteAsync($"apiCategoria/eliminar?IdCategoria={IdCategoria}");
                    if (msg.IsSuccessStatusCode)
                    {
                        mensaje = $"Se eliminó correctamente la categoria con ID {IdCategoria}";
                    }
                    else
                    {
                        mensaje = $"Error al eliminar el cliente. Detalles: {msg.ReasonPhrase}";
                    }
                }

                TempData["mensaje"] = mensaje;
                return RedirectToAction("Listado");
            }
            catch (Exception ex)
            {
                TempData["mensaje"] = "Error al eliminar el cliente. Detalles: " + ex.Message;
                return RedirectToAction("Listado");
            }
        }

    }
}
