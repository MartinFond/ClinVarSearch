using System;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

public class ElsevierApiClient
{
    private readonly HttpClient _client;
    private readonly string _apiKey;

    public ElsevierApiClient(string apiKey)
    {
        _client = new HttpClient();
        _apiKey = apiKey;
    }

    public async Task<string> GetArticleDownloadLink(string doi,string pmid)
    {
        string apiUrl = $"https://api.elsevier.com/content/article/doi/{doi}";

        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
        request.Headers.Add("X-ELS-APIKey", _apiKey);
        request.Headers.Add("Accept", "application/pdf");
        request.Headers.Add("view", "FULL");

        HttpResponseMessage response = await _client.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
             byte[] content = await response.Content.ReadAsByteArrayAsync();
             string jsonResponse = await response.Content.ReadAsStringAsync();
            string filePath = "PDF/" + pmid + ".pdf";
            File.WriteAllBytes(filePath, content);
            return "Ok";
        }
        else
        {
            Console.WriteLine("Failed to retrieve article. Status code: " + response.StatusCode);
            return null;
        }
    }
}
