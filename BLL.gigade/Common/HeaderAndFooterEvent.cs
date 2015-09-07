using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iTextSharp.text.pdf;
using iTextSharp.text;

namespace BLL.gigade.Common
{
    public class HeaderAndFooterEvent : PdfPageEventHelper, IPdfPageEvent
    {
        public static PdfTemplate tpl = null;
        
        public static PdfPTable pdftable;
        public static bool isPdfTable=false;
        public static bool PAGE_NUMBER = true;//默認显示分頁
        public static string header = string.Empty;
        public static BaseFont basefont;
        private static int defaultFontSize = 10;
        //重写 关闭一个页面时
        public override void OnEndPage(PdfWriter writer, Document document)
        {
            try
            {
                Font font = new Font(basefont, defaultFontSize);
                Phrase head = new Phrase(header, font);
                PdfContentByte cb = writer.DirectContent;
               
                //页眉显示的位置
                ColumnText.ShowTextAligned(cb, Element.ALIGN_RIGHT, head,
                        document.Right - 10 + document.LeftMargin, document.Top + 10, 0);
                if (PAGE_NUMBER)
                {
                    Phrase footer = new Phrase("第 " + writer.PageNumber + " / " + "   "+" 頁", font);
                    cb = writer.DirectContent;
                    //tpl = cb.CreateTemplate(100, 100);
                    //模版 显示总共页数
                    cb.AddTemplate(tpl, document.Left / 2 + document.Right / 2 , document.Bottom - 10);//调节模版显示的位置
                    //页脚显示的位置
                    ColumnText.ShowTextAligned(cb, Element.ALIGN_RIGHT, footer,
                            document.Left / 2+document.Right/ 2+23, document.Bottom - 10, 0);
                    

                }
            }
            catch (Exception ex)
            {
                throw new Exception("HeaderAndFooterEvent-->OnEndPage-->" + ex.Message);
            }
        }
        //重写 打开一个新页面时
        public override void OnStartPage(PdfWriter writer, Document document)
        {
            try
            {
                if (PAGE_NUMBER)
                {
                    writer.PageCount = writer.PageNumber;
                }
                if (isPdfTable)
                {
                    if (!document.IsOpen())
                    {
                        document.Open();
                    }
                    document.Add(pdftable);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("HeaderAndFooterEvent-->OnStartPage-->" + ex.Message);
            }
           
        }
        
        //关闭PDF文档时
        public override void OnCloseDocument(PdfWriter writer, Document document)
        {
            try
            {
                if (PAGE_NUMBER)
                {
                    tpl.BeginText();
                    tpl.SetFontAndSize(basefont, defaultFontSize);
                    //tpl.ShowText("共 " + (writer.PageNumber-1).ToString() + " 頁");//模版显示的内容
                    tpl.ShowText((writer.PageNumber - 1).ToString());//模版显示的内容
                    tpl.EndText();
                    tpl.ClosePath();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("HeaderAndFooterEvent-->OnCloseDocument-->" + ex.Message);
            }
            
        }
        public static Paragraph InsertTitleContent(string text)
        {
            try
            {
                Font fontTitle = new Font(basefont, 20, Font.BOLD);
                text = System.Web.HttpUtility.HtmlDecode(text);
                Paragraph paragraph = new Paragraph(text, fontTitle);//新建一行
                paragraph.Alignment = Element.ALIGN_CENTER;//居中
                paragraph.SpacingBefore = 10;
                paragraph.SpacingAfter = 10;
                paragraph.SetLeading(1, 2);//每行间的间隔

                return paragraph;
            }
            catch (Exception ex)
            {
                throw new Exception("HeaderAndFooterEvent-->InsertTitleContent-->" + ex.Message);
            }
        }
    }
}