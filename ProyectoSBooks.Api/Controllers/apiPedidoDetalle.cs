using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using ProyectoSBooks.Api.Models;

namespace ProyectoSBooks.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class apiPedidoDetalle : ControllerBase
	{
        string cadena = @"server = GHOSTHEART;
                        database = Sbook2023;" +
                        "Trusted_Connection = False;" +
                        "uid = sa;" +
                        "pwd = abc;" +
                        "TrustServerCertificate = False;" +
                        "Encrypt = False";

        IEnumerable<PedidoDetalle> listadoPedidoDetalle()
        {
            List<PedidoDetalle> objPedidoDetalle = new List<PedidoDetalle>();
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("exec usp_ventas_detalle", cn);
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    objPedidoDetalle.Add(new PedidoDetalle()
                    {
                        idpedido = dr.GetInt32(0),
                        idproducto = dr.GetInt32(1),
                        cantidad = dr.GetInt32(2),
                        precio = dr.GetDecimal(3),
                    });
                }
                cn.Close();
            }
            return objPedidoDetalle;
        }

        [HttpGet("ventasDetalle")]
        public async Task<ActionResult<IEnumerable<PedidoDetalle>>> ventasDetalle()
        {
            return Ok(await Task.Run(() => listadoPedidoDetalle()));
        }
    }
}
