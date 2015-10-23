/*******************************************************************
 * 版权所有： 
 * 类 名 称：PdfHelper
 * 作    者：xiaohui
 * 电子邮箱：xiaohui1027j@hz-mail.eamc.com.tw
 * 创建日期：2015/8/27 10:00:00 
 * 描述：支持DataTable导出到Pdf、自定义页眉、标题、宽度比例；表头多行显示；公共数据在每页开头显示；合併PDF等；
 * 修改描述：
 * 
 * *******************************************************************/
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.io;


namespace BLL.gigade.Common
{
    public class PdfHelper
    {
        #region 私有字段
        private BaseFont basefont;
        private int defaultFontSize;
        private Font font;
        private Document document;
        #endregion
        public PdfHelper()
        {
            try
            {
                defaultFontSize = 10;//默認字体大小
                basefont = BaseFont.CreateFont("C:/Windows/Fonts/msyh.ttf", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);//默認字体为微软雅黑

                font = new Font(basefont,defaultFontSize);
                document = new Document(PageSize.A4);
            }
            catch (Exception ex)
            {
                throw new Exception("PdfManagement-->CreateFont-->" + ex.Message);
            }

        }
        private void OpenDocument()
        {
            if (!document.IsOpen())
            {
                document.Open();
            }
            
        }
        private float[] GetArrColWidth(DataTable dtSource)
        {
            try
            {
                //取得列宽
                float[] arrColWidth = new float[dtSource.Columns.Count];
                foreach (DataColumn item in dtSource.Columns)
                {
                    arrColWidth[item.Ordinal] = Encoding.GetEncoding(936).GetBytes(item.ColumnName.ToString()).Length;//獲取要填充到單元格中內容的長度
                }
                for (int i = 0; i < dtSource.Rows.Count; i++)
                {
                    for (int j = 0; j < dtSource.Columns.Count; j++)
                    {
                        int intTemp = Encoding.GetEncoding(936).GetBytes(dtSource.Rows[i][j].ToString()).Length;
                        if (intTemp > arrColWidth[j])   //獲取并記錄內容最長的長度
                        {
                            arrColWidth[j] = intTemp;
                        }
                    }

                }
                int sum = 0;
                foreach (int item in arrColWidth)
                {
                    sum = sum + item;
                }
                for (int i = 0; i < arrColWidth.Length; i++)
                {
                    arrColWidth[i] = arrColWidth[i] * 100 / sum;
                }
                return arrColWidth;
            }
            catch (Exception ex)
            {
                throw new Exception("PdfManagement-->GetArrColWidth-->" + ex.Message);
            }
        }
        #region 導出DataTable数据到PDF +void ExportDataTableToPDF(DataTable dtSource, string FileName)
        /// <summary>
        /// 導出DataTable数据到PDF 
        /// </summary>
        /// <param name="dt">表名</param>
        /// <param name="FileName">文件完整路徑</param>
        public void ExportDataTableToPDF(DataTable dtSource, string FileName)
        {
            try
            {
                //創建實例
                document = new Document(PageSize.A4);
                PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(FileName, FileMode.Create));
                OpenDocument();
                //添加table
                PdfPTable table = new PdfPTable(dtSource.Columns.Count);
                table.WidthPercentage = 100;
                //沒有傳遞列寬比例時，獲取默認列表比較
                table.SetWidths(GetArrColWidth(dtSource));
                //列名代表表頭
                foreach (DataColumn column in dtSource.Columns)
                {
                    PdfPCell cell = new PdfPCell(new Phrase(column.ColumnName, new Font(basefont, defaultFontSize, Font.BOLD)));
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;//水平居中
                    table.AddCell(cell);
                }
                for (int i = 0; i < dtSource.Rows.Count; i++)
                {
                    for (int j = 0; j < dtSource.Columns.Count; j++)
                    {
                        PdfPCell cell = new PdfPCell(new Phrase(dtSource.Rows[i][j].ToString(), font));
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;//水平居右
                        table.AddCell(cell);
                    }

                }
                document.Add(table);
                //關閉document
                writer.Flush();
                writer.CloseStream = true;
                document.Close();  //
            }
            catch (Exception ex)
            {
                throw new Exception("PdfManagement-->ExportDataTableToPDF-->" + ex.Message);
            }
        } 
        #endregion

        #region 導出DataTable数据到PDF  +void ExportDataTableToPDF(DataTable dtSource, string FileName, string title)
        /// <summary>
        /// 導出DataTable数据到PDF 
        /// </summary>
        /// <param name="dt">表名</param>
        /// <param name="FileName">文件完整路徑</param>
        /// <param name="title">標題</param>
        public void ExportDataTableToPDF(DataTable dtSource, string FileName, string title)
        {
            try
            {
                //創建實例
                document = new Document(PageSize.A4);
                PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(FileName, FileMode.Create));
                OpenDocument();
                //設置title
                writer.PageEvent = new HeaderAndFooterEvent();
                HeaderAndFooterEvent.basefont = basefont;
                HeaderAndFooterEvent.PAGE_NUMBER = true;
                HeaderAndFooterEvent.tpl = writer.DirectContent.CreateTemplate(100, 100);
                if (!string.IsNullOrEmpty(title))
                {
                    document.Add(HeaderAndFooterEvent.InsertTitleContent(title));
                }
                //添加table
                PdfPTable table = new PdfPTable(dtSource.Columns.Count);
                table.WidthPercentage = 100;
                //沒有傳遞列寬比例時，獲取默認列表比較
                table.SetWidths(GetArrColWidth(dtSource));
                //列名代表表頭
                foreach (DataColumn column in dtSource.Columns)
                {
                    PdfPCell cell = new PdfPCell(new Phrase(column.ColumnName, new Font(basefont, defaultFontSize, Font.BOLD)));
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;//水平居中
                    table.AddCell(cell);
                }
                for (int i = 0; i < dtSource.Rows.Count; i++)
                {
                    for (int j = 0; j < dtSource.Columns.Count; j++)
                    {
                        PdfPCell cell = new PdfPCell(new Phrase(dtSource.Rows[i][j].ToString(), font));
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;//水平居右
                        table.AddCell(cell);
                    }

                }
                document.Add(table);
                //關閉document
                writer.Flush();
                writer.CloseStream = true;
                document.Close();  //
            }
            catch (Exception ex)
            {
                throw new Exception("PdfManagement-->ExportDataTableToPDF-->" + ex.Message);
            }
        } 
        #endregion

        #region 導出DataTable数据到PDF +void ExportDataTableToPDF(DataTable dtSource, string FileName, string title, string header)
        /// <summary>
        /// 導出DataTable数据到PDF 
        /// </summary>
        /// <param name="dt">表名</param>
        /// <param name="FileName">文件完整路徑</param>
        /// <param name="title">標題</param>
        /// <param name="header">頁眉</param>
        public void ExportDataTableToPDF(DataTable dtSource, string FileName, string title, string header)
        {
            try
            {
                //創建實例
                document = new Document(PageSize.A4);
                PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(FileName, FileMode.Create));
                OpenDocument();
                //設置title和頁眉
                writer.PageEvent = new HeaderAndFooterEvent();
                HeaderAndFooterEvent.basefont = basefont;
                HeaderAndFooterEvent.header = header;
                HeaderAndFooterEvent.PAGE_NUMBER = true;
                HeaderAndFooterEvent.tpl = writer.DirectContent.CreateTemplate(100, 100);
                if (!string.IsNullOrEmpty(title))
                {
                    document.Add(HeaderAndFooterEvent.InsertTitleContent(title));
                }
                //添加table
                PdfPTable table = new PdfPTable(dtSource.Columns.Count);
                table.WidthPercentage = 100;
                //沒有傳遞列寬比例時，獲取默認列表比較
                table.SetWidths(GetArrColWidth(dtSource));
                //列名代表表頭
                foreach (DataColumn column in dtSource.Columns)
                {
                    PdfPCell cell = new PdfPCell(new Phrase(column.ColumnName, new Font(basefont, defaultFontSize, Font.BOLD)));
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;//水平居中
                    table.AddCell(cell);
                }
                for (int i = 0; i < dtSource.Rows.Count; i++)
                {
                    for (int j = 0; j < dtSource.Columns.Count; j++)
                    {
                        PdfPCell cell = new PdfPCell(new Phrase(dtSource.Rows[i][j].ToString(), font));
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;//水平居右
                        table.AddCell(cell);
                    }

                }
                document.Add(table);
                //關閉document
                writer.Flush();
                writer.CloseStream = true;
                document.Close();  //
            }
            catch (Exception ex)
            {
                throw new Exception("PdfManagement-->ExportDataTableToPDF-->" + ex.Message);
            }
        } 
        #endregion
        #region 導出DataTable数据到PDF +void ExportDataTableToPDF(DataTable dtSource, string FileName, float[] arrColWidth, string title, string header)
        /// <summary>
        /// 導出DataTable数据到PDF 
        /// </summary>
        /// <param name="dt">表名</param>
        /// <param name="FileName">文件完整路徑</param>
        /// <param name="arrColWidth">列寬比例數組，數組長度必須等於DataTable列數；</param>
        /// <param name="title">標題</param>
        /// <param name="header">頁眉</param>
        public void ExportDataTableToPDF(DataTable dtSource, string FileName, float[] arrColWidth, string title, string header)
        {
            try
            {
                //創建實例
                document = new Document(PageSize.A4);
                PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(FileName, FileMode.Create));
                OpenDocument();
                //設置title和頁眉
                writer.PageEvent = new HeaderAndFooterEvent();
                HeaderAndFooterEvent.basefont = basefont;
                HeaderAndFooterEvent.header = header;
                HeaderAndFooterEvent.PAGE_NUMBER = true;
                HeaderAndFooterEvent.tpl = writer.DirectContent.CreateTemplate(100, 100);
                if (!string.IsNullOrEmpty(title))
                {
                    document.Add(HeaderAndFooterEvent.InsertTitleContent(title));
                }
                //添加table
                PdfPTable table = new PdfPTable(dtSource.Columns.Count);
                table.WidthPercentage = 100;
                //沒有傳遞列寬比例時，獲取默認列表比較
                table.SetWidths(arrColWidth);
                //列名代表表頭
                foreach (DataColumn column in dtSource.Columns)
                {
                    PdfPCell cell = new PdfPCell(new Phrase(column.ColumnName, new Font(basefont, defaultFontSize, Font.BOLD)));
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;//水平居中
                    table.AddCell(cell);
                }
                for (int i = 0; i < dtSource.Rows.Count; i++)
                {
                    for (int j = 0; j < dtSource.Columns.Count; j++)
                    {
                        PdfPCell cell = new PdfPCell(new Phrase(dtSource.Rows[i][j].ToString(), font));
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;//水平居右
                        table.AddCell(cell);
                    }

                }
                document.Add(table);
                //關閉document
                writer.Flush();
                writer.CloseStream = true;
                document.Close();  //
            }
            catch (Exception ex)
            {
                throw new Exception("PdfManagement-->ExportDataTableToPDF-->" + ex.Message);
            }
        } 
        #endregion

        #region 導出DataTable数据到PDF +void ExportDataTableToPDF(DataTable dtSource, string FileName, string title, string header, int cols)
        /// <summary>
        /// 導出DataTable数据到PDF 
        /// </summary>
        /// <param name="dt">表名</param>
        /// <param name="FileName">文件完整路徑</param>
        /// <param name="title">標題</param>
        /// <param name="header">頁眉</param>
        /// <param name="rows">表頭分多行</param>

        public void ExportDataTableToPDF(DataTable dtSource, string FileName, string title, string header, int cols)
        {
            try
            {
                //創建實例
                document = new Document(PageSize.A4);
                PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(FileName, FileMode.Create));
                OpenDocument();
                //設置title和頁眉
                writer.PageEvent = new HeaderAndFooterEvent();
                HeaderAndFooterEvent.basefont = basefont;
                HeaderAndFooterEvent.header = header;
                HeaderAndFooterEvent.PAGE_NUMBER = true;
                HeaderAndFooterEvent.tpl = writer.DirectContent.CreateTemplate(100, 100);
                if (!string.IsNullOrEmpty(title))
                {
                    document.Add(HeaderAndFooterEvent.InsertTitleContent(title));
                }
                //添加table
                int rows = (int)(dtSource.Columns.Count / cols);
                if (dtSource.Columns.Count % cols != 0)
                {
                    rows = rows + 1;
                }
                PdfPTable table = new PdfPTable(cols);
                table.WidthPercentage = 100;
                //沒有傳遞列寬比例時，獲取默認列表比較
                //table.SetWidths(arrColWidth);
                //列名代表表頭
                //foreach (DataColumn column in dtSource.Columns)
                for (int i = 0; i < dtSource.Columns.Count; i++)
                {
                    PdfPCell cell = new PdfPCell(new Phrase(dtSource.Columns[i].ColumnName, new Font(basefont, defaultFontSize, Font.BOLD)));
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;//水平居中
                    if (i / cols % rows == 0)
                    {
                        cell.DisableBorderSide(2); //1，2，4，8 分别对应每行的上，下，左，右四个边框．
                    }
                    else if (i / cols % (rows - 1) == 0)
                    {
                        cell.DisableBorderSide(1);
                    }
                    else
                    {
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                    }
                    table.AddCell(cell);

                }
                for (int j = dtSource.Columns.Count; j < cols * rows; j++)
                {
                    PdfPCell cell = new PdfPCell(new Phrase("", new Font(basefont, defaultFontSize, Font.BOLD)));
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;//水平居中
                    if (rows == 1)
                    {

                    }
                    else if (j / cols % rows == 0)
                    {
                        cell.DisableBorderSide(2); //1，2，4，8 分别对应每行的上，下，左，右四个边框．
                    }
                    else if (j / cols % (rows - 1) == 0)
                    {
                        cell.DisableBorderSide(1);
                    }
                    else
                    {
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                    }
                    table.AddCell(cell);
                }
                for (int i = 0; i < dtSource.Rows.Count; i++)
                {
                    for (int j = 0; j < dtSource.Columns.Count; j++)
                    {
                        PdfPCell cell = new PdfPCell(new Phrase(dtSource.Rows[i][j].ToString(), font));
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;//水平居右
                        if (rows == 1)
                        {

                        }
                        else if (j / cols % rows == 0)
                        {
                            cell.DisableBorderSide(2); //1，2，4，8 分别对应每行的上，下，左，右四个边框．
                        }
                        else if (j / cols % (rows - 1) == 0)
                        {
                            cell.DisableBorderSide(1);
                        }
                        else
                        {
                            cell.DisableBorderSide(1);
                            cell.DisableBorderSide(2);
                        }
                        table.AddCell(cell);
                    }
                    for (int j = dtSource.Columns.Count; j < cols * rows; j++)
                    {
                        PdfPCell cell = new PdfPCell(new Phrase("", font));
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;//水平居右
                        if (rows == 1)
                        {

                        }
                        else if (j / cols % rows == 0)
                        {
                            cell.DisableBorderSide(2); //1，2，4，8 分别对应每行的上，下，左，右四个边框．
                        }
                        else if (j / cols % (rows - 1) == 0)
                        {
                            cell.DisableBorderSide(1);
                        }
                        else
                        {
                            cell.DisableBorderSide(1);
                            cell.DisableBorderSide(2);
                        }
                        table.AddCell(cell);
                    }

                }
                document.Add(table);
                //關閉document
                writer.Flush();
                writer.CloseStream = true;
                document.Close();  //
            }
            catch (Exception ex)
            {
                throw new Exception("PdfManagement-->ExportDataTableToPDF-->" + ex.Message);
            }
        } 
        #endregion
        #region 導出DataTable数据到PDF +void ExportDataTableToPDF(DataTable dtSource, string FileName, PdfPTable pdftable, string title, string header, int cols)
        /// <summary>
        /// 導出DataTable数据到PDF 
        /// </summary>
        /// <param name="dt">表名</param>
        /// <param name="FileName">文件完整路徑</param>
        /// <param name="pdftable">公共模块</param>
        /// <param name="header">頁眉</param>
        /// <param name="rows">表頭分N列显示</param>
        public void ExportDataTableToPDF(DataTable dtSource, string FileName, PdfPTable pdftable, string title, string header, int cols)
        {
            try
            {
                //創建實例
                document = new Document(PageSize.A4);
                PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(FileName, FileMode.Create));
                OpenDocument();
                //設置title和頁眉
                writer.PageEvent = new HeaderAndFooterEvent();
                HeaderAndFooterEvent.basefont = basefont;
                HeaderAndFooterEvent.header = header;
                HeaderAndFooterEvent.isPdfTable = true;
                HeaderAndFooterEvent.pdftable = pdftable;
                HeaderAndFooterEvent.PAGE_NUMBER = true;
                HeaderAndFooterEvent.tpl = writer.DirectContent.CreateTemplate(100, 100);
                if (!string.IsNullOrEmpty(title))
                {
                    document.Add(HeaderAndFooterEvent.InsertTitleContent(title));
                }
                //添加table
                int rows = (int)(dtSource.Columns.Count / cols);
                if (dtSource.Columns.Count % cols != 0)
                {
                    rows = rows + 1;
                }
                PdfPTable table = new PdfPTable(cols);
                table.WidthPercentage = 100;
                //沒有傳遞列寬比例時，獲取默認列表比較
                //table.SetWidths(arrColWidth);
                //列名代表表頭
                for (int i = 0; i < dtSource.Columns.Count; i++)
                {
                    PdfPCell cell = new PdfPCell(new Phrase(dtSource.Columns[i].ColumnName, new Font(basefont, defaultFontSize, Font.BOLD)));
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;//水平居中
                    if (i / cols % rows == 0)
                    {
                        cell.DisableBorderSide(2); //1，2，4，8 分别对应每行的上，下，左，右四个边框．
                    }
                    else if (i / cols % (rows - 1) == 0)
                    {
                        cell.DisableBorderSide(1);
                    }
                    else
                    {
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                    }
                    table.AddCell(cell);

                }
                for (int j = dtSource.Columns.Count; j < cols * rows; j++)
                {
                    PdfPCell cell = new PdfPCell(new Phrase("", new Font(basefont, defaultFontSize, Font.BOLD)));
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;//水平居中
                    if (rows == 1)
                    {

                    }
                    else if (j / cols % rows == 0)
                    {
                        cell.DisableBorderSide(2); //1，2，4，8 分别对应每行的上，下，左，右四个边框．
                    }
                    else if (j / cols % (rows - 1) == 0)
                    {
                        cell.DisableBorderSide(1);
                    }
                    else
                    {
                        cell.DisableBorderSide(1);
                        cell.DisableBorderSide(2);
                    }
                    table.AddCell(cell);
                }
                for (int i = 0; i < dtSource.Rows.Count; i++)
                {
                    for (int j = 0; j < dtSource.Columns.Count; j++)
                    {
                        PdfPCell cell = new PdfPCell(new Phrase(dtSource.Rows[i][j].ToString(), font));
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;//水平居右
                        if (rows == 1)
                        {

                        }
                        else if (j / cols % rows == 0)
                        {
                            cell.DisableBorderSide(2); //1，2，4，8 分别对应每行的上，下，左，右四个边框．
                        }
                        else if (j / cols % (rows - 1) == 0)
                        {
                            cell.DisableBorderSide(1);
                        }
                        else
                        {
                            cell.DisableBorderSide(1);
                            cell.DisableBorderSide(2);
                        }
                        table.AddCell(cell);
                    }
                    for (int j = dtSource.Columns.Count; j < cols * rows; j++)
                    {
                        PdfPCell cell = new PdfPCell(new Phrase("", font));
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;//水平居右
                        if (rows == 1)
                        {

                        }
                        else if (j / cols % rows == 0)
                        {
                            cell.DisableBorderSide(2); //1，2，4，8 分别对应每行的上，下，左，右四个边框．
                        }
                        else if (j / cols % (rows - 1) == 0)
                        {
                            cell.DisableBorderSide(1);
                        }
                        else
                        {
                            cell.DisableBorderSide(1);
                            cell.DisableBorderSide(2);
                        }
                        table.AddCell(cell);
                    }

                }
                document.Add(pdftable);
                document.Add(table);
                ////關閉document
                //writer.Flush();
                //writer.CloseStream = true;
                document.Close();  //
            }
            catch (Exception ex)
            {
                throw new Exception("PdfManagement-->ExportDataTableToPDF-->" + ex.Message);
            }
        } 
        #endregion

        #region 導出DataTable数据到PDF +void ExportDataTableToPDF(DataTable dtSource,bool isColumnName, string FileName, float[] arrColWidth, PdfPTable pdftable, string title, string header, int cols,uint count)
        /// <summary>
        /// 導出DataTable数据到PDF 
        /// </summary>
        /// <param name="dtSource">表名</param>
        /// <param name="isColumnName">是否顯示列名</param>
        /// <param name="FileName">文件完整路徑</param>
        /// <param name="arrColWidth">列寬比例數組，數組長度必須等於DataTable列數；</param>
        /// <param name="pdftable">公共模块</param>
        /// <param name="title">標題</param>        
        /// <param name="header">頁眉</param>
        /// <param name="rows">表頭分rows列显示</param>
        /// <param name="count">每頁最多顯示count組記錄</param>
        public void ExportDataTableToPDF(DataTable dtSource,bool isColumnName, string FileName, float[] arrColWidth, PdfPTable pdftable, string title, string header, int cols,uint count)
        {
            try
            {
                //創建實例
                document = new Document(PageSize.A4);
                PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(FileName, FileMode.Create));
                OpenDocument();
                //設置title和頁眉
                writer.PageEvent = new HeaderAndFooterEvent();
                HeaderAndFooterEvent.basefont = basefont;
                HeaderAndFooterEvent.header = header;
                HeaderAndFooterEvent.isPdfTable = true;
                HeaderAndFooterEvent.pdftable = pdftable;
                HeaderAndFooterEvent.PAGE_NUMBER = true;
                HeaderAndFooterEvent.tpl = writer.DirectContent.CreateTemplate(100, 100);
                if (!string.IsNullOrEmpty(title))
                {
                    document.Add(HeaderAndFooterEvent.InsertTitleContent(title));
                }
                //添加table
                int rows = (int)(dtSource.Columns.Count / cols);
                if (dtSource.Columns.Count % cols != 0)
                {
                    rows = rows + 1;
                }
                PdfPTable table = new PdfPTable(cols);
                table.WidthPercentage = 100;
                //沒有傳遞列寬比例時，獲取默認列表比較
                float[] newArrColWidth = new float[dtSource.Columns.Count];
                for (int i = 0; i < dtSource.Columns.Count; i++)
                {
                    if (arrColWidth.Length > i)
                    {
                        newArrColWidth[i] = arrColWidth[i];
                    }
                    else
                    {
                        newArrColWidth[i] = newArrColWidth[i - cols];
                    }
                }
                
                table.SetWidths(arrColWidth);
                //列名代表表頭
                #region 是否顯示列名
                if (isColumnName)
                {
                    for (int i = 0; i < dtSource.Columns.Count; i++)
                    {
                        PdfPCell cell = new PdfPCell(new Phrase(dtSource.Columns[i].ColumnName, new Font(basefont, defaultFontSize, Font.BOLD)));
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;//水平居中
                        if (i / cols % rows == 0)
                        {
                            cell.DisableBorderSide(2); //1，2，4，8 分别对应每行的上，下，左，右四个边框．
                        }
                        else if (i / cols % (rows - 1) == 0)
                        {
                            cell.DisableBorderSide(1);
                        }
                        else
                        {
                            cell.DisableBorderSide(1);
                            cell.DisableBorderSide(2);
                        }
                        table.AddCell(cell);

                    }
                    for (int j = dtSource.Columns.Count; j < cols * rows; j++)
                    {
                        PdfPCell cell = new PdfPCell(new Phrase("", new Font(basefont, defaultFontSize)));
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;//水平居中
                        if (rows == 1)
                        {

                        }
                        else if (j / cols % rows == 0)
                        {
                            cell.DisableBorderSide(2); //1，2，4，8 分别对应每行的上，下，左，右四个边框．
                        }
                        else if (j / cols % (rows - 1) == 0)
                        {
                            cell.DisableBorderSide(1);
                        }
                        else
                        {
                            cell.DisableBorderSide(1);
                            cell.DisableBorderSide(2);
                        }
                        table.AddCell(cell);
                    }  
                }
                #endregion
                document.Add(pdftable);
                for (int i = 0; i < dtSource.Rows.Count; i++)
                {
                    for (int j = 0; j < dtSource.Columns.Count; j++)
                    {
                        PdfPCell cell = new PdfPCell(new Phrase(dtSource.Rows[i][j].ToString(), font));
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;//水平居右
                        if (rows == 1)
                        {

                        }
                        else if (j / cols % rows == 0)
                        {
                            cell.DisableBorderSide(2); //1，2，4，8 分别对应每行的上，下，左，右四个边框．
                        }
                        else if (j / cols % (rows - 1) == 0)
                        {
                            cell.DisableBorderSide(1);
                        }
                        else
                        {
                            cell.DisableBorderSide(1);
                            cell.DisableBorderSide(2);
                        }

                        table.AddCell(cell);
                    }
                    if (dtSource.Columns.Count == cols * rows)
                    {
                        if ((i + 1) % count == 0 && i != dtSource.Rows.Count - 1 )
                        {
                            document.Add(table);
                            table.Rows.Clear();
                            document.NewPage();
                        }
                    }
                    for (int j = dtSource.Columns.Count; j < cols * rows; j++)
                    {
                        PdfPCell cell = new PdfPCell(new Phrase("", font));
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;//水平居右
                        if (rows == 1)
                        {

                        }
                        else if (j / cols % rows == 0)
                        {
                            cell.DisableBorderSide(2); //1，2，4，8 分别对应每行的上，下，左，右四个边框．
                        }
                        else if (j / cols % (rows - 1) == 0)
                        {
                            cell.DisableBorderSide(1);
                        }
                        else
                        {
                            cell.DisableBorderSide(1);
                            cell.DisableBorderSide(2);
                        }
                        table.AddCell(cell);
                        if ((i + 1) % count == 0 && i != dtSource.Rows.Count-1 && j == cols * rows - 1)
                        {
                            document.Add(table);
                            table.Rows.Clear();
                            document.NewPage();
                        }
                    }

                }
                document.Add(table);
                ////關閉document
                //writer.Flush();
                //writer.CloseStream = true;
                document.Close();  //
            }
            catch (Exception ex)
            {
                throw new Exception("PdfManagement-->ExportDataTableToPDF-->" + ex.Message);
            }
        }
        #endregion

        #region 導出DataTable数据到PDF +void (DataTable dtSource, bool isColumnName, string FileName, float[] arrColWidth, PdfPTable pdftable,PdfPTable pdffooter, string title, string header, int cols, uint count)
        /// <summary>
        /// 導出DataTable数据到PDF 
        /// </summary>
        /// <param name="dtSource">表名</param>
        /// <param name="isColumnName">是否顯示列名</param>
        /// <param name="FileName">文件完整路徑</param>
        /// <param name="arrColWidth">列寬比例數組，數組長度必須等於DataTable列數；</param>
        /// <param name="pdftable">公共模块</param>
        /// <param name="pdffooter">落款</param>
        /// <param name="title">標題</param>        
        /// <param name="header">頁眉</param>
        /// <param name="rows">表頭分rows列显示</param>
        /// <param name="count">每頁最多顯示count組記錄</param>
        public void ExportDataTableToPDF(DataTable dtSource, bool isColumnName, string FileName, float[] arrColWidth, PdfPTable pdftable,PdfPTable pdffooter, string title, string header, int cols, uint count)
        {
            try
            {
                //創建實例
                document = new Document(PageSize.A4.Rotate());
                PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(FileName, FileMode.Create));
                OpenDocument();
                //設置title和頁眉
                writer.PageEvent = new HeaderAndFooterEvent();
                HeaderAndFooterEvent.basefont = basefont;
                HeaderAndFooterEvent.header = header;
                HeaderAndFooterEvent.isPdfTable = true;
                HeaderAndFooterEvent.pdftable = pdftable;
                HeaderAndFooterEvent.PAGE_NUMBER = true;
                HeaderAndFooterEvent.tpl = writer.DirectContent.CreateTemplate(100, 100);
                if (!string.IsNullOrEmpty(title))
                {
                    document.Add(HeaderAndFooterEvent.InsertTitleContent(title));
                }
                //添加table
                int rows = (int)(dtSource.Columns.Count / cols);
                if (dtSource.Columns.Count % cols != 0)
                {
                    rows = rows + 1;
                }
                PdfPTable table = new PdfPTable(cols);
                table.WidthPercentage = 100;
                //沒有傳遞列寬比例時，獲取默認列表比較
                float[] newArrColWidth = new float[dtSource.Columns.Count];
                for (int i = 0; i < dtSource.Columns.Count; i++)
                {
                    if (arrColWidth.Length > i)
                    {
                        newArrColWidth[i] = arrColWidth[i];
                    }
                    else
                    {
                        newArrColWidth[i] = newArrColWidth[i - cols];
                    }
                }

                table.SetWidths(arrColWidth);
                //列名代表表頭
                #region 是否顯示列名
                if (isColumnName)
                {
                    for (int i = 0; i < dtSource.Columns.Count; i++)
                    {
                        PdfPCell cell = new PdfPCell(new Phrase(dtSource.Columns[i].ColumnName, new Font(basefont, defaultFontSize, Font.BOLD)));
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;//水平居中
                        if (i / cols % rows == 0)
                        {
                            cell.DisableBorderSide(2); //1，2，4，8 分别对应每行的上，下，左，右四个边框．
                        }
                        else if (i / cols % (rows - 1) == 0)
                        {
                            cell.DisableBorderSide(1);
                        }
                        else
                        {
                            cell.DisableBorderSide(1);
                            cell.DisableBorderSide(2);
                        }
                        table.AddCell(cell);

                    }
                    for (int j = dtSource.Columns.Count; j < cols * rows; j++)
                    {
                        PdfPCell cell = new PdfPCell(new Phrase("", new Font(basefont, defaultFontSize)));
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;//水平居中
                        if (rows == 1)
                        {

                        }
                        else if (j / cols % rows == 0)
                        {
                            cell.DisableBorderSide(2); //1，2，4，8 分别对应每行的上，下，左，右四个边框．
                        }
                        else if (j / cols % (rows - 1) == 0)
                        {
                            cell.DisableBorderSide(1);
                        }
                        else
                        {
                            cell.DisableBorderSide(1);
                            cell.DisableBorderSide(2);
                        }
                        table.AddCell(cell);
                    }
                }
                #endregion
                document.Add(pdftable);
                for (int i = 0; i < dtSource.Rows.Count; i++)
                {
                    for (int j = 0; j < dtSource.Columns.Count; j++)
                    {
                        PdfPCell cell = new PdfPCell(new Phrase(dtSource.Rows[i][j].ToString(), font));
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;//水平居右
                        if (rows == 1)
                        {

                        }
                        else if (j / cols % rows == 0)
                        {
                            cell.DisableBorderSide(2); //1，2，4，8 分别对应每行的上，下，左，右四个边框．
                        }
                        else if (j / cols % (rows - 1) == 0)
                        {
                            cell.DisableBorderSide(1);
                        }
                        else
                        {
                            cell.DisableBorderSide(1);
                            cell.DisableBorderSide(2);
                        }

                        table.AddCell(cell);
                    }
                    if (dtSource.Columns.Count == cols * rows)
                    {
                        if ((i + 1) % count == 0 && i != dtSource.Rows.Count - 1)
                        {
                            document.Add(table);
                            table.Rows.Clear();
                            document.NewPage();
                        }
                    }
                    for (int j = dtSource.Columns.Count; j < cols * rows; j++)
                    {
                        PdfPCell cell = new PdfPCell(new Phrase("", font));
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;//水平居右
                        if (rows == 1)
                        {

                        }
                        else if (j / cols % rows == 0)
                        {
                            cell.DisableBorderSide(2); //1，2，4，8 分别对应每行的上，下，左，右四个边框．
                        }
                        else if (j / cols % (rows - 1) == 0)
                        {
                            cell.DisableBorderSide(1);
                        }
                        else
                        {
                            cell.DisableBorderSide(1);
                            cell.DisableBorderSide(2);
                        }
                        table.AddCell(cell);
                        if ((i + 1) % count == 0 && i != dtSource.Rows.Count - 1 && j == cols * rows - 1)
                        {
                            document.Add(table);
                            table.Rows.Clear();
                            document.NewPage();
                        }
                    }

                }
                document.Add(table);

                document.Add(pdffooter);
                ////關閉document
                //writer.Flush();
                //writer.CloseStream = true;
                document.Close();  //
            }
            catch (Exception ex)
            {
                throw new Exception("PdfManagement-->ExportDataTableToPDF-->" + ex.Message);
            }
        }
        #endregion

        #region 導出DataTable数据到PDF +void ExportDataTableToPDF(string FileName, PdfPTable pdftable, string title, string header)
        /// <summary>
        /// 導出DataTable数据到PDF 
        /// </summary>
        /// <param name="FileName">文件完整路徑</param>
        /// <param name="pdftable">公共模块</param>
        /// <param name="title">標題</param>
        /// <param name="header">頁眉</param>
        public void ExportDataTableToPDF(string FileName, PdfPTable pdftable, string title, string header)
        {
            try
            {
                //創建實例
                document = new Document(PageSize.A4);
                PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(FileName, FileMode.Create));
                OpenDocument();
                //設置title和頁眉
                writer.PageEvent = new HeaderAndFooterEvent();
                HeaderAndFooterEvent.basefont = basefont;
                HeaderAndFooterEvent.header = header;
                HeaderAndFooterEvent.isPdfTable = true;
                HeaderAndFooterEvent.pdftable = pdftable;
                HeaderAndFooterEvent.PAGE_NUMBER = true;
                HeaderAndFooterEvent.tpl = writer.DirectContent.CreateTemplate(100, 100);
                if (!string.IsNullOrEmpty(title))
                {
                    document.Add(HeaderAndFooterEvent.InsertTitleContent(title));
                }
                
                document.Add(pdftable);
                ////關閉document
                //writer.Flush();
                //writer.CloseStream = true;
                document.Close();  //
            }
            catch (Exception ex)
            {
                throw new Exception("PdfManagement-->ExportDataTableToPDF-->" + ex.Message);
            }
        }
        #endregion

        #region 合併Pdf +void MergePDF(List<string> pdfList, string FileName)
        /// <summary>
        /// 合併Pdf
        /// </summary>
        /// <param name="pdfList">文件路徑列表</param>
        /// <param name="FileName">合併后的文件路徑</param>
        public void MergePDF(List<string> pdfList, string FileName)
        {
            try
            {
                Document document_Merge = new Document(PageSize.A4);
                //創建實例
                PdfWriter writer = PdfWriter.GetInstance(document_Merge, new FileStream(FileName, FileMode.Create));
                document_Merge.Open();
                //設置title和頁眉
                writer.PageEvent = new HeaderAndFooterEvent();
                HeaderAndFooterEvent.header = "";
                HeaderAndFooterEvent.isPdfTable = false;
                HeaderAndFooterEvent.PAGE_NUMBER = false;
                HeaderAndFooterEvent.tpl = writer.DirectContent.CreateTemplate(100, 100);
                PdfReader reader;
                PdfContentByte cb = writer.DirectContent;
                PdfImportedPage newPage;
                for (int i = 0; i < pdfList.Count; i++)
                {
                    reader = new PdfReader(pdfList[i]);
                    iTextSharp.text.Rectangle psize = reader.GetPageSize(1);
                    int iPageNum = reader.NumberOfPages;
                    for (int j = 1; j <= iPageNum; j++)
                    {
                        document_Merge.NewPage();
                        newPage = writer.GetImportedPage(reader, j);
                        cb.AddTemplate(newPage, 0, 0);
                    }
                }
                document_Merge.Close();
            }
            catch (Exception ex)
            {
                throw new Exception("PdfManagement-->MergePDF-->" + ex.Message);
            }
        } 
        #endregion

    }
}