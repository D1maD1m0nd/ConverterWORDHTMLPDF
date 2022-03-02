using System;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Xml.Linq;
using DocumentFormat.OpenXml.Math;
using DocumentFormat.OpenXml.Packaging;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using OpenXmlPowerTools;
using PdfSharp.Pdf.IO;
using SelectPdf;
using PdfDocument = PdfSharp.Pdf.PdfDocument;
using PdfPageOrientation = HiQPdf.PdfPageOrientation;

namespace Converter
{
    internal class PdfConverter
    {
        private byte[] bytePdfDocument;
        private string CSS;
        private string directory;
        private string HTML;


        public PdfConverter SetCss(string CSS)
        {
            this.CSS = CSS;
            return this;
        }


        /**
         * Конвертирует переданный путь до doc Документа и конвертирует его в пдф
         * 
         * @param path - путь до .doc документа
         * @param pathSaveFile - путь куда необходимо сохранить
         */
        public void Convert(string path, string pathSaveFile)
        {
            const string body = "body { width: 36cm;  margin: 1cm auto; max-width: 36cm; padding: 1cm; }";
            var fileInfo = new FileInfo(path);

            if (!fileInfo.Exists) throw new FileNotFoundException();

            if (fileInfo.Extension != ".docx" && fileInfo.Extension != ".docx") throw new FileFormatException();


            var fullFilePath = fileInfo.FullName;

            try
            {
                HTML = ParseDOCX(fileInfo);
            }
            catch (OpenXmlPackageException e)
            {
                if (e.ToString().Contains("Invalid Hyperlink"))
                {
                    using (var fs = new FileStream(fullFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                    {
                        UriFixer.FixInvalidUri(fs, brokenUri => FixUri(brokenUri));
                    }

                    HTML = ParseDOCX(fileInfo);
                }
            }

            var keys = new[] {"Payer", "Cargo", "Services", "Net", "SWIFT", "general", "Reference"};
            HTML = FormatTable(keys);

            File.WriteAllBytes(pathSaveFile, SaveDocument());
            writeHtmlFile(pathSaveFile);
            
        }

        public PdfConverter writeHtmlFile(string pathSaveFile)
        {
            var writer = File.CreateText(pathSaveFile.Replace(".pdf", ".html"));
            writer.WriteLine(HTML);
            writer.Dispose();
            return this;
        }

        /*
      *@param landScapeMode - отвечает за альбомную ориентацию
      *@param landsacapeHtml - контент для альбомной ореинтации
      *
      *@return - массив байт пдф документа
      */
        private byte[] SaveDocument(string landsacapeHtml = "")
        {
            
            HTML = HTML.Replace(SubstringCssSelector("body"),
                "table { width:27cm } body { width: 36cm;  margin: 1cm auto; max-width: 36cm; padding: 1cm; }" + CssConstants.br);
            var converter = new HtmlToPdf();
            var doc = converter.ConvertHtmlString(HTML);

            byte[] two;
            byte[] one;

            using (var ms = new MemoryStream())
            {
                doc.Save(ms);
                one = ms.ToArray();
                ms.Close();
            }

            doc.Close();
            if (!string.IsNullOrEmpty(landsacapeHtml))
            {
                var htmlToPdfConverter = new HiQPdf.HtmlToPdf();
                htmlToPdfConverter.Document.PageOrientation = PdfPageOrientation.Landscape;
                using (var ms = new MemoryStream())
                {
                    htmlToPdfConverter.ConvertHtmlToStream(landsacapeHtml, null, ms);
                    htmlToPdfConverter.ConvertHtmlToFile(landsacapeHtml, null, "3131.pdf");
                    two = ms.ToArray();
                    ms.Close();
                }

                return concatPdfDoc(one, two);
            }

            return one;
        }

        /**
         * Объединение двух документов в байтовом представлении
         * 
         * @param doc1 - первый документ
         * @param doc2 - второй документ
         * 
         * @return массив байт нового документа
         */
        private byte[] concatPdfDoc(byte[] doc1, byte[] doc2)
        {
            PdfDocument one;
            PdfDocument two;

            //читаем первую часть документа
            using (var ms = new MemoryStream(doc1))
            {
                using (one = PdfReader.Open(ms, PdfDocumentOpenMode.Import))
                {
                }

                ms.Close();
            }

            one.Pages.RemoveAt(2);
            //читаем вторую часть документа
            using (var ms = new MemoryStream(doc2))
            {
                using (two = PdfReader.Open(ms, PdfDocumentOpenMode.Import))
                {
                }
                ms.Close();
            }

            //объединяем документы
            using (var outPdf = new PdfDocument())
            {
                CopyPages(one, outPdf);
                CopyPages(two, outPdf);
                using (var ms = new MemoryStream())
                {
                    outPdf.Save(ms);
                    return ms.ToArray();
                }
            }

            void CopyPages(PdfDocument from, PdfDocument to)
            {
                for (var i = 0; i < from.PageCount; i++) to.AddPage(@from.Pages[i]);
            }
        }

        /**
         * Возвращает новый документ взятый из старого)))
         * 
         * @child узел из пдф документа
         * 
         * @return новую html строку
         */
        private string NewDocument(HtmlNode child)
        {
            var html = new HtmlDocument();
            html.LoadHtml(HTML);
            var document = html.DocumentNode;
            var node = document.QuerySelector("body");
            //clear body
            node.RemoveAll();
            node.AppendChild(child);

            return document.OuterHtml;
        }

        private string SubstringCssSelector(string selctor)
        {
            var start = HTML.IndexOf(selctor, StringComparison.Ordinal);
            var end = HTML.IndexOf("}", start, StringComparison.Ordinal);
            Console.WriteLine(end + " " + start);
            return HTML.Substring(start, end - start + 1);
        }

        /**
         * Альбомная ореинтация под документ счет фактура
         * 
         * @return {string} - возвращает измененную html строку с страницей алтбомного формата отделенную от основного документа
         */
        private string FormatInvoice(bool isKommitent = false)
        {
            var html = new HtmlDocument();
            html.LoadHtml(HTML);
            var document = html.DocumentNode;

            //ищем таблицу страницу с счет фактурой
            var node = searchNode(document, "Счет-фактура", "div");

            if (isKommitent)
            {
                //клонируем текущий обхект, что бы в будущем его использовать и он не выпилился к хуям из кучи
                var cloneNode = node.Clone();
                //удаляем объект счет фактуры из основного документа
                node.Remove();
                //присваиваем измененнннннннный документ в основной
                HTML = document.OuterHtml;
                //создаем новый объект документа для счет фактуры
                html = new HtmlDocument();
                html.LoadHtml(NewDocument(cloneNode));
                document = html.DocumentNode;
                //осуществляем поиск в новом документе
                node = searchNode(document, "Счет-фактура", "div");
            }


            if (node != null)
            {
                var tableNode = node;
                node = searchNode(node, "Сумма", "table");


                if (node != null)
                {
                    InsertClass(tableNode, "table", "table", true);
                    InsertClass(node, "changeTdItem", "td");
                }
            }

            deleteIntoTableTagBR(document);

            return document.OuterHtml;
        }

        /**
         * форматирует таблицу по заданному ключу
         * 
         * @param key {string} -  ключ по которому происходит поиск страницы
         */
        private string FormatTable(string[] keys)
        {
            var html = new HtmlDocument();
            html.LoadHtml(HTML);
            var document = html.DocumentNode;
            Parallel.ForEach(keys, key =>
            {
                var node = searchNode(document, key, "table");
                if (node != null)
                {
                    InsertClass(node.ParentNode, "tableWidth", "table", true);
                    InsertClass(node, "columnTdX", "td");
                    InsertClass(node, "actFormatText", "span");
                }
            });
            
            return document.OuterHtml;
        }
        
        
        


        /**
         * Метод который убирает тег br , от которого идет логика разрыва страницы
         * 
         * @param node - связь , в которой происходит удаление
         * @param tag - тег по которому необходимо построить связь
         * @param delTag - тег, который необходимо удалить
         */
        private void deleteIntoTableTagBR(HtmlNode node)
        {
            var htmlArr = node.QuerySelectorAll("table");
            foreach (var item in htmlArr)
            {
                var node1 = item.QuerySelectorAll("span").ToArray();
                for (var i = 0; i < node1.Length; i++)
                    if (node1[i].InnerHtml.Contains("<br>"))
                        node1[i].Remove();
            }
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
        private HtmlNode searchNode(HtmlNode document, string keySearchValue,
            string tag)
        {
            var htmlArr = document.QuerySelectorAll(tag);
            HtmlNode node = null;
            Parallel.ForEach(htmlArr, (item, state) =>
            {
                if (item.InnerText.IndexOf(keySearchValue, StringComparison.Ordinal) != -1)
                {
                    node = item;
                    state.Break();
                }
                    
            });
            return node;
        }

        /**
         * Вставляет в тег определенный класс
         *  
         * @param node - связь в которйо необходимо произвести изменения
         * @param currendClass - связь , который вставляем
         * @param tag тег, чью связь необходимо найти 
         * @param exp выражение, по которому необходимо найти элемент
         * @param флаг указывающий на количество проходов по массиву
         */
        private PdfConverter InsertClass(HtmlNode node, string currentClass, string tag,
            bool firstElem = false)
        {
            var nodes = node.QuerySelectorAll(tag);
            if (nodes == null)
                return this;

            if (firstElem)
            {
                nodes.First().Attributes["class"].Value = currentClass;
                return this;
            }

            foreach (var item in nodes)
                item.Attributes["class"].Value = currentClass;

            return this;
        }

        private Uri FixUri(string brokenUri)
        {
            var newURI = string.Empty;
            if (brokenUri.Contains("mailto:"))
            {
                var mailToCount = "mailto:".Length;
                brokenUri = brokenUri.Remove(0, mailToCount);
                newURI = brokenUri;
            }
            else
            {
                newURI = " ";
            }

            return new Uri(newURI);
        }

        private string ParseDocx(byte[] byteArray, string nameDoc = "Name", string fullName = "fullName")
        {
            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    memoryStream.Write(byteArray, 0, byteArray.Length);
                    using (var wDoc =
                        WordprocessingDocument.Open(memoryStream, true))
                    {
                        var pageTitle = nameDoc;
                        var part = wDoc.CoreFilePropertiesPart;
                        if (part != null)
                            pageTitle = (string) part.GetXDocument()
                                .Descendants(DC.title)
                                .FirstOrDefault() ?? fullName;
                        var spanElem = "";
                        //обработка шапки для изменения наложения элементов
                        for (var i = 0; i < 10; i++) spanElem += $"span.pt-a0-00000{i}" + "{margin-right:40px;}";

                        var settings = new WmlToHtmlConverterSettings
                        {
                            AdditionalCss = CSS + $"{spanElem}",

                            PageTitle = pageTitle,
                            FabricateCssClasses = true,
                            CssClassPrefix = "pt-",
                            RestrictToSupportedLanguages = false,
                            RestrictToSupportedNumberingFormats = false,
                            ImageHandler = imageInfo =>
                            {
                                var extension = imageInfo.ContentType.Split('/')[1].ToLower();
                                ImageFormat imageFormat = null;
                                if (extension == "png")
                                {
                                    imageFormat = ImageFormat.Png;
                                }
                                else if (extension == "gif")
                                {
                                    imageFormat = ImageFormat.Gif;
                                }
                                else if (extension == "bmp")
                                {
                                    imageFormat = ImageFormat.Bmp;
                                }
                                else if (extension == "jpeg")
                                {
                                    imageFormat = ImageFormat.Jpeg;
                                }
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
                                    using (var ms = new MemoryStream())
                                    {
                                        imageInfo.Bitmap.Save(ms, imageFormat);
                                        var ba = ms.ToArray();
                                        base64 = System.Convert.ToBase64String(ba);
                                    }
                                }
                                catch (ExternalException)
                                {
                                    return null;
                                }

                                var format = imageInfo.Bitmap.RawFormat;
                                var codec = ImageCodecInfo.GetImageDecoders()
                                    .First(c => c.FormatID == format.Guid);
                                var mimeType = codec.MimeType;

                                var imageSource =
                                    string.Format("data:{0};base64,{1}", mimeType, base64);

                                var img = new XElement(Xhtml.img,
                                    new XAttribute(NoNamespace.src, imageSource),
                                    imageInfo.ImgStyleAttribute,
                                    imageInfo.AltText != null
                                        ? new XAttribute(NoNamespace.alt, imageInfo.AltText)
                                        : null);
                                return img;
                            }
                        };

                        var htmlElement = WmlToHtmlConverter.ConvertToHtml(wDoc, settings);
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

        private string ParseDOCX(FileInfo fileInfo)
        {
            try
            {
                var byteArray = File.ReadAllBytes(fileInfo.FullName);
                using (var memoryStream = new MemoryStream())
                {
                    memoryStream.Write(byteArray, 0, byteArray.Length);
                    using (var wDoc =
                        WordprocessingDocument.Open(memoryStream, true))
                    {
                        var pageTitle = fileInfo.FullName;
                        var part = wDoc.CoreFilePropertiesPart;
                        if (part != null)
                            pageTitle = (string) part.GetXDocument()
                                .Descendants(DC.title)
                                .FirstOrDefault() ?? fileInfo.FullName;
                        var spanElem = "";
                        //обработка шапки для изменения наложения элементов
                        //for (var i = 0; i < 10; i++) spanElem += $"span.pt-a0-00000{i}" + "{margin-right:40px;}";

                        CSS += spanElem;
                        var settings = new WmlToHtmlConverterSettings
                        {
                            AdditionalCss = CSS,
                            PageTitle = pageTitle,
                            FabricateCssClasses = true,
                            CssClassPrefix = "pt-",
                            RestrictToSupportedLanguages = false,
                            RestrictToSupportedNumberingFormats = false,
                            ImageHandler = imageInfo =>
                            {
                                var extension = imageInfo.ContentType.Split('/')[1].ToLower();
                                ImageFormat imageFormat = null;
                                if (extension == "png")
                                {
                                    imageFormat = ImageFormat.Png;
                                }
                                else if (extension == "gif")
                                {
                                    imageFormat = ImageFormat.Gif;
                                }
                                else if (extension == "bmp")
                                {
                                    imageFormat = ImageFormat.Bmp;
                                }
                                else if (extension == "jpeg")
                                {
                                    imageFormat = ImageFormat.Jpeg;
                                }
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
                                    using (var ms = new MemoryStream())
                                    {
                                        imageInfo.Bitmap.Save(ms, imageFormat);
                                        var ba = ms.ToArray();
                                        base64 = System.Convert.ToBase64String(ba);
                                    }
                                }
                                catch (ExternalException)
                                {
                                    return null;
                                }

                                var format = imageInfo.Bitmap.RawFormat;
                                var codec = ImageCodecInfo.GetImageDecoders()
                                    .First(c => c.FormatID == format.Guid);
                                var mimeType = codec.MimeType;

                                var imageSource =
                                    $"data:{mimeType};base64,{base64}";

                                var img = new XElement(Xhtml.img,
                                    new XAttribute(NoNamespace.src, imageSource),
                                    imageInfo.ImgStyleAttribute,
                                    imageInfo.AltText != null
                                        ? new XAttribute(NoNamespace.alt, imageInfo.AltText)
                                        : null);
                                return img;
                            }
                        };

                        var htmlElement = WmlToHtmlConverter.ConvertToHtml(wDoc, settings);
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