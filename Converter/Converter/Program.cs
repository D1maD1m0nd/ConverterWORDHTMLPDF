using System;
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

            if (htmlText.IndexOf("Счет-фактура") != -1)
            {
                var html = new HtmlAgilityPack.HtmlDocument();
                html.LoadHtml(htmlText);
                var document = html.DocumentNode;
                //ищем таблицу с счет фактурой
                var node = searchNode(document, "Счет-фактура", "div");
                if(node != null)
                {
                    //вставляем класс table что бы ее перевернуть
                    insertClass(node, "table", "table", "Сумма", true);
                    node = searchNode(node, "Сумма", "table");
                    if(node != null)
                    {
                        //меняем шрифт
                        insertClass(node, "changeTextIntable", "span");
                        //меняем размер ячейки и их обводку
                        insertClass(node, "changeTdItem", "td");

                        deleteTag(document, "table", "br");
                        htmlText = document.OuterHtml;
                    }
                    
                }
                
            }

            using (MemoryStream ms = new MemoryStream())
            {
                var a = EO.Pdf.HtmlToPdf.ConvertHtml(htmlText.ToString(), "file99.pdf");
                var c = ms.ToArray();
       

            }

            var writer = File.CreateText("html2.html");
            
            
           
            writer.WriteLine(htmlText);
            writer.Dispose();
            Console.ReadKey();

            
        }

        /**
         * Ищет экземпляр Node в document по
         * @param document - исходный объект документа
         * @param keySearchValue -  текстовый ключ, по которому необходимо найти элемент, по типу в теге содержится слово счет фактура
         * @param tag - тег, чей объект необходимо найти, указывать нужно без дополнительных знаков, 
         * пример правильного запроса QuerySelectorAll("br")
         * 
         * @return {HtmlAgilityPack.HtmlNode} возвращает найденный экземпляр HtmlNode
         */
        private static HtmlAgilityPack.HtmlNode searchNode(HtmlAgilityPack.HtmlNode document, string keySearchValue, string tag)
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
        /**
         * Удаляет тег в определенной свяи
         * 
         * @param node - связь , в которой происходит удаление
         * @param tag - тег по которому необходимо построить связь
         * @param delTag - тег, который необходимо удалить
         */
        private static void deleteTag(HtmlAgilityPack.HtmlNode node, string tag, string delTag)
        {
            var htmlArr = node.QuerySelectorAll(tag).ToArray();
            foreach(var item in htmlArr)
            {
                item.OuterHtml.Replace(delTag, string.Empty);
            }
            
        }
        /**
         * Вставляет в тег определенный класс
         * 
         *@param node - связь в которйо необходимо произвести изменения
         *@param currendClass - связь , который вставляем
         *@param tag тег, чью связь необходимо найти 
         *@param exp выражение, по которому необходимо найти элемент
         *@param флаг указывающий на количество проходов по массиву
         */
        private static void insertClass(HtmlAgilityPack.HtmlNode node, string currentClass, string tag, string exp = null, bool oneIter = false) {
            var htmlArr = node.QuerySelectorAll(tag).ToArray();
            
            foreach(var item in htmlArr)
            {
                if(exp == null || item.OuterHtml.IndexOf(exp) != -1)
                {
                    foreach (var itemAttrib in item.Attributes)
                    {
                            
                        if (itemAttrib.Name == "class")
                        {

                            itemAttrib.Value = currentClass;
                            Console.WriteLine(item.OuterHtml);
                            if(oneIter)
                                return;
                        }
                    }
                }
                    
            }

        }

        private static Uri FixUri(string brokenUri)
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

        private static string ParseDOCX(FileInfo fileInfo)
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
