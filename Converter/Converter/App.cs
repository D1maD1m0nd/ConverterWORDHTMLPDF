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
                           CssConstants.tableWidth
                )
                .Convert(
                    @"Z:\Загрузки\Отчет комитенту №85 от 02.02.2021 (2).docx",
                    @"C:\Users\Dimond97\Desktop\test\fileTest.pdf"
                );
        }
    }
}