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
    public class apiProductoController : ControllerBase
    {
        string cadena = @"server = GHOSTHEART;
                        database = Sbook2023;" +
                        "Trusted_Connection = False;" +
                        "uid = sa;" +
                        "pwd = abc;" +
                        "TrustServerCertificate = False;" +
                        "Encrypt = False";

        IEnumerable<Producto> listadoProductos()
        {
            List<Producto> objProducto = new List<Producto>();
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("exec usp_productos", cn);
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    objProducto.Add(new Producto()
                    {
                        IdProducto = dr.GetInt32(0),
                        NombreProducto = dr.GetString(1),
                        NombreCia = dr.GetString(2),
                        IdProveedor = dr.GetInt32(3),
                        NombreCategoria = dr.GetString(4),
                        IdCategoria = dr.GetInt32(5),
                        umedida = dr.GetString(6),
                        PrecioUnidad = dr.GetDecimal(7),
                        UnidadesEnExistencia = dr.GetInt16(8)
                    });
                }
                cn.Close();
            }
            return objProducto;
        }

        Producto buscarLibro(int IdProducto)
        {
            return listadoProductos().FirstOrDefault(c => c.IdProducto == IdProducto);
        }

        string agregarLibro(Producto reg)
        {
            string mensaje = "";
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                cn.Open();
                try
                {
                    SqlCommand cmd = new SqlCommand("usp_libro_inserta", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@NombreProducto", reg.NombreProducto);
                    cmd.Parameters.AddWithValue("@IdProveedor", reg.IdProveedor);
                    cmd.Parameters.AddWithValue("@IdCategoria", reg.IdCategoria);
                    cmd.Parameters.AddWithValue("@umedida", reg.umedida);
                    cmd.Parameters.AddWithValue("@PrecioUnidad", reg.PrecioUnidad);
                    cmd.Parameters.AddWithValue("@UnidadesEnExistencia", reg.UnidadesEnExistencia);
                    cmd.ExecuteNonQuery();
                    mensaje = $"Se registró correctamente el libro {reg.NombreProducto.ToUpper()}";
                }
                catch (SqlException ex)
                {
                    mensaje = ex.Message;
                }
                finally { cn.Close(); }
            }
            return mensaje;
        }

        string actualizarLibro(Producto reg)
        {
            string mensaje = "";
            using (SqlConnection cn = new SqlConnection(cadena))
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
                finally { cn.Close(); }
            }
            return mensaje;
        }

        string eliminarLibro(int IdProducto)
        {
            String mensaje = "";
            using (SqlConnection cn = new SqlConnection(cadena))
                try
                {
                    cn.Open();
                    SqlCommand cmd = new SqlCommand("usp_libro_eliminar", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdProducto", IdProducto);
                    cmd.ExecuteNonQuery();
                    mensaje = "El libro con el ID: " + IdProducto + " se eliminó correctamente";
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


        [HttpGet("productos")]
        public async Task<ActionResult<IEnumerable<Producto>>> productos()
        {
            return Ok(await Task.Run(() => listadoProductos()));
        }

        [HttpGet("buscar/{IdProducto}")]
        public async Task<ActionResult<Producto>> buscar(int IdProducto)
        {
            return Ok(await Task.Run(() => buscarLibro(IdProducto)));
        }

        [HttpPost("agregar")]
        public async Task<ActionResult<string>> agregar(Producto reg)
        {
            return Ok(await Task.Run(() => agregarLibro(reg)));
        }

        [HttpPut("actualizar")]
        public async Task<ActionResult<string>> actualizar(Producto reg)
        {
            return Ok(await Task.Run(() => actualizarLibro(reg)));
        }

        [HttpDelete("eliminar")]
        public async Task<ActionResult<string>> eliminar(int IdProducto)
        {
            return Ok(await Task.Run(() => eliminarLibro(IdProducto)));
        }
    }
}
