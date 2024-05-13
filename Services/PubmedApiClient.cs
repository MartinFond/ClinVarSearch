using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;

public class PubmedApiClient
{
    private readonly HttpClient _httpClient;

    public PubmedApiClient()
    {
        _httpClient = new HttpClient();
    }

    public async Task<string> GetDoiFromPmid(string pmid)
    {
        string pubmedApiUrl = $"https://eutils.ncbi.nlm.nih.gov/entrez/eutils/esummary.fcgi?db=pubmed&id={pmid}&retmode=json";

        HttpResponseMessage response = await _httpClient.GetAsync(pubmedApiUrl);

        if (response.IsSuccessStatusCode)
        {
            string jsonString = await response.Content.ReadAsStringAsync();
            JObject jsonObj = JObject.Parse(jsonString);

        // Get the DOI value
        string doi = (string)jsonObj["result"][pmid]["articleids"]
            .FirstOrDefault(a => (string)a["idtype"] == "doi")?["value"];

        // Print the DOI
        return("DOI: " + doi);

        }
        else
        {
            // Handle error
            return($"Failed to retrieve metadata for PMID {pmid}");
        }

        return "";
    }

    public async Task<string> GetPmcFromPmid(string pmid)
    {
        string pubmedApiUrl = $"https://eutils.ncbi.nlm.nih.gov/entrez/eutils/esummary.fcgi?db=pubmed&id={pmid}&retmode=json";

        HttpResponseMessage response = await _httpClient.GetAsync(pubmedApiUrl);

        if (response.IsSuccessStatusCode)
        {
            string jsonString = await response.Content.ReadAsStringAsync();
            JObject jsonObj = JObject.Parse(jsonString);

        // Get the pmc value
        string pmc = (string)jsonObj["result"][pmid]["articleids"]
            .FirstOrDefault(a => (string)a["idtype"] == "pmc")?["value"];

        // Print the PMC
        return(pmc);

        }
        else
        {
            // Handle error
            return($"Failed to retrieve metadata for PMID {pmid}");
        }

        return "";
    }

    public class ArticleId
    {
        public string idtype { get; set; }
        public int idtypen { get; set; }
        public string value { get; set; }
    }

    public class Result
    {
        public string uid { get; set; }
        public ArticleId[] articleids { get; set; }
    }

    public class Header
    {
        public string type { get; set; }
        public string version { get; set; }
    }

    public class RootObject
    {
        public Header header { get; set; }
        public List<string> uids { get; set; }
        public Dictionary<string, Result> result { get; set; }
    }
}
