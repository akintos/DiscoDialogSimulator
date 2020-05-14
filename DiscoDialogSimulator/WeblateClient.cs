using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;

namespace DiscoDialogSimulator
{
    public class WeblateClient
    {
        private readonly string baseUrl;

        private HttpClient client;

        public WeblateClient(string baseUrl, string key)
        {
            this.baseUrl = baseUrl;

            client = new HttpClient();

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/javascript"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", key);
            
        }

        /// <summary>
        /// Return true if user can access given project
        /// </summary>
        /// <param name="projectName"></param>
        /// <returns></returns>
        public bool TestAuth(string projectName)
        {
            return client.GetAsync(baseUrl + "/projects/" + projectName + "/").Result.IsSuccessStatusCode;
        }

        public T Request<T>(string path)
        {
            var response = client.GetAsync(baseUrl + path).Result;
            if (response.IsSuccessStatusCode)
            {
                string content = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<T>(content);
            }
            return default;
        }

        public TranslationUnit GetTranslation(int id)
        {
            return Request<TranslationUnit>("units/" + id + "/");
        }
    }

    public class TranslationUnit
    {
        public string translation { get; set; }
        public string source { get; set; }
        public string target { get; set; }
        public string location { get; set; }
        public string context { get; set; }
        public string comment { get; set; }
        public string flags { get; set; }
        public bool fuzzy { get; set; }
        public bool translated { get; set; }
        public int position { get; set; }
        public int id { get; set; }
        public string web_url { get; set; }
    }
}
