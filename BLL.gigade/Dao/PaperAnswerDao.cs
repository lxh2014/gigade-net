using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBAccess;
using BLL.gigade.Model;
using BLL.gigade.Dao.Impl;
using System.Data;

namespace BLL.gigade.Dao
{
    public class PaperAnswerDao : IPaperAnswerImplDao
    {
        private IDBAccess _access;
        public PaperAnswerDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }
        public List<PaperAnswer> GetPaperAnswerList(PaperAnswer pa, out int totalCount)
        {
            StringBuilder sqlfield = new StringBuilder();
            StringBuilder sqlfrom = new StringBuilder();
            StringBuilder sqlwhere = new StringBuilder();
            StringBuilder sql = new StringBuilder();
            StringBuilder sq1count = new StringBuilder();
            sqlfield.AppendLine(@"SELECT pa.answerID,pa.paperID,pa.userid,pa.userMail,pa.classID,");
            sqlfield.AppendLine(@"pa.answerContent,pcc.classContent,pa.classType,pa.answerDate,pa.order_id ");
            sql.Append(sqlfield);
            sql.AppendFormat(@" ,pc.className,p.paperName ");
            sqlfrom.AppendLine(@" from paper_answer pa  ");
            sqlfrom.AppendFormat(@" LEFT JOIN (SELECT DISTINCT(classID),className from  paper_class) pc ON pa.classID=pc.classID  ");
            sqlfrom.AppendFormat(@" LEFT JOIN paper_class pcc ON pa.answerContent=pcc.id ");
            sqlfrom.AppendFormat(@" LEFT JOIN paper p ON pa.paperID=p.paperID  WHERE 1=1");
            sq1count.Append(" select count(answerID) as totalCount ");
            sql.Append(sqlfrom);
            if (pa.paperID != 0)
            {
                sqlwhere.AppendFormat(@" AND pa.paperID='{0}' ", pa.paperID);
            }
            if (pa.classID != 0)
            {
                sqlwhere.AppendFormat(@" AND pa.classID='{0}' ", pa.classID);
            }
            if (pa.userid != 0)
            {
                sqlwhere.AppendFormat(@" AND pa.userid='{0}' ", pa.userid);
            }
            if (pa.answerID != 0)
            {
                sqlwhere.AppendFormat(@" AND pa.answerID='{0}' ", pa.answerID);
            }
            sql.Append(sqlwhere);
            sql.AppendFormat(" ORDER BY pa.answerDate DESC,pa.userid ASC,pa.paperID DESC,pa.classID ASC ");
            totalCount = 0;
            try
            {
                if (pa.IsPage)
                {
                    sq1count.Append(sqlfrom.ToString() + sqlwhere.ToString());
                    sql.AppendFormat(@"  limit {0},{1} ", pa.Start, pa.Limit);
                    DataTable dt = _access.getDataTable(sq1count.ToString());
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        totalCount =Convert.ToInt32(dt.Rows[0]["totalCount"]);
                    }
                }
                return _access.getDataTableForObj<PaperAnswer>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PaperAnswerDao-->GetPaperAnswerList" + ex.Message + sql.ToString() + sq1count.ToString(), ex);
            }
        }

        public System.Data.DataTable Export(PaperAnswer pa)
        {
            StringBuilder sql = new StringBuilder();
            //sql.AppendLine(@"SELECT pa.answerID,pa.paperID,pa.userid,p.paperName,pa.userMail,pa.order_id,");
            //sql.AppendLine(@" pa.classID,pc.className,pa.answerContent,pa.classType,pa.answerDate ");
            //sql.AppendLine(@" from paper_answer pa LEFT JOIN paper_class pc ON pa.classID=pc.classID");
            //sql.AppendLine(@" LEFT JOIN paper p ON pa.paperID=p.paperID WHERE 1=1 ");
            sql.AppendLine(@"SELECT pa.answerID,pa.paperID,p.paperName,pa.userid,pa.userMail,pa.order_id,");
            sql.AppendLine(@" pa.classID,pc.className,pa.classType,pa.answerContent,pa.answerDate,pcc.classContent,pa.order_id  ");
            sql.AppendLine(@" from paper_answer pa ");
            sql.AppendLine(@" LEFT JOIN (SELECT DISTINCT(classID),className from  paper_class) pc ON pa.classID=pc.classID  ");
            sql.AppendLine(@" LEFT JOIN paper_class pcc ON pa.answerContent=pcc.id ");
            sql.AppendLine(@" LEFT JOIN paper p ON pa.paperID=p.paperID WHERE 1=1 ");
            if (pa.paperID != 0)
            {
                sql.AppendFormat(@" AND pa.paperID='{0}' ", pa.paperID);
            }
            if (pa.userid != 0)
            {
                sql.AppendFormat(@" AND pa.userid='{0}' ", pa.userid);
            }
            sql.AppendFormat(@" ORDER BY pa.answerDate DESC,pa.userid ASC,pa.paperID DESC,pa.classID ASC  ");
            try
            {
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PaperAnswerDao-->Export" + ex.Message + sql.ToString(), ex);
            }
        }

        /// <summary>
        /// 獲得問卷的所有題目編號
        /// </summary>
        /// <param name="pc"></param>
        /// <returns></returns>
        public DataTable GetPaperClassID(PaperClass pc)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat(@"SELECT DISTINCT(classID),classType FROM paper_class WHERE paperID='{0}' ORDER BY classID ASC;", pc.paperID);
            try
            {
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PaperAnswerDao-->GetPaperClassID" + ex.Message + sql.ToString(), ex);
            }
 
        }
        /// <summary>
        /// 獲得填寫問卷的所有用戶
        /// </summary>
        /// <param name="pc"></param>
        /// <returns></returns>
        public DataTable GetPaperAnswerUser(PaperAnswer pa)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat(@"SELECT DISTINCT(answerDate),userid,userMail,order_id FROM paper_answer  WHERE paperID='{0}'", pa.paperID);
            sql.AppendFormat(@" ORDER BY answerDate DESC,userid ASC");
            try
            {
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PaperAnswerDao-->GetPaperAnswerUser" + ex.Message + sql.ToString(), ex);
            }

        }
        
        public DataTable ExportSinglePaperAnswer(PaperAnswer pa)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine(@"");
            sql.AppendLine(@" SELECT pa.userid,pa.order_id,pa.answerDate,pa.classID,pa.classType,pa.answerContent,pc.classContent ");
            sql.AppendLine(@" FROM paper_answer pa");
            sql.AppendLine(@" LEFT JOIN paper_class pc ON pa.answerContent=pc.id");
            sql.AppendLine(@" WHERE 1=1  ");
            if(pa.paperID!=0)
            {
                sql.AppendFormat(@" AND pa.paperID='{0}'  ",pa.paperID);
            }
            sql.AppendLine(@" ORDER BY answerDate DESC,pa.userid ASC, pa.classID ASC;");
            try
            {
                return _access.getDataTable(sql.ToString());
            }
            catch(Exception ex)
            {
                throw new Exception("PaperAnswerDao-->ExportSinglePaperAnswer" + ex.Message + sql.ToString(), ex);
            }
        }
    }
}
