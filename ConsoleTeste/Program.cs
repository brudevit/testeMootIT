using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;



namespace ConsoleTeste
{
    class Program
    {
        
        static string apiUrl = "http://localhost:52691/api/";        

        public static void Main(string[] args)
        {
            PopulaCadastro();                      
            ObterResultado("Bruno");
            Console.ReadKey();

        }

        static void PopulaCadastro()
        {
            List<Amigo>lstCadastros = new List<Amigo>();

            #region Cadastros chumbados

            var bruno = new Amigo
            {
                nome = "Bruno",
                latitude = 2,
                longitude = 3
            };
            
            lstCadastros.Add(bruno);

            var joao = new Amigo
            {
                nome = "João",
                latitude = 1,
                longitude = 2
            };

            lstCadastros.Add(joao);

            var raul = new Amigo
            {
                nome = "Raul",
                latitude = 1,
                longitude = -1
            };

            lstCadastros.Add(raul);


            var rick = new Amigo
            {
                nome = "Rick",
                latitude = 2,
                longitude = -1
            };

            lstCadastros.Add(rick);

            var carlos = new Amigo
            {
                nome = "Carlos",
                latitude = 1,
                longitude = -2
            };

            lstCadastros.Add(carlos);

            var pedro = new Amigo
            {
                nome = "Pedro",
                latitude = 3,
                longitude = -2
            };

            lstCadastros.Add(pedro);

            #endregion Cadastros chumbados


            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(Program.apiUrl);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //var byteArray = Encoding.ASCII.GetBytes("teste:psw");
            var byteArray = Encoding.ASCII.GetBytes("34bgty7877ssd");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            

            foreach (Amigo cadastro in lstCadastros)
            {
                var myContent = JsonConvert.SerializeObject(cadastro);
                var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                HttpResponseMessage response = client.PostAsync("meuprojeto/amigo/", byteContent).Result;
                
                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    Console.WriteLine(result);
                }
                else
                {
                    Console.WriteLine(response.RequestMessage);
                }
            }
        }

        static void ObterResultado(string nome)
        {      
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(Program.apiUrl);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //var byteArray = Encoding.ASCII.GetBytes("teste:psw");
            var byteArray = Encoding.ASCII.GetBytes("34bgty7877ssd");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));


            HttpResponseMessage response = client.GetAsync("meuprojeto/proximos/" + nome).Result;            

            if (response.IsSuccessStatusCode)
            {                
                var result = response.Content.ReadAsStringAsync().Result;
                Console.WriteLine(result);
            }
            else
            {
                Console.WriteLine(response.RequestMessage);
            }
        }

       

    }
    
    public class Amigo
    {
        public string nome { get; set; }
        public float latitude { get; set; }
        public float longitude { get; set; }
        public static string log { get; set; }
        private float distancia { get; set; }
    }

}
