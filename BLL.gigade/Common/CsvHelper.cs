using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;

namespace BLL.gigade.Common
{
    public class CsvHelper
    {
        public CsvHelper()
        {

        }


        #region 匯入匯出csv檔案
        ///// <summary>
        ///// Exports the datagrid to CSV.
        ///// </summary>
        ///// <param name="dataGridView1">The data grid view1.</param>
        ///// <param name="FileName">Name of the file.</param>
        ///// <param name="ColumnName">Name of the column.</param>
        ///// <param name="HasColumnName">if set to <c>true</c> [has column name].</param>
        //public static void ExportDatagridToCsv(DataGridView dataGridView1, string FileName, string[] ColumnName, bool HasColumnName)
        //{
        //    string strValue = string.Empty;
        //    //CSV 匯出的標題 要先塞一樣的格式字串 充當標題
        //    if (HasColumnName == true)
        //        strValue = string.Join(",", ColumnName);
        //    for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
        //    {
        //        for (int j = 0; j < dataGridView1.Rows[i].Cells.Count; j++)
        //        {
        //            if (!string.IsNullOrEmpty(dataGridView1[j, i].Value.ToString()))
        //            {
        //                if (j > 0)
        //                    strValue = strValue + "," + dataGridView1[j, i].Value.ToString();
        //                else
        //                {
        //                    if (string.IsNullOrEmpty(strValue))
        //                        strValue = dataGridView1[j, i].Value.ToString();
        //                    else
        //                        strValue = strValue + Environment.NewLine + dataGridView1[j, i].Value.ToString();
        //                }
        //            }
        //            else
        //            {
        //                if (j > 0)
        //                    strValue = strValue + ",";
        //                else
        //                    strValue = strValue + Environment.NewLine;
        //            }
        //        }

        //    }
        //    //存成檔案
        //    string strFile = FileName;
        //    if (!string.IsNullOrEmpty(strValue))
        //    {
        //        File.WriteAllText(strFile, strValue, Encoding.Default);
        //    }
        //}


        /// <summary>
        /// Exports the data table to CSV.
        /// </summary>
        /// <param name="dt">The dt.</param>
        /// <param name="FileName">Name of the file.</param>
        /// <param name="ColumnName">Name of the column.</param>
        /// <param name="HasColumnName">if set to <c>true</c> [has column name].</param>
        public static void ExportDataTableToCsv(DataTable dt, string FileName, string[] ColumnName, bool HasColumnName)
        {
            //string strValue = string.Empty;
            ////CSV 匯出的標題 要先塞一樣的格式字串 充當標題
            //if (HasColumnName == true)
            //    strValue = string.Join(",", ColumnName);
            //for (int i = 0; i < dt.Rows.Count; i++)
            //{
            //    for (int j = 0; j < dt.Columns.Count; j++)
            //    {
            //        if (!string.IsNullOrEmpty(dt.Rows[i][j].ToString()))
            //        {
            //            if (j > 0)
            //                strValue = strValue + "," + dt.Rows[i][j].ToString();
            //            else
            //            {
            //                if (string.IsNullOrEmpty(strValue))
            //                    strValue = dt.Rows[i][j].ToString();
            //                else
            //                    strValue = strValue + Environment.NewLine + dt.Rows[i][j].ToString();
            //            }
            //        }
            //        else
            //        {
            //            if (j > 0)
            //                strValue = strValue + ",";
            //            else
            //                strValue = strValue + Environment.NewLine;
            //        }
            //    }

            //}
            ////存成檔案
            //string strFile = FileName;
            //if (!string.IsNullOrEmpty(strValue))
            //{
            //    File.WriteAllText(strFile, strValue, Encoding.Default);
            //}

            StringBuilder sbValue = new StringBuilder();
            //CSV 匯出的標題 要先塞一樣的格式字串 充當標題
            if (HasColumnName == true)
            {
                //strValue = string.Join(",", ColumnName);
                sbValue.Append(string.Join(",", ColumnName));
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    if (!string.IsNullOrEmpty(dt.Rows[i][j].ToString()))
                    {
                        if (j > 0)
                        {
                            //strValue = strValue + "," + dt.Rows[i][j].ToString();
                            sbValue.Append("," + dt.Rows[i][j].ToString());
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(sbValue.ToString()))
                            {
                                //strValue = dt.Rows[i][j].ToString();
                                sbValue.Clear();
                                sbValue.Append(dt.Rows[i][j].ToString());
                            }
                            else
                            {
                                //strValue = strValue + Environment.NewLine + dt.Rows[i][j].ToString();
                                sbValue.Append(Environment.NewLine + dt.Rows[i][j].ToString());
                            }
                        }
                    }
                    else
                    {
                        if (j > 0)
                        {
                            //strValue = strValue + ",";
                            sbValue.Append(",");
                        }
                        else
                        {
                            //strValue = strValue + Environment.NewLine;
                            sbValue.Append(Environment.NewLine);
                        }
                    }
                }

            }
            //存成檔案
            string strFile = FileName;
            if (!string.IsNullOrEmpty(sbValue.ToString()))
            {
                File.WriteAllText(strFile, sbValue.ToString(), Encoding.Default);
            }
        }
        /// <summary>
        /// Reads the CSV to list.
        /// </summary>
        /// <param name="FileName">Name of the file.</param>
        /// <returns></returns>
        public static List<string> ReadCsvToList(string FileName)
        {
            FileInfo fi = new FileInfo(FileName);
            if (fi.Exists)
            {
                List<string> result = new List<string>();
                using (StreamReader sr = new StreamReader(FileName))
                {
                    while (sr.Peek() >= 0)
                    {
                        result.Add(sr.ReadLine());
                    }

                }
                return result;
            }
            else return null;
        }
        public static List<string> ReadCsvToList_CN(string FileName)
        {
            FileInfo fi = new FileInfo(FileName);
            if (fi.Exists)
            {
                List<string> result = new List<string>();
                using (StreamReader sr = new StreamReader(FileName, System.Text.Encoding.Default))
                {
                    while (sr.Peek() >= 0)
                    {
                        result.Add(sr.ReadLine());
                    }

                }
                return result;
            }
            else return null;
        }
        /// <summary>
        /// Reads the CSV to data table.
        /// </summary>
        /// <param name="FileName">Name of the file.</param>
        /// <param name="HasColumnName">if set to <c>true</c> [has column name].</param>
        /// <returns></returns>
        public static DataTable ReadCsvToDataTable(string FileName, bool HasColumnName)
        {
            List<string> Input = ReadCsvToList(FileName);
            if (Input != null)
            {
                string[] sep = new string[] { "," };
                DataTable dt = new DataTable();
                int StartCount = (HasColumnName == true) ? 1 : 0;
                string[] ColumnName = Input[0].Split(sep, StringSplitOptions.None);
                for (int i = 0; i < ColumnName.Length; i++)
                    dt.Columns.Add((HasColumnName == true) ? ColumnName[i] : "C" + i.ToString(), typeof(string));
                for (int j = StartCount; j < Input.Count; j++)
                {
                    string[] valuetemp = Input[j].Split(sep, StringSplitOptions.None);
                    dt.Rows.Add(valuetemp);
                }
                return dt;
            }
            else return null;
        }
        public static DataTable ReadCsvToDataTable_CN(string FileName, bool HasColumnName)
        {
            List<string> Input = ReadCsvToList_CN(FileName);
            if (Input != null)
            {
                string[] sep = new string[] { "," };
                DataTable dt = new DataTable();
                int StartCount = (HasColumnName == true) ? 1 : 0;
                string[] ColumnName = Input[0].Split(sep, StringSplitOptions.None);
                for (int i = 0; i < ColumnName.Length; i++)
                    dt.Columns.Add((HasColumnName == true) ? ColumnName[i] : "C" + i.ToString(), typeof(string));
                for (int j = StartCount; j < Input.Count; j++)
                {
                    string[] valuetemp = Input[j].Split(sep, StringSplitOptions.None);
                    dt.Rows.Add(valuetemp);
                }
                return dt;
            }
            else return null;
        }
        #endregion


        #region 匯出csv解決特殊符號亂碼
        public static void ExportDataTableToCsvBySdy(DataTable dt, string FileName, string[] ColumnName, bool HasColumnName)
        {
            StringBuilder sbValue = new StringBuilder();
            //CSV 匯出的標題 要先塞一樣的格式字串 充當標題
            if (HasColumnName == true)
            {
                sbValue.Append(string.Join(",", ColumnName));
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    if (!string.IsNullOrEmpty(dt.Rows[i][j].ToString()))
                    {
                        if (j > 0)
                        {
                            sbValue.Append("," + dt.Rows[i][j].ToString());
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(sbValue.ToString()))
                            {
                                sbValue.Clear();
                                sbValue.Append(dt.Rows[i][j].ToString());
                            }
                            else
                            {
                                sbValue.Append(Environment.NewLine + dt.Rows[i][j].ToString());
                            }
                        }
                    }
                    else
                    {
                        if (j > 0)
                        {
                            sbValue.Append(",");
                        }
                        else
                        {
                            sbValue.Append(Environment.NewLine);
                        }
                    }
                }

            }
            //存成檔案
            string strFile = FileName;
            if (!string.IsNullOrEmpty(sbValue.ToString()))
            {
                File.WriteAllText(strFile, sbValue.ToString(), Encoding.UTF8);
            }
        }
        #endregion
        //add by shiwei0620j 201501221105
        public static void TitleCsv(DataTable dt, string FileName,string title, string[] ColumnName, bool HasColumnName)
        {
            string strValue = string.Empty;
            strValue = title + strValue + Environment.NewLine;
            //CSV 匯出的標題 要先塞一樣的格式字串 充當標題
            if (HasColumnName == true)
                strValue += string.Join(",", ColumnName);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    if (!string.IsNullOrEmpty(dt.Rows[i][j].ToString()))
                    {
                        if (j > 0)
                            strValue = strValue + "," + dt.Rows[i][j].ToString();
                        else
                        {
                            if (string.IsNullOrEmpty(strValue))
                                strValue = dt.Rows[i][j].ToString();
                            else
                                strValue = strValue + Environment.NewLine + dt.Rows[i][j].ToString();
                        }
                    }
                    else
                    {
                        if (j > 0)
                            strValue = strValue + ",";
                        else
                            strValue = strValue + Environment.NewLine;
                    }
                }

            }
            //存成檔案
            string strFile = FileName;
            if (!string.IsNullOrEmpty(strValue))
            {
                File.WriteAllText(strFile, strValue, Encoding.Default);
            }
        }

    }
}
