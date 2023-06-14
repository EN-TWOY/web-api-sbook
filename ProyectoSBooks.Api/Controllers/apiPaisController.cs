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
    public class apiPaisController : ControllerBase
    {
        string cadena = @"server = GHOSTHEART;
                        database = Sbook2023;" +
                        "Trusted_Connection = False;" +
                        "uid = sa;" +
                        "pwd = abc;" +
                        "TrustServerCertificate = False;" +
                        "Encrypt = False";

        IEnumerable<Pais> listadoPaises()
        {
            List<Pais> objPais = new List<Pais>();
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("exec usp_paises", cn);
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    objPais.Add(new Pais()
                    {
                        Idpais = dr.GetInt32(0),
                        NombrePais = dr.GetString(1)
                    });
                }
                cn.Close();
            }
            return objPais;
        }

        [HttpGet("paises")]
        public async Task<ActionResult<IEnumerable<Pais>>> paises()
        {
            return Ok(await Task.Run(() => listadoPaises()));
        }

    }
}
