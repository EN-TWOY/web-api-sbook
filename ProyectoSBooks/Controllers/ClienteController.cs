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
using Microsoft.IdentityModel.Tokens;

namespace ProyectoSBooks.Web.Controllers
{
    public class ClienteController : Controller
    {
        public readonly IConfiguration iconfig;
        public ClienteController(IConfiguration _iconfig)
        {
            iconfig = _iconfig;
        }

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
        public IActionResult Edit(ClienteDTO reg)
        {
            string mensaje = "";
            using (SqlConnection cn = new SqlConnection(iconfig["ConnectionStrings:cadena"]))
            {
                cn.Open();
                try
                {
                    SqlCommand cmd = new SqlCommand("usp_cliente_actualiza", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdCliente", reg.IdCliente);
                    cmd.Parameters.AddWithValue("@NombreCia", reg.NombreCia);
                    cmd.Parameters.AddWithValue("@Direccion", reg.Direccion);
                    cmd.Parameters.AddWithValue("@idpais", reg.idPais);
                    cmd.Parameters.AddWithValue("@Telefono", reg.Telefono);
                    cmd.Parameters.AddWithValue("@dni", reg.dni);
                    cmd.ExecuteNonQuery();
                    mensaje = $"Se actualizó correctamente el cliente {reg.NombreCia.ToUpper()}";
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

        /*
        public IActionResult Delete(int IdCliente)
        {
            string mensaje = "";
            using (SqlConnection cn = new SqlConnection(iconfig["ConnectionStrings:cadena"]))
            {
                cn.Open();
                try
                {
                    SqlCommand cmd = new SqlCommand("usp_cliente_eliminar", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdCliente", IdCliente);
                    cmd.ExecuteNonQuery();
                    mensaje = $"Se eliminó correctamente el cliente con ID {IdCliente}";
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
         */
    }
}
