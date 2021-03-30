
namespace Converter
{
 
    class App
    {
        static void Main(string[] args)
        {
            PdfConverter convert = new PdfConverter();
            convert.Convert(@"C:\Users\DimaD1m0nd\Desktop\тесты документов\Отчет комитенту №22 от 04.02.2021.docx", @"C:\Users\DimaD1m0nd\Desktop\тесты документов\file.pdf");
        }
     
    }
}

