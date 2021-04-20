
namespace Converter
{

    class App
    {
        static void Main(string[] args)
        {
            PdfConverter convert = new PdfConverter();
            convert.setCSS(" body { width: 35cm;  margin: 1cm auto; max-width: 23cm; padding: 1cm; }" +
                                "img {page-break-before: auto; page-break-after: auto; page-break-inside: avoid; position: relative; }" +
                                "br {page-break-before: always;} .changeTextIntable{font-size:12px;}  " +
                                ".changeTdItem{border:1px solid; height: auto; padding-top:6px; vertical-align:middlle;}"+
                                ".table {   border-collapse: collapse; width: 20cm;}"

                                )
                    .Convert(
                            @"D:\Download\Счет (1).docx",
                            @"D:\Desktop\тесты документов\file.pdf"
                            );

        }
    }
}

