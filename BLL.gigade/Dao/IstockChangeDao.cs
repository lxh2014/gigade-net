using BLL.gigade.Model.Query;
using DBAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Common;
using BLL.gigade.Mgr;
using MySql.Data.MySqlClient;

namespace BLL.gigade.Dao
{
    public class IstockChangeDao
    {
        IDBAccess _accessMySql;
        string connStr = string.Empty;
        public IstockChangeDao(string connectionstring)
        {
            _accessMySql = DBFactory.getDBAccess(DBType.MySql, connectionstring);
            this.connStr = connectionstring;
        }

        public int insert(IstockChange q)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat("insert into istock_change(sc_trans_id,sc_cd_id,item_id,sc_trans_type,sc_num_old,sc_num_chg,sc_num_new,sc_time,sc_user,sc_note,sc_istock_why) Values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}')", q.sc_trans_id, q.sc_cd_id, q.item_id, q.sc_trans_type, q.sc_num_old, q.sc_num_chg, q.sc_num_new, CommonFunction.DateTimeToString(q.sc_time), q.sc_user, q.sc_note,q.sc_istock_why);
                return _accessMySql.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("IstockChangeDao-->insert-->" + ex.Message + sb.ToString(), ex);
            }
        }

        public List<IstockChangeQuery> GetIstockChangeList(IstockChangeQuery query, out int totalCount)
        {
            query.Replace4MySQL();
            StringBuilder sbStr= new StringBuilder();
            StringBuilder sbAll = new StringBuilder();
            StringBuilder sbJoin = new StringBuilder();
            StringBuilder sbWhr = new StringBuilder();
            StringBuilder sbPage = new StringBuilder();
            
            sbAll.Append("SELECT ic.item_id,ic.sc_trans_id,ic.sc_cd_id,(CASE WHEN ic.sc_trans_type=1 THEN '收貨上架' WHEN ic.sc_trans_type=2 THEN '庫存調整' WHEN ic.sc_trans_type=3 THEN 'RF理貨' when ic.sc_trans_type=4 then '初盤復盤'END) AS typename,(CASE WHEN ic.sc_istock_why=1 THEN '庫鎖' WHEN ic.sc_istock_why=2 THEN '庫調' WHEN ic.sc_istock_why=3 THEN '收貨上架' WHEN ic.sc_istock_why=4 THEN '理貨' else '' END) AS istockwhy,ic.sc_num_chg,ic.sc_num_new,ic.sc_time,ic.sc_note,p.product_name,mu.user_username AS manager ");
            sbAll.Append("  ,CONCAT(ps.spec_name,'-',ps2.spec_name)as specname ");
            sbJoin.Append(" from istock_change ic LEFT JOIN product_item pi ON ic.item_id=pi.item_id LEFT JOIN product p ON pi.product_id=p.product_id ");
            sbJoin.Append("LEFT JOIN manage_user mu ON ic.sc_user=mu.user_id left join product_spec ps on pi.spec_id_1=ps.spec_id left join product_spec ps2 on pi.spec_id_2=ps2.spec_id where 1=1 ");
            if (query.item_id != 0)
            {
                sbStr.AppendFormat("select item_id from product_item where item_id='{0}';", query.item_id);
                DataTable _dtresult = _accessMySql.getDataTable(sbStr.ToString());
                if (_dtresult.Rows.Count > 0)
                {
                    sbWhr.AppendFormat(" and ic.item_id={0}", query.item_id);
                }
                else 
                {
                    sbWhr.AppendFormat(" and ic.item_id =(select item_id from iupc where upc_id='{0}'limit 1 )", query.item_id); 
                }
            }
            if (query.item_upc != string.Empty)
            {
                sbStr.Clear();
                sbStr.AppendFormat("select item_id from product_item where item_id='{0}';", query.item_upc);
                DataTable _dtresult = _accessMySql.getDataTable(sbStr.ToString());
                if (_dtresult.Rows.Count > 0)
                {
                    sbWhr.AppendFormat(" and ic.item_id={0}", query.item_upc);
                }
                else
                {
                    sbWhr.AppendFormat(" and ic.item_id =(select item_id from iupc where upc_id='{0}' limit 1 )", query.item_upc); 
                }
                //sbWhr.AppendFormat(" and (ic.item_id='{0}' or  pi.item_id in (select item_id from iupc where upc_id='{0}' or item_id='{0}' )) ", query.item_upc);
            }
            if (query.starttime != DateTime.MinValue && query.endtime != DateTime.MinValue)
            {
                sbWhr.AppendFormat(" and ic.sc_time between '{0}' and '{1}'", Common.CommonFunction.DateTimeToString(query.starttime), Common.CommonFunction.DateTimeToString(query.endtime));
            }
            sbWhr.Append(" order by ic.sc_id desc ");
            if (query.IsPage)
            {
                sbPage.AppendFormat("LIMIT {0},{1};", query.Start, query.Limit);
            }
            try
            {
                DataTable dt = _accessMySql.getDataTable("select count(ic.sc_id)" + sbJoin.ToString() + sbWhr.ToString());
                totalCount = int.Parse(dt.Rows[0][0].ToString());
                return _accessMySql.getDataTableForObj<IstockChangeQuery>(sbAll.ToString() + sbJoin.ToString() + sbWhr.ToString() + sbPage.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("IstockChangeDao-->GetIstockChangeList-->" + ex.Message + sbAll.ToString() + sbJoin.ToString() + sbWhr.ToString() + sbPage.ToString(), ex);
            }
        }

        public int insertistocksome(CbjobDetailQuery cbQuery)
        {
            string iinvdidstr = string.Empty;
            int jbcount = 0;
            int result = 0;
            StringBuilder sb = new StringBuilder();
            StringBuilder str = new StringBuilder();
            StringBuilder strsqltwo = new StringBuilder();
            try
            {
                if (!string.IsNullOrEmpty(cbQuery.cb_jobid))
                {
                    str.AppendFormat("select cbjob_id from cbjob_master where cbjob_id='{0}'and status=1 and create_datetime>='{1}' and create_datetime<= '{2}'", cbQuery.cb_jobid, cbQuery.StartDate, cbQuery.EndDate);
                    jbcount = _accessMySql.getDataTable(str.ToString()).Rows.Count;
                    if (jbcount == 0)//程序到此直接跳轉出去
                    {
                        return -1;
                    }
                    sb.AppendFormat("select cbjob_id from cbjob_master where cbjob_id='{0}' and status=1 and sta_id= '{1}' and create_datetime>='{2}' and create_datetime<= '{3}'", cbQuery.cb_jobid, "COM", cbQuery.StartDate, cbQuery.EndDate);
                    if (_accessMySql.getDataTable(sb.ToString()).Rows.Count == 0)//判斷該工作編號是否能被蓋帳
                    {
                        return -1;
                    }
                    else//如果該工作編號未被蓋帳,並且處於COM狀態//and invd.st_qty <> 0
                    {
                        strsqltwo.AppendFormat(@"SELECT cd.iinvd_id,cd.cb_jobid,invd.item_id,sum(invd.prod_qty) as prod_qty,sum(invd.st_qty) as st_qty FROM cbjob_master cm LEFT JOIN cbjob_detail cd on cd.cb_jobid=cm.cbjob_id
LEFT JOIN iinvd invd on invd.row_id=cd.iinvd_id 
WHERE cm.cbjob_id='{0}'
and invd.st_qty<> invd.prod_qty 
and cm.status=1 and sta_id= 'COM' and invd.st_qty <> invd.prod_qty and invd.ista_id='A' and cm.create_datetime>='{1}' and cm.create_datetime<= '{2}' group by invd.item_id;", cbQuery.cb_jobid, cbQuery.StartDate, cbQuery.EndDate);
                        DataTable _dttwo = _accessMySql.getDataTable(strsqltwo.ToString());
                        if (_dttwo.Rows.Count > 0)
                        {
                            for (int i = 0; i < _dttwo.Rows.Count; i++)
                            {
                                StringBuilder sbstr = new StringBuilder();
                                sbstr.AppendFormat(" select sum(prod_qty) from iinvd where item_id='{0}' and ista_id='A' ", _dttwo.Rows[i]["item_id"]);
                                DataTable _tdSt = _accessMySql.getDataTable(sbstr.ToString());
                                IstockChange Icg = new IstockChange();
                                Icg.sc_trans_type = 4;
                                Icg.sc_time = DateTime.Now;
                                Icg.sc_user = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
                                Icg.sc_trans_id = cbQuery.cb_jobid;
                                Icg.sc_cd_id = string.Empty;
                                Icg.item_id = Convert.ToUInt32(_dttwo.Rows[i]["item_id"]);
                                Icg.sc_note = string.Empty;
                                Icg.sc_num_old = Convert.ToInt32(_dttwo.Rows[i]["prod_qty"]);//總庫存;
                                Icg.sc_num_new = Convert.ToInt32(_dttwo.Rows[i]["st_qty"]);
                                Icg.sc_num_chg =  Convert.ToInt32(_dttwo.Rows[i]["st_qty"])-Convert.ToInt32(_dttwo.Rows[i]["prod_qty"]);
                                Icg.sc_num_old = Convert.ToInt32(_tdSt.Rows[0][0]);
                                Icg.sc_num_new = Icg.sc_num_old+ Icg.sc_num_chg;
                                Icg.sc_istock_why = 2;//庫調
                                iinvdidstr = iinvdidstr + string.Format(@"insert into istock_change(sc_trans_id,sc_cd_id,item_id,sc_trans_type,sc_num_old,sc_num_chg,sc_num_new,sc_time,sc_user,sc_note,sc_istock_why) Values 
('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}');",
                                   Icg.sc_trans_id, Icg.sc_cd_id, Icg.item_id, Icg.sc_trans_type, Icg.sc_num_old, Icg.sc_num_chg, Icg.sc_num_new,
   CommonFunction.DateTimeToString(Icg.sc_time), Icg.sc_user, Icg.sc_note,Icg.sc_istock_why);
                            }
                        }
                        else
                        {
                            return 1;
                        }
                    }
                }
                else
                {
                    #region 其他情況
                    str.AppendFormat("select cbjob_id from cbjob_master where  sta_id= '{0}' and status=1 and  create_datetime>='{1}' and create_datetime<= '{2}'", "COM", cbQuery.StartDate, cbQuery.EndDate);
                    jbcount = _accessMySql.getDataTable(str.ToString()).Rows.Count;
                    DataTable _dtresult=_accessMySql.getDataTable(str.ToString());
                    if (jbcount == 0)//程序到此直接跳轉出去
                    {
                        return -1;//
                    }
                    else//如果存在可蓋帳的 查詢出可蓋帳的信息
                    {
                        strsqltwo.AppendFormat(@"SELECT iinvd_id,cb_jobid,item_id,sum(prod_qty) as prod_qty,sum(st_qty) as st_qty FROM(SELECT cd.iinvd_id,cd.cb_jobid,invd.item_id,invd.prod_qty as prod_qty,invd.st_qty as st_qty FROM cbjob_master cm 
LEFT JOIN cbjob_detail cd on cd.cb_jobid=cm.cbjob_id 
LEFT JOIN iinvd invd on invd.row_id=cd.iinvd_id 
WHERE invd.st_qty<> invd.prod_qty 
                        and cm.status=1 and sta_id= 'COM'and invd.st_qty <> 0 and invd.st_qty <> invd.prod_qty and invd.ista_id='A' and cm.create_datetime>='{0}' and cm.create_datetime<= '{1}' group by cd.iinvd_id) as tbone GROUP BY item_id;", cbQuery.StartDate, cbQuery.EndDate);
                               DataTable _dttwo = _accessMySql.getDataTable(strsqltwo.ToString());
                               if (_dttwo.Rows.Count > 0)
                               {
                                   for (int i = 0; i < _dttwo.Rows.Count; i++)
                                   {
                                       StringBuilder sbstr = new StringBuilder();
                                       sbstr.AppendFormat(" select sum(prod_qty) from iinvd where item_id='{0}' and ista_id='A' ", _dttwo.Rows[i]["item_id"]);
                                       DataTable _tdSt = _accessMySql.getDataTable(sbstr.ToString());
                                       IstockChange Icg = new IstockChange();
                                       Icg.sc_trans_type = 4;
                                       Icg.sc_time = DateTime.Now;
                                       Icg.sc_user = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
                                       Icg.sc_trans_id = cbQuery.cb_jobid;
                                       Icg.sc_cd_id = string.Empty;
                                       Icg.item_id = Convert.ToUInt32(_dttwo.Rows[i]["item_id"]);
                                       Icg.sc_note = string.Empty;
                                       Icg.sc_num_old = Convert.ToInt32(_dttwo.Rows[i]["prod_qty"]);//總庫存;
                                       Icg.sc_num_new = Convert.ToInt32(_dttwo.Rows[i]["st_qty"]);
                                       Icg.sc_num_chg = Convert.ToInt32(_dttwo.Rows[i]["st_qty"]) - Convert.ToInt32(_dttwo.Rows[i]["prod_qty"]);
                                       Icg.sc_num_old = Convert.ToInt32(_tdSt.Rows[0][0]);
                                       Icg.sc_num_new = Icg.sc_num_old + Icg.sc_num_chg;
                                       Icg.sc_istock_why = 2;//庫調
                                       iinvdidstr = iinvdidstr + string.Format(@"insert into istock_change(sc_trans_id,sc_cd_id,item_id,sc_trans_type,sc_num_old,sc_num_chg,sc_num_new,sc_time,sc_user,sc_note,sc_istock_why) Values 
('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}');",
                                   Icg.sc_trans_id, Icg.sc_cd_id, Icg.item_id, Icg.sc_trans_type, Icg.sc_num_old, Icg.sc_num_chg, Icg.sc_num_new,
   CommonFunction.DateTimeToString(Icg.sc_time), Icg.sc_user, Icg.sc_note, Icg.sc_istock_why);
                                   }
                               }
                               else
                               {
                                   return 1;
                               }
                          
                      
                    }
                    #endregion
                }
                MySqlCommand mySqlCmd = new MySqlCommand();
                MySqlConnection mySqlConn = new MySqlConnection(connStr);
                try
                {
                    if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Closed)
                    {
                        mySqlConn.Open();
                    }
                    mySqlCmd.Connection = mySqlConn;
                    mySqlCmd.Transaction = mySqlConn.BeginTransaction();
                    mySqlCmd.CommandType = System.Data.CommandType.Text;
                    mySqlCmd.CommandText = iinvdidstr.ToString();
                    result = mySqlCmd.ExecuteNonQuery();
                    mySqlCmd.Transaction.Commit();
                }
                catch (Exception ex)
                {
                    mySqlCmd.Transaction.Rollback();
                    throw new Exception("IstockChangeDao.insertistocksome-->" + ex.Message + str.ToString() + iinvdidstr.ToString(), ex);
                }
                finally
                {
                    if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Open)
                    {
                        mySqlConn.Close();
                    }
                }
                if (result > 0)//操作成功
                {
                    return jbcount;
                }
                else//操作失敗
                {
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("IstockChangeDao-->insertistocksome-->" + ex.Message + iinvdidstr.ToString(), ex);
            }
        }




    }
}
