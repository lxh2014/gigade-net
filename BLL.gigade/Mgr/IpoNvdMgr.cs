using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace BLL.gigade.Mgr
{
    public class IpoNvdMgr
    {
        private IpoNvdDao _IpoNvdDao;
        private MySqlDao myDao;
        private IinvdImplDao _iinvdDao;
        private IstockChangeDao istockchangeDao;
        public IpoNvdMgr(string connectionString)
        {
            _IpoNvdDao = new IpoNvdDao(connectionString);
            myDao = new MySqlDao(connectionString);
            _iinvdDao = new IinvdDao(connectionString);
            istockchangeDao = new IstockChangeDao(connectionString); 
        }
        public List<IpoNvdQuery> GetIpoNvdList(IpoNvdQuery query, out int totalcount)
        {
            try
            {
                return _IpoNvdDao.GetIpoNvdList(query, out totalcount);
            }
            catch (Exception ex)
            {
                throw new Exception("IpoNvdMgr-->GetIpoNvdList-->" + ex.Message, ex);
            }
        }
        public IpoNvdQuery GetIpoNvd(IpoNvdQuery query)
        {
            try
            {
                return _IpoNvdDao.GetIpoNvd(query);
            }
            catch (Exception ex)
            {
                throw new Exception("IpoNvdMgr-->IpoNvdQuery-->" + ex.Message, ex);
            }
        }
        public bool SaveReceiptShelves(IpoNvdQuery invd_query,int pick_num)
        {
            try
            {
                //更新IpoNvd 表
                IpoNvdQuery query = _IpoNvdDao.GetIpoNvd(invd_query);

                query.modify_user = invd_query.modify_user;
                query.made_date = invd_query.made_date;
                query.cde_dt = invd_query.cde_dt;
                query.out_qty = query.out_qty - pick_num;
                query.com_qty = query.com_qty + pick_num;

                if (query.out_qty > 0)
                {
                    query.work_status = "SKP";
                }
                else if (query.out_qty == 0)
                {
                    query.work_status = "COM";
                }
                string UpdateIpoNvdSql = _IpoNvdDao.UpdateIpoNvdSql(query);

                //更新IpoNvdLog表
                IpoNvdLogQuery invdLog = new IpoNvdLogQuery();
                invdLog.work_id = query.work_id;
                invdLog.ipo_id = query.ipo_id;
                invdLog.item_id = (uint)query.item_id;
                invdLog.add_qty = pick_num;
                invdLog.cde_date = query.cde_dt;
                invdLog.made_date = query.made_date;
                invdLog.create_user = query.modify_user;

                string InsertIpoNvdLogSql = _IpoNvdDao.InsertIpoNvdLogSql(invdLog);
                //更新iinvd表
                IinvdQuery iinvd_query = new IinvdQuery();
                iinvd_query.made_date = query.made_date;
                iinvd_query.cde_dt = query.cde_dt;
                iinvd_query.prod_qty = pick_num;
                iinvd_query.ista_id = "A";
                iinvd_query.create_user = invd_query.modify_user;
                iinvd_query.change_user = invd_query.modify_user;
                iinvd_query.plas_loc_id = invd_query.loc_id;
                iinvd_query.item_id = query.item_id;
                string ista_id = string.Empty;
                string UpdateIinvdSql = _iinvdDao.UpdateIinvdSql(iinvd_query, out ista_id);
                
                //更新istockchange表
                IstockChangeQuery stock_query = new IstockChangeQuery();
                stock_query.sc_trans_id = ""; 
                stock_query.sc_cd_id = "";
                stock_query.item_id = query.item_id; 
                stock_query.sc_trans_type = 1; 
                stock_query.sc_num_chg = pick_num;
                stock_query.sc_time = DateTime.Now;
                stock_query.sc_user = invd_query.modify_user;
                stock_query.sc_note = "收貨上架";
                stock_query.sc_istock_why = 4;
                string insertIstockChangeSql = string.Empty;
                if (ista_id != "H")
                {
                    insertIstockChangeSql = istockchangeDao.insertIstockChangeSql(stock_query);
                }

                //執行SQL
                ArrayList arrList = new ArrayList();
                arrList.Add(UpdateIpoNvdSql);
                arrList.Add(InsertIpoNvdLogSql);
                arrList.Add(UpdateIinvdSql);
                if (ista_id != "H")
                {
                    arrList.Add(insertIstockChangeSql);
                }

                bool result = myDao.ExcuteSqls(arrList);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("IpoNvdMgr-->SaveReceiptShelves-->" + ex.Message, ex);
            }
        }
        public int CreateTallyList(IpoNvdQuery query, string id)
        {
            try
            {
                return _IpoNvdDao.CreateTallyList(query,id);
            }
            catch (Exception ex)
            {
                throw new Exception("IpoNvdMgr-->CreateTallyList-->" + ex.Message, ex);
            }
        }
    }
}