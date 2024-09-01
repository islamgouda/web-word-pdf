using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tesseract;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

namespace YourNamespace.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PdfController : ControllerBase
    {
        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            // Save the file to a temporary location
            var filePath = Path.GetTempFileName();
            using (var stream = System.IO.File.Create(filePath))
            {
                await file.CopyToAsync(stream);
            }

            // Extract text from the PDF
            var extractedText = ExtractTextFromPdf(filePath);

            // Clean up the temporary file
            System.IO.File.Delete(filePath);

            return Ok(new { text = extractedText });
        }

        private string ExtractTextFromPdf(string filePath)
        {
            StringBuilder text = new StringBuilder();

            using (var document = PdfDocument.Open(filePath))
            {
                foreach (var page in document.GetPages())
                {
                    text.AppendLine(page.Text);

                    // If the text is not extracted or if the page contains images, use OCR
                    if (!string.IsNullOrEmpty(page.Text))
                    {
                        using (var engine = new TesseractEngine(@"./tessdata", "ara", EngineMode.Default))
                        {
                            using (var pix = Pix.LoadFromFile(filePath))
                            {
                                using (var page1 = engine.Process(pix))
                                {
                                    text.AppendLine(page1.GetText());
                                }
                            }
                        }
                    }
                }
            }

            return text.ToString();
        }


        //private string ExtractTextFromPdf(string filePath)
        //{
        //    StringBuilder text = new StringBuilder();

        //    // Use PdfPig to extract text
        //    using (var document = PdfDocument.Open(filePath))
        //    {
        //        foreach (var page in document.GetPages())
        //        {
        //            text.AppendLine(page.Text);
        //        }
        //    }

        //    return text.ToString();
        //}
    }
}
