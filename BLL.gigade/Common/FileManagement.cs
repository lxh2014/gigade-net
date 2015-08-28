/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：FileManagement 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/8/16 13:59:43 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.Collections;
using System.Web;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using BLL.gigade.Mgr;
using System.Net;
using System.Drawing.Imaging;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace BLL.gigade.Common
{
    public class NPOI4ExcelHelper
    {
        private IWorkbook hssfworkbook = null;
        private string filePath;
        public NPOI4ExcelHelper(string filePath)
        {
            this.filePath = filePath;
            Init();
        }

        private void Init()
        {
            #region//初始化信息
            try
            {
                using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    if (filePath.Trim().ToLower().EndsWith(".xls"))
                    {
                        hssfworkbook = new HSSFWorkbook(file);
                    }
                    else if (filePath.Trim().ToLower().EndsWith(".xlsx"))
                    {
                        hssfworkbook = new XSSFWorkbook(file);
                    }
                    else if (filePath.Trim().ToLower().EndsWith(".csv"))
                    {
                        hssfworkbook = new XSSFWorkbook(file);
                    }
                   
                   
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            #endregion
        }

        public ArrayList SheetNameForExcel()
        {
            if (hssfworkbook == null) { return null; };

            ArrayList names = new ArrayList();
            int i = hssfworkbook.NumberOfSheets;
            for (int j = 0; j < i; j++)
            {
                string sheet = hssfworkbook.GetSheetName(j);
                if (!names.Contains(sheet))
                {
                    names.Add(sheet);
                }
            }
            return names;
        }

        public DataTable SheetData(int sheetIndex = 0)
        {
            if (filePath.Trim().ToLower().EndsWith(".xls"))
            {
                return ExcelToTableForXLS(sheetIndex);
            }
            else if (filePath.Trim().ToLower().EndsWith(".xlsx"))
            {
                return ExcelToTableForXLSX(sheetIndex);
            }
            else if (filePath.Trim().ToLower().EndsWith(".csv"))
            {
                return ExcelToTableForXLSX(sheetIndex);
            }
           
            return null;
        }

        /// <summary>
        /// 讀取excel文件第一個sheet的資料  07
        /// </summary>
        /// <param name="filePath">文件路徑</param>
        /// <returns></returns>
        private DataTable ExcelToTableForXLSX(int sheetIndex = 0)
        {
            if (hssfworkbook == null) { return null; };

            ISheet sheet = hssfworkbook.GetSheetAt(sheetIndex);
            System.Collections.IEnumerator rows = sheet.GetRowEnumerator();

            DataTable dt = new DataTable();

            //第一行添加未table的列名
            //一行最后一个方格的编号 即总的列数
            rows.MoveNext();
            IRow headerRow = (XSSFRow)rows.Current;
            for (int j = 0; j < (sheet.GetRow(0).LastCellNum); j++)
            {
                dt.Columns.Add(headerRow.GetCell(j) != null ? headerRow.GetCell(j).StringCellValue.Trim() : "");
            }

            while (rows.MoveNext())
            {
                IRow row = (XSSFRow)rows.Current;
                DataRow dr = dt.NewRow();

                for (int i = 0; i < (row.LastCellNum <= dt.Columns.Count ? row.LastCellNum : dt.Columns.Count); i++)
                {
                    ICell cell = row.GetCell(i);

                    if (cell == null)
                    {
                        dr[i] = null;
                    }
                    else
                    {
                        switch (cell.CellType) { 
                            case CellType.NUMERIC:
                                DateTime value = new DateTime();
                                if (DateTime.TryParse(cell.ToString(), out value))
                                    dr[i] = cell.DateCellValue.ToString("yyyy/MM/dd hh:mm:ss");
                                else
                                    dr[i] = cell.NumericCellValue.ToString();
                                break;
                            default:
                                dr[i] = cell.ToString().Trim();
                                break;
                        }
                        
                    }
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }
        /// <summary>
        /// 讀取excel文件第一個sheet的資料  03
        /// </summary>
        /// <param name="filePath">文件路徑</param>
        /// <returns></returns>
        private DataTable ExcelToTableForXLS(int sheetIndex = 0)
        {
            if (hssfworkbook == null) { return null; };

            ISheet sheet = hssfworkbook.GetSheetAt(sheetIndex);
            System.Collections.IEnumerator rows = sheet.GetRowEnumerator();

            DataTable dt = new DataTable();

            //第一行添加未table的列名
            //一行最后一个方格的编号 即总的列数
            rows.MoveNext();
            IRow headerRow = (HSSFRow)rows.Current;
            for (int j = 0; j < (sheet.GetRow(0).LastCellNum); j++)
            {
                dt.Columns.Add(headerRow.GetCell(j) != null ? headerRow.GetCell(j).StringCellValue.Trim() : "");
            }

            while (rows.MoveNext())
            {
                IRow row = (HSSFRow)rows.Current;
                DataRow dr = dt.NewRow();

                for (int i = 0; i < (row.LastCellNum <= dt.Columns.Count ? row.LastCellNum : dt.Columns.Count); i++)
                {
                    ICell cell = row.GetCell(i);

                    if (cell == null)
                    {
                        dr[i] = null;
                    }
                    else
                    {
                        switch (cell.CellType)
                        {
                            case CellType.NUMERIC:
                                DateTime value = new DateTime();
                                if (DateTime.TryParseExact(cell.ToString(),"M/d/y H:m",null,System.Globalization.DateTimeStyles.None, out value)
                                    ||DateTime.TryParse(cell.ToString(),out value))
                                    dr[i] = cell.DateCellValue.ToString("yyyy/MM/dd HH:mm:ss");//將Excel單元格中的時間轉為24小時制   jinfeng 
                                else
                                    dr[i] = cell.ToString();
                                break;
                            default:
                                dr[i] = cell.ToString().Trim();
                                break;
                        }
                    }
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }
    }

    public class FileManagement
    {
        public FileManagement()
        {

        }

        /// <summary>
        /// 返回連接excel字符串
        /// </summary>
        /// <param name="fileName">數據源地址</param>
        /// <returns></returns>
        public string GetExcelConnStr(string fileName)
        {
            string strConOLE = string.Empty;
            if (!System.IO.File.Exists(fileName))
            {
                return strConOLE;
            }
            string extension = System.IO.Path.GetExtension(fileName).ToLower().ToString();
            if (extension.CompareTo(".xls") == 0)//针对2003格式进行的判断
            {
                strConOLE = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileName + ";Extended Properties='Excel 8.0;HDR=YES;IMEX=1;'";
            }
            else if (extension.CompareTo(".xlsx") == 0)//针对2007格式进行的判断
            {
                strConOLE = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileName + ";Extended Properties='Excel 12.0 Xml;HDR=YES;IMEX=1;'";
            }
            return strConOLE;
        }

        /// <summary>
        /// 返回Excel 所有sheet名稱
        /// </summary>
        /// <param name="fileName">連接字符串</param>
        /// <returns></returns>
        public ArrayList GetExcelTables(string connStr)
        {
            OleDbConnection oleConn = new OleDbConnection(connStr);
            ArrayList sheetList = new ArrayList();
            try
            {
                if (oleConn.State == ConnectionState.Closed)
                {
                    oleConn.Open();
                }
                DataTable dt_Sheets = oleConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
                foreach (DataRow row in dt_Sheets.Rows)
                {
                    sheetList.Add(row["TABLE_NAME"].ToString());
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                oleConn.Close();
            }
            return sheetList;
        }

        /// <summary>
        /// 上傳文件
        /// </summary>
        /// <param name="httpPostedFile">源文件</param>
        /// <param name="fileName">保存的文件名</param>
        /// <param name="extensions">允許上傳格式</param>
        /// <param name="maxSize">允許文件上傳最大值 單位KB</param>
        /// <param name="minSize">語序上傳最小值 單位KB</param>
        /// <returns></returns>
        public bool UpLoadFile(HttpPostedFileBase httpPostedFile, string fileName, string extensions, int maxSize, int minSize)
        {
            //增加錯誤提示  edit by zhuoqin0830w  2015/05/05
            string ErrorMsg = string.Empty;
            try
            {
                WebClient wc = new WebClient();
                int fileSize = httpPostedFile.ContentLength;
                string extension = System.IO.Path.GetExtension(fileName).ToLower().ToString();
                extension = extension.Remove(extension.LastIndexOf("."), 1);
                string[] types = extensions.ToLower().Split(',');
                if (extensions.ToLower().IndexOf(extension) < 0)
                {
                    ErrorMsg = Resource.CoreMessage.GetResource("FILE_EXTENTION_NO_CURRECT") + extensions;
                    throw new Exception(ErrorMsg);
                }
                if (fileSize > maxSize * 1024)
                {
                    ErrorMsg = Resource.CoreMessage.GetResource("MAXSIZE_LIMIT") + maxSize + Resource.CoreMessage.GetResource("UNIT");
                    throw new Exception(ErrorMsg);
                }
                if (fileSize < minSize * 1024)
                {
                    ErrorMsg = Resource.CoreMessage.GetResource("MINSIZE_LIMIT") + minSize + Resource.CoreMessage.GetResource("UNIT");
                    throw new Exception(ErrorMsg);
                }
                httpPostedFile.SaveAs(fileName);
                return true;
            }
            catch (Exception ex)
            {
                ErrorMsg = Resource.CoreMessage.GetResource("UPLOAD_FAILURE") + "，" + ex.Message + "|" + fileName;
                throw new Exception(ErrorMsg);
            }
        }

        public bool UpLoadFile(HttpPostedFileBase httpPostedFile, string serverPath, string fileName, string extensions, int maxSize, int minSize, ref string ErrorMsg, string ftpUser, string ftpPasswd)
        {
            int fileSize = httpPostedFile.ContentLength;
            string extension = System.IO.Path.GetExtension(fileName).ToLower().ToString();
            string name = System.IO.Path.GetFileName(fileName).ToLower().ToString();
            extension = extension.Remove(extension.LastIndexOf("."), 1);
            string[] types = extensions.ToLower().Split(',');
            if (extensions.ToLower().IndexOf(extension) < 0)
            {
                ErrorMsg = Resource.CoreMessage.GetResource("FILE_EXTENTION_NO_CURRECT") + extensions;
                return false;
            }
            try
            {
                if (fileSize > maxSize * 1024)
                {
                    ErrorMsg = Resource.CoreMessage.GetResource("MAXSIZE_LIMIT") + maxSize + Resource.CoreMessage.GetResource("UNIT");
                    return false;
                }
                if (fileSize < minSize * 1024)
                {
                    ErrorMsg = Resource.CoreMessage.GetResource("MINSIZE_LIMIT") + minSize + Resource.CoreMessage.GetResource("UNIT");
                    return false;
                }
                if (!Directory.Exists(serverPath))
                    Directory.CreateDirectory(serverPath);
                httpPostedFile.SaveAs(serverPath + "\\" + name);
                FTP ftp = new FTP(fileName, ftpUser, ftpPasswd);
                ftp.UploadFile(serverPath + "\\" + name);
                //ftp.UploadFile(serverPath + name);
                return true;
            }
            catch (Exception ex)
            {
                ErrorMsg = Resource.CoreMessage.GetResource("UPLOAD_FAILURE") + "，" + ex.Message + serverPath + "|" + fileName;
                return false;
            }
        }
        //yafeng0715j 201507131845 該方法不上傳FTP圖片
        public bool UpLoadFileNoFTP(HttpPostedFileBase httpPostedFile, string serverPath, string fileName, string extensions, int maxSize, int minSize, ref string ErrorMsg, string ftpUser, string ftpPasswd)
        {
            int fileSize = httpPostedFile.ContentLength;
            string extension = System.IO.Path.GetExtension(fileName).ToLower().ToString();
            string name = System.IO.Path.GetFileName(fileName).ToLower().ToString();
            extension = extension.Remove(extension.LastIndexOf("."), 1);
            string[] types = extensions.ToLower().Split(',');
            if (extensions.ToLower().IndexOf(extension) < 0)
            {
                ErrorMsg = Resource.CoreMessage.GetResource("FILE_EXTENTION_NO_CURRECT") + extensions;
                return false;
            }
            try
            {
                if (fileSize > maxSize * 1024)
                {
                    ErrorMsg = Resource.CoreMessage.GetResource("MAXSIZE_LIMIT") + maxSize + Resource.CoreMessage.GetResource("UNIT");
                    return false;
                }
                if (fileSize < minSize * 1024)
                {
                    ErrorMsg = Resource.CoreMessage.GetResource("MINSIZE_LIMIT") + minSize + Resource.CoreMessage.GetResource("UNIT");
                    return false;
                }
                if (!Directory.Exists(serverPath))
                    Directory.CreateDirectory(serverPath);
                httpPostedFile.SaveAs(serverPath + "\\" + name);
                return true;
            }
            catch (Exception ex)
            {
                ErrorMsg = Resource.CoreMessage.GetResource("UPLOAD_FAILURE") + "，" + ex.Message + serverPath + "|" + fileName;
                return false;
            }
        }
        //shiwei0620j 201506031922
        public bool ZIPUpLoadFile(HttpPostedFileBase httpPostedFile, string serverPath, string fileName, string extensions, int maxSize, int minSize, ref string ErrorMsg, string ftpUser, string ftpPasswd,string sourcePath)
        {
            int fileSize = httpPostedFile.ContentLength;
         
            string extension = System.IO.Path.GetExtension(fileName).ToLower().ToString();
            string name = System.IO.Path.GetFileName(fileName).ToLower().ToString();
            extension = extension.Remove(extension.LastIndexOf("."), 1);
            string[] types = extensions.ToLower().Split(',');
            if (extensions.ToLower().IndexOf(extension) < 0)
            {
                ErrorMsg = Resource.CoreMessage.GetResource("FILE_EXTENTION_NO_CURRECT") + extensions;
                return false;
            }
            try
            {
                if (fileSize > 200 * 1024)
                {
                    ErrorMsg = "文件大小不能超過200KB";
                    return false;
                }
                if (fileSize < minSize * 1024)
                {
                    ErrorMsg = Resource.CoreMessage.GetResource("MINSIZE_LIMIT") + minSize + Resource.CoreMessage.GetResource("UNIT");
                    return false;
                }
                if (!Directory.Exists(serverPath))
                    Directory.CreateDirectory(serverPath);
                //if (fileSize > 150 * 1024)
                //{
                //    #region 如果要壓縮圖片則限制寬度為725，高度保留原圖尺寸
                //    WebRequest request = WebRequest.Create(sourcePath);
                //    request.Credentials = CredentialCache.DefaultCredentials;
                //    Stream s = request.GetResponse().GetResponseStream();
                //    byte[]b=new byte[fileSize];
                //    MemoryStream mes_keleyi_com = new MemoryStream(b);
                //    s.Read(b,0,fileSize);
                //    s.Close();
                //    Image image = Image.FromStream(mes_keleyi_com);
                //    int width = 725;
                //    int height = image.Height;
                //    #endregion
                //    GetPicThumbnail(sourcePath, serverPath + "\\" + name, height, width, 150);
                //}
                //else
                //{
                    httpPostedFile.SaveAs(serverPath + "\\" + name);
              //  }
                FTP ftp = new FTP(fileName, ftpUser, ftpPasswd);
                ftp.UploadFile(serverPath + "\\" + name);
                return true;
            }
            catch (Exception ex)
            {
                ErrorMsg = Resource.CoreMessage.GetResource("UPLOAD_FAILURE") + "，" + ex.Message + serverPath + "|" + fileName;
                return false;
            }
        }
        //shiwei0620j 201506031922
        public bool GetPicThumbnail(string sFile, string dFile, int dHeight, int dWidth, int flag)
        {
            System.Drawing.Image iSource = System.Drawing.Image.FromFile(sFile);

            ImageFormat tFormat = iSource.RawFormat;

            int sW = 0, sH = 0;

            //按比例缩放

            Size tem_size = new Size(iSource.Width, iSource.Height);



            if (tem_size.Width > dHeight || tem_size.Width > dWidth) //将**改成c#中的或者操作符号
            {

                if ((tem_size.Width * dHeight) > (tem_size.Height * dWidth))
                {

                    sW = dWidth;

                    sH = (dWidth * tem_size.Height) / tem_size.Width;

                }

                else
                {
                    sH = dHeight;
                    sW = (tem_size.Width * dHeight) / tem_size.Height;
                }
            }
            else
            {
                sW = tem_size.Width;
                sH = tem_size.Height;
            }
            Bitmap ob = new Bitmap(dWidth, dHeight);
            Graphics g = Graphics.FromImage(ob);
            g.Clear(Color.WhiteSmoke);
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.DrawImage(iSource, new Rectangle((dWidth - sW) / 2, (dHeight - sH) / 2, sW, sH), 0, 0, iSource.Width, iSource.Height, GraphicsUnit.Pixel);
            g.Dispose();
            //以下代码为保存图片时，设置压缩质量
            EncoderParameters ep = new EncoderParameters();
            long[] qy = new long[1];
            qy[0] = flag;//设置压缩的比例1-100
            EncoderParameter eParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, qy);
            ep.Param[0] = eParam;
            try
            {
                ImageCodecInfo[] arrayICI = ImageCodecInfo.GetImageEncoders();
                ImageCodecInfo jpegICIinfo = null;
                for (int x = 0; x < arrayICI.Length; x++)
                {
                    if (arrayICI[x].FormatDescription.Equals("JPEG"))
                    {
                        jpegICIinfo = arrayICI[x];

                        break;
                    }

                }
                if (jpegICIinfo != null)
                {
                    ob.Save(dFile, jpegICIinfo, ep);//dFile是压缩后的新路径
                }
                else
                {

                    ob.Save(dFile, tFormat);
                }

                return true;

            }

            catch
            {

                return false;

            }

            finally
            {

                iSource.Dispose();

                ob.Dispose();

            }
        }
        /// <summary>
        /// 上傳文件
        /// </summary>
        /// <param name="httpPostedFile">源文件</param>
        /// <param name="fileName">保存的文件名</param>
        /// <param name="extensions">允許上傳格式</param>
        /// <param name="maxSize">允許文件上傳最大值 單位KB</param>
        /// <param name="minSize">語序上傳最小值 單位KB</param>
        /// <returns></returns>
        public bool UpLoadFile(HttpPostedFileBase httpPostedFile, string fileName, string extensions, int maxSize, int minSize, ref string ErrorMsg)
        {
            int fileSize = httpPostedFile.ContentLength;
            string extension = System.IO.Path.GetExtension(fileName).ToLower().ToString();
            extension = extension.Remove(extension.LastIndexOf("."), 1);
            string[] types = extensions.ToLower().Split(',');
            if (extensions.ToLower().IndexOf(extension) < 0)
            {
                ErrorMsg = Resource.CoreMessage.GetResource("FILE_EXTENTION_NO_CURRECT") + extensions;
                return false;
            }
            try
            {
                if (fileSize > maxSize * 1024)
                {
                    ErrorMsg = Resource.CoreMessage.GetResource("MAXSIZE_LIMIT") + maxSize + Resource.CoreMessage.GetResource("UNIT");
                    return false;
                }
                if (fileSize < minSize * 1024)
                {
                    ErrorMsg = Resource.CoreMessage.GetResource("MINSIZE_LIMIT") + minSize + Resource.CoreMessage.GetResource("UNIT");
                    return false;
                }
                httpPostedFile.SaveAs(fileName);
                return true;
            }
            catch (Exception)
            {
                ErrorMsg = Resource.CoreMessage.GetResource("UPLOAD_FAILURE");
                return false;
            }
        }

        public string NewFileName(string oldName)
        {
            string newName = string.Empty;
            if (oldName.LastIndexOf(".") != -1)
            {
                string[] strs = oldName.Split('.');
                newName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + "." + strs[strs.Length - 1];
            }
            return newName;
        }
    }
}
