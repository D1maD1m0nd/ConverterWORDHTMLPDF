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
                    @"D:\Download\Акт АБИПА,$.docx",
                    @"D:\Desktop\тесты документов\file.pdf"
                );
        }
    }
}