using BLL.gigade.Common;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr;
using BLL.gigade.Model.Query;
using DBAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace BLL.gigade.Dao
{
    public class TicketDetailDao : ITicketDetailImplDao
    {
         private IDBAccess _access;
         public TicketDetailDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }
        /// <summary>
        /// 新站訂單詳情管理:列表頁
        /// </summary>
        /// <param name="query"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
         public DataTable GetTicketDetailTable(TicketDetailQuery query, out int totalCount)
         { 
             StringBuilder sql = new StringBuilder();
             StringBuilder sqlCondi = new StringBuilder();
             try
             {
                query.Replace4MySQL();
                sql.AppendFormat(@" select td.ticket_detail_id,td.ticket_master_id,td.single_cost,td.single_money,td.single_price,td.vendor_id,cd.course_id,td.product_name,
 cd.start_date,cd.end_date ");
                sql.AppendFormat(" ,(SELECT spec_name from product_spec ps where pi.product_id=ps.product_id and pi.spec_id_1=ps.spec_id )as spec_id_1 ");
                sql.AppendFormat(" ,(SELECT spec_name from product_spec ps where pi.product_id=ps.product_id and pi.spec_id_2=ps.spec_id )as spec_id_2 ");
                sql.AppendFormat(" ,(SELECT vendor_name_simple FROM vendor v WHERE v.vendor_id=td.vendor_id)AS vendor_name_simple,ct.flag,ct.ticket_code ");
                sqlCondi.AppendFormat("  from ticket_detail td ");
                sqlCondi.AppendFormat("  LEFT JOIN course_detail_item cdi ON cdi.course_detail_item_id=td.cd_item_id ");
                sqlCondi.AppendFormat("  LEFT JOIN course_detail cd ON cd.course_detail_id=cdi.course_detail_id ");
                sqlCondi.AppendFormat("  LEFT JOIN product_item pi ON pi.item_id=cdi.item_id ");
                sqlCondi.AppendFormat("  LEFT JOIN course_ticket ct ON ct.ticket_detail_id=td.ticket_detail_id ");
                sqlCondi.AppendFormat(" where 1=1 "); 
                 if (query.flag != -1)
                 {
                     sqlCondi.AppendFormat(" and ct.flag='{0}' ", query.flag);
                 }
                 if (!string.IsNullOrEmpty(query.TimeStart))
                 {
                     sqlCondi.AppendFormat(" and cd.start_date>='{0}' ", query.TimeStart);
                 }
                 if (!string.IsNullOrEmpty(query.TimeEnd))
                 {
                     sqlCondi.AppendFormat(" and cd.end_date<='{0}' ", query.TimeEnd);
                 }
                 if (query.MDID != 0)
                 {
                     sqlCondi.AppendFormat(" and ( td.ticket_detail_id='{0}' or td.ticket_master_id='{0}' or cd.course_id='{0}') ", query.MDID);
                 }
                sqlCondi.AppendFormat("  ORDER BY td.ticket_detail_id DESC ");
                totalCount = 0;
                if (query.IsPage)
                {
                    DataTable _dt = _access.getDataTable("select count(td.ticket_detail_id) as totalCount " + sqlCondi.ToString());
                    if (_dt.Rows.Count > 0)
                    {
                        totalCount = int.Parse(_dt.Rows[0]["totalCount"].ToString());
                    }

                    sqlCondi.AppendFormat(" limit {0},{1} ", query.Start, query.Limit);
                }

                sql.Append(sqlCondi.ToString());

                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {

                throw new Exception("TicketDetailDao.GetTicketDetailList-->" + ex.Message + sql.ToString(), ex);
            }
        }

        /// <summary>
        /// 供應商後臺檢視使用序號功能 列表
        /// </summary>
        /// <param name="query"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public DataTable GetTicketDetailAllCodeTable(TicketDetailQuery query, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlCondi = new StringBuilder();
            try
            {
                sql.AppendFormat(@"select tm.ticket_master_id,c.course_name,cd.start_date,cd.end_date,ct.ticket_code,ct.flag,");
                sql.AppendFormat("(SELECT spec_name from product_spec ps where pi.product_id=ps.product_id and pi.spec_id_1=ps.spec_id )as spec_id_1, ");
                sql.AppendFormat("(SELECT spec_name from product_spec ps where pi.product_id=ps.product_id and pi.spec_id_2=ps.spec_id )as spec_id_2 ");

                sqlCondi.AppendFormat("  from ticket_detail td ");
                sqlCondi.AppendFormat("  LEFT JOIN ticket_master tm ON td.ticket_master_id=tm.ticket_master_id ");
                sqlCondi.AppendFormat("  LEFT JOIN course_ticket ct ON ct.ticket_detail_id=td.ticket_detail_id ");
                sqlCondi.AppendFormat("  LEFT JOIN course_detail_item cdi ON cdi.course_detail_item_id=td.cd_item_id ");
                sqlCondi.AppendFormat("  LEFT JOIN course_detail cd ON cd.course_detail_id=cdi.course_detail_id ");
                sqlCondi.AppendFormat("  LEFT JOIN product_item pi ON pi.item_id=cdi.item_id ");
                sqlCondi.AppendFormat("LEFT JOIN course c ON c.course_id=cd.course_id ");
                sqlCondi.Append(" where 1=1 ");
                if (query.flag != -1)
                {
                    sqlCondi.AppendFormat(" and ct.flag='{0}' ", query.flag);
                }
                //if (!string.IsNullOrEmpty(query.TimeStart))
                //{
                //    sqlCondi.AppendFormat(" and cd.start_date>='{0}' ", query.TimeStart);
                //}
                //if (!string.IsNullOrEmpty(query.TimeEnd))
                //{
                //    sqlCondi.AppendFormat(" and cd.end_date<='{0}' ", query.TimeEnd);
                //}
                if (!string.IsNullOrEmpty(query.ticket_code))
                {
                    sqlCondi.AppendFormat(" and ( ct.ticket_code='{0}' or td.ticket_master_id='{0}' ) ", query.ticket_code);
                }
                 sqlCondi.Append("  ORDER BY td.ticket_detail_id DESC ");
                totalCount = 0;
                if (query.IsPage)
                {
                    DataTable _dt = _access.getDataTable("select count(td.ticket_detail_id) as totalCount " + sqlCondi.ToString());
                    if (_dt.Rows.Count > 0)
                    {
                        totalCount = int.Parse(_dt.Rows[0]["totalCount"].ToString());
                    }

                    sqlCondi.AppendFormat(" limit {0},{1} ", query.Start, query.Limit);
                }

                sql.Append(sqlCondi.ToString());

                return _access.getDataTable(sql.ToString());
             }
            catch (Exception ex)
             {

                throw new Exception("TicketDetailDao.GetTicketDetailList-->" + ex.Message + sql.ToString(), ex);
             }
         } 
        /// <summary>
        /// 核可
        /// </summary>
        /// <param name="RowId"></param>
        /// <returns></returns>
         public string UpdateTicketStatus(string RowId)
         {
             StringBuilder sql = new StringBuilder();
             try
             {

                 sql.AppendFormat(@" set sql_safe_updates=0;update course_ticket ct set ct.flag=1 where ct.ticket_id in (select td.ticket_id from ticket_detail td  where td.ticket_detail_id  in ({0}) );set sql_safe_updates=1; ", RowId);
                // sql.AppendFormat(" and ct.course_detail_item_id in(select cdi.course_detail_item_id from course_detail_item cdi where cdi.course_detail_id in ({0}) ) ", RowId);
                 return sql.ToString();
             }
             catch (Exception ex)
             {
                 throw new Exception("TicketDetailDao.UpdateTicketStatus-->" + ex.Message + sql.ToString(), ex);
             }


         }
        /// <summary>
        /// 上傳檔案到web
        /// </summary>
        /// <param name="httpPostedFile"></param>
        /// <param name="serverPath"></param>
        /// <param name="fileName"></param>
        /// <param name="extensions"></param>
        /// <param name="maxSize"></param>
        /// <param name="minSize"></param>
        /// <param name="ErrorMsg"></param>
        /// <param name="ftpUser"></param>
        /// <param name="ftpPasswd"></param>
        /// <returns></returns>
         public bool UpLoadFile(HttpPostedFileBase httpPostedFile, string serverPath, string fileName, string extensions, int maxSize, int minSize, ref string ErrorMsg, string ftpUser, string ftpPasswd)
         {
             try
             {
                 int fileSize = httpPostedFile.ContentLength;
                 string extension = System.IO.Path.GetExtension(fileName).ToLower().ToString();
                 string name = System.IO.Path.GetFileName(fileName).ToLower().ToString();
                 extension = extension.Remove(extension.LastIndexOf("."), 1);
                 string[] types = extensions.ToLower().Split(',');

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
                 return true;
             }
             catch (Exception ex)
             {
                 ErrorMsg = Resource.CoreMessage.GetResource("UPLOAD_FAILURE") + "，" + ex.Message + serverPath + "|" + fileName;
                 return false;
             }
         }

    }
}
