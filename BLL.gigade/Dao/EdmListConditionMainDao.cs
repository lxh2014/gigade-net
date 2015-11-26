/*
* 文件名稱 :EDM
* 文件功能描述 :EDM條件篩選主表功能
* 版權宣告 :
* 開發人員 : jialei,jiaohe
* 版本資訊 : 1.0
* 創建日期 : 2015/07/21
* 修改人員 :
* 版本資訊 : 
* 修改日期 : 2015/07/21
* 修改備註 : 
	*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBAccess;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using BLL.gigade.Common;
using System.Data;

namespace BLL.gigade.Dao
{
    public class EdmListConditionMainDao
    {
        private IDBAccess _dbAccess;
        private string connStr;
        public EdmListConditionMainDao(string connectionStr)
        {
            _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectionStr);
            this.connStr = connectionStr;
        }

        /// <summary>
        /// 得到篩選條件名稱列表
        /// </summary>
        /// <returns></returns>
        public List<EdmListConditionMain> GetConditionList()
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.AppendFormat(@"SELECT elcm_id,elcm_name FROM edm_list_condition_main  ");
                return _dbAccess.getDataTableForObj<EdmListConditionMain>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(" EdmListConditionMainDao-->GetConditionList " + ex.Message, ex);
            }
        }
        
        /// <summary>
        /// 刪除篩選條件
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public int DeleteListInfo(EdmListConditionMain query)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.AppendFormat(@"DELETE from edm_list_conditoin_sub WHERE elcm_id={0}; ", query.elcm_id);          
                sql.AppendFormat(@"DELETE from edm_list_condition_main WHERE elcm_id={0};", query.elcm_id);                
                return _dbAccess.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(" EdmListConditionMainDao-->DeleteListInfo " + ex.Message, ex);
            }
        }
       
        /// <summary>
        /// 根據篩選條件名稱查詢elcm_id
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public EdmListConditionMain SelectElcmIDByConditionName(string name)
        {
            try
            {
                  StringBuilder sql = new StringBuilder();
                  sql.AppendFormat(@"SELECT elcm_id from edm_list_condition_main WHERE 1=1 ");
                  if (name.Length > 0)
                  {
                      sql.AppendFormat(@"and elcm_name='{0}'; ", name);
                  }
                  EdmListConditionMain model=_dbAccess.getSinggleObj<EdmListConditionMain>(sql.ToString());
                  return model;
            }
            catch (Exception ex)
            {
                 throw new Exception(" EdmListConditionMainDao-->SelectElcmIDByConditionName " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 查詢符合該條件的所有會員以及人數
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public DataTable GetUserNum(EdmListConditoinSubQuery q)
        {//需要主表條件的id
            StringBuilder sql = new StringBuilder();
            StringBuilder join = new StringBuilder();
            StringBuilder where = new StringBuilder();
            StringBuilder buywhere = new StringBuilder();
            DateTime dtime = DateTime.Parse("2010-01-01");
            try
            {
                if (q.MailorPhone == 1)
                {
                    sql.Append(@"SELECT u.user_email,u.user_name,u.user_id FROM  users u ");
                }
                else
                {
                    sql.Append(@"SELECT u.user_mobile,u.user_name,u.user_id FROM  users u ");
                }
                where.Append(@"WHERE 1=1 ");
                if(q.chkGender)
                {//性別
                    if (q.genderCondition < 2)
                    {
                        where.AppendFormat(" AND u.user_gender ='{0}' ", q.genderCondition);
                    }
                }
                if (q.ChkBuy)
                {//購買次數
                    if (q.buyTimeMin >= dtime)
                    {//時間限制
                        buywhere.AppendFormat(" AND FROM_UNIXTIME(order_createdate) >= '{0}' ", q.buyTimeMin.ToString("yyyy/MM/dd 00:00:00"));
                    }
                    if (q.buyTimeMax >= q.buyTimeMin && q.buyTimeMax != DateTime.MinValue)
                    {
                        buywhere.AppendFormat(" AND FROM_UNIXTIME(order_createdate) <= '{0}' ", q.buyTimeMax.ToString("yyyy/MM/dd 23:59:59"));
                    }
                    if (q.buyCondition != 0 && q.buyTimes <= 1)
                    {
                        where.AppendFormat(" AND NOT EXISTS (SELECT om.user_id FROM order_master AS om WHERE om.user_id = u.user_id AND om.order_status NOT IN(90,91) {0}  GROUP BY om.user_id) ", buywhere);
                    }
                    else
                    {
                        where.AppendFormat(" AND gm.gmcs {0} {1} ", q.buyCondition == 0 ? ">" : "<", q.buyTimes);
                        join.AppendFormat(" LEFT JOIN (select user_id,count(om.order_id) as 'gmcs' FROM order_master om WHERE order_status NOT IN(90,91) {0} GROUP BY user_id) gm ON u.user_id=gm.user_id ", buywhere);
                    }
                }
                if (q.ChkAge)
                {//年齡
                    where.AppendFormat(" AND u.user_birthday_year BETWEEN {0} AND {1} ", DateTime.Now.Year - q.ageMax, DateTime.Now.Year - q.ageMin);
                }
                if (q.ChkCancel)
                {//取消次數
                    buywhere.Clear();
                    if (q.cancelTimeMin >= dtime)
                    {//時間限制
                        buywhere.AppendFormat(" AND FROM_UNIXTIME(order_createdate) >= '{0}' ", q.cancelTimeMin.ToString("yyyy/MM/dd 00:00:00"));
                    }
                    if (q.cancelTimeMax >= q.cancelTimeMin && q.cancelTimeMax != DateTime.MinValue)
                    {
                        buywhere.AppendFormat(" AND FROM_UNIXTIME(order_createdate) <= '{0}' ", q.cancelTimeMax.ToString("yyyy/MM/dd 23:59:59"));
                    }
                    if (q.cancelCondition != 0 && q.cancelTimes <= 1)
                    {
                        where.AppendFormat(" AND NOT EXISTS (select user_id FROM order_master om WHERE om.user_id = u.user_id AND order_status IN(90) {0} GROUP BY om.user_id)", buywhere);
                    }
                    else
                    {
                        where.AppendFormat(" AND qx.qxcs {0} {1} ", q.cancelCondition == 0 ? ">" : "<", q.cancelTimes);
                        join.AppendFormat(" LEFT JOIN (select user_id,count(om.order_id) as 'qxcs' FROM order_master om WHERE order_status IN (90) {0} GROUP BY user_id) qx ON u.user_id=qx.user_id ", buywhere);
                    }
                }
                if (q.ChkRegisterTime)
                {//註冊時間
                    if (q.registerTimeMin >= dtime)
                    {
                        where.AppendFormat(" AND u.user_reg_date >= '{0}'", CommonFunction.GetPHPTime(q.registerTimeMin.ToString("yyyy/MM/dd 00:00:00")));
                    }
                    if (q.registerTimeMax > dtime)
                    {
                        where.AppendFormat(" AND u.user_reg_date <= '{0}'", CommonFunction.GetPHPTime(q.registerTimeMax.ToString("yyyy/MM/dd 23:59:59")));
                    }
                }
                if (q.ChkReturn)
                {//退貨次數
                    buywhere.Clear();
                    if (q.returnTimeMin >= dtime)
                    {//時間限制
                        buywhere.AppendFormat(" AND FROM_UNIXTIME(return_createdate) >= '{0}' ", q.returnTimeMin.ToString("yyyy/MM/dd 00:00:00"));
                    }
                    if (q.returnTimeMax >= q.returnTimeMin && q.returnTimeMax != DateTime.MinValue)
                    {
                        buywhere.AppendFormat(" AND FROM_UNIXTIME(return_createdate) <= '{0}' ", q.returnTimeMax.ToString("yyyy/MM/dd 23:59:59"));
                    }
                    if (q.returnCondition != 0 && q.returnTimes <= 1)
                    {
                        where.AppendFormat("AND NOT EXISTS (select DISTINCT om.user_id FROM order_return_master orm LEFT JOIN order_master om on orm.order_id=om.order_id WHERE om.user_id = u.user_id AND return_status=1 {0}  GROUP BY om.user_id)", buywhere);
                    }
                    else
                    {
                        where.AppendFormat(" AND th.thcs {0} {1} ", q.returnCondition == 0 ? ">" : "<", q.returnTimes);
                        join.AppendFormat(" LEFT JOIN (select om.user_id,Count(orm.return_id) as thcs FROM order_return_master orm LEFT JOIN order_master om on orm.order_id=om.order_id WHERE return_status=1 {0} GROUP BY om.user_id) th ON u.user_id=th.user_id ", buywhere);
                    }
                }
                if (q.ChkLastOrder)
                {//最後訂單
                    join.Append(" LEFT JOIN (SELECT user_id,MAX(order_createdate) as 'order_createdate' from order_master GROUP BY user_id) om ON u.user_id = om.user_id ");
                    if (q.lastOrderMin >= dtime)
                    {
                        where.AppendFormat(" AND FROM_UNIXTIME(om.order_createdate) >= '{0}'", q.lastOrderMin.ToString("yyyy/MM/dd 00:00:00"));
                    }
                    if (q.lastOrderMax > dtime)
                    {
                        where.AppendFormat(" AND FROM_UNIXTIME(om.order_createdate) <= '{0}'", q.lastOrderMax.ToString("yyyy/MM/dd 23:59:59"));
                    }
                }
                if (q.ChkNotice)
                {//貨到通知
                    if (q.noticeCondition != 0 && q.noticeTimes <= 1)
                    {
                        where.Append(" AND NOT EXISTS (SELECT a.user_id,Count(a.id) as hdtz from arrival_notice a WHERE a.user_id = u.user_id GROUP BY user_id )");
                    }
                    else
                    {
                        join.Append(" LEFT JOIN (SELECT user_id,Count(id) as hdtz from arrival_notice GROUP BY user_id) an ON u.user_id = an.user_id ");
                        where.AppendFormat(" AND an.hdtz {0} {1} ", q.noticeCondition == 0 ? ">" : "<", q.noticeTimes);
                    }
                }
                if (q.ChkLastLogin)
                {//最後登入
                    join.Append(" LEFT JOIN (select user_id,MAX(login_createdate) as 'login_createdate' FROM users_login GROUP BY user_id) ul ON u.user_id = ul.user_id ");
                    if (q.lastLoginMin >= dtime)
                    {
                        where.AppendFormat(" AND FROM_UNIXTIME(ul.login_createdate) >= '{0}'", q.lastLoginMin.ToString("yyyy/MM/dd 00:00:00"));
                    }
                    if (q.lastLoginMax > dtime)
                    {
                        where.AppendFormat(" AND FROM_UNIXTIME(ul.login_createdate) <= '{0}'", q.lastLoginMax.ToString("yyyy/MM/dd 23:59:59"));
                    }
                }
                if (q.ChkTotalConsumption) 
                {//消費金額
                    join.Append(" LEFT JOIN (SELECT om.user_id,od.single_money*od.buy_num as 'price' from order_master om LEFT JOIN order_slave os on om.order_id=os.order_id left JOIN order_detail od ON os.slave_id=od.slave_id where od.item_mode in (0,1) GROUP BY om.user_id ) money ON u.user_id = money.user_id ");
                    if (q.totalConsumptionMin >0)
                    {
                        where.AppendFormat(" AND money.price >= '{0}'", q.totalConsumptionMin);
                    }
                    if (q.totalConsumptionMax >= q.totalConsumptionMin && q.totalConsumptionMax != 0)
                    {
                        where.AppendFormat(" AND  money.price <= '{0}'", q.totalConsumptionMax);
                    }
                }
                if (q.ChkBlackList)
                {//不排除黑名單
                    where.Append(" AND u.user_id NOT IN (SELECT user_id from vip_user  WHERE group_id='48' ) ");
                }
                if (q.ChkPhone)
                {//排除拒收廣告簡訊的人
                    where.Append(" AND send_sms_ad=1 ");
                }
                return _dbAccess.getDataTable(sql.ToString() + join.ToString() + where.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(" EdmListConditionMainDao-->GetUserNum " + ex.Message + "sql:" + sql.ToString() + join.ToString() + where.ToString(), ex);
            }
        }



        /// <summary>
        /// 儲存篩選條件名稱
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public int SaveListInfoName(EdmListConditionMain query)
        {
            query.Replace4MySQL();
            StringBuilder sql = new StringBuilder();
            try
            {
                if (query.elcm_name != string.Empty)
                {
                    sql.AppendFormat(@"INSERT INTO edm_list_condition_main (elcm_creator_id, elcm_name, elcm_created) VALUES ('{0}', '{1}', '{2}')", query.elcm_creator_id, query.elcm_name, CommonFunction.DateTimeToString(query.elcm_created));
                }
               return  _dbAccess.execCommand(sql.ToString());               
            }
            catch (Exception ex)
            {
                throw new Exception(" EdmListConditionMainDao-->SaveListInfo " + ex.Message, ex);
            }
        }

        /// <summary>
        /// EDM電子報發送使用
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        //public DataTable GetEmail(EdmListConditoinSubQuery q)
        //{
        //    q.Replace4MySQL();
        //    StringBuilder sql = new StringBuilder();
        //    try
        //    {
        //        if (q.elcm_name != string.Empty)
        //        {
        //            sql.AppendFormat(@"INSERT INTO edm_list_condition_main (elcm_creator_id, elcm_name, elcm_created) VALUES ('{0}', '{1}', '{2}')", q.elcm_creator_id, q.elcm_name, CommonFunction.DateTimeToString(q.elcm_created));
        //        }
        //        return _dbAccess.getDataTable(sql.ToString());
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(" EdmListConditionMainDao-->SaveListInfo " + ex.Message, ex);
        //    }
        //}
    }
}
