using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;

public class WebPageScanner
{
    public async Task<List<string>> ScanForPMIDs(string url)
    {
        List<string> pmids = new List<string>();

        using (var httpClient = new HttpClient())
        {
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string htmlContent = await response.Content.ReadAsStringAsync();

                    // Load the HTML content into HtmlAgilityPack
                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(htmlContent);

                    // Select all <a> tags with href containing 'pubmed.ncbi.nlm.nih.gov'
                    foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//a[contains(@href,'pubmed.ncbi.nlm.nih.gov')]"))
                    {
                        // Get the href attribute value
                        string hrefValue = link.GetAttributeValue("href", string.Empty);

                        // Find the index of the last "/"
                        int lastIndex = hrefValue.Substring(0, hrefValue.Length - 1).LastIndexOf("/");
                        if (lastIndex != -1)
                        {
                            // Extract the PMID from the href attribute
                            string pmid = hrefValue.Substring(lastIndex + 1);
                            // Add the PMID to the list
                            pmids.Add(pmid.Substring(0, pmid.Length - 1));
                        }
                        else
                        {
                            Console.WriteLine("Failed to extract PMID from href: " + hrefValue);
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"Failed to download the web page. Status code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        return pmids;
    }
}


