using netex_client_cs.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static netex_client_cs.Data.ProxySettings;

namespace netex_client_cs.Components {
    public class Request {
        public string BASE_URL = "";
        public NetexSettings settings = null;
        public ProxySettingsItem proxy = null;

        public string HttpGet(string action, int timeout = 0) {
            var request = (HttpWebRequest)WebRequest.Create(BASE_URL + action);
            if (timeout != 0)
                request.Timeout = timeout;

            request.Method = "GET";
            request.ContentType = "application/x-www-form-urlencoded";
            if (proxy != null) request.Proxy = proxy.CreateProxyClient();
            var response = (HttpWebResponse)request.GetResponse();

            return new StreamReader(response.GetResponseStream()).ReadToEnd();
        }

        public string HttpJson(string action, string parameters = "") {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(BASE_URL + action);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(parameters);
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                return streamReader.ReadToEnd();
            }
        }

        public string HttpPut(string action, string parameters)
        {
            var request = (HttpWebRequest)WebRequest.Create(BASE_URL + action);
            var data = Encoding.ASCII.GetBytes(parameters);

            request.Method = "PUT";
            request.ContentType = "application/xml";
            request.ContentLength = data.Length;
            if (proxy != null)
                request.Proxy = proxy.CreateProxyClient();

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }
            var response = (HttpWebResponse)request.GetResponse();
            return new StreamReader(response.GetResponseStream()).ReadToEnd();
        }

        public string getMyIp()
        {
            var request = (HttpWebRequest)WebRequest.Create("https://api.ipify.org?format=json");            
            request.Method = "GET";
            request.ContentType = "application/x-www-form-urlencoded";
            if (proxy != null) request.Proxy = proxy.CreateProxyClient();
            var response = (HttpWebResponse)request.GetResponse();
            return new StreamReader(response.GetResponseStream()).ReadToEnd();
        }
    }
}
