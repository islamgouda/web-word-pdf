using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using web_word_pdf;

namespace WordToPdfApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Convert2Controller : ControllerBase
    {
        private readonly WordConversionService _wordConversionService;

        public Convert2Controller(WordConversionService wordConversionService)
        {
            _wordConversionService = wordConversionService;
        }

        [HttpPost("word-to-pdf")]
        public IActionResult ConvertWordToPdf(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            string tempPath = Path.GetTempFileName();

            try
            {
                using (var stream = new FileStream(tempPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                byte[] pdfBytes = _wordConversionService.ConvertWordToPdf(tempPath);
                return File(pdfBytes, "application/pdf", "converted.pdf");
            }
            finally
            {
                System.IO.File.Delete(tempPath);
            }
        }
    }
}
