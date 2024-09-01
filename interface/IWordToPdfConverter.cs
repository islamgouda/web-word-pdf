namespace web_word_pdf
{
    public interface IWordToPdfConverter
    {
        byte[] ConvertWordToPdf(Stream wordStream);
    }
}
