using Microsoft.AspNetCore.Mvc;
using System.Data;
// imports
using Newtonsoft.Json;
using System.Text;
using ProyectoSBooks.Web.Models;
using System.Net;

namespace ProyectoSBooks.Web.Controllers

{
    public class EmpleadoController : Controller
    {
        public async Task<IActionResult> Index()
        {
            ViewBag.mensaje = TempData["mensaje"];
            return View(await listadoEmpleados());
        }

        public async Task<IActionResult> Create()
        {
            return View(await Task.Run(() => new Empleado()));
        }

        public async Task<IActionResult> Edit(int IdEmpleado)
        {
            Empleado empleadoExistente = await ObtenerEmpleadoPorId(IdEmpleado);
            if (empleadoExistente == null)
            {
                return NotFound();
            }
            return View(empleadoExistente);
        }

        async Task<List<Empleado>> listadoEmpleados()
        {
            List<Empleado> objEmpleado = new List<Empleado>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7194/api/apiEmpleado/");
                HttpResponseMessage mensaje = await client.GetAsync("empleados");
                string cadena = await mensaje.Content.ReadAsStringAsync();

                objEmpleado = JsonConvert.DeserializeObject<List<Empleado>>(cadena).Select(
                    c => new Empleado
                    {
                        IdEmpleado = c.IdEmpleado,
                        ApeEmpleado = c.ApeEmpleado,
                        NomEmpleado = c.NomEmpleado,
                        FecNac = c.FecNac,
                        DirEmpleado = c.DirEmpleado,
                        idDistrito = c.idDistrito,
                        fonoEmpleado = c.fonoEmpleado,
                        idCargo = c.idCargo,
                        FecContrata = c.FecContrata
                    }).ToList();
            }
            return objEmpleado;
        }

        [HttpPost]
        public async Task<IActionResult> Create(Empleado reg)
        {
            string mensaje = "";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7194/api/apiEmpleado/");
                StringContent contenido = new StringContent(JsonConvert.SerializeObject(reg), System.Text.Encoding.UTF8, "application/json");

                HttpResponseMessage msg = await client.PostAsync("agregar", contenido);
                mensaje = await msg.Content.ReadAsStringAsync();
            }
            TempData["mensaje"] = mensaje;
            return RedirectToAction("Index");
        }

        public async Task<Empleado> ObtenerEmpleadoPorId(int IdEmpleado)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7194/api/apiEmpleado/");
                HttpResponseMessage response = await client.GetAsync($"buscar/{IdEmpleado}");
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    Empleado empleado = JsonConvert.DeserializeObject<Empleado>(json);
                    return empleado;
                }
                else
                {
                    return null;
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int IdEmpleado, Empleado reg)
        {
            try
            {
                string mensaje = "";
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:7194/api/");
                    string jsonData = JsonConvert.SerializeObject(reg);
                    var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                    HttpResponseMessage msg = await client.PutAsync($"apiEmpleado/actualizar?IdEmpleado={IdEmpleado}", content);
                    if (msg.StatusCode == HttpStatusCode.OK)
                    {
                        mensaje = $"Se actualizó correctamente el empleado con ID {IdEmpleado}";
                    }
                    else
                    {
                        mensaje = $"Error al actualizar el empleado. Detalles: {msg.ReasonPhrase}";
                    }
                }

                TempData["mensaje"] = mensaje;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["mensaje"] = "Error al actualizar el empleado. Detalles: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> Delete(int IdEmpleado)
        {
            try
            {
                string mensaje = "";
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:7194/api/");

                    HttpResponseMessage msg = await client.DeleteAsync($"apiEmpleado/eliminar?IdEmpleado={IdEmpleado}");
                    if (msg.IsSuccessStatusCode)
                    {
                        mensaje = $"Se eliminó correctamente el empleado con ID {IdEmpleado}";
                    }
                    else
                    {
                        mensaje = $"Error al eliminar el empleado. Detalles: {msg.ReasonPhrase}";
                    }
                }

                TempData["mensaje"] = mensaje;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["mensaje"] = "Error al eliminar el empleado. Detalles: " + ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}
