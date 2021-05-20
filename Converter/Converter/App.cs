
using System.Windows.Forms.VisualStyles;

namespace Converter
{

    class App
    {
        static void Main(string[] args)
        {
            PdfConverter convert = new PdfConverter();
            convert.SetCss(CssConstants.body +
                           CssConstants.img +
                           CssConstants.br +
                           CssConstants.changeTdItem +
                           CssConstants.table +
                           CssConstants.tdTextWidth +
                           CssConstants.tableWidth


                                )
                    .Convert(
                            @"Z:\Загрузки\Акт АБИПА,$.docx",
                            @"C:\Users\Dimond97\Desktop\New folder\file.pdf"
                            );

        }
    }
}

