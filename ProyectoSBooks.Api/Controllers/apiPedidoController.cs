using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using ProyectoSBooks.Api.Models;

namespace ProyectoSBooks.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class apiPedidoController : ControllerBase
    {
        string cadena = @"server = GHOSTHEART;
                        database = Sbook2023;" +
                        "Trusted_Connection = False;" +
                        "uid = sa;" +
                        "pwd = abc;" +
                        "TrustServerCertificate = False;" +
                        "Encrypt = False";

        IEnumerable<Pedido> listadoPedidos()
        {
            List<Pedido> objPedido = new List<Pedido>();
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("exec usp_ventas_realizadas", cn);
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    objPedido.Add(new Pedido()
                    {
                        idpedido = dr.GetInt32(0),
                        fpedido = dr.GetDateTime(1),
                        dni = dr.GetString(2),
                        nombre = dr.GetString(3),
                        email = dr.GetString(4)
                    });
                }
                cn.Close();
            }
            return objPedido;
        }

        [HttpGet("ventas")]
        public async Task<ActionResult<IEnumerable<Pedido>>> ventas()
        {
            return Ok(await Task.Run(() => listadoPedidos()));
        }
    }
}
