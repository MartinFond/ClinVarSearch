using Newtonsoft.Json;
class Program
{
    static async Task Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Please, enter at least one command line argument");
            Environment.Exit(1);
        }
        
        WebPageScanner scanner = new WebPageScanner();
        string url = "https://www.ncbi.nlm.nih.gov/clinvar/variation/"+ args[0] + "/"; // Replace with the URL of the webpage you want to scan
        List<string> pmids = await scanner.ScanForPMIDs(url);
        pmids = pmids.Distinct().ToList();
        Console.WriteLine("Found PMIDs:");
        foreach (var pmid in pmids)
        {
            Console.WriteLine(pmid);
        }

        PythonRunner pythonRunner = new PythonRunner();
        string pythonInterpreter = "python"; // Or provide the path to your Python interpreter
        string pythonScript = "Python/download_scihub.py"; // Replace with the path to your Python script
        string output = pythonRunner.RunPythonScript(pythonInterpreter, pythonScript, pmids.ToArray());
        // string[] array = ["1351946", "11334614"];
        // string output = pythonRunner.RunPythonScript(pythonInterpreter, pythonScript, array);
        string failedFile = "failed_ids.txt";
        if (File.Exists(failedFile))
        {
            List<string> failedIds = new List<string>();
            using (StreamReader sr = new StreamReader(failedFile))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    failedIds.Add(line);
                }
            }
            Console.WriteLine("Failed to download papers for the following IDs:");
            string MissingFilesPath = "PDF/missingfiles.txt";
            using (StreamWriter writer = new StreamWriter(MissingFilesPath))
            {
                foreach (string id in failedIds)
                {
                    Console.WriteLine(id);

                    var pubmedApiClient = new PubmedApiClient();
                    string pmid = id;
                    string doi = await pubmedApiClient.GetDoiFromPmid(pmid);
                    Console.WriteLine($"DOI for PMID {pmid}: {doi}");

                    string KeyPath = "ElsevierKey.txt";
                    string apiKey;
                    
                    using (StreamReader reader = new StreamReader(KeyPath))
                    {
                        // Read the first line
                        apiKey = reader.ReadLine();

                        // Check if the first line is not null (file is not empty)
 
                    }
                    if (apiKey == null)
                    {
                        Console.WriteLine("WARNING: NO ELSEVIER API KEY");
                        Environment.Exit(1);
                    }

                    ElsevierApiClient client = new ElsevierApiClient(apiKey);
                    string downloadLink = await client.GetArticleDownloadLink(doi, pmid);

                    if (downloadLink != null)
                    {
                        Console.WriteLine("Download Link: " + downloadLink);
                    }
                    else
                    {
                        string pmc = await pubmedApiClient.GetPmcFromPmid(pmid);
                        string savePath = "TarFiles/" + pmid + ".tar.gz"; // Specify the path where you want to save the PDF file

                        try
                        {
                            PubMedCentralApi pmcApi = new PubMedCentralApi();
                            await pmcApi.DownloadOpenAccessPdf(pmc, savePath);
                        }
                        catch (Exception ex)
                        {
                            writer.WriteLine(pmid);
                            Console.WriteLine($"Error: {ex.Message}");
                        }


                        string sourceArchivePath = savePath;
                        string destinationFolderPath = "PDF/";
                        //string filePathInArchive = pmc+"/*.pdf";

                        TarGzExtractor extractor = new TarGzExtractor();
                        extractor.ExtractAndRenamePdfFile(sourceArchivePath, destinationFolderPath, pmid+".pdf");
                    }
                }
            } 
        }
        
        if (args.Length < 2)
        {
            Console.WriteLine("Done");
            Environment.Exit(0);
        }

        string folderPath = @"PDF/";
        string[] searchWords = new string[args.Length -1]; // Add the words you want to search for
        for (int i = 1; i < args.Length; i++)
        {
           searchWords[i - 1] = args[i];
        }


        PdfSearcher searcher = new PdfSearcher(folderPath, searchWords);
        List<string> files = searcher.Search();

        foreach (string file in files)
        {
            Console.WriteLine(file);
        }
        Console.WriteLine("Done");
        Console.ReadLine();
        Environment.Exit(0);
    }
}

