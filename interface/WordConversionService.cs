using Microsoft.Office.Interop.Word;
using System;
using System.IO;

public class WordConversionService : IDisposable
{
    private readonly Application _wordApp;

    public WordConversionService()
    {
        _wordApp = new Application();
    }

    public byte[] ConvertWordToPdf(string inputFilePath)
    {
        string outputFilePath = Path.ChangeExtension(inputFilePath, ".pdf");

        try
        {
            Document wordDocument = _wordApp.Documents.Open(inputFilePath);
            wordDocument.ExportAsFixedFormat(outputFilePath, WdExportFormat.wdExportFormatPDF);
            wordDocument.Close();

            return File.ReadAllBytes(outputFilePath);
        }
        finally
        {
            File.Delete(outputFilePath);
        }
    }

    public void Dispose()
    {
        _wordApp.Quit();
        System.Runtime.InteropServices.Marshal.ReleaseComObject(_wordApp);
    }
}
