
using System.Text;

using Microsoft.AspNetCore.Mvc;
using PdfiumViewer;
using Tesseract;

namespace YourNamespace.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Pdf3Controller : ControllerBase
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

            // Extract text from the PDF using OCR
            var extractedText = ExtractTextFromPdf(filePath);

            // Clean up the temporary file
            System.IO.File.Delete(filePath);

            return Ok(new { text = extractedText });
        }

        private string ExtractTextFromPdf(string filePath)
        {
            StringBuilder text = new StringBuilder();

            using (var pdfDocument = PdfDocument.Load(filePath))
            {
                for (int page = 0; page < pdfDocument.PageCount; page++)
                {
                    using (var pageImage = pdfDocument.Render(page, 300, 300, PdfRenderFlags.CorrectFromDpi))
                    {
                        using (var ms = new MemoryStream())
                        {
                            pageImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                            ms.Position = 0;

                            using (var engine = new TesseractEngine(@"./tessdata", "ara", EngineMode.Default))
                            {
                                using (var img = Pix.LoadFromMemory(ms.ToArray()))
                                {

                                    using (var pageOcr = engine.Process(img))
                                    {
                                        text.AppendLine(pageOcr.GetText());
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return text.ToString();
        }
    }
}
