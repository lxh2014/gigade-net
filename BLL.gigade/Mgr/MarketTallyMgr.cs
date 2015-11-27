using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr
{
    public class MarketTallyMgr
    {
        private IinvdImplDao _ivddao;
        private IAseldImplDao _aseldDao;

        public MarketTallyMgr(string connectionString)
        {
            _ivddao = new IinvdDao(connectionString);
            _aseldDao = new AseldDao(connectionString);
        }
        public bool RFAutoMarketTally(int seld_id)
        {
            int totalCount = 0;

            //根據seld_id獲取aseld記錄信息；
            AseldQuery query_ase = new AseldQuery();
            query_ase.seld_id=seld_id;
            List<AseldQuery> ase = _aseldDao.GetAllAseldList(query_ase, out totalCount);
            if (ase.Count == 1)
            {

            }
            else
            {
                return false;
            }


            //根據item_id獲取庫存信息；
            IinvdQuery query_ivd = new IinvdQuery()
            {
                ista_id = "A"
            };
            query_ivd.item_id = ase[0].item_id;
            
            List<IinvdQuery> store_ivd = _ivddao.GetIinvdListByItemid(query_ivd, out totalCount);

            if (ase[0].sel_loc == "YY999999")
            {
                //沒有主料位,記庫存帳卡

            }
            else
            {
                //統計庫存是否夠揀,不夠時嘗試庫調
                GetStockSum(store_ivd, ase[0].out_qty);
            }

            return true;
        }
        /// <summary>
        /// 統計庫存是否夠揀;如果不夠嘗試庫調；
        /// </summary>
        /// <returns></returns>
        private int GetStockSum(List<IinvdQuery> store_ivd, int out_qty)
        {
            int sum = 0;
            foreach (IinvdQuery item in store_ivd)
            {
                sum += item.prod_qty;
            }

            return 0;
        }
    }
}
