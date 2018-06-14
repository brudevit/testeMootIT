using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections;
using System.Dynamic;

namespace ProjetoAspNetWebApi.Models
{
    public class Amigo
    {
        public string nome { get; set; }
        public float latitude { get; set; }
        public float longitude { get; set; }
        public static string log { get; set; }  
        private float distancia { get; set; }      

        public bool create(Amigo _amigo)
        {
            bool resultado = false;

            try
            {
                using (SqlConnection connection = new SqlConnection(CtrlConnections.getStringConexao))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = "insert into amigo(nome, latitude, longitude) values(@nome, @latitude, @longitude)";

                        command.Parameters.AddWithValue("nome", _amigo.nome);
                        command.Parameters.AddWithValue("latitude", _amigo.latitude);
                        command.Parameters.AddWithValue("longitude", _amigo.longitude);

                        int i = command.ExecuteNonQuery();
                        resultado = i > 0;
                    }

                    connection.Close();
                }            
            }
            catch (Exception ex)
            {
                Amigo.log = ex.Message;
            }

            return resultado;
        }

        public List<Amigo> listarTodos()
        {
            List<Amigo> lstAmigos = new List<Amigo>();

            try
            {
                using (SqlConnection connection = new SqlConnection(CtrlConnections.getStringConexao))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = "select nome, latitude, longitude from amigo";

                        SqlDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            Amigo amigo = new Amigo()
                            {
                                nome = reader["nome"] == DBNull.Value ? string.Empty : reader["nome"].ToString(),
                                longitude = reader["longitude"] == DBNull.Value ? 0 : float.Parse(reader["longitude"].ToString()),
                                latitude = reader["latitude"] == DBNull.Value ? 0 : float.Parse(reader["latitude"].ToString())
                            };

                            lstAmigos.Add(amigo);
                        }
                    }

                    connection.Close();
                }
            }
            catch(Exception ex)
            {

            }

            return lstAmigos;
        }

        public List<Amigo> listarAmigosProximos(string _nome)
        {
            List<Amigo> lstAmigos = this.listarTodos();
            Amigo programador = lstAmigos.Where<Amigo>(x => x.nome.Equals(_nome)).Single<Amigo>();

            List<Amigo> lstDistancias = new List<Amigo>();

            foreach (Amigo amigo in lstAmigos)
            {
                if(amigo != programador)
                {
                    amigo.calcDist(programador, amigo);
                    lstDistancias.Add(amigo);
                }
            }

            //Ordena a distancia da menor para a maior
            lstDistancias.Sort(delegate (Amigo amigo1, Amigo amigo2)
            {
                return amigo1.distancia.CompareTo(amigo2.distancia);
            });

            return lstDistancias.Take(3).ToList();
        }


        //Calcula a distancia entre as pessoas
        private void calcDist(Amigo _amigo1, Amigo _amigo2)
        {
            float xA, xB, yA, yB;

            xA = _amigo1.latitude;
            xB = _amigo2.latitude;

            yA = _amigo1.longitude;
            yB = _amigo2.longitude;

            this.distancia = (float)Math.Sqrt(Math.Pow(xB - xA, 2) + Math.Pow(yB - yA, 2));            
        }

    }

    public static class CtrlConnections
    {
        public static string getStringConexao
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["TesteBD"].ToString();
            }
        }
        
    }
}