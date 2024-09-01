using Microsoft.Office.Interop.Word;
using System.IO;
using web_word_pdf;

public class WordToPdfConverter : IWordToPdfConverter
{
    private readonly Application _wordApp;

    public WordToPdfConverter()
    {
        _wordApp = new Application();
        // Optionally, make the Word application invisible
        _wordApp.Visible = false;
    }

    public byte[] ConvertWordToPdf(Stream wordStream)
    {
        // Generate unique temp file paths
        string tempWordPath = Path.GetTempFileName();
        string tempPdfPath = Path.ChangeExtension(tempWordPath, ".pdf");

        try
        {
            // Save the stream to a file
            using (var fileStream = new FileStream(tempWordPath, FileMode.Create))
            {
                wordStream.CopyTo(fileStream);
            }

            // Open the Word document
            Document wordDocument = _wordApp.Documents.Open(tempWordPath);

            // Convert to PDF
            wordDocument.ExportAsFixedFormat(tempPdfPath, WdExportFormat.wdExportFormatPDF);

            // Close the document
            wordDocument.Close();

            // Read and return the PDF file as a byte array
            return File.ReadAllBytes(tempPdfPath);
        }
        finally
        {
            // Clean up temporary files
            File.Delete(tempWordPath);
            File.Delete(tempPdfPath);
        }
    }
}
