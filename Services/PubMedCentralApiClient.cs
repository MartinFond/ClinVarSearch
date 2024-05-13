using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

public class PubMedCentralApi
{
    private HttpClient _httpClient;

    public PubMedCentralApi()
    {
        _httpClient = new HttpClient();
    }

    public async Task DownloadOpenAccessPdf(string pmcId, string savePath)
    {
        try
        {
            // Construct the URL for the PubMedCentral API
            string apiUrl = $"https://www.ncbi.nlm.nih.gov/pmc/utils/oa/oa.fcgi?id={pmcId}";

            // Send the GET request to the PubMedCentral API
            HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);

            // Check if the request was successful
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Response Ok");
                // Read the content of the response as a string
                string responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseBody);
                // Extract the link to the PDF file from the response
                string pdfUrl = ExtractPdfUrl(responseBody);
                if (pdfUrl == "MissingFile")
                {
                    throw new Exception($"Failed to retrieve data. File not OpenAccess");
                }

                // Download the PDF file
                using (WebClient client = new WebClient())
                {
                    client.DownloadFile(pdfUrl, savePath);
                }

                Console.WriteLine($"PDF file downloaded and saved to: {savePath}");
            }
            else
            {
                // If the request was not successful, throw an exception with the status code
                Console.WriteLine("Failed to retrieve data");
                throw new Exception($"Failed to retrieve data. Status code: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            // If an exception occurs, throw it
            throw new Exception($"An error occurred: {ex.Message}");
        }
    }

    private string ExtractPdfUrl(string responseBody)
    {
        // Extract the URL of the PDF file from the response XML
        int hrefIndex = responseBody.IndexOf("href=\"");
        if (hrefIndex == -1)
        {
            return "MissingFile";
        }
        int startIndex = hrefIndex + "href=\"".Length;
        int endIndex = responseBody.IndexOf("\"", startIndex);
        return responseBody.Substring(startIndex, endIndex - startIndex);
    }
}



