using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DinkToPdf;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;

namespace Converter2
{
    class Program
    {
        static void Main(string[] args)
        {
            string htmlText = string.Empty;
            using (StreamReader sw = new StreamReader(@"D:\Download\Converter (1)\Converter\Converter\bin\Debug\html2.html"))
            {
                htmlText = sw.ReadToEnd();
            }
            insertClass("dsads", "table", htmlText);



            Console.ReadKey();
        }

        static void insertClass(string currentClass, string node, string htmlText)
        {
            var html = new HtmlDocument();
            html.LoadHtml(htmlText);
            var document = html.DocumentNode;
            var arrHtml = document.QuerySelectorAll(node).ToArray();
            
            for (int i = 0; i < arrHtml.Length; i++)
            {
                arrHtml[i].Attributes[1].Value = "claaaaaaaaaaaaaaaas";
                arrHtml[i].InnerHtml = "123";
            }
            Console.WriteLine(document.OuterHtml);
        }
    }
}
