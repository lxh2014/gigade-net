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
    public class PaperDao : IPaperImplDao
    {
        private IDBAccess _access;
        public PaperDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }

        public List<Paper> GetPaperList(Paper p, out int totalCount)
        {
            StringBuilder sqlfield = new StringBuilder();
            StringBuilder sqlfrom = new StringBuilder();
            StringBuilder sqlwhere = new StringBuilder();
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlcount = new StringBuilder();
            sqlfield.AppendLine(@"SELECT paperID,paperName,paperMemo,paperBanner,bannerUrl,");
            sqlfield.AppendFormat(@" isRepeatGift,event_ID,isNewMember,");
            //sqlfield.AppendLine(@"isPromotion,promotionUrl,isGiveBonus,bonusNum,isGiveProduct,productID,");
            sqlfield.AppendLine(@"isRepeatWrite,paperStart,paperEnd,status,creator,created,modifier,modified,ipfrom ");
            sqlcount.Append(" select count(paperID) as totalCount ");
            sql.Append(sqlfield);
            sqlfrom.AppendFormat(" FROM paper WHERE 1=1 ");
            sql.Append(sqlfrom);
            if (p.paperID != 0)
            {
                sqlwhere.AppendFormat(@" AND paperID='{0}' ", p.paperID);
            }
            if (!string.IsNullOrEmpty(p.paperName))
            {
                sqlwhere.AppendFormat(@" AND paperName like N'%{0}%' ", p.paperName);
            }
            if (p.status != 0)
            {
                sqlwhere.AppendFormat(@" AND status='{0}' ", p.status);
            }
            sql.Append(sqlwhere);
            sql.AppendFormat(" order by paperID DESC ");
            totalCount = 0;
            if (p.IsPage)
            {
                sqlcount.Append(sqlfrom.ToString() + sqlwhere.ToString());
                DataTable dt = _access.getDataTable(sqlcount.ToString());
                if (dt != null && dt.Rows.Count > 0)
                {
                    totalCount =Convert.ToInt32(dt.Rows[0]["totalCount"]);
                }
                sql.AppendFormat(@"limit {0},{1};", p.Start, p.Limit);
            }
            try
            {
                return _access.getDataTableForObj<Paper>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PaperDao-->GetPaperList" + ex.Message + sql.ToString() + sqlcount.ToString(), ex);
            }
        }

        public int Add(Paper p)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine(@"INSERT INTO paper (paperName,paperMemo,paperBanner,bannerUrl,");
            //sql.AppendLine(@"isPromotion,promotionUrl,isGiveBonus,bonusNum,isGiveProduct,productID,");
            sql.AppendLine(@" event_id,isRepeatGift,isNewMember,");
            sql.AppendLine(@" isRepeatWrite,paperStart,paperEnd,");
            sql.AppendLine(@"status,creator,created,ipfrom ) VALUES (");
            sql.AppendFormat(@"'{0}','{1}','{2}','{3}',", p.paperName, p.paperMemo, p.paperBanner, p.bannerUrl);
            //sql.AppendFormat(@"'{0}','{1}','{2}','{3}','{4}','{5}',", p.promotionUrl, p.isGiveBonus, p.bonusNum, p.isGiveBonus, p.productID, p.isRepeatWrite);
            sql.AppendFormat(@"'{0}','{1}','{2}',", p.event_ID, p.isRepeatGift, p.isNewMember);
            sql.AppendFormat(@" '{0}','{1}','{2}',", p.isRepeatWrite, p.paperStart.ToString("yyyy-MM-dd HH:mm:ss"), p.paperEnd.ToString("yyyy-MM-dd HH:mm:ss"));
            sql.AppendFormat(@"'{0}','{1}','{2}','{3}')", p.status, p.creator, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), p.ipfrom);
            try
            {
                return _access.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PaperDao-->Add" + ex.Message + sql.ToString(), ex);
            }
        }

        public int Update(Paper p)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat(@"UPDATE paper SET paperName='{0}',paperMemo='{1}',paperBanner='{2}',", p.paperName, p.paperMemo, p.paperBanner);
            sql.AppendFormat(@"bannerUrl='{0}',event_ID='{1}',isRepeatGift='{2}',isNewMember='{3}',", p.bannerUrl, p.event_ID, p.isRepeatGift, p.isNewMember);
            //sql.AppendFormat(@"bannerUrl='{0}',isPromotion='{1}',promotionUrl='{2}',isGiveBonus='{3}',", p.bannerUrl, p.isPromotion, p.promotionUrl, p.isGiveBonus);
            //sql.AppendFormat(@"bonusNum='{0}',isGiveProduct='{1}',productID='{2}',", p.bonusNum, p.isGiveProduct, p.productID, p.isRepeatWrite);
            sql.AppendFormat(@" isRepeatWrite='{0}',paperStart='{1}',paperEnd='{2}',", p.isRepeatWrite, p.paperStart.ToString("yyyy-MM-dd HH:mm:ss"), p.paperEnd.ToString("yyyy-MM-dd HH:mm:ss"));
            sql.AppendFormat(@"modifier='{0}',modified='{1}',ipfrom='{2}' ", p.modifier, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), p.ipfrom);
            sql.AppendFormat(@" where paperID='{0}';", p.paperID);
            try
            {
                return _access.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PaperDao-->Update" + ex.Message + sql.ToString(), ex);
            }
        }
        public int UpdateState(Paper p)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat(@"UPDATE paper SET `status`='{0}',modifier='{1}',", p.status, p.modifier);
            sql.AppendFormat(@"modified='{0}',ipfrom='{1}' ", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), p.ipfrom);
            sql.AppendFormat(@" where paperID='{0}';", p.paperID);
            try
            {
                return _access.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PaperDao-->UpdateState" + ex.Message + sql.ToString(), ex);
            }
        }
    }
}
