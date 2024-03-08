using System.Net.Http.Headers;

namespace PatientClient
{
    public class ApiSender
    {
        static HttpClient client = new HttpClient();
        public ApiSender(string baseUrl)
        {
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        }
        public async Task<bool> CreateMany(List<PatientModel> model)
        {
            HttpResponseMessage response = await client.PostAsJsonAsync("api/patient/createRange", model);
            response.EnsureSuccessStatusCode();

            return response.IsSuccessStatusCode;
        }
    }
}
