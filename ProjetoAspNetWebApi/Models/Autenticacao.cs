using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security;
using System.Security.Principal;
using System.Text;
using System.Net.Http;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Configuration;


namespace ProjetoAspNetWebApi.Models
{
    public class Autenticacao : IHttpModule
    {
        private const string Realm = "Autenticacao_WS";

        public void Init(HttpApplication context)
        {
            // Register event handlers
            context.AuthenticateRequest += OnApplicationAuthenticateRequest;
            context.EndRequest += OnApplicationEndRequest;
        }


        private static void SetPrincipal(IPrincipal principal)
        {
            Thread.CurrentPrincipal = principal;
            if (HttpContext.Current != null)
            {
                HttpContext.Current.User = principal;
            }
        }

        private static void AuthenticateUser(string credentials)
        {
            try
            {
                var encoding = Encoding.GetEncoding("iso-8859-1");
                credentials = encoding.GetString(Convert.FromBase64String(credentials));

                //int separator = credentials.IndexOf(':');
                //string name = credentials.Substring(0, separator);
                //string password = credentials.Substring(separator + 1);
                bool bValidacao = false;



                //if (name == "teste" && password =="psw")
                //{
                //    bValidacao = true;
                //}

                string credencials_conf = "";
                if (ConfigurationManager.AppSettings["TokenTeste"] != null)
                {
                    credencials_conf = ConfigurationManager.AppSettings["TokenTeste"].ToString();
                }
                else
                {
                    HttpContext.Current.Response.StatusCode = 401;
                }
               
                if (credentials == credencials_conf)
                {
                    bValidacao = true;
                }

                if (bValidacao)
                {
                    MyPrincipal principal = new MyPrincipal(new GenericIdentity("MootIT"), null);
                    principal.user = "teste";
                    Thread.CurrentPrincipal = principal;
                    HttpContext.Current.User = principal;
                }
                else
                {
                    // Invalid username or password.
                    HttpContext.Current.Response.StatusCode = 401;
                }
            }
            catch (FormatException)
            {
                // Credentials were not formatted correctly.
                HttpContext.Current.Response.StatusCode = 401;
            }
        }

        private static void OnApplicationAuthenticateRequest(object sender, EventArgs e)
        {
            var request = HttpContext.Current.Request;
            var authHeader = request.Headers["Authorization"];
            if (authHeader != null)
            {
                var authHeaderVal = System.Net.Http.Headers.AuthenticationHeaderValue.Parse(authHeader);

                // RFC 2617 sec 1.2, "scheme" name is case-insensitive
                if (authHeaderVal.Scheme.Equals("basic",
                        StringComparison.OrdinalIgnoreCase) &&
                    authHeaderVal.Parameter != null)
                {
                    AuthenticateUser(authHeaderVal.Parameter);
                }
            }
            else
            {
                callWithoutBasicHeader(sender);
            }
        }

        // If the request was unauthorized, add the WWW-Authenticate header 
        // to the response.
        private static void OnApplicationEndRequest(object sender, EventArgs e)
        {
            var response = HttpContext.Current.Response;
            if (response.StatusCode == 401)
            {
                response.Headers.Add("WWW-Authenticate",
                    string.Format("Basic realm=\"{0}\"", Realm));
            }
        }

        public void Dispose()
        {
        }

        /// <summary>
        /// Função que verifica se o request feito sem Basic Security Header
        /// pode ser retornado
        /// </summary>
        private static void callWithoutBasicHeader(object sender)
        {
            if (sender != null)
            {
                PropertyInfo pInfoRequest = sender.GetType().GetProperty("Request");

                if (pInfoRequest != null)
                {
                    object url = pInfoRequest.GetValue(sender, null);

                    if (url != null)
                    {
                        PropertyInfo pInfoUrl = url.GetType().GetProperty("Url");

                        if (pInfoUrl != null)
                        {
                            string urlRequest = pInfoUrl.GetValue(url, null).ToString();

                            if (!string.IsNullOrEmpty(urlRequest))
                            {
                                if (urlRequest.Contains("AppConsultorServ_RequestFA") || urlRequest.Contains("AppConsultorServ_SendFA")
                                        || urlRequest.Contains("RetornaPoliticaReembolso"))
                                {
                                    var identity = new GenericIdentity("linx");
                                    SetPrincipal(new GenericPrincipal(identity, null));
                                }
                                else
                                {
                                    HttpContext.Current.Response.StatusCode = 401;
                                }
                            }
                        }

                    }

                }

            }
        }
    }
}

public class MyPrincipal : GenericPrincipal
{
    public string user { get; set; }

    public MyPrincipal(IIdentity identity, string[] roles)
        : base(identity, roles)
    {


    }
}