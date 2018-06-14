using ProjetoAspNetWebApi.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ProjetoAspNetWebApi.Controllers
{
    [RoutePrefix("api/meuprojeto")]
    public class AmigoController : ApiController
    {
        /// <summary>
        /// Altere a connectionString de acordo com os dados de seu banco de dados
        /// </summary>
        private string ConnectionString = "Data Source=server;User Id=sa;Password=123456;Initial Catalog=ProjetoAspNetWebApi";

        /// <summary>
        /// Metodo para consultar a lista de amigos
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("todos")]
        public HttpResponseMessage GetAll()
        {
            try
            {
                List<Amigo> lstAmigos = new List<Amigo>();
                Amigo amigo = new Amigo();
                lstAmigos = amigo.listarTodos();

                //using (SqlConnection connection = new SqlConnection(this.ConnectionString))
                //{
                //    connection.Open();

                //    using (SqlCommand command = new SqlCommand())
                //    {
                //        command.Connection = connection;
                //        command.CommandText = "select nome, latitude, longitude from amigo";

                //        SqlDataReader reader = command.ExecuteReader();

                //        while (reader.Read())
                //        {
                //            Amigo amigo = new Amigo()
                //            {                                
                //                nome = reader["nome"] == DBNull.Value ? string.Empty : reader["nome"].ToString(),
                //                longitude = reader["longitude"] == DBNull.Value ? 0: float.Parse(reader["longitude"].ToString()),
                //                latitude = reader["latitude"] == DBNull.Value ? 0 : float.Parse(reader["longitude"].ToString())
                //            };

                //            lstAmigos.Add(amigo);
                //        }
                //    }

                //    connection.Close();
                //}

                return Request.CreateResponse(HttpStatusCode.OK, lstAmigos.ToArray());
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        /// <summary>
        /// Metodo para consultar a lista de amigos
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("proximos/{nome}")]
        public HttpResponseMessage GetProximos(string nome)
        {
            try
            {
                List<Amigo> lstAmigos = new List<Amigo>();
                Amigo amigo = new Amigo();
                lstAmigos = amigo.listarAmigosProximos(nome);

                return Request.CreateResponse(HttpStatusCode.OK, lstAmigos.ToArray());
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        /// <summary>
        /// Metodo para consultar detalhes de um determinado amigo
        /// </summary>        
        /// <returns></returns>
        [HttpGet]
        [Route("amigo/{nome}")]
        public HttpResponseMessage GetById(string nome)
        {
            try
            {
                Amigo amigo = null;

                using (SqlConnection connection = new SqlConnection(this.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = "select nome, latitude, longitude from amigo where nome = @nome";
                        command.Parameters.AddWithValue("nome", nome);

                        SqlDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            amigo = new Amigo()
                            {
                                nome = reader["nome"] == DBNull.Value ? string.Empty : reader["nome"].ToString(),
                                longitude = reader["longitude"] == DBNull.Value ? 0 : float.Parse(reader["longitude"].ToString()),
                                latitude = reader["latitude"] == DBNull.Value ? 0 : float.Parse(reader["longitude"].ToString())
                            };
                        }
                    }

                    connection.Close();
                }

                return Request.CreateResponse(HttpStatusCode.OK, amigo);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }


        /// <summary>
        /// Metodo para cadastrar um novo amigo
        /// </summary>
        /// <param name="amigo"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("amigo")]
        public HttpResponseMessage Post(Amigo amigo)
        {
            try
            {
                if (amigo == null) throw new ArgumentNullException("amigo");
           
                if(amigo.create(amigo))
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "Cadastro efetuado!");
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, Amigo.log);
                }                
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

    }
}
     