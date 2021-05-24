namespace Converter
{
    internal class App
    {
        private static void Main(string[] args)
        {
            var convert = new PdfConverter();
            convert.SetCss(CssConstants.body +
                           CssConstants.img +
                           CssConstants.br +
                           CssConstants.changeTdItem +
                           CssConstants.table +
                           CssConstants.tdTextWidth +
                           CssConstants.tableWidth
                )
                .Convert(
                    @"D:\Download\Отчет комитенту №13 от 21.01.2021.docx",
                    @"D:\Desktop\тесты документов\file.pdf"
                );
        }
    }
}