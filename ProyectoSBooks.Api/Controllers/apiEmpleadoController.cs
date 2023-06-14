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
    public class apiEmpleadoController : ControllerBase
    {
        string cadena = @"server = GHOSTHEART;
                        database = Sbook2023;" +
                        "Trusted_Connection = False;" +
                        "uid = sa;" +
                        "pwd = abc;" +
                        "TrustServerCertificate = False;" +
                        "Encrypt = False";

        IEnumerable<Empleado> listadoEmpleados()
        {
            List<Empleado> objEmpleado = new List<Empleado>();
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("exec usp_empleados", cn);
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    objEmpleado.Add(new Empleado()
                    {
                        IdEmpleado = dr.GetInt32(0),
                        ApeEmpleado = dr.GetString(1),
                        NomEmpleado = dr.GetString(2),
                        FecNac = dr.GetDateTime(3),
                        DirEmpleado = dr.GetString(4),
                        idDistrito = dr.GetInt32(5),
                        fonoEmpleado = dr.GetString(6),
                        idCargo = dr.GetInt32(7),
                        FecContrata = dr.GetDateTime(8)
                    });
                }
                cn.Close();
            }
            return objEmpleado;
        }

        Empleado buscarEmpleado(int IdEmpleado)
        {
            return listadoEmpleados().FirstOrDefault(c => c.IdEmpleado == IdEmpleado);
        }

        string agregarEmpleado(Empleado reg)
        {
            string mensaje = "";
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                cn.Open();
                try
                {
                    SqlCommand cmd = new SqlCommand("usp_empleado_inserta", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ApeEmpleado", reg.ApeEmpleado);
                    cmd.Parameters.AddWithValue("@NomEmpleado", reg.NomEmpleado);
                    cmd.Parameters.AddWithValue("@FecNac", reg.FecNac);
                    cmd.Parameters.AddWithValue("@DirEmpleado", reg.DirEmpleado);
                    cmd.Parameters.AddWithValue("@idDistrito", reg.idDistrito);
                    cmd.Parameters.AddWithValue("@fonoEmpleado", reg.fonoEmpleado);
                    cmd.Parameters.AddWithValue("@idCargo", reg.idCargo);
                    cmd.Parameters.AddWithValue("@FecContrata", reg.FecContrata);
                    cmd.ExecuteNonQuery();
                    mensaje = $"Se registró correctamente el empleado {reg.NomEmpleado.ToUpper()}";
                }
                catch (SqlException ex)
                {
                    mensaje = ex.Message;
                }
                finally { cn.Close(); }
            }
            return mensaje;
        }

        string actualizarEmpleado(Empleado reg)
        {
            string mensaje = "";
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                cn.Open();
                try
                {
                    SqlCommand cmd = new SqlCommand("usp_empleado_actualiza", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdEmpleado", reg.IdEmpleado);
                    cmd.Parameters.AddWithValue("@ApeEmpleado", reg.ApeEmpleado);
                    cmd.Parameters.AddWithValue("@NomEmpleado", reg.NomEmpleado);
                    cmd.Parameters.AddWithValue("@FecNac", reg.FecNac);
                    cmd.Parameters.AddWithValue("@DirEmpleado", reg.DirEmpleado);
                    cmd.Parameters.AddWithValue("@idDistrito", reg.idDistrito);
                    cmd.Parameters.AddWithValue("@fonoEmpleado", reg.fonoEmpleado);
                    cmd.Parameters.AddWithValue("@idCargo", reg.idCargo);
                    cmd.Parameters.AddWithValue("@FecContrata", reg.FecContrata);
                    cmd.ExecuteNonQuery();
                    mensaje = $"Se actualizó correctamente el empleado {reg.NomEmpleado.ToUpper()}";
                }
                catch (SqlException ex)
                {
                    mensaje = ex.Message;
                }
                finally { cn.Close(); }
            }
            return mensaje;
        }

        string eliminarEmpleado(int IdEmpleado)
        {
            String mensaje = "";
            using (SqlConnection cn = new SqlConnection(cadena))
                try
                {
                    cn.Open();
                    SqlCommand cmd = new SqlCommand("usp_empleado_eliminar", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdEmpleado", IdEmpleado);
                    cmd.ExecuteNonQuery();
                    mensaje = "El empleado con el ID: " + IdEmpleado + " se eliminó correctamente";
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


        [HttpGet("empleados")]
        public async Task<ActionResult<IEnumerable<Empleado>>> empleados()
        {
            return Ok(await Task.Run(() => listadoEmpleados()));
        }

        [HttpGet("buscar/{IdEmpleado}")]
        public async Task<ActionResult<Empleado>> buscar(int IdEmpleado)
        {
            return Ok(await Task.Run(() => buscarEmpleado(IdEmpleado)));
        }

        [HttpPost("agregar")]
        public async Task<ActionResult<string>> agregar(Empleado reg)
        {
            return Ok(await Task.Run(() => agregarEmpleado(reg)));
        }

        [HttpPut("actualizar")]
        public async Task<ActionResult<string>> actualizar(Empleado reg)
        {
            return Ok(await Task.Run(() => actualizarEmpleado(reg)));
        }

        [HttpDelete("eliminar")]
        public async Task<ActionResult<string>> eliminar(int IdEmpleado)
        {
            return Ok(await Task.Run(() => eliminarEmpleado(IdEmpleado)));
        }
    }
}
