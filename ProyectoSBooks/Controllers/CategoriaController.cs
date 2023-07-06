using Microsoft.AspNetCore.Mvc;
using ProyectoSBooks.Models;
using System.Data;
// imports
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace ProyectoSBooks.Controllers
{
    public class CategoriaController : Controller {

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
        public async Task<IActionResult> Edit(int IdCategoria, Categoria reg)
        {
            try
            {
                string mensaje = "";
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:7194/api/");
                    string jsonData = JsonConvert.SerializeObject(reg);
                    var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                    HttpResponseMessage msg = await client.PutAsync($"apiCategoria/actualizar?IdCategoria={IdCategoria}", content);
                    if (msg.StatusCode == HttpStatusCode.OK)
                    {
                        mensaje = $"Se actualizó correctamente la categoria con ID {IdCategoria}";
                    }
                    else
                    {
                        mensaje = $"Error al actualizar la categoria. Detalles: {msg.ReasonPhrase}";
                    }
                }

                TempData["mensaje"] = mensaje;
                return RedirectToAction("Listado");
            }
            catch (Exception ex)
            {
                TempData["mensaje"] = "Error al actualizar la categoria. Detalles: " + ex.Message;
                return RedirectToAction("Listado");
            }
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
