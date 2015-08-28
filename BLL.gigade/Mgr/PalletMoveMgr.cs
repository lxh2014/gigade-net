using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using System.Data;
namespace BLL.gigade.Mgr
{
    public class PalletMoveMgr:IPalletMoveImplMgr
    {
        private IPalletMoveImplDao _palletMove;
        public PalletMoveMgr(string connectionString)
        {
            _palletMove = new PalletMoveDao(connectionString);
        }


        #region IIPalletMoveImplMgr 成员

        public List<IinvdQuery> GetPalletList(IinvdQuery Iinvd, string invposcat = null)
        {
            try
            {
                return _palletMove.GetPalletList(Iinvd, invposcat);
            }
            catch (Exception ex)
            {

                throw new Exception("PalletMoveMgr-->GetPalletList" + ex.Message, ex);
            }
        }


        #endregion
        #region 补货至目标料位
        public string UpPallet(IinvdQuery Iinvd, string newinvd, string num, int userId)
        {
         try
            {
                return _palletMove.UpPallet(Iinvd, newinvd, num,userId);
            }
            catch (Exception ex)
            {

                throw new Exception("PalletMoveMgr-->UpPallet" + ex.Message, ex);
            }
        }
        #endregion

        #region 补货至主料位之及时更新时间
        /// <summary>
        /// 啊啊啊啊
        /// </summary>
        /// <param name="Iinvd"></param>
        /// <returns></returns>
        public int UpPalletTime(IinvdQuery Iinvd)
        { 
         try
            {
                return _palletMove.UpPalletTime(Iinvd);
            }
            catch (Exception ex)
            {

                throw new Exception("PalletMoveMgr-->UpPalletTime" + ex.Message, ex);
            }

        }

        #endregion
        /// <summary>
        /// 根據商品編號或條碼編號查詢商品名稱
        /// </summary>
        /// <param name="pid">商品編號或條碼編號</param>
        /// <returns></returns>
        public DataTable GetProdInfo(string pid)
        {
            try
            {
                return _palletMove.GetProdInfo(pid);
            }
            catch (Exception ex)
            {

                throw new Exception("PalletMoveMgr-->GetProdInfo" + ex.Message, ex);
            }
        }


        public int updatemadedate(IinvdQuery Iinvd)
        {
            try
            {
                return _palletMove.updatemadedate(Iinvd);
            }
            catch (Exception ex)
            {

                throw new Exception("PalletMoveMgr-->updatemadedate" + ex.Message, ex);
            }
        }


        public List<ProductExt> selectproductexttime(string item_id)
        {
            try
            {
                return _palletMove.selectproductexttime(item_id);
            }
            catch (Exception ex)
            {

                throw new Exception("PalletMoveMgr-->selectproductexttime" + ex.Message, ex);
            }
        }


        public int selectcount(IinvdQuery invd)
        {
            try
            {
                return _palletMove.selectcount(invd);
            }
            catch (Exception ex)
            {

                throw new Exception("PalletMoveMgr-->selectcount" + ex.Message, ex);
            }
        }


        public DataTable selectrow_id(IinvdQuery invd)
        {
            try
            {
                return _palletMove.selectrow_id(invd);
            }
            catch (Exception ex)
            {

                throw new Exception("PalletMoveMgr-->selectrow_id" + ex.Message, ex);
            }
        }


        public int UpdateordeleteIinvd(IinvdQuery invd, IinvdQuery newinvd)
        {
            try
            {
                return _palletMove.UpdateordeleteIinvd(invd, newinvd);
            }
            catch (Exception ex)
            {

                throw new Exception("PalletMoveMgr-->UpdateordeleteIinvd" + ex.Message, ex);
            }
        }


        public DataTable GetProductMsgByLocId(string loc_id)
        {
            try
            {
                return _palletMove.GetProductMsgByLocId(loc_id);
            }
            catch (Exception ex)
            {

                throw new Exception("PalletMoveMgr-->GetProductMsgByLocId" + ex.Message, ex);
            }
        }
    }
}
