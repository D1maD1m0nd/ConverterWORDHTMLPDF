
namespace Converter
{

    class App
    {
        static void Main(string[] args)
        {
            PdfConverter convert = new PdfConverter();
            convert.setCSS(" body { width: 23cm; margin: 1cm auto; max-width: 23cm; padding: 1cm; }" +
                                "img {page-break-before: auto; page-break-after: auto; page-break-inside: avoid; position: relative; }" +
                                "br {page-break-before: always;} .changeTextIntable{font-size:14px;}  " +
                                ".changeTdItem{border:1px solid; height: auto; padding-top:6px; vertical-align:middlle;}"+
                                ".table {transform: rotate(-90deg); margin-top:47px; margin-bottom:35px; border-collapse: collapse; height: 29.5cm; }"+
                                ".tableFiveElem {transform: rotate(-90deg); margin-top:5px; margin-bottom:5px; border-collapse: collapse; height: 29.5cm; }"
                                )
                    .Convert(
                            @"C:\Users\DimaD1m0nd\Desktop\тесты документов\Отчет комитенту №22 от 04.02.2021.docx",
                            @"C:\Users\DimaD1m0nd\Desktop\тесты документов\file.pdf"
                            );

        }
    }
}

