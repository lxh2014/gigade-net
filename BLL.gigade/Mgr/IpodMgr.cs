using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using BLL.gigade.Model;

namespace BLL.gigade.Mgr
{
    public class IpodMgr : IIpodImplMgr
    {
        private IIpodImplDao _IpodDao;
        private string conStr = "";
        IFunctionImplMgr _functionMgr;
       public IpodMgr(string connectionString)
        {
            _IpodDao = new IpodDao(connectionString);
            _functionMgr = new FunctionMgr(connectionString);
            conStr = connectionString;
        }
       public List<IpodQuery> GetIpodList(IpodQuery query, out int totalcount)
        {
            try
            {
                return _IpodDao.GetIpodList(query, out totalcount);
            }
            catch (Exception ex)
            {

                throw new Exception("IpoMgr-->GetIpodList-->" + ex.Message, ex);
            }
        }
       public int GetPodID(IpodQuery query)
       {
           try
           {
               return _IpodDao.GetPodID(query);
           }
           catch (Exception ex)
           {

               throw new Exception("IpoMgr-->GetPodID-->" + ex.Message, ex);
           }
       }
       public int AddIpod(IpodQuery query)
       {
           try
           {
               return _IpodDao.AddIpod(query);
           }
           catch (Exception ex)
           {

               throw new Exception("IpoMgr-->AddIpod-->" + ex.Message, ex);
           }
       
       }
       public int UpdateIpod(IpodQuery query)
       {
           try
           {
               return _IpodDao.UpdateIpod(query);
           }
           catch (Exception ex)
           {

               throw new Exception("IpoMgr-->UpdateIpod-->" + ex.Message, ex);
           }
       }
       public bool UpdateIpodCheck(IpodQuery query)
       {
           Boolean result = false;
           try
           {
               ArrayList aList = new ArrayList();
               ITableHistoryImplMgr _tableHistoryMgr = new TableHistoryMgr(conStr);//實例化歷史記錄的類
               
               Int64 n_Time = BLL.gigade.Common.CommonFunction.GetPHPTime();
               Function myFun = new Function();
               myFun.FunctionCode = "/WareHouse/Check";
               List<Function> funList = _functionMgr.Query(myFun);
               int functionid = funList.Count == 0 ? 0 : funList[0].RowId;
               HistoryBatch batch = new HistoryBatch { functionid = functionid };
               batch.kuser = query.user_email;

               //獲取歷史記錄SQL
               string Check = _IpodDao.UpdateIpodCheck(query);

               //獲取修改庫存SQL  
                string Stock = string.Empty;
                ProductItem item = new ProductItem();
                item = _IpodDao.GetStockHistorySql(query, out Stock);
                
                batch.batchno = n_Time + "_" + query.change_user + "_" + item.Product_Id;
                if (item != null)
                {
                    item.Item_Stock = query.item_stock;
                    aList.Add(Stock);
                    aList.Add(Check);
                    result = _tableHistoryMgr.SaveHistory<ProductItem>(item, batch, aList);
                }

                //獲取修改商品Ignore SQL  
                string Ignore_Stock = string.Empty;
                Product product = new Product();
                product = _IpodDao.GetIgnoreHistorySql(query, out Ignore_Stock);
                if (product != null)
                {
                    product.Ignore_Stock = 0;
                    aList.Clear();
                    aList.Add(Ignore_Stock);
                    result = _tableHistoryMgr.SaveHistory<Product>(product, batch, aList);
                }

               
               return result;
           }
           catch (Exception ex)
           {

               throw new Exception("IpoMgr-->UpdateIpodCheck-->" + ex.Message, ex);
           }
       }
       public int DeletIpod(IpodQuery query)
       {
           try
           {
               return _IpodDao.DeletIpod(query);
           }
           catch (Exception ex)
           {

               throw new Exception("IpoMgr-->DeletIpod-->" + ex.Message, ex);
           }
       }
       public List<IpodQuery> GetIpodListExprot(IpodQuery query)
       {
           try
           {
               return _IpodDao.GetIpodListExprot(query);
           }
           catch (Exception ex)
           {

               throw new Exception("IpoMgr-->GetIpodListExprot-->" + ex.Message, ex);
           }
       }
       public bool GetIpodfreight(string po_id, int freight)
       {
           try
           {
               return _IpodDao.GetIpodfreight(po_id, freight);
           }
           catch (Exception ex)
           {

               throw new Exception("IpoMgr-->GetIpodfreight-->" + ex.Message, ex);
           }
       }
    }
}
