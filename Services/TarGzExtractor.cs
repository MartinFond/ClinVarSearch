using System;
using System.IO;
using SharpCompress.Common;
using SharpCompress.Readers;

public class TarGzExtractor
{
    public void ExtractAndRenamePdfFile(string sourceArchivePath, string destinationFolderPath, string newFileName)
    {
        try
        {
            using (var stream = File.OpenRead(sourceArchivePath))
            using (var reader = ReaderFactory.Open(stream))
            {
                while (reader.MoveToNextEntry())
                {
                    if (!reader.Entry.IsDirectory && reader.Entry.Key.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                    {
                        string destinationFilePath = Path.Combine(destinationFolderPath, newFileName);
                        reader.WriteEntryToFile(destinationFilePath, new ExtractionOptions()
                        {
                            ExtractFullPath = true,
                            Overwrite = true
                        });
                        Console.WriteLine($"PDF file extracted and renamed to '{newFileName}'.");
                        return;
                    }
                }
                Console.WriteLine("No PDF file found in the archive.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
}




