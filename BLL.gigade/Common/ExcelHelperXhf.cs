/*******************************************************************
 * 版权所有： 
 * 类 名 称：ExcelHelper
 * 作    者：zk
 * 电子邮箱：77148918@QQ.com
 * 创建日期：2012/2/25 10:17:21 
 * 修改描述：从excel导入datatable时，可以导入日期类型。
 *           但对excel中的日期类型有一定要求，要求至少是yyyy/mm/dd类型日期； *           
 * 修改描述：将datatable导入excel中，对类型为字符串的数字进行处理，
 *           导出数字为double类型；
 * 修改描述：针对NPOI 2.0 alpha版本更新，修改了导入excel的方法，划分为2003版本和2007版本；
 *           将导入方法里的HSSFWorkbook改为接口；
 *           将 NPOI.HSSF.UserModel.HSSFRow改为了NPOI.XSSF.UserModel.XSSFRow(只存在导入excel2007的方法中）
 *           将 导入方法的参数HSSFSheet sheet改为了接口类型ISheet（2003的导入方法和2007均有修改） 
 *           将 导入方法区分为导入Excel2003以及导入Excel2007；
 * 修改日期：2012年5月4日22:06:29 for Jnz Update to NPOI 1.25 正式版
 * 修改日期：2012年8月30日17:13:49 for Jnz Update to NPOI 2.0 alpha版
 * 
 * *******************************************************************/
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Web;
using System.Linq;
using NPOI;
using NPOI.HPSF;
using NPOI.HSSF;
using NPOI.HSSF.Record;//NPOI.HSSF.Record.Formula.Eval改为了NPOI.SS.Formula.Eval;
using NPOI.SS.Formula.Eval;//同上
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.POIFS;
using NPOI.SS.UserModel;
using NPOI.Util;
using NPOI.SS;
using NPOI.DDF;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;//2007
using System.Collections;
using System.Text.RegularExpressions;
using BLL.gigade.Common;

using System.Security.Cryptography.X509Certificates;
using System.Reflection;
using DBAccess;
using System.Xml.Linq;

namespace gigadeExcel.Comment
{
    public class ExcelHelperXhf
    {
        //private static WriteLog wl = new WriteLog();
        #region 从datatable中将数据导出到excel
        /// <summary>
        /// DataTable导出到Excel的MemoryStream
        /// </summary>
        /// <param name="dtSource">源DataTable</param>
        /// <param name="strHeaderText">表头文本</param>
        public static MemoryStream ExportDT(DataTable dtSource, string strHeaderText)
        {
            HSSFWorkbook workbook = new HSSFWorkbook();
            HSSFSheet sheet = null;
            int sheetCount = 0;
            sheet = workbook.CreateSheet("Sheet" + sheetCount) as HSSFSheet;
            #region 右击文件 属性信息

            //{
            //    DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            //    dsi.Company = "http://www.yongfa365.com/";
            //    workbook.DocumentSummaryInformation = dsi;

            //    SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            //    si.Author = "柳永法"; //填加xls文件作者信息
            //    si.ApplicationName = "NPOI测试程序"; //填加xls文件创建程序信息
            //    si.LastAuthor = "柳永法2"; //填加xls文件最后保存者信息
            //    si.Comments = "说明信息"; //填加xls文件作者信息
            //    si.Title = "NPOI测试"; //填加xls文件标题信息
            //    si.Subject = "NPOI测试Demo"; //填加文件主题信息
            //    si.CreateDateTime = DateTime.Now;
            //    workbook.SummaryInformation = si;
            //}

            #endregion

            HSSFCellStyle dateStyle = workbook.CreateCellStyle() as HSSFCellStyle;
            HSSFDataFormat format = workbook.CreateDataFormat() as HSSFDataFormat;
            dateStyle.DataFormat = format.GetFormat("yyyy-mm-dd");

            //取得列宽
            int[] arrColWidth = new int[dtSource.Columns.Count];
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
            int rowIndex = 0;
            foreach (DataRow row in dtSource.Rows)
            {
                #region 新建表，填充表头，填充列头，样式
                //分頁
                if (rowIndex % 65536 == 0 || rowIndex == 0)
                {
                    if (rowIndex != 0)
                    {
                        sheetCount++;
                        sheet = workbook.CreateSheet("Sheet" + sheetCount) as HSSFSheet;
                        rowIndex = 0;
                    }
                    #region 表头及样式
                    if ("" != strHeaderText)
                    {
                        HSSFRow headerRow = sheet.CreateRow(0) as HSSFRow;
                        headerRow.HeightInPoints = 25;
                        headerRow.CreateCell(0).SetCellValue(strHeaderText);
                        HSSFCellStyle headStyle = workbook.CreateCellStyle() as HSSFCellStyle;
                        headStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.CENTER;
                        HSSFFont font = workbook.CreateFont() as HSSFFont;
                        font.FontHeightInPoints = 20;
                        font.Boldweight = 700;
                        headStyle.SetFont(font);
                        headerRow.GetCell(0).CellStyle = headStyle;
                        sheet.AddMergedRegion(new Region(0, 0, 0, dtSource.Columns.Count - 1));
                        //headerRow.Dispose();
                        rowIndex++;
                    }
                    #endregion

                    #region 列头及样式
                    {
                        HSSFRow headerRow = sheet.CreateRow(rowIndex) as HSSFRow;
                        HSSFCellStyle headStyle = workbook.CreateCellStyle() as HSSFCellStyle;
                        headStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.CENTER;
                        HSSFFont font = workbook.CreateFont() as HSSFFont;
                        font.FontHeightInPoints = 10;
                        font.Boldweight = 700;
                        headStyle.SetFont(font);
                        foreach (DataColumn column in dtSource.Columns)
                        {
                            headerRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);
                            // headerRow.GetCell(column.Ordinal).CellStyle = headStyle;
                            //设置列宽
                            //sheet.SetColumnWidth(column.Ordinal, (arrColWidth[column.Ordinal] + 1) * 300);
                            if (arrColWidth[column.Ordinal] > 100)
                            {
                                sheet.SetColumnWidth(column.Ordinal, 100 * 256);
                            }
                            else
                            {
                                sheet.SetColumnWidth(column.Ordinal, (arrColWidth[column.Ordinal] + 1) * 256);
                            }
                        }
                        //headerRow.Dispose();
                    }
                    #endregion
                    rowIndex++;
                }
                #endregion

                #region 填充内容
                HSSFRow dataRow = sheet.CreateRow(rowIndex) as HSSFRow;
                foreach (DataColumn column in dtSource.Columns)
                {
                    HSSFCell newCell = dataRow.CreateCell(column.Ordinal) as HSSFCell;
                    string drValue = row[column].ToString();
                    newCell.SetCellValue(drValue);
                    switch (column.DataType.ToString()) //根據數據類型填充單元格
                    {
                        case "System.String": //字符串类型
                            double result;
                            if (isNumeric(drValue, out result))
                            {
                                double.TryParse(drValue, out result);
                                newCell.SetCellValue(result);
                            }
                            else
                            {
                                newCell.SetCellValue(drValue);
                            }
                            break;
                        case "System.DateTime": //日期类型
                            DateTime dateV;
                            DateTime.TryParse(drValue, out dateV);
                            newCell.SetCellValue(dateV);
                            newCell.CellStyle = dateStyle; //格式化显示
                            break;
                        case "System.Boolean": //布尔型
                            bool boolV = false;
                            bool.TryParse(drValue, out boolV);
                            newCell.SetCellValue(boolV);
                            break;
                        case "System.Int16": //整型
                        case "System.Int32":
                        case "System.Int64":
                        case "System.Byte":
                            int intV = 0;
                            int.TryParse(drValue, out intV);
                            newCell.SetCellValue(intV);
                            break;
                        case "System.Decimal": //浮点型
                        case "System.Double":
                            double doubV = 0;
                            double.TryParse(drValue, out doubV);
                            newCell.SetCellValue(doubV);
                            break;
                        case "System.DBNull": //空值处理
                            newCell.SetCellValue("");
                            break;
                        case "System.UInt16":
                        case "System.UInt32":
                        case "System.UInt64":
                            uint uintV = 0;
                            uint.TryParse(drValue, out uintV);
                            newCell.SetCellValue(uintV);
                            break;
                        default:
                            newCell.SetCellValue("");
                            break;
                    }
                }

                #endregion
                rowIndex++;
            }
            using (MemoryStream ms = new MemoryStream())
            {
                workbook.Write(ms);
                ms.Flush();
                ms.Position = 0;

                //sheet;
                //workbook.Dispose();

                return ms;
            }
        }
        public static MemoryStream ExportDataTable(DataTable dtSource, string strHeaderText)
        {
            HSSFWorkbook workbook = new HSSFWorkbook();
            HSSFSheet sheet = null;
            int sheetCount = 0;
            sheet = workbook.CreateSheet("Sheet" + sheetCount) as HSSFSheet;
            #region 右击文件 属性信息

            //{
            //    DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            //    dsi.Company = "http://www.yongfa365.com/";
            //    workbook.DocumentSummaryInformation = dsi;

            //    SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            //    si.Author = "柳永法"; //填加xls文件作者信息
            //    si.ApplicationName = "NPOI测试程序"; //填加xls文件创建程序信息
            //    si.LastAuthor = "柳永法2"; //填加xls文件最后保存者信息
            //    si.Comments = "说明信息"; //填加xls文件作者信息
            //    si.Title = "NPOI测试"; //填加xls文件标题信息
            //    si.Subject = "NPOI测试Demo"; //填加文件主题信息
            //    si.CreateDateTime = DateTime.Now;
            //    workbook.SummaryInformation = si;
            //}

            #endregion

            HSSFCellStyle dateStyle = workbook.CreateCellStyle() as HSSFCellStyle;
            HSSFDataFormat format = workbook.CreateDataFormat() as HSSFDataFormat;
            dateStyle.DataFormat = format.GetFormat("yyyy-mm-dd");

            //取得列宽
            int[] arrColWidth = new int[dtSource.Columns.Count];
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
            int rowIndex = 0;
            foreach (DataRow row in dtSource.Rows)
            {
                #region 新建表，填充表头，填充列头，样式
                //分頁
                if (rowIndex % 65536 == 0 || rowIndex == 0)
                {
                    if (rowIndex != 0)
                    {
                        sheetCount++;
                        sheet = workbook.CreateSheet("Sheet" + sheetCount) as HSSFSheet;
                        rowIndex = 0;
                    }
                    #region 表头及样式
                    if ("" != strHeaderText)
                    {
                        HSSFRow headerRow = sheet.CreateRow(0) as HSSFRow;
                        headerRow.HeightInPoints = 25;
                        headerRow.CreateCell(0).SetCellValue(strHeaderText);
                        HSSFCellStyle headStyle = workbook.CreateCellStyle() as HSSFCellStyle;
                        headStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.CENTER;
                        HSSFFont font = workbook.CreateFont() as HSSFFont;
                        font.FontHeightInPoints = 20;
                        font.Boldweight = 700;
                        headStyle.SetFont(font);
                        headerRow.GetCell(0).CellStyle = headStyle;
                        sheet.AddMergedRegion(new Region(0, 0, 0, dtSource.Columns.Count - 1));
                        //headerRow.Dispose();
                        rowIndex++;
                    }
                    #endregion

                    #region 列头及样式
                    {
                        HSSFRow headerRow = sheet.CreateRow(rowIndex) as HSSFRow;
                        HSSFCellStyle headStyle = workbook.CreateCellStyle() as HSSFCellStyle;
                        headStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.CENTER;
                        HSSFFont font = workbook.CreateFont() as HSSFFont;
                        font.FontHeightInPoints = 10;
                        font.Boldweight = 700;
                        headStyle.SetFont(font);
                        foreach (DataColumn column in dtSource.Columns)
                        {
                            headerRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);
                            // headerRow.GetCell(column.Ordinal).CellStyle = headStyle;
                            //设置列宽
                            //sheet.SetColumnWidth(column.Ordinal, (arrColWidth[column.Ordinal] + 1) * 300);
                            if (arrColWidth[column.Ordinal] > 100)
                            {
                                sheet.SetColumnWidth(column.Ordinal, 100 * 256);
                            }
                            else
                            {
                                sheet.SetColumnWidth(column.Ordinal, (arrColWidth[column.Ordinal] + 1) * 256);
                            }
                        }
                        //headerRow.Dispose();
                    }
                    #endregion
                    rowIndex++;
                }
                #endregion

                #region 填充内容
                HSSFRow dataRow = sheet.CreateRow(rowIndex) as HSSFRow;
                foreach (DataColumn column in dtSource.Columns)
                {
                    HSSFCell newCell = dataRow.CreateCell(column.Ordinal) as HSSFCell;
                    string drValue = row[column].ToString();
                    newCell.SetCellValue(drValue);
                    switch (column.DataType.ToString()) //根據數據類型填充單元格
                    {
                        case "System.String": //字符串类型
                            double result;
                            //if (isNumeric(drValue, out result))
                            //{
                            //    //double.TryParse(drValue, out result);
                            //    newCell.SetCellValue(result);
                            //}
                            //else
                            //{
                            newCell.SetCellValue(drValue);
                            //}
                            break;
                        case "System.DateTime": //日期类型
                            DateTime dateV;
                            DateTime.TryParse(drValue, out dateV);
                            newCell.SetCellValue(dateV);
                            newCell.CellStyle = dateStyle; //格式化显示
                            break;
                        case "System.Boolean": //布尔型
                            bool boolV = false;
                            bool.TryParse(drValue, out boolV);
                            newCell.SetCellValue(boolV);
                            break;
                        case "System.Int16": //整型
                        case "System.Int32":
                        case "System.Int64":
                        case "System.Byte":
                            int intV = 0;
                            int.TryParse(drValue, out intV);
                            newCell.SetCellValue(intV);
                            break;
                        case "System.Decimal": //浮点型
                        case "System.Double":
                            double doubV = 0;
                            double.TryParse(drValue, out doubV);
                            newCell.SetCellValue(doubV);
                            break;
                        case "System.DBNull": //空值处理
                            newCell.SetCellValue("");
                            break;
                        case "System.UInt16":
                        case "System.UInt32":
                        case "System.UInt64":
                            uint uintV = 0;
                            uint.TryParse(drValue, out uintV);
                            newCell.SetCellValue(uintV);
                            break;
                        default:
                            newCell.SetCellValue("");
                            break;
                    }
                }

                #endregion
                rowIndex++;
            }
            using (MemoryStream ms = new MemoryStream())
            {
                workbook.Write(ms);
                ms.Flush();
                ms.Position = 0;

                //sheet;
                //workbook.Dispose();

                return ms;
            }
        }
        /// <summary>
        /// DataTable导出到Excel的MemoryStream
        /// </summary>
        /// <param name="dtSource">源DataTable</param>
        /// <param name="strHeaderText">SheetName</param>
        public static MemoryStream ExportDTNoColumns(List<DataTable> dtSource, List<string> SheetName, List<bool> ColumnsName)
        {
            HSSFWorkbook workbook = new HSSFWorkbook();
            for (int k = 0; k < dtSource.Count; k++)
            {
                HSSFSheet sheet = workbook.CreateSheet(SheetName[k].ToString()) as HSSFSheet;
                HSSFCellStyle dateStyle = workbook.CreateCellStyle() as HSSFCellStyle;
                HSSFDataFormat format = workbook.CreateDataFormat() as HSSFDataFormat;
                dateStyle.DataFormat = format.GetFormat("yyyy-mm-dd");
                int rowIndex = -1;
                foreach (DataRow row in dtSource[k].Rows)
                {
                    #region 新建表，填充表头，填充列头，样式
                    if (rowIndex == 65535 || rowIndex == -1)
                    {
                        if (rowIndex != -1)
                        {
                            sheet = workbook.CreateSheet() as HSSFSheet;
                        }
                        rowIndex++;
                    }
                    #endregion

                    #region 填充内容

                    HSSFRow dataRow = sheet.CreateRow(rowIndex) as HSSFRow;
                    foreach (DataColumn column in dtSource[k].Columns)
                    {
                        string drValue = row[column].ToString();
                        if (!string.IsNullOrEmpty(drValue))
                        {
                            HSSFCell newCell = dataRow.CreateCell(column.Ordinal) as HSSFCell;
                            newCell.SetCellValue(drValue);
                            switch (column.DataType.ToString())
                            {
                                case "System.String": //字符串类型
                                    double result;
                                    if (isNumeric(drValue, out result))
                                    {
                                        double.TryParse(drValue, out result);
                                        newCell.SetCellValue(result);
                                    }
                                    else
                                    {
                                        newCell.SetCellValue(drValue);
                                    }
                                    break;
                                case "System.DateTime": //日期类型
                                    DateTime dateV;
                                    DateTime.TryParse(drValue, out dateV);
                                    newCell.SetCellValue(dateV);
                                    newCell.CellStyle = dateStyle; //格式化显示
                                    break;
                                case "System.Boolean": //布尔型
                                    bool boolV = false;
                                    bool.TryParse(drValue, out boolV);
                                    newCell.SetCellValue(boolV);
                                    break;
                                case "System.Int16": //整型
                                case "System.Int32":
                                case "System.Int64":
                                case "System.Byte":
                                    int intV = 0;
                                    int.TryParse(drValue, out intV);
                                    newCell.SetCellValue(intV);
                                    break;
                                case "System.Decimal": //浮点型
                                case "System.Double":
                                    double doubV = 0;
                                    double.TryParse(drValue, out doubV);
                                    newCell.SetCellValue(doubV);
                                    break;
                                case "System.DBNull": //空值处理
                                    newCell.SetCellValue("");
                                    break;
                                default:
                                    newCell.SetCellValue("");
                                    break;
                            }
                        }
                    }
                    #endregion
                    rowIndex++;
                }
                sheet.DefaultColumnWidth = 10;
                sheet.AutoSizeColumn(1, true);
                sheet.AutoSizeColumn(2, true);
                sheet.AutoSizeColumn(3, true);
                sheet.AutoSizeColumn(4, true);
                sheet.AutoSizeColumn(5, true);
                sheet.AutoSizeColumn(6, true);
                sheet.AutoSizeColumn(7, true);
                sheet.AutoSizeColumn(8, true);
                sheet.AutoSizeColumn(9, true);
                sheet.AutoSizeColumn(10, true);
                sheet.AutoSizeColumn(11, true);
                sheet.AutoSizeColumn(12, true);
               // sheet.AutoSizeColumn(13, true);
                sheet.AutoSizeColumn(14, true);
                sheet.AutoSizeColumn(15, true);
                sheet.AutoSizeColumn(16, true);
                sheet.AutoSizeColumn(17, true);
                sheet.AutoSizeColumn(18, true);
                sheet.AutoSizeColumn(19, true);
                sheet.AutoSizeColumn(20, true);
                sheet.AutoSizeColumn(21, true);
                sheet.AutoSizeColumn(22, true);
                sheet.AutoSizeColumn(23, true);
                sheet.AutoSizeColumn(24, true);
                sheet.AutoSizeColumn(25, true);
                sheet.AutoSizeColumn(26, true);
                sheet.AutoSizeColumn(27, true);
                sheet.AutoSizeColumn(28, true);
                sheet.AutoSizeColumn(29, true);
                sheet.AutoSizeColumn(30, true);
                sheet.AutoSizeColumn(31, true);
                sheet.AutoSizeColumn(32, true);
                sheet.AutoSizeColumn(33, true);
            }
            using (MemoryStream ms = new MemoryStream())
            {
                workbook.Write(ms);
                ms.Flush();
                ms.Position = 0;

                //sheet;
                //workbook.Dispose();

                return ms;
            }
        }

        /// <summary>
        /// 根據應欄位匯出Excel
        /// </summary>
        /// <typeparam name="T">實體類型</typeparam>
        /// <param name="lists">實體集合</param>
        /// <param name="dics">顯示名稱與實體屬性對照字典</param>
        /// <param name="cols">內容顯示為空的欄位</param>
        /// <returns>excel文件流</returns>
        public static MemoryStream ExportExcel<T>(List<T> lists, Dictionary<string, string> dics, List<string> cols = null, string sheetName = "sheet1")
        {
            var dictionary = new Dictionary<string, List<T>>();
            dictionary.Add(sheetName, lists);
            return ExportExcel(dictionary, dics, cols);
        }
        /// <summary>
        /// 根據應欄位匯出Excel
        /// </summary>
        /// <typeparam name="T">實體類型</typeparam>
        /// <param name="lists">實體集合</param>
        /// <param name="xmlPath">xml路徑</param>
        /// <param name="cols">內容顯示為空的欄位</param>
        /// <param name="sheetName">表名</param>
        /// <returns></returns>
        public static MemoryStream ExportExcel<T>(List<T> lists, string xmlPath, List<string> cols = null, string sheetName = "sheet1")
        {
            var dictionary = new Dictionary<string, List<T>>();
            dictionary.Add(sheetName, lists);
            return ExportExcel(lists, xmlPath, cols, sheetName);
        }
        /// <summary>
        /// 根據應欄位匯出Excel
        /// </summary>
        /// <typeparam name="T">實體類型</typeparam>
        /// <param name="lists">key：表名 value:實體集合</param>
        /// <param name="xmlPath">xml路徑</param>
        /// <param name="cols">內容顯示為空的欄位</param>
        /// <returns>excel文件流</returns>
        public static MemoryStream ExportExcel<T>(Dictionary<string, List<T>> lists, string xmlPath, List<string> cols = null)
        {
            XDocument xml = XDocument.Load(xmlPath);//加载xml 
            Dictionary<string, string> dics = xml.Elements().Elements().ToDictionary(p => p.Attribute("key").Value, p => p.Value);//將xml轉換成Dictionary
            return ExportExcel(lists, dics, null);
        }

        /// <summary>
        /// 根據應欄位匯出Excel
        /// </summary>
        /// <typeparam name="T">實體類型</typeparam>
        /// <param name="lists">key：表名 value:實體集合</param>
        /// <param name="dics">顯示名稱與實體屬性對照字典</param>
        /// <param name="cols">內容顯示為空的欄位</param>
        /// <returns>excel文件流</returns>
        public static MemoryStream ExportExcel<T>(Dictionary<string, List<T>> lists, Dictionary<string, string> dics, List<string> cols = null)
        {

            Type type = typeof(T);
            HSSFWorkbook workbook = new HSSFWorkbook();

            foreach (var list in lists)
            {
                ISheet sheet = workbook.CreateSheet(list.Key);

                #region 表頭
                IRow headerRow = sheet.CreateRow(0);
                int colIndex = 0;
                foreach (var dic in dics)
                {
                    headerRow.CreateCell(colIndex).SetCellValue(dic.Value);
                    colIndex++;
                }
                #endregion

                #region 內容
                int rowIndex = 1;
                foreach (var item in list.Value)
                {
                    IRow dataRow = sheet.CreateRow(rowIndex);
                    colIndex = 0;
                    foreach (var dic in dics)
                    {
                        object value = (cols != null && cols.Contains(dic.Key)) ? string.Empty : type.GetProperty(dic.Key).GetValue(item, null);
                        dataRow.CreateCell(colIndex).SetCellValue(value.ToString());
                        colIndex++;
                    }
                    rowIndex++;
                }
                #endregion


            }
            using (MemoryStream ms = new MemoryStream())
            {
                workbook.Write(ms);
                ms.Flush();
                ms.Position = 0;
                return ms;
            }
        }


        /// <summary>
        /// 讀取整個excel文件數據
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static List<T> ReadExcel<T>(string filePath)
        {
            List<T> objs = new List<T>();
            try
            {
                NPOI4ExcelHelper excelHelper = new NPOI4ExcelHelper(filePath);
                IDBAccess _dbAccess = DBFactory.getDBAccess(DBType.MySql, "");

                System.Data.DataTable _dt = excelHelper.SheetData();
                if (_dt != null && _dt.Rows.Count > 0)
                {
                    objs = _dbAccess.getObjByTable<T>(_dt);
                    objs = objs.Skip(1).ToList();
                }

            }
            catch (Exception ex)
            {
                throw new Exception("ExcelHelperXhf-->ReadExcel<T>-->" + ex.Message, ex);
            }
            return objs;
        }

        /// <summary>
        /// DataTable导出到Excel文件
        /// </summary>
        /// <param name="dtSource">源DataTable</param>
        /// <param name="strHeaderText">表头文本</param>
        /// <param name="strFileName">保存位置</param>
        public static void ExportDTtoExcel(DataTable dtSource, string strHeaderText, string strFileName)
        {
            using (MemoryStream ms = ExportDT(dtSource, strHeaderText))
            {
                using (FileStream fs = new FileStream(strFileName, FileMode.Create, FileAccess.Write))
                {
                    byte[] data = ms.ToArray();
                    fs.Write(data, 0, data.Length);
                    fs.Flush();
                }
            }
        }
        public static void ExportDataTabletoExcel(DataTable dtSource, string strHeaderText, string strFileName)
        {
            using (MemoryStream ms = ExportDataTable(dtSource, strHeaderText))
            {
                using (FileStream fs = new FileStream(strFileName, FileMode.Create, FileAccess.Write))
                {
                    byte[] data = ms.ToArray();
                    fs.Write(data, 0, data.Length);
                    fs.Flush();
                }
            }
        }
        #endregion

        #region 从excel2003中将数据导出到datatable
        /// <summary>读取excel
        /// 默认第一行为标头
        /// </summary>
        /// <param name="strFileName">excel文档路径</param>
        /// <returns></returns>
        public static DataTable ImportExcel2003toDt(string strFileName)
        {
            DataTable dt = new DataTable();
            IWorkbook hssfworkbook;
            using (FileStream file = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
            {
                hssfworkbook = new HSSFWorkbook(file);
            }
            HSSFSheet sheet = hssfworkbook.GetSheetAt(0) as HSSFSheet;
            dt = ImportExcel2003InDt(sheet, 0, true);
            return dt;
        }

        /// <summary>读取excel
        /// 默认第一行为标头
        /// </summary>
        /// <param name="strFileName">excel文档路径</param>
        /// <returns></returns>



        /// <summary>
        /// 读取excel
        /// </summary>
        /// <param name="strFileName">excel文件路径</param>
        /// <param name="sheet">需要导出的sheet</param>
        /// <param name="HeaderRowIndex">列头所在行号，-1表示没有列头</param>
        /// <returns></returns>
        public static DataTable ImportExcel2003toDt(string strFileName, string SheetName, int HeaderRowIndex)
        {
            IWorkbook workbook;
            using (FileStream file = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
            {
                workbook = new HSSFWorkbook(file);
            }
            HSSFSheet sheet = workbook.GetSheet(SheetName) as HSSFSheet;
            DataTable table = new DataTable();
            table = ImportExcel2003InDt(sheet, HeaderRowIndex, true);
            //ExcelFileStream.Close();
            workbook = null;
            sheet = null;
            return table;
        }
        /// <summary>读取excel
        /// 默认第一行为标头
        /// </summary>
        /// <param name="strFileName">excel文档路径</param>
        /// <returns></returns>
        public static DataTable ImportExcel2007toDt(string strFileName)
        {
            DataTable dt = new DataTable();
            IWorkbook hssfworkbook;
            using (FileStream file = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
            {
                hssfworkbook = new XSSFWorkbook(file);
            }
            ISheet sheet = hssfworkbook.GetSheetAt(0);
            dt = ImportExcel2007InDt(sheet, 0, true);
            return dt;
        }
        /// <summary>
        /// 读取excel
        /// </summary>
        /// <param name="strFileName">excel文件路径</param>
        /// <param name="sheet">需要导出的sheet</param>
        /// <param name="HeaderRowIndex">列头所在行号，-1表示没有列头</param>
        /// <returns></returns>
        public static DataTable ImportExcel2007toDt(string strFileName, string SheetName, int HeaderRowIndex)
        {
            IWorkbook workbook;
            using (FileStream file = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
            {
                workbook = new XSSFWorkbook(file);
            }
            HSSFSheet sheet = workbook.GetSheet(SheetName) as HSSFSheet;
            DataTable table = new DataTable();
            table = ImportExcel2007InDt(sheet, HeaderRowIndex, true);
            //ExcelFileStream.Close();
            workbook = null;
            sheet = null;
            return table;
        }

        /// <summary>
        /// 读取excel
        /// </summary>
        /// <param name="strFileName">excel文件路径</param>
        /// <param name="sheet">需要导出的sheet序号</param>
        /// <param name="HeaderRowIndex">列头所在行号，-1表示没有列头</param>
        /// <returns></returns>
        public static DataTable ImportExcel2003toDt(string strFileName, int SheetIndex, int HeaderRowIndex)
        {
            HSSFWorkbook workbook;
            using (FileStream file = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
            {
                workbook = new HSSFWorkbook(file);
            }
            HSSFSheet sheet = workbook.GetSheetAt(SheetIndex) as HSSFSheet;
            DataTable table = new DataTable();
            table = ImportExcel2003InDt(sheet, HeaderRowIndex, true);
            //ExcelFileStream.Close();
            workbook = null;
            sheet = null;
            return table;
        }
        /// <summary>
        /// 读取excel
        /// </summary>
        /// <param name="strFileName">excel文件路径</param>
        /// <param name="sheet">需要导出的sheet序号</param>
        /// <param name="HeaderRowIndex">列头所在行号，-1表示没有列头</param>
        /// <returns></returns>
        public static DataTable ImportExcel2007toDt(string strFileName, int SheetIndex, int HeaderRowIndex)
        {
            IWorkbook workbook;
            using (FileStream file = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
            {
                workbook = new XSSFWorkbook(file);
            }
            HSSFSheet sheet = workbook.GetSheetAt(SheetIndex) as HSSFSheet;
            DataTable table = new DataTable();
            table = ImportExcel2007InDt(sheet, HeaderRowIndex, true);
            //ExcelFileStream.Close();
            workbook = null;
            sheet = null;
            return table;
        }

        /// <summary>
        /// 读取excel
        /// </summary>
        /// <param name="strFileName">excel文件路径</param>
        /// <param name="sheet">需要导出的sheet</param>
        /// <param name="HeaderRowIndex">列头所在行号，-1表示没有列头</param>
        /// <returns></returns>
        public static DataTable ImportExcel2003toDt(string strFileName, string SheetName, int HeaderRowIndex, bool needHeader)
        {
            IWorkbook workbook;
            using (FileStream file = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
            {
                workbook = new HSSFWorkbook(file);
            }
            HSSFSheet sheet = workbook.GetSheet(SheetName) as HSSFSheet;
            DataTable table = new DataTable();
            table = ImportExcel2003InDt(sheet, HeaderRowIndex, needHeader);
            //ExcelFileStream.Close();
            workbook = null;
            sheet = null;
            return table;
        }

        /// <summary>
        /// 读取excel
        /// </summary>
        /// <param name="strFileName">excel文件路径</param>
        /// <param name="sheet">需要导出的sheet序号</param>
        /// <param name="HeaderRowIndex">列头所在行号，-1表示没有列头</param>
        /// <returns></returns>
        public static DataTable ImportExcel2003toDt(string strFileName, int SheetIndex, int HeaderRowIndex, bool needHeader)
        {
            HSSFWorkbook workbook;
            using (FileStream file = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
            {
                workbook = new HSSFWorkbook(file);
            }
            HSSFSheet sheet = workbook.GetSheetAt(SheetIndex) as HSSFSheet;
            DataTable table = new DataTable();
            table = ImportExcel2003InDt(sheet, HeaderRowIndex, needHeader);
            //ExcelFileStream.Close();
            workbook = null;
            sheet = null;
            return table;
        }

        static DataTable ImportExcel2003InDt(ISheet sheet, int HeaderRowIndex, bool needHeader)
        {
            DataTable table = new DataTable();
            HSSFRow headerRow;
            int cellCount;
            try
            {
                if (HeaderRowIndex < 0 || !needHeader)
                {
                    headerRow = sheet.GetRow(0) as HSSFRow;
                    cellCount = headerRow.LastCellNum;

                    for (int i = headerRow.FirstCellNum; i <= cellCount; i++)
                    {
                        DataColumn column = new DataColumn(Convert.ToString(i));
                        table.Columns.Add(column);
                    }
                }
                else
                {
                    headerRow = sheet.GetRow(HeaderRowIndex) as HSSFRow;
                    cellCount = headerRow.LastCellNum;

                    for (int i = headerRow.FirstCellNum; i <= cellCount; i++)
                    {
                        if (headerRow.GetCell(i) == null)
                        {
                            if (table.Columns.IndexOf(Convert.ToString(i)) > 0)
                            {
                                DataColumn column = new DataColumn(Convert.ToString("重复列名" + i));
                                table.Columns.Add(column);
                            }
                            else
                            {
                                DataColumn column = new DataColumn(Convert.ToString(i));
                                table.Columns.Add(column);
                            }

                        }
                        else if (table.Columns.IndexOf(headerRow.GetCell(i).ToString()) > 0)
                        {
                            DataColumn column = new DataColumn(Convert.ToString("重复列名" + i));
                            table.Columns.Add(column);
                        }
                        else
                        {
                            DataColumn column = new DataColumn(headerRow.GetCell(i).ToString());
                            table.Columns.Add(column);
                        }
                    }
                }
                int rowCount = sheet.LastRowNum;
                for (int i = (HeaderRowIndex + 1); i <= sheet.LastRowNum; i++)
                {
                    try
                    {
                        HSSFRow row;
                        if (sheet.GetRow(i) == null)
                        {
                            row = sheet.CreateRow(i) as HSSFRow;
                        }
                        else
                        {
                            row = sheet.GetRow(i) as HSSFRow;
                        }

                        DataRow dataRow = table.NewRow();

                        for (int j = row.FirstCellNum; j <= cellCount; j++)
                        {
                            try
                            {
                                if (row.GetCell(j) != null)
                                {
                                    switch (row.GetCell(j).CellType)
                                    {
                                        case CellType.STRING:
                                            string str = row.GetCell(j).StringCellValue;
                                            if (str != null && str.Length > 0)
                                            {
                                                dataRow[j] = str.ToString();
                                            }
                                            else
                                            {
                                                dataRow[j] = null;
                                            }
                                            break;
                                        case CellType.NUMERIC:
                                            if (DateUtil.IsCellDateFormatted(row.GetCell(j)))
                                            {
                                                dataRow[j] = DateTime.FromOADate(row.GetCell(j).NumericCellValue);
                                            }
                                            else
                                            {
                                                dataRow[j] = Convert.ToDouble(row.GetCell(j).NumericCellValue);
                                            }
                                            break;
                                        case CellType.BOOLEAN:
                                            dataRow[j] = Convert.ToString(row.GetCell(j).BooleanCellValue);
                                            break;
                                        case CellType.ERROR:
                                            dataRow[j] = ErrorEval.GetText(row.GetCell(j).ErrorCellValue);
                                            break;
                                        case CellType.FORMULA:
                                            switch (row.GetCell(j).CachedFormulaResultType)
                                            {
                                                case CellType.STRING:
                                                    string strFORMULA = row.GetCell(j).StringCellValue;
                                                    if (strFORMULA != null && strFORMULA.Length > 0)
                                                    {
                                                        dataRow[j] = strFORMULA.ToString();
                                                    }
                                                    else
                                                    {
                                                        dataRow[j] = null;
                                                    }
                                                    break;
                                                case CellType.NUMERIC:
                                                    dataRow[j] = Convert.ToString(row.GetCell(j).NumericCellValue);
                                                    break;
                                                case CellType.BOOLEAN:
                                                    dataRow[j] = Convert.ToString(row.GetCell(j).BooleanCellValue);
                                                    break;
                                                case CellType.ERROR:
                                                    dataRow[j] = ErrorEval.GetText(row.GetCell(j).ErrorCellValue);
                                                    break;
                                                default:
                                                    dataRow[j] = "";
                                                    break;
                                            }
                                            break;
                                        default:
                                            dataRow[j] = "";
                                            break;
                                    }
                                }
                            }
                            catch (Exception exception)
                            {
                                //wl.WriteLogs(exception.ToString());
                            }
                        }
                        table.Rows.Add(dataRow);
                    }
                    catch (Exception exception)
                    {
                        //wl.WriteLogs(exception.ToString());
                    }
                }
            }
            catch (Exception exception)
            {
                //wl.WriteLogs(exception.ToString());
            }
            return table;
        }

        /// <summary>
        /// 将制定sheet中的数据导出到datatable中
        /// </summary>
        /// <param name="sheet">需要导出的sheet</param>
        /// <param name="HeaderRowIndex">列头所在行号，-1表示没有列头</param>
        /// <returns></returns>
        static DataTable ImportExcel2007InDt(ISheet sheet, int HeaderRowIndex, bool needHeader)
        {
            DataTable table = new DataTable();
            NPOI.XSSF.UserModel.XSSFRow headerRow;
            int cellCount;
            try
            {
                if (HeaderRowIndex < 0 || !needHeader)
                {
                    headerRow = sheet.GetRow(0) as NPOI.XSSF.UserModel.XSSFRow;
                    cellCount = headerRow.LastCellNum;

                    for (int i = headerRow.FirstCellNum; i <= cellCount; i++)
                    {
                        DataColumn column = new DataColumn(Convert.ToString(i));
                        table.Columns.Add(column);
                    }
                }
                else
                {
                    headerRow = sheet.GetRow(HeaderRowIndex) as NPOI.XSSF.UserModel.XSSFRow;
                    cellCount = headerRow.LastCellNum;

                    for (int i = headerRow.FirstCellNum; i <= cellCount; i++)
                    {
                        if (headerRow.GetCell(i) == null)
                        {
                            if (table.Columns.IndexOf(Convert.ToString(i)) > 0)
                            {
                                DataColumn column = new DataColumn(Convert.ToString("重复列名" + i));
                                table.Columns.Add(column);
                            }
                            else
                            {
                                DataColumn column = new DataColumn(Convert.ToString(i));
                                table.Columns.Add(column);
                            }

                        }
                        else if (table.Columns.IndexOf(headerRow.GetCell(i).ToString()) > 0)
                        {
                            DataColumn column = new DataColumn(Convert.ToString("重复列名" + i));
                            table.Columns.Add(column);
                        }
                        else
                        {
                            DataColumn column = new DataColumn(headerRow.GetCell(i).ToString());
                            table.Columns.Add(column);
                        }
                    }
                }
                int rowCount = sheet.LastRowNum;
                for (int i = (HeaderRowIndex + 1); i <= sheet.LastRowNum; i++)
                {
                    try
                    {
                        NPOI.XSSF.UserModel.XSSFRow row;
                        if (sheet.GetRow(i) == null)
                        {
                            row = sheet.CreateRow(i) as NPOI.XSSF.UserModel.XSSFRow;
                        }
                        else
                        {
                            row = sheet.GetRow(i) as NPOI.XSSF.UserModel.XSSFRow;
                        }

                        DataRow dataRow = table.NewRow();

                        for (int j = row.FirstCellNum; j <= cellCount; j++)
                        {
                            try
                            {
                                if (row.GetCell(j) != null)
                                {
                                    switch (row.GetCell(j).CellType)
                                    {
                                        case CellType.STRING:
                                            string str = row.GetCell(j).StringCellValue;
                                            if (str != null && str.Length > 0)
                                            {
                                                dataRow[j] = str.ToString();
                                            }
                                            else
                                            {
                                                dataRow[j] = null;
                                            }
                                            break;
                                        case CellType.NUMERIC:
                                            if (DateUtil.IsCellDateFormatted(row.GetCell(j)))
                                            {
                                                dataRow[j] = DateTime.FromOADate(row.GetCell(j).NumericCellValue);
                                            }
                                            else
                                            {
                                                dataRow[j] = Convert.ToDouble(row.GetCell(j).NumericCellValue);
                                            }
                                            break;
                                        case CellType.BOOLEAN:
                                            dataRow[j] = Convert.ToString(row.GetCell(j).BooleanCellValue);
                                            break;
                                        case CellType.ERROR:
                                            dataRow[j] = ErrorEval.GetText(row.GetCell(j).ErrorCellValue);
                                            break;
                                        case CellType.FORMULA:
                                            switch (row.GetCell(j).CachedFormulaResultType)
                                            {
                                                case CellType.STRING:
                                                    string strFORMULA = row.GetCell(j).StringCellValue;
                                                    if (strFORMULA != null && strFORMULA.Length > 0)
                                                    {
                                                        dataRow[j] = strFORMULA.ToString();
                                                    }
                                                    else
                                                    {
                                                        dataRow[j] = null;
                                                    }
                                                    break;
                                                case CellType.NUMERIC:
                                                    dataRow[j] = Convert.ToString(row.GetCell(j).NumericCellValue);
                                                    break;
                                                case CellType.BOOLEAN:
                                                    dataRow[j] = Convert.ToString(row.GetCell(j).BooleanCellValue);
                                                    break;
                                                case CellType.ERROR:
                                                    dataRow[j] = ErrorEval.GetText(row.GetCell(j).ErrorCellValue);
                                                    break;
                                                default:
                                                    dataRow[j] = "";
                                                    break;
                                            }
                                            break;
                                        default:
                                            dataRow[j] = "";
                                            break;
                                    }
                                }
                            }
                            catch (Exception exception)
                            {
                                //wl.WriteLogs(exception.ToString());
                            }
                        }
                        table.Rows.Add(dataRow);
                    }
                    catch (Exception exception)
                    {
                        //wl.WriteLogs(exception.ToString());
                    }
                }
            }
            catch (Exception exception)
            {
                //wl.WriteLogs(exception.ToString());
            }
            return table;
        }
        #endregion

        #region 更新excel中的数据
        /// <summary>
        /// 更新Excel表格
        /// </summary>
        /// <param name="outputFile">需更新的excel表格路径</param>
        /// <param name="sheetname">sheet名</param>
        /// <param name="updateData">需更新的数据</param>
        /// <param name="coluid">需更新的列号</param>
        /// <param name="rowid">需更新的开始行号</param>
        public static void UpdateExcel(string outputFile, string sheetname, string[] updateData, int coluid, int rowid)
        {
            FileStream readfile = new FileStream(outputFile, FileMode.Open, FileAccess.Read);

            HSSFWorkbook hssfworkbook = new HSSFWorkbook(readfile);
            ISheet sheet1 = hssfworkbook.GetSheet(sheetname);
            for (int i = 0; i < updateData.Length; i++)
            {
                try
                {
                    if (sheet1.GetRow(i + rowid) == null)
                    {
                        sheet1.CreateRow(i + rowid);
                    }
                    if (sheet1.GetRow(i + rowid).GetCell(coluid) == null)
                    {
                        sheet1.GetRow(i + rowid).CreateCell(coluid);
                    }

                    sheet1.GetRow(i + rowid).GetCell(coluid).SetCellValue(updateData[i]);
                }
                catch (Exception ex)
                {
                    // wl.WriteLogs(ex.ToString());
                    throw;
                }
            }
            try
            {
                readfile.Close();
                FileStream writefile = new FileStream(outputFile, FileMode.Create, FileAccess.Write);
                hssfworkbook.Write(writefile);
                writefile.Close();
            }
            catch (Exception ex)
            {
                // wl.WriteLogs(ex.ToString());
            }

        }

        /// <summary>
        /// 更新Excel表格
        /// </summary>
        /// <param name="outputFile">需更新的excel表格路径</param>
        /// <param name="sheetname">sheet名</param>
        /// <param name="updateData">需更新的数据</param>
        /// <param name="coluids">需更新的列号</param>
        /// <param name="rowid">需更新的开始行号</param>
        public static void UpdateExcel(string outputFile, string sheetname, string[][] updateData, int[] coluids, int rowid)
        {
            FileStream readfile = new FileStream(outputFile, FileMode.Open, FileAccess.Read);

            HSSFWorkbook hssfworkbook = new HSSFWorkbook(readfile);
            readfile.Close();
            ISheet sheet1 = hssfworkbook.GetSheet(sheetname);
            for (int j = 0; j < coluids.Length; j++)
            {
                for (int i = 0; i < updateData[j].Length; i++)
                {
                    try
                    {
                        if (sheet1.GetRow(i + rowid) == null)
                        {
                            sheet1.CreateRow(i + rowid);
                        }
                        if (sheet1.GetRow(i + rowid).GetCell(coluids[j]) == null)
                        {
                            sheet1.GetRow(i + rowid).CreateCell(coluids[j]);
                        }
                        sheet1.GetRow(i + rowid).GetCell(coluids[j]).SetCellValue(updateData[j][i]);
                    }
                    catch (Exception ex)
                    {
                        // wl.WriteLogs(ex.ToString());
                    }
                }
            }
            try
            {
                FileStream writefile = new FileStream(outputFile, FileMode.Create);
                hssfworkbook.Write(writefile);
                writefile.Close();
            }
            catch (Exception ex)
            {
                //wl.WriteLogs(ex.ToString());
            }
        }

        /// <summary>
        /// 更新Excel表格
        /// </summary>
        /// <param name="outputFile">需更新的excel表格路径</param>
        /// <param name="sheetname">sheet名</param>
        /// <param name="updateData">需更新的数据</param>
        /// <param name="coluid">需更新的列号</param>
        /// <param name="rowid">需更新的开始行号</param>
        public static void UpdateExcel(string outputFile, string sheetname, double[] updateData, int coluid, int rowid)
        {
            FileStream readfile = new FileStream(outputFile, FileMode.Open, FileAccess.Read);

            HSSFWorkbook hssfworkbook = new HSSFWorkbook(readfile);
            ISheet sheet1 = hssfworkbook.GetSheet(sheetname);
            for (int i = 0; i < updateData.Length; i++)
            {
                try
                {
                    if (sheet1.GetRow(i + rowid) == null)
                    {
                        sheet1.CreateRow(i + rowid);
                    }
                    if (sheet1.GetRow(i + rowid).GetCell(coluid) == null)
                    {
                        sheet1.GetRow(i + rowid).CreateCell(coluid);
                    }

                    sheet1.GetRow(i + rowid).GetCell(coluid).SetCellValue(updateData[i]);
                }
                catch (Exception ex)
                {
                    //wl.WriteLogs(ex.ToString());
                    throw;
                }
            }
            try
            {
                readfile.Close();
                FileStream writefile = new FileStream(outputFile, FileMode.Create, FileAccess.Write);
                hssfworkbook.Write(writefile);
                writefile.Close();
            }
            catch (Exception ex)
            {
                //wl.WriteLogs(ex.ToString());
            }

        }

        /// <summary>
        /// 更新Excel表格
        /// </summary>
        /// <param name="outputFile">需更新的excel表格路径</param>
        /// <param name="sheetname">sheet名</param>
        /// <param name="updateData">需更新的数据</param>
        /// <param name="coluids">需更新的列号</param>
        /// <param name="rowid">需更新的开始行号</param>
        public static void UpdateExcel(string outputFile, string sheetname, double[][] updateData, int[] coluids, int rowid)
        {
            FileStream readfile = new FileStream(outputFile, FileMode.Open, FileAccess.Read);

            HSSFWorkbook hssfworkbook = new HSSFWorkbook(readfile);
            readfile.Close();
            ISheet sheet1 = hssfworkbook.GetSheet(sheetname);
            for (int j = 0; j < coluids.Length; j++)
            {
                for (int i = 0; i < updateData[j].Length; i++)
                {
                    try
                    {
                        if (sheet1.GetRow(i + rowid) == null)
                        {
                            sheet1.CreateRow(i + rowid);
                        }
                        if (sheet1.GetRow(i + rowid).GetCell(coluids[j]) == null)
                        {
                            sheet1.GetRow(i + rowid).CreateCell(coluids[j]);
                        }
                        sheet1.GetRow(i + rowid).GetCell(coluids[j]).SetCellValue(updateData[j][i]);
                    }
                    catch (Exception ex)
                    {
                        //wl.WriteLogs(ex.ToString());
                    }
                }
            }
            try
            {
                FileStream writefile = new FileStream(outputFile, FileMode.Create);
                hssfworkbook.Write(writefile);
                writefile.Close();
            }
            catch (Exception ex)
            {
                //wl.WriteLogs(ex.ToString());
            }
        }

        #endregion

        public static int GetSheetNumber(string outputFile)
        {
            int number = 0;
            try
            {
                FileStream readfile = new FileStream(outputFile, FileMode.Open, FileAccess.Read);

                HSSFWorkbook hssfworkbook = new HSSFWorkbook(readfile);
                number = hssfworkbook.NumberOfSheets;

            }
            catch (Exception exception)
            {
                //wl.WriteLogs(exception.ToString());
            }
            return number;
        }

        public static ArrayList GetSheetName(string outputFile)
        {
            ArrayList arrayList = new ArrayList();
            try
            {
                FileStream readfile = new FileStream(outputFile, FileMode.Open, FileAccess.Read);

                HSSFWorkbook hssfworkbook = new HSSFWorkbook(readfile);
                for (int i = 0; i < hssfworkbook.NumberOfSheets; i++)
                {
                    arrayList.Add(hssfworkbook.GetSheetName(i));
                }
            }
            catch (Exception exception)
            {
                //wl.WriteLogs(exception.ToString());
            }
            return arrayList;
        }

        public static bool isNumeric(String message, out double result)
        {
            Regex rex = new Regex(@"^[-]?\d+[.]?\d*$");
            result = -1;
            if (rex.IsMatch(message))
            {
                result = double.Parse(message);
                return true;
            }
            else
                return false;

        }

        public static StringWriter SetCsvFromDataBySdy(System.Data.DataTable dt, string FileName)
        {
            StringWriter sw = new StringWriter();
            StringBuilder sb = new StringBuilder();
            //for (int i = 0; i < dt.Columns.Count; i++)
            //{
            //    sb.Append(dt.Columns[i].ColumnName);
            //    if (i != dt.Columns.Count - 1)
            //    {
            //        sb.Append(",");
            //    }
            //}
            //sw.WriteLine(sb.ToString());
            foreach (DataRow dr in dt.Rows)
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    sw.Write(dr[i].ToString().Replace(',', '，').Replace("\r", "").Replace("\n", ""));
                    if (i != dt.Columns.Count - 1)
                    {
                        sw.Write(",");
                    }
                }
                sw.WriteLine("");
            }
            sw.Close();
            return sw;
        }


        public static StringWriter SetCsvFromData(System.Data.DataTable dt, string FileName)
        {
            StringWriter sw = new StringWriter();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                sb.Append(dt.Columns[i].ColumnName);
                if (i != dt.Columns.Count - 1)
                {
                    sb.Append(",");
                }
            }
            sw.WriteLine(sb.ToString());
            foreach (DataRow dr in dt.Rows)
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    sw.Write(dr[i].ToString());
                    if (i != dt.Columns.Count - 1)
                    {
                        sw.Write(",");
                    }
                }
                sw.WriteLine("");
            }
            sw.Close();
            return sw;
        }



        /// <summary>
        /// DataTable导出到Excel的MemoryStream
        /// </summary>
        /// <param name="dtSource">源DataTable</param>
        /// <param name="strHeaderText">SheetName</param>
        public static MemoryStream ExportDTNoColumnsBySdy(List<DataTable> dtSource, List<string> SheetName, List<bool> ColumnsName)
        {
            HSSFWorkbook workbook = new HSSFWorkbook();
            for (int k = 0; k < dtSource.Count; k++)
            {
                HSSFSheet sheet = workbook.CreateSheet(SheetName[k].ToString()) as HSSFSheet;
                #region 右击文件 属性信息

                //{
                //    DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
                //    dsi.Company = "http://www.yongfa365.com/";
                //    workbook.DocumentSummaryInformation = dsi;

                //    SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
                //    si.Author = "柳永法"; //填加xls文件作者信息
                //    si.ApplicationName = "NPOI测试程序"; //填加xls文件创建程序信息
                //    si.LastAuthor = "柳永法2"; //填加xls文件最后保存者信息
                //    si.Comments = "说明信息"; //填加xls文件作者信息
                //    si.Title = "NPOI测试"; //填加xls文件标题信息
                //    si.Subject = "NPOI测试Demo"; //填加文件主题信息
                //    si.CreateDateTime = DateTime.Now;
                //    workbook.SummaryInformation = si;
                //}

                #endregion
                HSSFCellStyle dateStyle = workbook.CreateCellStyle() as HSSFCellStyle;
                HSSFDataFormat format = workbook.CreateDataFormat() as HSSFDataFormat;
                dateStyle.DataFormat = format.GetFormat("yyyy-mm-dd");
                //取得列宽
                int[] arrColWidth = new int[dtSource[k].Columns.Count];
                foreach (DataColumn item in dtSource[k].Columns)
                {
                    arrColWidth[item.Ordinal] = Encoding.GetEncoding(936).GetBytes(item.ColumnName.ToString()).Length;
                }
                for (int i = 0; i < dtSource[k].Rows.Count; i++)
                {
                    for (int j = 0; j < dtSource[k].Columns.Count; j++)
                    {
                        int intTemp = Encoding.GetEncoding(936).GetBytes(dtSource[k].Rows[i][j].ToString()).Length;
                        if (intTemp > arrColWidth[j])
                        {
                            arrColWidth[j] = intTemp;
                        }
                    }
                }
                int rowIndex = 0;

                foreach (DataRow row in dtSource[k].Rows)
                {
                    #region 新建表，填充表头，填充列头，样式
                    if (rowIndex == 65535 || rowIndex == 0)
                    {
                        if (rowIndex != 0)
                        {
                            sheet = workbook.CreateSheet() as HSSFSheet;
                        }
                        #region 表头及样式
                        //if ("" != strHeaderText)
                        //{
                        //    HSSFRow headerRow = sheet.CreateRow(0) as HSSFRow;
                        //    headerRow.HeightInPoints = 25;
                        //    headerRow.CreateCell(0).SetCellValue(strHeaderText);
                        //    HSSFCellStyle headStyle = workbook.CreateCellStyle() as HSSFCellStyle;
                        //    headStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.CENTER;
                        //    HSSFFont font = workbook.CreateFont() as HSSFFont;
                        //    font.FontHeightInPoints = 20;
                        //    font.Boldweight = 700;
                        //    headStyle.SetFont(font);
                        //    headerRow.GetCell(0).CellStyle = headStyle;
                        //    sheet.AddMergedRegion(new Region(0, 0, 0, dtSource.Columns.Count - 1));
                        //    //headerRow.Dispose();
                        //    rowIndex++;
                        //}
                        #endregion

                        #region 列头及样式
                        if (ColumnsName[k])
                        {
                            HSSFRow headerRow = sheet.CreateRow(rowIndex) as HSSFRow;
                            HSSFCellStyle headStyle = workbook.CreateCellStyle() as HSSFCellStyle;
                            headStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.CENTER;
                            HSSFFont font = workbook.CreateFont() as HSSFFont;
                            font.FontHeightInPoints = 10;
                            font.Boldweight = 700;
                            headStyle.SetFont(font);
                            foreach (DataColumn column in dtSource[k].Columns)
                            {
                                headerRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);
                                // headerRow.GetCell(column.Ordinal).CellStyle = headStyle;

                                //设置列宽
                                if (arrColWidth[column.Ordinal] > 100)
                                {
                                    sheet.SetColumnWidth(column.Ordinal, 100 * 256);
                                }
                                else
                                {
                                    sheet.SetColumnWidth(column.Ordinal, (arrColWidth[column.Ordinal] + 1) * 256);
                                }
                            }
                            //headerRow.Dispose();
                        }
                        #endregion
                        rowIndex++;
                    }
                    #endregion

                    #region 填充内容

                    HSSFRow dataRow = sheet.CreateRow(rowIndex) as HSSFRow;
                    foreach (DataColumn column in dtSource[k].Columns)
                    {
                        HSSFCell newCell = dataRow.CreateCell(column.Ordinal) as HSSFCell;
                        string drValue = row[column].ToString();
                        newCell.SetCellValue(drValue);
                        switch (column.DataType.ToString())
                        {
                            case "System.String": //字符串类型
                                double result;
                                if (isNumeric(drValue, out result))
                                {
                                    double.TryParse(drValue, out result);
                                    newCell.SetCellValue(result);
                                }
                                else
                                {
                                    newCell.SetCellValue(drValue);
                                }
                                break;
                            case "System.DateTime": //日期类型
                                DateTime dateV;
                                DateTime.TryParse(drValue, out dateV);
                                newCell.SetCellValue(dateV);
                                newCell.CellStyle = dateStyle; //格式化显示
                                break;
                            case "System.Boolean": //布尔型
                                bool boolV = false;
                                bool.TryParse(drValue, out boolV);
                                newCell.SetCellValue(boolV);
                                break;
                            case "System.Int16": //整型
                            case "System.Int32":
                            case "System.Int64":
                            case "System.Byte":
                            case "System.UInt32":
                                int intV = 0;
                                int.TryParse(drValue, out intV);
                                newCell.SetCellValue(intV);
                                break;
                            case "System.Decimal": //浮点型
                            case "System.Double":
                                double doubV = 0;
                                double.TryParse(drValue, out doubV);
                                newCell.SetCellValue(doubV);
                                break;
                            case "System.DBNull": //空值处理
                                newCell.SetCellValue("");
                                break;
                            default:
                                newCell.SetCellValue("");
                                break;
                        }
                    }
                    #endregion
                    rowIndex++;
                }
            }
            using (MemoryStream ms = new MemoryStream())
            {
                workbook.Write(ms);
                ms.Flush();
                ms.Position = 0;

                //sheet;
                //workbook.Dispose();

                return ms;
            }
        }
        #region Excel讀取多個Sheet
        public DataSet GetExcelModel(string Path)
        {

            DataSet ExcelData = new DataSet();
            using (FileStream fs = new FileStream(Path, FileMode.OpenOrCreate))
            {
                IWorkbook workbook = WorkbookFactory.Create(fs);
                int num = workbook.NumberOfSheets;//獲取Excel裡面的表數量
                for (int a = 0; a < num; a++)
                {
                    ISheet sheet = workbook.GetSheetAt(a);//通過索引獲取循環中的表

                    DataTable table = new DataTable();
                    //获取sheet的首行
                    IRow headerRow = sheet.GetRow(0);

                    //一行最后一个方格的编号 即总的列数
                    int cellCount = headerRow.LastCellNum;

                    ///用户生成表头
                    for (int i = headerRow.FirstCellNum; i < cellCount; i++)
                    {
                        DataColumn column = new DataColumn(headerRow.GetCell(i).StringCellValue);
                        table.Columns.Add(column);
                    }

                    //最后一列的标号  即总的行数
                    //int rowCount = sheet.LastRowNum;
                    for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
                    {
                        IRow row = sheet.GetRow(i);
                        DataRow dataRow = table.NewRow();

                        for (int j = row.FirstCellNum; j < cellCount; j++)
                        {
                            if (row.GetCell(j) != null)
                                dataRow[j] = row.GetCell(j).ToString();
                        }
                        table.Rows.Add(dataRow);
                    }
                    ExcelData.Tables.Add(table);
                }
            }
            return ExcelData;
        }
        #endregion

    }

}