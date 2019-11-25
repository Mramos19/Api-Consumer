using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;

namespace CapaNegocio
{

    public class ApiConsumerManager
    {
     

        public List<T> EnviarSolicitud <T> (string Raiz, string Metodo, object Data, string Tipo, string TipoContenido)
        {

           List<T> resultado = default(List<T>);

            try
            {
                //Serializamos el objeto en formato json

                string dataJason = (new JavaScriptSerializer()).Serialize(Data);
               

                //Creamos el Objeto HttpRequest con la url Raiz y metodo
                HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(new Uri(string.Format("{0}/{1}", Raiz, Metodo)));
                httpRequest.ContentType = TipoContenido; //"application/json"
                httpRequest.Method = Tipo; //"POST"

                byte[] bytes = Encoding.UTF8.GetBytes(dataJason);

                using (Stream stream = httpRequest.GetRequestStream())
                {
                    stream.Write(bytes, 0, bytes.Length);
                    stream.Close();
                }

                //Definimos el httpwebResponse
                using (HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse())
                {
                    using (Stream stream = httpResponse.GetResponseStream())
                    {
                        string json = (new StreamReader(stream)).ReadToEnd();

                        resultado = (new JavaScriptSerializer()).Deserialize<List<T>>(json);
                       
                    }
                }

               
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return resultado;

        }    
        
    }

}
