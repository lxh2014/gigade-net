using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Dao.Impl;
using DBAccess;
using System.Data;

namespace BLL.gigade.Dao
{
    public class PaperClassDao : IPaperClassImplDao
    {
        private IDBAccess _access;
        public PaperClassDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }
        public List<PaperClass> GetPaperClassList(PaperClass pc, out int totalCount)
        {
            StringBuilder sqlfield = new StringBuilder();
            StringBuilder sqlfrom = new StringBuilder();
            StringBuilder sqlwhere = new StringBuilder();
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlcount = new StringBuilder();
            sqlfield.AppendLine(@"SELECT pc.id,pc.paperID,pc.classID,pc.className,pc.classType,pc.projectNum,");
            sqlfield.AppendLine(@"pc.classContent,pc.orderNum,pc.isMust,pc.`status`");
            sql.Append(sqlfield);
            sql.AppendFormat(@" ,p.paperName ");
            sqlfrom.AppendFormat(@" FROM paper_class pc ");
            sqlfrom.AppendFormat(@" LEFT JOIN paper p ON p.paperID=pc.paperID  WHERE 1=1 ");
            sqlcount.Append(" select count(pc.paperID) as totalCount ");
            sql.Append(sqlfrom);
            if (pc.paperID != 0)
            {
                sqlwhere.AppendFormat(@" AND pc.paperID='{0}' ", pc.paperID);
            }
            if (pc.classID != 0)
            {
                sqlwhere.AppendFormat(@" AND pc.classID='{0}' ", pc.classID);
            }
            sql.Append(sqlwhere);
            sql.AppendFormat(" order by pc.paperID DESC,pc.classID ASC,pc.orderNum ASC ");
            totalCount = 0;
            if (pc.IsPage)
            {
                sqlcount.Append(sqlfrom.ToString() + sqlwhere.ToString());
                sql.AppendFormat(@" limit {0},{1} ", pc.Start, pc.Limit);
                DataTable dt = _access.getDataTable( sqlcount.ToString() );
                if (dt != null && dt.Rows.Count > 0)
                {
                    totalCount =Convert.ToInt32(dt.Rows[0]["totalCount"]);
                }
               
            }
            try
            {
                return _access.getDataTableForObj<PaperClass>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PaperClassDao-->GetPaperClassList" + ex.Message + sql.ToString() + sqlcount.ToString(), ex);
            }
        }


        public int Add(PaperClass pc)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine(@"INSERT INTO paper_class (paperID,classID,className,classType,projectNum,");
            sql.AppendLine(@"classContent,orderNum,isMust,`status` ) VALUES(");
            sql.AppendFormat(@"'{0}','{1}','{2}','{3}','{4}',", pc.paperID, pc.classID, pc.className, pc.classType, pc.projectNum);
            sql.AppendFormat(@"'{0}','{1}','{2}','{3}')", pc.classContent, pc.orderNum, pc.isMust, pc.status);
            try
            {
                return _access.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PaperClassDao-->Add" + ex.Message + sql.ToString(), ex);
            }
        }

        public int Update(PaperClass pc)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat(@"UPDATE paper_class SET ");
            sql.AppendFormat(@" paperID='{0}',classID='{1}',className='{2}',", pc.paperID, pc.classID, pc.className);
            sql.AppendFormat(@" classType='{0}',projectNum='{1}',isMust='{2}', ", pc.classType, pc.projectNum, pc.isMust);
            sql.AppendFormat(@" classContent='{0}',orderNum='{1}' ", pc.classContent, pc.orderNum);
            sql.AppendFormat(@" WHERE id='{0}' ;", pc.id);//根據id來更新題目選項
            try
            {
                return _access.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PaperClassDao-->Update" + ex.Message + sql.ToString(), ex);
            }

        }
        /// <summary>
        /// 更改問卷后，再重組原來的問卷的題目編號
        /// </summary>
        /// <param name="pc"></param>
        /// <returns></returns>
        public int UpdateClassID(PaperClass pc)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat(@"UPDATE paper_class SET ");
            sql.AppendFormat(@" classID='{0}' ", pc.classID);
            sql.AppendFormat(@" WHERE id='{0}' ;", pc.id);//根據id來更新題目選項
            try
            {
                return _access.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PaperClassDao-->UpdateClassID" + ex.Message + sql.ToString(), ex);
            }

        }
        public int UpdateState(string id,int status)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat(@"UPDATE paper_class SET status='{0}' ", status);
            sql.AppendFormat(@" WHERE id in ({0}) ;", id);//根據id來更新狀態
            try
            {
                return _access.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PaperClassDao-->UpdateState" + ex.Message + sql.ToString(), ex);
            }

        }
        /// <summary>
        /// 根據id來刪除一條數據
        /// </summary>
        /// <param name="pc"></param>
        /// <returns></returns>
        public int Delete(PaperClass pc)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat(@"DELETE FROM paper_class WHERE id='{0}';", pc.id);
            try
{
                return _access.execCommand(sql.ToString());
            }
            catch (Exception ex)
    {
                throw new Exception("PaperClassDao-->Delete" + ex.Message + sql.ToString(), ex);
            }

    }
}
}
