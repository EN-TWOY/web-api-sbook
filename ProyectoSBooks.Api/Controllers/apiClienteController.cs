using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
// imports
using Microsoft.Data;
using Microsoft.Data.SqlClient;
using ProyectoSBooks.Api.Controllers;
using ProyectoSBooks.Api.Models;
using System.Data;

namespace ProyectoSBooks.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class apiClienteController : ControllerBase
    {
        string cadena = @"server = GHOSTHEART;
                        database = Sbook2023;" +
                        "Trusted_Connection = False;" +
                        "uid = sa;" +
                        "pwd = abc;" +
                        "TrustServerCertificate = False;" +
                        "Encrypt = False";

        IEnumerable<Cliente> listadoClientes()
        {
            List<Cliente> objCliente = new List<Cliente>();
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("exec usp_clientes", cn);
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    objCliente.Add(new Cliente()
                    {
                        IdCliente = dr.GetInt32(0),
                        NombreCia = dr.GetString(1),
                        Direccion = dr.GetString(2),
                        NombrePais   = dr.GetString(3),
                        idPais = dr.GetInt32(4),
                        Telefono = dr.GetString(5),
                        dni = dr.IsDBNull(6) ? (int?)null : dr.GetInt32(6)
                    });
                }
                cn.Close();
            }
            return objCliente;
        }

        Cliente buscarCliente(int IdCliente)
        {
            return listadoClientes().FirstOrDefault(c => c.IdCliente == IdCliente);
        }

        string agregarCliente(Cliente reg)
        {
            string mensaje = "";
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                cn.Open();
                try
                {
                    SqlCommand cmd = new SqlCommand("usp_cliente_inserta", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@NombreCia", reg.NombreCia);
                    cmd.Parameters.AddWithValue("@Direccion", reg.Direccion);
                    cmd.Parameters.AddWithValue("@idpais", reg.idPais);
                    cmd.Parameters.AddWithValue("@Telefono", reg.Telefono);
                    cmd.Parameters.AddWithValue("@dni", reg.dni);
                    cmd.ExecuteNonQuery();
                    mensaje = $"Se registró correctamente el cliente {reg.NombreCia.ToUpper()}";
                }
                catch (SqlException ex)
                {
                    mensaje = ex.Message;
                }
                finally { cn.Close(); }
            }
            return mensaje;
        }

        string actualizarCliente(Cliente reg)
        {
            string mensaje = "";
            using (SqlConnection cn = new SqlConnection(cadena))
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
                finally { cn.Close(); }
            }
            return mensaje;
        }

        string eliminarCliente(int IdCliente)
        {
            String mensaje = "";
            using (SqlConnection cn = new SqlConnection(cadena))
                try
                {
                    cn.Open();
                    SqlCommand cmd = new SqlCommand("usp_cliente_eliminar", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdCliente", IdCliente);
                    cmd.ExecuteNonQuery();
                    mensaje = "El cliente: " + IdCliente + " se elimino correctamente";
                }
                catch (Exception ex1)
                {
                    mensaje = "error: " + ex1.Message;
                }
                finally
                {
                    cn.Close();
                }
            return mensaje;
        }

        [HttpGet("clientes")]
        public async Task<ActionResult<IEnumerable<Cliente>>> clientes()
        {
            return Ok(await Task.Run(() => listadoClientes()));
        }

        [HttpGet("buscar/{IdCliente}")]
        public async Task<ActionResult<Cliente>> buscar(int IdCliente)
        {
            return Ok(await Task.Run(() => buscarCliente(IdCliente)));
        }

        [HttpPost("agregar")]
        public async Task<ActionResult<string>> agregar(Cliente reg)
        {
            return Ok(await Task.Run(() => agregarCliente(reg)));
        }

        [HttpPut("actualizar")]
        public async Task<ActionResult<string>> actualizar(Cliente reg)
        {
            return Ok(await Task.Run(() => actualizarCliente(reg)));
        }

        [HttpDelete("eliminar")]
        public async Task<ActionResult<string>> eliminar(int IdCliente)
        {
            return Ok(await Task.Run(() => eliminarCliente(IdCliente)));
        }
    }
}
