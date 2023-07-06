using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
// imports
using Newtonsoft.Json;
using System.Text;
using ProyectoSBooks.Web.Models;
using System.Net;

namespace ProyectoSBooks.Web.Controllers
{
    public class ClienteController : Controller
    {
        public async Task<IActionResult> Index()
        {
            ViewBag.mensaje = TempData["mensaje"];
            return View(await listadoClientes());
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.paises = new SelectList(await listadoPaises(), "Idpais", "NombrePais");
            return View(await Task.Run(() => new ClienteDTO()));
        }

        public async Task<IActionResult> Edit(int IdCliente)
        {
            ClienteDTO clienteExistente = await ObtenerClientePorId(IdCliente);
            if (clienteExistente == null)
            {
                return NotFound();
            }
            ViewBag.paises = new SelectList(await listadoPaises(), "Idpais", "NombrePais");
            return View(clienteExistente);
        }

        async Task<List<ClienteDTO>> listadoClientes()
        {
            List<ClienteDTO> objCliente = new List<ClienteDTO>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7194/api/apiCliente/");
                HttpResponseMessage mensaje = await client.GetAsync("clientes");
                string cadena = await mensaje.Content.ReadAsStringAsync();

                objCliente = JsonConvert.DeserializeObject<List<ClienteDTO>>(cadena).Select(
                    c => new ClienteDTO
                    {
                        IdCliente = c.IdCliente,
                        NombreCia = c.NombreCia,
                        Direccion = c.Direccion,
                        NombrePais = c.NombrePais,
                        Telefono = c.Telefono,     
                        dni = c.dni
                    }).ToList();
            }
            return objCliente;
        }

        async Task<List<Pais>> listadoPaises()
        {
            List<Pais> objPais = new List<Pais>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7194/api/apiPais/");
                HttpResponseMessage mensaje = await client.GetAsync("paises");
                string cadena = await mensaje.Content.ReadAsStringAsync();

                objPais = JsonConvert.DeserializeObject<List<Pais>>(cadena).Select(
                    s => new Pais
                    {
                        Idpais = s.Idpais,
                        NombrePais = s.NombrePais
                    }).ToList();
            }
            return objPais;
        }

        [HttpPost]
        public async Task<IActionResult> Create(ClienteDTO reg)
        {
            string mensaje = "";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7194/api/apiCliente/");
                StringContent contenido = new StringContent(JsonConvert.SerializeObject(reg), System.Text.Encoding.UTF8, "application/json");

                HttpResponseMessage msg = await client.PostAsync("agregar", contenido);
                mensaje = await msg.Content.ReadAsStringAsync();
            }
            TempData["mensaje"] = mensaje;
            return RedirectToAction("Index");
        }

        public async Task<ClienteDTO> ObtenerClientePorId(int IdCliente)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7194/api/apiCliente/");
                HttpResponseMessage response = await client.GetAsync($"buscar/{IdCliente}");
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    ClienteDTO cliente = JsonConvert.DeserializeObject<ClienteDTO>(json);
                    return cliente;
                }
                else
                {
                    return null;
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int IdCliente, ClienteDTO reg)
        {
            try
            {
                string mensaje = "";
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:7194/api/");
                    string jsonData = JsonConvert.SerializeObject(reg);
                    var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                    HttpResponseMessage msg = await client.PutAsync($"apiCliente/actualizar?IdCliente={IdCliente}", content);
                    if (msg.StatusCode == HttpStatusCode.OK)
                    {
                        mensaje = $"Se actualizó correctamente el cliente con ID {IdCliente}";
                    }
                    else
                    {
                        mensaje = $"Error al actualizar el cliente. Detalles: {msg.ReasonPhrase}";
                    }
                }

                TempData["mensaje"] = mensaje;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["mensaje"] = "Error al actualizar el cliente. Detalles: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> Delete(int IdCliente)
        {
            try
            {
                string mensaje = "";
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:7194/api/");

                    HttpResponseMessage msg = await client.DeleteAsync($"apiCliente/eliminar?IdCliente={IdCliente}");
                    if (msg.IsSuccessStatusCode)
                    {
                        mensaje = $"Se eliminó correctamente el cliente con ID {IdCliente}";
                    }
                    else
                    {
                        mensaje = $"Error al eliminar el cliente. Detalles: {msg.ReasonPhrase}";
                    }
                }

                TempData["mensaje"] = mensaje;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["mensaje"] = "Error al eliminar el cliente. Detalles: " + ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}
