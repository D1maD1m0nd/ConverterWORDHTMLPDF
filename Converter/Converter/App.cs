namespace Converter
{
    internal class App
    {
        private static void Main(string[] args)
        {
            var convert = new PdfConverter();
            convert.SetCss(CssConstants.body +
                           CssConstants.img +
                           CssConstants.changeTdItem +
                           CssConstants.table +
                           CssConstants.tdTextWidth +
                           CssConstants.tableWidth +
                           CssConstants.imageRight +
                           CssConstants.logoContainer
                )
                .Convert(
                    @"C:\Users\dima\Desktop\test\Invoice_№ABLG42_22_от_16.02.2022_1.docx",
                    @"C:\Users\dima\Desktop\test\Invoice_№ABLG42_22_от_16.02.2022_11.pdf"
                );
        }
    }
}