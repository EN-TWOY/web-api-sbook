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
    public class apiCategoriaController : ControllerBase
    {

        // conexion a la bd
        string cadena = @"server = GHOSTHEART;
                        database = Sbook2023;" +
                        "Trusted_Connection = False;" +
                        "uid = sa;" +
                        "pwd = abc;" +
                        "TrustServerCertificate = False;" +
                        "Encrypt = False";

        IEnumerable<Categoria> listadoCategorias()
        {
            List<Categoria> objCategoria = new List<Categoria>();
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("exec usp_categorias", cn);
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    objCategoria.Add(new Categoria()
                    {
                        IdCategoria = dr.GetInt32(0),
                        NombreCategoria = dr.GetString(1),
                        Descripcion = dr.GetString(2)
                    });
                }
                cn.Close();
            }
            return objCategoria;
        }

        Categoria buscarCategoria(int IdCategoria)
        {
            return listadoCategorias().FirstOrDefault(c => c.IdCategoria == IdCategoria);
        }

        string agregarCategoria(Categoria reg)
        {
            string mensaje = "";
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                cn.Open();
                try
                {
                    SqlCommand cmd = new SqlCommand("usp_categoria_inserta", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@nombre", reg.NombreCategoria);
                    cmd.Parameters.AddWithValue("@descripcion", reg.Descripcion);
                    cmd.ExecuteNonQuery();
                    mensaje = $"Se registró correctamente la categoria {reg.NombreCategoria.ToUpper()}";
                }
                catch (SqlException ex)
                {
                    mensaje = ex.Message;
                }
                finally { cn.Close(); }
            }
            // ViewBag.mensaje = mensaje;
            return mensaje;
        }

        string actualizarCategoria(Categoria reg)
        {
            string mensaje = "";
            using (SqlConnection cn = new SqlConnection(cadena))
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
                finally { cn.Close(); }
            }
            return mensaje;
        }

        string eliminarCategoria(int IdCategoria)
        {
            String mensaje = "";
            using (SqlConnection cn = new SqlConnection(cadena))
                try
                {
                    cn.Open();
                    SqlCommand cmd = new SqlCommand("usp_categoria_eliminar", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@idcategoria", IdCategoria);
                    cmd.ExecuteNonQuery();
                    mensaje = "La categoria: " + IdCategoria + " se elimino correctamente";
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

        [HttpGet("categorias")]
        public async Task<ActionResult<IEnumerable<Categoria>>> categorias() {
            return Ok(await Task.Run(() => listadoCategorias()));
        }

        [HttpGet("buscar/{IdCategoria}")]
        public async Task<ActionResult<Categoria>> buscar(int IdCategoria) {
            return Ok(await Task.Run(() => buscarCategoria(IdCategoria)));
        }

        [HttpPost("agregar")]
        public async Task<ActionResult<string>> agregar(Categoria reg) {
            return Ok(await Task.Run(() => agregarCategoria(reg)));
        }

        [HttpPut("actualizar")]
        public async Task<ActionResult<string>> actualizar(Categoria reg) {
            return Ok(await Task.Run(() => actualizarCategoria(reg)));
        }

        [HttpDelete("eliminar")]
        public async Task<ActionResult<string>> eliminar(int IdCategoria)
        {
            return Ok(await Task.Run(() => eliminarCategoria(IdCategoria)));
        }
    }
}
