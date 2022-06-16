using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace ApiService
{
    public static class ApiTool
    {
        private const string _adress = "http://localhost:50761/api/";
        public static string Get(string method)
        {
            WebClient client = new WebClient();
            client.Encoding = Encoding.UTF8;
            return client.DownloadString(_adress + method);
        }

        public static string Put<T>(string method, T editObject)
        {
            WebClient client = new WebClient();
            client.Encoding = Encoding.UTF8;
            client.Headers.Add(HttpRequestHeader.ContentType, "application/Json");
            string data = JsonConvert.SerializeObject(editObject);
            return client.UploadString(_adress + method, "PUT" , data);
        }
        public static string Post<T>(string method, T editObject)
        {
            WebClient client = new WebClient();
            client.Encoding = Encoding.UTF8;
            client.Headers.Add(HttpRequestHeader.ContentType, "application/Json");
            string data = JsonConvert.SerializeObject(editObject);
            return client.UploadString(_adress + method, "POST", data);
        }
        public static string Delete(string method)
        {
            WebClient client = new WebClient();
            client.Encoding = Encoding.UTF8;
            return client.UploadString(_adress + method, "DELETE", "");
        }
    }
}
