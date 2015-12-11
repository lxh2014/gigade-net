using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using System.Data;

namespace BLL.gigade.Mgr
{
    public class AseldMgr : IAseldImplMgr 
    {
        private IAseldImplDao _aseldDao;
        private IinvdImplDao _iinvdDao;
        public AseldMgr(string connectionStr)
        { 
            _aseldDao = new AseldDao(connectionStr);
            _iinvdDao = new IinvdDao(connectionStr);
        }
        public List<Model.Query.OrderMasterQuery> GetOrderMasterList(Model.Query.OrderMasterQuery oderMaster, out int totalCount)
        {
            try
            {
                return _aseldDao.GetOrderMasterList(oderMaster, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("AseldMgr-->GetOrderMasterList-->" + ex.Message, ex);
            }
        }

        public List<Model.Query.OrderMasterQuery> GetAllOrderDetail(Model.Query.OrderMasterQuery oderMaster)
        {
            try
            {
                return _aseldDao.GetAllOrderDetail(oderMaster);
            }
            catch (Exception ex)
            {
                throw new Exception("AseldMgr-->GetAllOrderDetail-->" + ex.Message, ex);
            }
        }
        public string Insert(Aseld m)
        {
            try
            {
                return _aseldDao.Insert(m);
            }
            catch (Exception ex)
            {
                throw new Exception("AseldMgr-->Insert-->" + ex.Message, ex);
            }
        }
        public DataTable SelOrderDetail(string id, string fre, int radioselect)
        {
            try
            {
                return _aseldDao.SelOrderDetail(id, fre, radioselect);
            }
            catch (Exception ex)
            {
                throw new Exception("AseldMgr-->SelOrderDetail-->" + ex.Message, ex);
            }
        }
        public int InsertSql(string sql)
        {
            try
            {
                return _aseldDao.InsertSql(sql);
            }
            catch (Exception ex)
            {
                throw new Exception("AseldMgr-->InsertSql-->" + ex.Message, ex);
            }
        }
        public DataTable GetOrderProductInformation(AseldQuery ase)
        {
            try
            {
                return _aseldDao.GetOrderProductInformation(ase);
            }
            catch (Exception ex)
            {
                throw new Exception("AseidMgr-->GetOrderProductInformation-->" + ex.Message, ex);
            }
        }
        #region 
        public List<AseldQuery> GetAseldList(Aseld ase)
        {
            try
            {
                return _aseldDao.GetAseldList(ase);
            }
            catch (Exception ex)
            {
                throw new Exception("AseldMgr-->GetAseldList-->" + ex.Message, ex);
            }
        }
        #endregion
        #region
        public List<AseldQuery> GetAseldListByItemid(Aseld ase)
        {
            try
            {
                return _aseldDao.GetAseldListByItemid(ase);
            }
            catch (Exception ex)
            {
                throw new Exception("AseldMgr-->GetAseldListByItemid-->" + ex.Message, ex);
            }
        }
        #endregion
        #region
        /// <summary>
        /// 自動理貨 獲取所有需要理貨的商品
        /// </summary>
        /// <param name="ase"></param>
        /// <returns></returns>

        public List<AseldQuery> GetAllAseldList(AseldQuery ase,out int totalCount)
        {
            try
            {
                return _aseldDao.GetAllAseldList(ase,out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("AseldMgr-->GetAllAseldList-->" + ex.Message, ex);
            }
        }
        #endregion
        #region
        /// <summary>
        /// 判斷itemid是否在某個工作項中
        /// </summary>
        /// <param name="ase"></param>
        /// <returns></returns>

        public int GetCountByItem(Aseld a)
        {
            try
            {
                return _aseldDao.GetCountByItem(a);
            }
            catch (Exception ex)
            {
                throw new Exception("AseldMgr-->GetCountByItem-->" + ex.Message, ex);
            }
        }
        #endregion
        public string UpdTicker(string m)
        {
            try
            {
                return _aseldDao.UpdTicker(m);
            }
            catch (Exception ex)
            {
                throw new Exception("AseldMgr-->UpdTicker-->" + ex.Message, ex);
            }
        }

        public string UpdAseld(Aseld a)
        {
            try
            {
                return _aseldDao.UpdAseld(a);
            }
            catch (Exception ex)
            {
                throw new Exception("AseldMgr-->UpdAseld-->" + ex.Message, ex);
            }
        }
        public int SelCom(Aseld a)
        {
            try
            {
                return _aseldDao.SelCom(a);
            }
            catch (Exception ex)
            {
                throw new Exception("AseldMgr-->SelCom-->" + ex.Message, ex);
            }
        }
        public int SelComA(Aseld a)
        {
            try
            {
                return _aseldDao.SelComA(a);
            }
            catch (Exception ex)
            {
                throw new Exception("AseldMgr-->SelComA-->" + ex.Message, ex);
            }
        }

        public int SelComC(Aseld m)
        {
            try
            {
                return _aseldDao.SelComC(m);
            }
            catch (Exception ex)
            {
                throw new Exception("AseldMgr-->SelComC-->" + ex.Message, ex);
            }
        }
        
        public int SelectCount(Aseld m)
        {
            try
            {
                return _aseldDao.SelectCount(m);
            }
            catch (Exception ex)
            {
                throw new Exception("AseldMgr-->SelectCount-->" + ex.Message, ex);
            }
        }
        public string updgry(Aseld a, Dictionary<string, string> str)
        {
            try
            {
                return _iinvdDao.updgry(a,str);
            }
            catch (Exception ex)
            {
                throw new Exception("AseldMgr-->updgry-->" + ex.Message, ex);
            }
        }

        public string AddIwsRecord(AseldQuery a)
        {
            try
            {
                return _aseldDao.AddIwsRecord(a);
            }
            catch (Exception ex)
            {
                throw new Exception("AseldMgr-->UpdateAseld-->" + ex.Message, ex);
            }
        }

        public int DecisionBulkPicking(AseldQuery ase, int commodity_type) 
        {
            try
            {
                return _aseldDao.DecisionBulkPicking(ase, commodity_type);
            }
            catch (Exception ex)
            {

                throw new Exception("AseldMgr-->DecisionBulkPicking-->" + ex.Message, ex);
            }
        }
        public int UpdScaned(Aseld m)
        {
            try
            {
                return _aseldDao.UpdScaned(m);
            }
            catch (Exception ex)
            {
                throw new Exception("AseldMgr-->UpdScaned-->" + ex.Message, ex);
            }
        }
        public DataTable GetDetailOrSimple(string type, string jobNumbers, AseldQuery query = null)
        {
            try
            {
                if (type == "0")
                {
                    return _aseldDao.GetNComJobSimple(query);
                }
                else
                {
                    return _aseldDao.GetNComJobDetail(jobNumbers,query);
                } 
            }
            catch (Exception ex)
            {
                throw new Exception("AseldMgr-->GetDetailOrSimple-->" + ex.Message, ex);
        }

        }



        public DataTable ExportDeliveryStatement(int counts, int types)
        {
            try
            {
                return _aseldDao.ExportDeliveryStatement(counts,types);
            }
            catch (Exception ex)
            {
                throw new Exception("AseldMgr-->ExportDeliveryStatement-->" + ex.Message, ex);
            }
        }


        public int UpdateScnd(Aseld ase)
        {
            try
            {
                return _aseldDao.UpdateScnd(ase);
            }
            catch (Exception ex)
            {
                throw new Exception("AseldMgr-->UpdateScnd-->" + ex.Message, ex);
            }
        }

        public int Updwust(Aseld a)
        {
            try
            {
                return _aseldDao.Updwust(a);
            }
            catch (Exception ex)
            {
                throw new Exception("AseldMgr-->Updwust-->" + ex.Message, ex);
            }
        }


        public DataTable ExportAseldMessage(AseldQuery ase)
        {
            try
            {
                return _aseldDao.ExportAseldMessage(ase);
            }
            catch (Exception ex)
            {
                throw new Exception("AseldMgr-->ExportAseldMessage-->" + ex.Message, ex);
            }
        }
        public DataTable getTime(AseldQuery a)
        {
            try
            {
                return _iinvdDao.getTime(a);
            }
            catch (Exception ex)
            {
                throw new Exception("AseldMgr-->getTime-->" + ex.Message, ex);
            }
        }

        public void ConsoleAseldBeforeInsert(int detail_id)
        {
            try
            {
                _aseldDao.ConsoleAseldBeforeInsert(detail_id);
            }
            catch (Exception ex)
            {
                throw new Exception("AseldMgr-->ConsoleAseldBeforeInsert-->" + ex.Message, ex);
            }
        }
        public string Getfreight(string ord_id)
        {
            try
            {
               return  _aseldDao.Getfreight(ord_id);
            }
            catch (Exception ex)
            {
                throw new Exception("AseldMgr-->Getfreight-->" + ex.Message, ex);
            }
        }
        public DataTable GetAseldTable(AseldQuery ase, out int total)
        {
            try
            {
                return _aseldDao.GetAseldTable(ase, out total);
            }
            catch (Exception ex)
            {
                throw new Exception("AseldMgr-->GetAseldTable-->" + ex.Message, ex);
            }
        } 
        public DataTable GetAseldTablePDF(AseldQuery aseld)
        {
            try
            {
                return _aseldDao.GetAseldTablePDF(aseld);
            }
            catch (Exception ex)
            {
                throw new Exception("AseldMgr-->GetAseldTablePDF-->" + ex.Message, ex);
            }
        }
    } 
}
