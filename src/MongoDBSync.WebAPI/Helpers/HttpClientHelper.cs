using System;
using System.Net;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace MongoDBSync.WebAPI.Helpers
{
    public class HttpClientHelper
    {
        public static T Get<T>(string url)
        {
            using (var client = new HttpClient())
            {
                using (HttpResponseMessage response = client.GetAsync(url).Result)
                {
                    response.EnsureSuccessStatusCode();
                    string responseBody = response.Content.ReadAsStringAsync().Result;

                    return JsonConvert.DeserializeObject<T>(responseBody);
                }
            }
        }

        public static bool Post(string url, object model)
        {
            using (var client = new HttpClient())
            {
                var response = client.PostAsync(url, new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json"));
                response.Wait();

                return response.Result.StatusCode == HttpStatusCode.OK;
            }
        }

        public static T Post<T>(string url, object model)
        {
            using (var client = new HttpClient())
            {
                var response = client.PostAsync(url, new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json"));
                response.Wait();

                if (response.Result.StatusCode == HttpStatusCode.OK)
                {
                    return JsonConvert.DeserializeObject<T>(response.Result.Content.ReadAsStringAsync().Result);
                }

                throw new Exception(HttpStatusCode.InternalServerError.ToString());
            }
        }
    }
}
