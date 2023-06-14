using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
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
    public class apiProveedorController : ControllerBase
    {

        string cadena = @"server = GHOSTHEART;
                        database = Sbook2023;" +
                        "Trusted_Connection = False;" +
                        "uid = sa;" +
                        "pwd = abc;" +
                        "TrustServerCertificate = False;" +
                        "Encrypt = False";

        IEnumerable<Proveedor> listadoProveedores()
        {
            List<Proveedor> objProveedor = new List<Proveedor>();
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("exec usp_proveedores", cn);
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    objProveedor.Add(new Proveedor()
                    {
                        IdProveedor = dr.GetInt32(0),
                        NombreCia = dr.GetString(1),
                        NombreContacto = dr.GetString(2),
                        CargoContacto = dr.GetString(3),
                        Direccion = dr.GetString(4),
                        NombrePais = dr.GetString(5),
                        idpais = dr.GetInt32(6),
                        Telefono = dr.GetString(7),
                        Fax = dr.GetString(8)
                    });
                }
                cn.Close();
            }
            return objProveedor;
        }

        Proveedor buscarProveedor(int IdProveedor)
        {
            return listadoProveedores().FirstOrDefault(c => c.IdProveedor == IdProveedor);
        }

        string agregarProveedor(Proveedor reg)
        {
            string mensaje = "";
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                cn.Open();
                try
                {
                    SqlCommand cmd = new SqlCommand("usp_proveedor_inserta", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@NombreCia", reg.NombreCia);
                    cmd.Parameters.AddWithValue("@NombreContacto", reg.NombreContacto);
                    cmd.Parameters.AddWithValue("@CargoContacto", reg.CargoContacto);
                    cmd.Parameters.AddWithValue("@Direccion", reg.Direccion);
                    cmd.Parameters.AddWithValue("@idpais", reg.idpais);
                    cmd.Parameters.AddWithValue("@Telefono", reg.Telefono);
                    cmd.Parameters.AddWithValue("@Fax", reg.Fax);
                    cmd.ExecuteNonQuery();
                    mensaje = $"Se registró correctamente el proveedor {reg.NombreCia.ToUpper()}";
                }
                catch (SqlException ex)
                {
                    mensaje = ex.Message;
                }
                finally { cn.Close(); }
            }
            return mensaje;
        }

        string actualizarProveedor(Proveedor reg)
        {
            string mensaje = "";
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                cn.Open();
                try
                {
                    SqlCommand cmd = new SqlCommand("usp_proveedor_actualiza", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdProveedor", reg.IdProveedor);
                    cmd.Parameters.AddWithValue("@NombreCia", reg.NombreCia);
                    cmd.Parameters.AddWithValue("@NombreContacto", reg.NombreContacto);
                    cmd.Parameters.AddWithValue("@CargoContacto", reg.CargoContacto);
                    cmd.Parameters.AddWithValue("@Direccion", reg.Direccion);
                    cmd.Parameters.AddWithValue("@idpais", reg.idpais);
                    cmd.Parameters.AddWithValue("@Telefono", reg.Telefono);
                    cmd.Parameters.AddWithValue("@Fax", reg.Fax);
                    cmd.ExecuteNonQuery();
                    mensaje = $"Se actualizó correctamente el proveedor {reg.NombreCia.ToUpper()}";
                }
                catch (SqlException ex)
                {
                    mensaje = ex.Message;
                }
                finally { cn.Close(); }
            }
            return mensaje;
        }

        string eliminarProveedor(int IdProveedor)
        {
            String mensaje = "";
            using (SqlConnection cn = new SqlConnection(cadena))
                try
                {
                    cn.Open();
                    SqlCommand cmd = new SqlCommand("usp_proveedor_eliminar", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdProveedor", IdProveedor);
                    cmd.ExecuteNonQuery();
                    mensaje = "El proveedor con el ID: " + IdProveedor + " se elimino correctamente";
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


        [HttpGet("proveedores")]
        public async Task<ActionResult<IEnumerable<Proveedor>>> proveedores()
        {
            return Ok(await Task.Run(() => listadoProveedores()));
        }

        [HttpGet("buscar/{IdProveedor}")]
        public async Task<ActionResult<Proveedor>> buscar(int IdProveedor)
        {
            return Ok(await Task.Run(() => buscarProveedor(IdProveedor)));
        }

        [HttpPost("agregar")]
        public async Task<ActionResult<string>> agregar(Proveedor reg)
        {
            return Ok(await Task.Run(() => agregarProveedor(reg)));
        }

        [HttpPut("actualizar")]
        public async Task<ActionResult<string>> actualizar(Proveedor reg)
        {
            return Ok(await Task.Run(() => actualizarProveedor(reg)));
        }

        [HttpDelete("eliminar")]
        public async Task<ActionResult<string>> eliminar(int IdProveedor)
        {
            return Ok(await Task.Run(() => eliminarProveedor(IdProveedor)));
        }
    }
}
