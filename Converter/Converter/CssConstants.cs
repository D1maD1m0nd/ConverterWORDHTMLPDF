﻿namespace Converter
{
    public static class CssConstants
    {
        public static string tableWidth = ".tableWidth{width:25cm; border-collapse: collapse;}";

        public static string tdTextWidth =
            ".columnTdX {vertical-align: middle;" +
            "border-top: solid windowtext 2.0pt;" +
            "border-right: solid windowtext 2.0pt;" +
            "border-bottom: solid windowtext 2.0pt;" +
            "border-left: solid windowtext 2.0pt;" +
            "padding: 4pt;}";


        public static string logoContainer = ".logo-container{line-height: 107.9%;" +
                                             "margin-bottom: 1.5cm;" +
                                             "font-family: Calibri;" +
                                             "font-size: 11pt;" +
                                             "margin-top: 0;" +
                                             "margin-left: 0;" +
                                             "margin-right: 12cm;}";
        public static string imageRight =
            ".logo-format {  font-size: 11pt;"+
                    "font-style: normal;"+
                    "font-weight: normal;"+
                    "margin: 0;"+
                    "padding: 0;"+
                    "float: right;}";

        public static string body = "body { width: 35cm;" +
                                    "margin: 1cm auto;" +
                                    "max-width: 23cm;" +
                                    "padding: 1cm; }";
        public static string br = "br {page-break-before: always;}";
        public static string img =
            "img {page-break-before: auto;" +
            " page-break-after: auto;" +
            " page-break-inside: avoid;" +
            " position: relative; }";

        //public static string br = "br {page-break-before: always;} .changeTextIntable{font-size:12px;}";


        public static string changeTdItem = ".changeTdItem{border:1px solid;" +
                                            " height: auto;" +
                                            " padding-top:6px;" +
                                            " vertical-align:middlle;}";


        public static string table = ".table {   border-collapse: collapse;}";
        public static string changeText =  ".changeTextIntable{font-size:12px;}";
    }
}