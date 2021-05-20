﻿using System;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using DocumentFormat.OpenXml.Packaging;
using OpenXmlPowerTools;
using Fizzler.Systems.HtmlAgilityPack;
using System.Collections.Generic;
using PdfSharp;
using TheArtOfDev.HtmlRenderer.PdfSharp;
using PdfSharp.Pdf;
using HiQPdf;
using HtmlAgilityPack;
using PdfSharp.Pdf.IO;

namespace Converter
{
    class PdfConverter
    {
        private string CSS;
        private string HTML;
        private byte[] bytePdfDocument;
        private string directory;


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
            var fileInfo = new FileInfo(path);

            if (!fileInfo.Exists)
            {
                new FileNotFoundException();
            }

            if (fileInfo.Extension != ".docx" && fileInfo.Extension != ".docx")
            {
                new FileFormatException();
            }


            string fullFilePath = fileInfo.FullName;

            try
            {
                HTML = ParseDOCX(fileInfo);
            }
            catch (OpenXmlPackageException e)
            {
                if (e.ToString().Contains("Invalid Hyperlink"))
                {
                    using (FileStream fs = new FileStream(fullFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                    {
                        UriFixer.FixInvalidUri(fs, brokenUri => FixUri(brokenUri));
                    }

                    HTML = ParseDOCX(fileInfo);
                }
            }

            //string landscapehtml = FormatInvoice(true);
            HTML = formatTable("Акт");
            File.WriteAllBytes(pathSaveFile, SaveDocument());
            writeHtmlFile(pathSaveFile);
            Console.ReadKey();
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
            HTML = HTML.Replace(substringCSSSelector("body"),
                "body { width: 36cm;  margin: 1cm auto; max-width: 36cm; padding: 1cm; }");
            SelectPdf.HtmlToPdf converter = new SelectPdf.HtmlToPdf();
            SelectPdf.PdfDocument doc = converter.ConvertHtmlString(HTML);

            byte[] two;
            byte[] one;

            using (MemoryStream ms = new MemoryStream())
            {
                doc.Save(ms);
                one = ms.ToArray();
                ms.Close();
            }

            doc.Close();
            if (!string.IsNullOrEmpty(landsacapeHtml))
            {
                HtmlToPdf htmlToPdfConverter = new HtmlToPdf();
                htmlToPdfConverter.Document.PageOrientation = PdfPageOrientation.Landscape;
                using (MemoryStream ms = new MemoryStream())
                {
                    htmlToPdfConverter.ConvertHtmlToStream(landsacapeHtml, null, ms);
                    htmlToPdfConverter.ConvertHtmlToFile(landsacapeHtml, null, "3131.pdf");
                    two = ms.ToArray();
                    ms.Close();
                }

                return concatPdfDoc(one, two);
            }
            else
            {
                return one;
            }
        }

        /**
        *Объединение двух документов в байтовом представлении
        *
        *@param doc1 - первый документ
        *@param doc2 - второй документ
        *
        *@return массив байт нового документа
        */
        private byte[] concatPdfDoc(byte[] doc1, byte[] doc2)
        {
            PdfDocument one;
            PdfDocument two;

            //читаем первую часть документа
            using (MemoryStream ms = new MemoryStream(doc1))
            {
                using (one = PdfReader.Open(ms, PdfDocumentOpenMode.Import)) ;
                ms.Close();
            }

            one.Pages.RemoveAt(2);
            //читаем вторую часть документа
            using (MemoryStream ms = new MemoryStream(doc2))
            {
                using (two = PdfReader.Open(ms, PdfDocumentOpenMode.Import)) ;
                ms.Close();
            }

            //объединяем документы
            using (PdfDocument outPdf = new PdfDocument())
            {
                CopyPages(one, outPdf);
                CopyPages(two, outPdf);
                using (MemoryStream ms = new MemoryStream())
                {
                    outPdf.Save(ms);
                    return ms.ToArray();
                }
            }

            void CopyPages(PdfDocument from, PdfDocument to)
            {
                for (int i = 0; i < from.PageCount; i++)
                {
                    to.AddPage(from.Pages[i]);
                }
            }
        }

        /**
        *Возвращает новый документ взятый из старого)))
        *
        *@child узел из пдф документа
        *
        *@return новую html строку
        */
        private string NewDocument(HtmlAgilityPack.HtmlNode child)
        {
            var html = new HtmlAgilityPack.HtmlDocument();
            html.LoadHtml(HTML);
            var document = html.DocumentNode;
            var node = document.QuerySelector("body");
            //clear body
            node.RemoveAll();
            node.AppendChild(child);

            return document.OuterHtml;
        }

        private string substringCSSSelector(string selctor)
        {
            int start = HTML.IndexOf(selctor);
            int end = HTML.IndexOf("}", start);
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
            var html = new HtmlAgilityPack.HtmlDocument();
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
                html = new HtmlAgilityPack.HtmlDocument();
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
                    insertClass(tableNode, "table", "table", true);
                    insertClass(node, "changeTdItem", "td");
                    insertClass(node, "changeTextIntable", "span");
                }
            }

            deleteIntoTableTagBR(document);

            return document.OuterHtml;
        }

        private string formatTable(string key)
        {
            var html = new HtmlAgilityPack.HtmlDocument();
            html.LoadHtml(HTML);
            var document = html.DocumentNode;


            //ищем таблицу страницу с счет фактурой
            var node = searchNode(document, key, "div");
            if (node != null)
            {
                var tableNode = node;
                node = searchNode(node, "№", "table");


                if (node != null)
                {
                    insertClass(tableNode, "tableWidth", "table", true);
                    insertClass(node, "columnTdX", "td");
                }
            }

            return document.OuterHtml;
        }



        /**
         * Метод который убирает тег br , от которого идет логика разрыва страницы
         * 
         * @param node - связь , в которой происходит удаление
         * @param tag - тег по которому необходимо построить связь
         * @param delTag - тег, который необходимо удалить
         */
        private void deleteIntoTableTagBR(HtmlAgilityPack.HtmlNode node)
        {
            var htmlArr = node.QuerySelectorAll("table");
            foreach (var item in htmlArr)
            {
                var node1 = item.QuerySelectorAll("span").ToArray();
                for (int i = 0; i < node1.Length; i++)
                {
                    if (node1[i].InnerHtml.Contains("<br>"))
                    {
                        node1[i].Remove();
                    }
                }
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
        private HtmlAgilityPack.HtmlNode searchNode(HtmlAgilityPack.HtmlNode document, string keySearchValue,
            string tag)
        {
            var htmlArr = document.QuerySelectorAll(tag);

            foreach (var item in htmlArr)
            {
                if (item.InnerText.IndexOf(keySearchValue) != -1)
                {
                    return item;
                }
            }

            return null;
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
        private PdfConverter insertClass(HtmlAgilityPack.HtmlNode node, string currentClass, string tag,
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

        private string ParseDOCX(byte[] byteArray, string nameDoc = "Name", string fullName = "fullName")
        {
            try
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    memoryStream.Write(byteArray, 0, byteArray.Length);
                    using (WordprocessingDocument wDoc =
                        WordprocessingDocument.Open(memoryStream, true))
                    {
                        int imageCounter = 0;
                        var pageTitle = nameDoc;
                        var part = wDoc.CoreFilePropertiesPart;
                        if (part != null)
                            pageTitle = (string) part.GetXDocument()
                                .Descendants(DC.title)
                                .FirstOrDefault() ?? fullName;
                        string spanElem = "";
                        //обработка шапки для изменения наложения элементов
                        for (int i = 0; i < 10; i++)
                        {
                            spanElem += $"span.pt-a0-00000{i}" + "{margin-right:40px;}";
                        }

                        WmlToHtmlConverterSettings settings = new WmlToHtmlConverterSettings()
                        {
                            AdditionalCss = CSS + $"{spanElem}",

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
                                {
                                    return null;
                                }

                                ImageFormat format = imageInfo.Bitmap.RawFormat;
                                ImageCodecInfo codec = ImageCodecInfo.GetImageDecoders()
                                    .First(c => c.FormatID == format.Guid);
                                string mimeType = codec.MimeType;

                                string imageSource =
                                    string.Format("data:{0};base64,{1}", mimeType, base64);

                                XElement img = new XElement(Xhtml.img,
                                    new XAttribute(NoNamespace.src, imageSource),
                                    imageInfo.ImgStyleAttribute,
                                    imageInfo.AltText != null
                                        ? new XAttribute(NoNamespace.alt, imageInfo.AltText)
                                        : null);
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

        private string ParseDOCX(FileInfo fileInfo)
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
                            pageTitle = (string) part.GetXDocument()
                                .Descendants(DC.title)
                                .FirstOrDefault() ?? fileInfo.FullName;
                        string spanElem = "";
                        //обработка шапки для изменения наложения элементов
                        for (int i = 0; i < 10; i++)
                        {
                            spanElem += $"span.pt-a0-00000{i}" + "{margin-right:40px;}";
                        }

                        CSS += spanElem;
                        WmlToHtmlConverterSettings settings = new WmlToHtmlConverterSettings()
                        {
                            AdditionalCss = CSS,
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
                                {
                                    return null;
                                }

                                ImageFormat format = imageInfo.Bitmap.RawFormat;
                                ImageCodecInfo codec = ImageCodecInfo.GetImageDecoders()
                                    .First(c => c.FormatID == format.Guid);
                                string mimeType = codec.MimeType;

                                string imageSource =
                                    string.Format("data:{0};base64,{1}", mimeType, base64);

                                XElement img = new XElement(Xhtml.img,
                                    new XAttribute(NoNamespace.src, imageSource),
                                    imageInfo.ImgStyleAttribute,
                                    imageInfo.AltText != null
                                        ? new XAttribute(NoNamespace.alt, imageInfo.AltText)
                                        : null);
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