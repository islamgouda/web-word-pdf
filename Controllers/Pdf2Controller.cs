using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using ImageMagick;
using Tesseract;

namespace YourNamespace.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Pdf2Controller : ControllerBase
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

            using (var pdf = PdfReader.Open(filePath, PdfDocumentOpenMode.Import))
            {
                for (int page = 0; page < pdf.PageCount; page++)
                {
                    using (var ms = new MemoryStream())
                    {
                        // Convert PDF page to image
                        using (var image = new MagickImage())
                        {
                            image.Read(filePath + "[" + page + "]", new MagickReadSettings { Density = new Density(300) });
                            image.Write(ms, MagickFormat.Png);
                        }

                        ms.Position = 0;

                        // Perform OCR on the image
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

            return text.ToString();
        }
    }
}
