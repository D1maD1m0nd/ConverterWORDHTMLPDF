﻿using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using DocumentFormat.OpenXml.Packaging;
using EO.Pdf;
using OpenXmlPowerTools;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;

namespace Converter
{
    class CoordinateTag
    {
        public List<int> start;
        public List<int> end;
    }
    class Program
    {
        static void Main(string[] args)
        {
            var fileInfo = new FileInfo(@"D:\Download\Отчет комитенту №44 от 02.03.2021.docx");
            string fullFilePath = fileInfo.FullName;
            string htmlText = string.Empty;
            try
            {
                htmlText = ParseDOCX(fileInfo);
            }
            catch (OpenXmlPackageException e)
            {
                if (e.ToString().Contains("Invalid Hyperlink"))
                {
                    using (FileStream fs = new FileStream(fullFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                    {
                        UriFixer.FixInvalidUri(fs, brokenUri => FixUri(brokenUri));
                    }
                    htmlText = ParseDOCX(fileInfo);
                }
            }
            var html = new HtmlAgilityPack.HtmlDocument();
            html.LoadHtml(htmlText);
            var document = html.DocumentNode;
            var node = searchNode(document, "Счет-фактура", "div");

            
            Console.WriteLine(node.OuterHtml);

            Console.WriteLine(htmlText.IndexOf("lrm"));
            //string testHtml = "<table> <td> <td> <td> <br> <eqwew<>ewqewqeQW<> </table>";
            htmlText = fixSubTag(htmlText, "<table", "</table", "<br");
            var idxes = searchAllOccurrences(htmlText, "<table");
            //меняем положение таблицы
            htmlText = insertClass("table", "table", htmlText, idxes.Count - 2);
            //меняем шрифт
            htmlText = insertClass("changeTextIntable", "table", "span", htmlText, idxes.Count - 2);
            //меняем размер ячейки и их обводку
            htmlText = insertClass("changeTdItem", "table", "td", htmlText, idxes.Count - 2);

            
           // htmlText = insertClass("changeTextInTable", "table", "p", htmlText, idxes.Count - 2); 
            using (MemoryStream ms = new MemoryStream())
            {
                //string html, string pdfFileName,

                var a = EO.Pdf.HtmlToPdf.ConvertHtml(htmlText.ToString(), "file99.pdf");
                var c = ms.ToArray();
       

            }

            var writer = File.CreateText("html2.html");
            
            
           
            writer.WriteLine(htmlText);
            writer.Dispose();
            Console.ReadKey();

            
        }
        static HtmlAgilityPack.HtmlNode searchNode(HtmlAgilityPack.HtmlNode document, string keySearchValue, string tag)
        {
            var htmlArr = document.QuerySelectorAll(tag).ToArray();

            foreach(var item in htmlArr)
            {
                if(item.InnerText.IndexOf(keySearchValue) != -1)
                {
                    return item;
                }
            }
            return null;
        }
        static string insertClass(string currentClass, string node,string subNode,string htmlText, int numReplaceElem)
        {
            if (numReplaceElem <= 0) return htmlText;

            var html = new HtmlAgilityPack.HtmlDocument();
            html.LoadHtml(htmlText);
            var document = html.DocumentNode;
            var arrHtml = document.QuerySelectorAll(node).ToArray();

            if (numReplaceElem <= arrHtml.Length - 1)
            {
                var arrSubTags = arrHtml[numReplaceElem].QuerySelectorAll(subNode).ToArray();
                for(int i = 0; i < arrSubTags.Length; i++)
                {
                    
                    var attribArr = arrSubTags[i].Attributes.ToArray();
                    for (int k = 0; k < attribArr.Length; k++)
                    {

                        if (attribArr[k].Name == "class")
                        {
                            attribArr[k].Value = currentClass;
                           
                        }
                    }
                }
               
                
            }
            return document.OuterHtml;
        }
        static string insertClass(string currentClass, string node, string htmlText, int numReplaceElem)
        {
            if (numReplaceElem <= 0) return htmlText;

            var html = new HtmlAgilityPack.HtmlDocument();
            html.LoadHtml(htmlText);
            var document = html.DocumentNode;
            var arrHtml = document.QuerySelectorAll(node).ToArray();

            if(numReplaceElem <= arrHtml.Length - 1)
            {
                var attribArr = arrHtml[numReplaceElem].Attributes.ToArray();
                for(int i = 0; i < attribArr.Length; i++)
                {
                        
                    if(attribArr[i].Name == "class")
                    {
                            
                        attribArr[i].Value = currentClass;
                    }
                }
            }
            return document.OuterHtml;

        }
        public static HtmlAgilityPack.HtmlNode searchNode(string reqText, string tag, string htmlText)
        {
            var html = new HtmlAgilityPack.HtmlDocument();
            html.LoadHtml(htmlText);
            var document = html.DocumentNode;
            var arrHtml = document.QuerySelectorAll(tag).ToArray();
            for(int i = 0; i < arrHtml.Length; i++)
            {
                if (arrHtml[i].OuterHtml.IndexOf("Счет-фактура") != -1)
                {
                    return arrHtml[i];
                }
            }
            return null;
        }

        public static string insertClass(string currentClass, string node, string htmlText)
        {
            var html = new HtmlAgilityPack.HtmlDocument();
            html.LoadHtml(htmlText);
            var document = html.DocumentNode;
            var arrHtml = document.QuerySelectorAll(node).ToArray();
            for (int i = 0; i < arrHtml.Length; i++)
            {
                arrHtml[i].Attributes[1].Value = currentClass;

            }
            return document.OuterHtml;
        }
        public static void insertClassIntoTableElem(string text, string tag, string css)
        {
            CoordinateTag coordinate = searchAllOccurrences(text, "<table", "</table");
            int startLast = coordinate.start[coordinate.start.Count - 2];
            int endLast = coordinate.end[coordinate.end.Count - 2];
            List<int> tags = searchAllOccurrences(text, tag, startLast, endLast);
            foreach(int tagItem in tags)
            {
                Console.WriteLine(tagItem);
            }
            Console.WriteLine("Длинна отфильтрованных вхождений" + tags.Count);

        }
       
        public static string fixSubTag(string text, string str1, string str2, string delStr)
        {
            // если входит в диапозон то удаляем
            int delElem = text.IndexOf(delStr);

            var indices = searchAllOccurrences(text, str1);

            var indices1 = searchAllOccurrences(text, str2);


            for (int i = 0; i < indices.Count; i++)
            {
                if (indices[i] < delElem && indices1[i] > delElem)
                {
                    text = text.Remove(delElem, delStr.Length);
                    return text;
                }
            }

            return text;
        }
        /*
         * Поиск всех вхождений подстроки в строку
         * @param text - строка где производится поиск
         * @param searchStr - строка , которую необходимо найти
         * 
         * @return список вхождений строки 
         */
        public static List<int> searchAllOccurrences(string text, string searchStr)
        {
            List<int> indices = new List<int>();
            int index = text.IndexOf(searchStr, 0);
            while (index > -1)
            {
                indices.Add(index);
                index = text.IndexOf(searchStr, index + searchStr.Length);
            }
            return indices;
        }

        /*
         * Поиск начала и конца тега
         * @param text - строка где производится поиск
         * @param searchStrStart - начало строки
         * @param searchStrEnd-2 - конец строки
         * 
         * @return CoordinateTag с свойствами старта и конца, два параллельных списка
         */
        public static CoordinateTag searchAllOccurrences(string text, string searchStrStart, string searchStrEnd)
        {
            var coordinate = new CoordinateTag();
            coordinate.start = searchAllOccurrences(text, searchStrStart);
            coordinate.end = searchAllOccurrences(text, searchStrEnd);
            return coordinate;
        }

        /*
         * Поиск все вхождений в заданном диапозоне
         * @param text - строка где производится поиск
         * @param tag - тег чьи вхождения ищим
         * @param coordinate1-2 - диапозон поиска
         * 
         * @return список найденных тегов в заданном диапозоне
         */
        public static List<int> searchAllOccurrences(string text, string tag,  int cooridinate1, int coordinate2)
        {
            
            List<int> coordinateTag = searchAllOccurrences(text, tag);
            List<int> results = new List<int>();

            for(int i = 0; i < coordinateTag.Count; i++)
            {
                if (coordinateTag[i] > cooridinate1 && coordinateTag[i] < coordinate2)
                {
                        results.Add(coordinateTag[i]);
                }
            }
           
            return results;
        }
        public static Uri FixUri(string brokenUri)
        {
            string newURI = string.Empty;
            if (brokenUri.Contains("mailto:"))
            {
                int mailToCount = "mailto:".Length;
                brokenUri = brokenUri.Remove(0, mailToCount);
                newURI = brokenUri;
            }
            else
            {
                newURI = " ";
            }
            return new Uri(newURI);
        }

        public static string ParseDOCX(FileInfo fileInfo)
        {
            try
            {
                byte[] byteArray = File.ReadAllBytes(fileInfo.FullName);
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    memoryStream.Write(byteArray, 0, byteArray.Length);
                    using (WordprocessingDocument wDoc =
                                                WordprocessingDocument.Open(memoryStream, true))
                    {
                        int imageCounter = 0;
                        var pageTitle = fileInfo.FullName;
                        var part = wDoc.CoreFilePropertiesPart;
                        if (part != null)
                            pageTitle = (string)part.GetXDocument()
                                                    .Descendants(DC.title)
                                                    .FirstOrDefault() ?? fileInfo.FullName;
                        string spanElem = "";
                        //обработка шапки для изменения наложения элементов
                        for (int i = 0; i < 10; i++)
                        {
                            spanElem += $"span.pt-a0-00000{i}" + "{margin-right:40px;}";
                        }

                        WmlToHtmlConverterSettings settings = new WmlToHtmlConverterSettings()
                        {
                            AdditionalCss = " body { width: 21cm; margin: 1cm auto; max-width: 21cm; padding: 1cm; }" +
                                "img {page-break-before: auto; page-break-after: auto; page-break-inside: avoid; position: relative; }" +
                                "br {page-break-before: always;} .changeTextIntable{font-size:14px;}  .changeTdItem{border:1px solid; height: auto; padding-top:6px; vertical-align:middlle;}" +
                                ".table {transform: rotate(-90deg);" +
                                    "margin-top:20px;" +
                                    "margin-bottom:20px;" +
                                    "border-collapse: collapse;" +
                                    "height: 30cm; }" 
                                + $"{spanElem}",

                            PageTitle = pageTitle,
                            FabricateCssClasses = true,
                            CssClassPrefix = "pt-",
                            RestrictToSupportedLanguages = false,
                            RestrictToSupportedNumberingFormats = false,
                            ImageHandler = imageInfo =>
                            {
                                ++imageCounter;
                                string extension = imageInfo.ContentType.Split('/')[1].ToLower();
                                ImageFormat imageFormat = null;
                                if (extension == "png") imageFormat = ImageFormat.Png;
                                else if (extension == "gif") imageFormat = ImageFormat.Gif;
                                else if (extension == "bmp") imageFormat = ImageFormat.Bmp;
                                else if (extension == "jpeg") imageFormat = ImageFormat.Jpeg;
                                else if (extension == "tiff")
                                {
                                    extension = "gif";
                                    imageFormat = ImageFormat.Gif;
                                }
                                else if (extension == "x-wmf")
                                {
                                    extension = "wmf";
                                    imageFormat = ImageFormat.Wmf;
                                }

                                if (imageFormat == null) return null;

                                string base64 = null;
                                try
                                {
                                    using (MemoryStream ms = new MemoryStream())
                                    {
                                        imageInfo.Bitmap.Save(ms, imageFormat);
                                        var ba = ms.ToArray();
                                        base64 = System.Convert.ToBase64String(ba);
                                    }
                                }
                                catch (System.Runtime.InteropServices.ExternalException)
                                { return null; }

                                ImageFormat format = imageInfo.Bitmap.RawFormat;
                                ImageCodecInfo codec = ImageCodecInfo.GetImageDecoders()
                                                            .First(c => c.FormatID == format.Guid);
                                string mimeType = codec.MimeType;

                                string imageSource =
                                        string.Format("data:{0};base64,{1}", mimeType, base64);

                                XElement img = new XElement(Xhtml.img,
                                        new XAttribute(NoNamespace.src, imageSource),
                                        imageInfo.ImgStyleAttribute,
                                        imageInfo.AltText != null ?
                                            new XAttribute(NoNamespace.alt, imageInfo.AltText) : null);
                                return img;
                            }
                        };

                        XElement htmlElement = WmlToHtmlConverter.ConvertToHtml(wDoc, settings);
                        var html = new XDocument(new XDocumentType("html", null, null, null),
                                                                                    htmlElement);
                        var htmlString = html.ToString(SaveOptions.DisableFormatting);
                        return htmlString;
                    }
                }
            }
            catch
            {
                return "The file is either open, please close it or contains corrupt data";
            }
        }
    }
}
