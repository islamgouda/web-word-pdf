using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Office.Interop.Word;
using Application = Microsoft.Office.Interop.Word.Application;

namespace WordToPdfApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConvertController : ControllerBase
    {
        [HttpPost("word-to-pdf")]
        public IActionResult ConvertWordToPdf(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            string tempPath = Path.GetTempFileName();
            string outputPath = Path.ChangeExtension(tempPath, ".pdf");

            try
            {
                using (var stream = new FileStream(tempPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                Application wordApp = new();
                Microsoft.Office.Interop.Word.Document wordDocument = wordApp.Documents.Open(tempPath);

                wordDocument.ExportAsFixedFormat(outputPath, WdExportFormat.wdExportFormatPDF);

                wordDocument.Close();
                wordApp.Quit();

                byte[] pdfBytes = System.IO.File.ReadAllBytes(outputPath);
                return File(pdfBytes, "application/pdf", "converted.pdf");
            }
            finally
            {
                System.IO.File.Delete(tempPath);
                System.IO.File.Delete(outputPath);
            }
        }
    }
}
