using System;
using System.Collections.Generic;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

public class PdfSearcher
{
    private string _folderPath;
    private string[] _searchWords;

    public PdfSearcher(string folderPath, string[] searchWords)
    {
        _folderPath = folderPath;
        _searchWords = searchWords;
    }

    public List<string> Search()
    {
        List<string> filesContainingWords = new List<string>();

        // Check if the folder exists
        if (Directory.Exists(_folderPath))
        {
            // Get all PDF files in the folder
            string[] pdfFiles = Directory.GetFiles(_folderPath, "*.pdf");

            foreach (string pdfFile in pdfFiles)
            {
                if (PdfContainsWords(pdfFile, _searchWords))
                {
                    filesContainingWords.Add(pdfFile);
                }
            }
        }
        else
        {
            Console.WriteLine("Folder does not exist.");
        }

        return filesContainingWords;
    }

    private bool PdfContainsWords(string filePath, string[] words)
    {
        try
        {
            using (PdfReader reader = new PdfReader(filePath))
            {
                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    string text = PdfTextExtractor.GetTextFromPage(reader, i, new LocationTextExtractionStrategy());
                    foreach (string word in words)
                    {
                        if (text.Contains(word, StringComparison.OrdinalIgnoreCase))
                        {
                            return true;
                        }
                    }
                }
            }
        }
        catch (NotSupportedException ex)
        {
            Console.WriteLine($"Error processing file '{filePath}': {ex.Message}");
        }
        return false;
    }


}
