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
            var html = new HtmlDocument();
            html.LoadHtml(htmlText);
            var document = html.DocumentNode;

            // yields: [<p class="content">Fizzler</p>]

            Console.WriteLine(document.QuerySelectorAll("table").Count());
            foreach (var i in document.QuerySelectorAll("table").ToList())
            {
                foreach(var k in i.QuerySelectorAll("span").ToList())
                {
                    k.AddClass("6564");
                    Console.WriteLine(k.OuterHtml);

                }
            }

            
            Console.ReadKey();
        }
    }
}
