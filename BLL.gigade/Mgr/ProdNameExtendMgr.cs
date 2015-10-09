using BLL.gigade.Dao;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model.Custom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model;
using System.Collections;
using System.Text.RegularExpressions;

namespace BLL.gigade.Mgr
{
    public class ProdNameExtendMgr : IProdNameExtendImplMgr
    {
        private IProdNameExtendImplDao peDao;

        private string connectionStr;
        public ProdNameExtendMgr(string connectionStr)
        {
            peDao = new ProdNameExtendDao(connectionStr);
            this.connectionStr = connectionStr;
        }

        /// <summary>
        /// 查詢語句
        /// </summary>
        /// <param name="productId">商品id</param>
        /// <returns>符合條件的集合</returns>
        public List<ProdNameExtendCustom> Query(ProdNameExtendCustom pec, string ids)
        {
            List<ProdNameExtendCustom> list = peDao.Query(pec, ids);
            List<ProdNameExtendCustom> listTemp = new List<ProdNameExtendCustom>();
            /// 如果商品前後綴時間過期,則重新添加一個可編輯的行賦予該商品,但是如果商品已在作用中(2)或者出於審核狀態(1)則不進行添加
            try
            {
                //獲取 前一天 時間的時間戳  edit by zhuoqin0830w  2015/05/08
                long time = BLL.gigade.Common.CommonFunction.GetPHPTime(Convert.ToString(DateTime.Now)) - 86400;///將當前時間-24小時(既時間戳中86400)
                //var prodExts = list.FindAll(p => p.Flag == 1);
                //foreach (var item in prodExts) edit by wwei0216w 2015/6/30 將只判斷審核狀態改為所有狀態都判斷,防止其它操作造成的flag狀態錯誤
                foreach (var item in list)
                {
                    if (item.Product_Prefix == "" && item.Product_Suffix == "")///前后綴都為空,此種情況可編輯
                    {
                        item.Flag = 0;
                    }
                    else if ((item.Event_End > time) && ((item.Product_Name.Contains(item.Product_Prefix) && item.Product_Prefix.Length > 0) || (item.Product_Name.Contains(item.Product_Suffix) && item.Product_Suffix.Length > 0))) //擁有前後綴的情況
                    {
                        item.Flag = 2;  ///Flag==2 為商品作用中
                    }
                    else if (item.Type == 3)///Type為3表示申請被駁回///駁回后商品應該顯示可編輯,所以將flag設置為0
                    {
                         item.Flag = 0;///Flag ==0 表示商品可編輯
                    }
                    else if (item.Event_End <= time && item.Event_End != 0) ///add by wwei0215/8/25 添加item.Event_End!=0
                    {
                        item.Flag = 3;///Flag ==3 表示過期
                    }
                    else if (item.Flag == 2)
                    {
                        item.Flag = 3;
                    }
                    else
                    {
                        item.Flag = item.Flag;
                    }
                }

                Update(list.Select(p => (ProdNameExtend)p).ToList());
                
                //排除重複的可編輯行
                var a = (from obj in list
                         where obj.Flag == 3
                         select new
                         {
                             obj.Site_Id,
                             obj.User_Level,
                             obj.User_Id,
                             obj.Product_Id,
                             obj.Site_Name,
                             obj.Level_Name,
                             obj.Price_Master_Id,
                             obj.Product_Name,
                             obj.Flag
                         }).Distinct();

                foreach (var p in a)
                {
                    ProdNameExtendCustom p_temp = new ProdNameExtendCustom();
                    p_temp.Product_Id = p.Product_Id;
                    p_temp.Site_Name = p.Site_Name;
                    p_temp.Site_Id = p.Site_Id;
                    p_temp.User_Level = p.User_Level;
                    p_temp.User_Id = p.User_Id;
                    p_temp.Level_Name = p.Level_Name;
                    //刪除 Apply_id 保證 商品名稱過期后只有一行數據
                    //p_temp.Apply_id = p.Apply_id;
                    p_temp.Price_Master_Id = p.Price_Master_Id;
                    p_temp.Product_Name = p.Product_Name;
                    p_temp.Event_End = 0;
                    var count = list.Find(m => (m.Flag == 2 || m.Flag == 0 || m.Flag == 1) && m.Price_Master_Id == p.Price_Master_Id);//如果存在作用中的商品就不向前臺傳遞編輯列
                    if (count == null)
                    {
                        listTemp.Add(p_temp); //添加可編輯行
                    }
                }
                list.InsertRange(0, listTemp);

                list.ForEach(p =>
                {
                    p.Product_Name = ClearPreSuName(ClearPreSuName(p.Product_Name, PriceMaster.L_HKH.ToString(), PriceMaster.R_HKH.ToString()), PriceMaster.L_HKH.ToString(), PriceMaster.R_HKH.ToString());///在前臺顯示時,不需要添加了商品前後綴的名字,所以在這裡去掉前後綴
                });
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("ProdNameExtendMgr-->Query-->" + ex.Message, ex);
            }
        }
        public List<ProdNameExtend> QueryByFlag(int flag)
        {
            return peDao.QueryByFlag(flag);
        }

        /// <summary>
        /// 將傳遞的字符串去掉lChar到rCahr之間的字符(包括lChar與rCahr)
        /// </summary>
        /// <param name="productName">需要去掉前後綴的字符串</param>
        /// <returns>刪除前後綴的productName</returns>
        //add by wwei0216w 2014/12/29
        private string ClearPreSuName(string productName, string lChar, string rChar)
        {
            if (productName == "")
                return "";
            int first = productName.IndexOf(lChar) == -1 ? 0 : productName.IndexOf(lChar);//第一個左括號
            int last = productName.IndexOf(rChar) == -1 ? 0 : productName.IndexOf(rChar);//第一個右括號
            int length = (last - first) - 1; //前綴的長度

            if (length != 0 && length != -1) //如果有前綴,執行去掉前綴的操作
            {
                productName = productName.Remove(first, length + lChar.Length + rChar.Length);//除去
            }
            return productName;
        }

        public bool Update(List<ProdNameExtend> prodExts)
        {
            return peDao.Update(prodExts) > -1;
        }

        /// <summary>
        /// 更新,保存ProdNameExtend表
        /// </summary>
        /// <param name="pn">需要保存的數據</param>
        /// <returns>保存結果</returns>
        public bool SaveByList(List<ProdNameExtendCustom> listpn, Caller caller)
        {
            try
            {
                List<ProdNameExtendCustom> listUpdate = listpn.FindAll(m => m.Flag == 2 || m.Flag == 1);//從中過濾出 Flag==2 (作用中)的 Flag==1 (審核中)商品
                listUpdate.ForEach(m => listpn.Remove(m));                    //移除掉狀態等於2,4的商品
                if (UpdateTime(listUpdate))//作用中的商品,審核中的商品只有時間是可以修改的~所以只用更新時間//edit by wwei0216w
                {
                    var result = peDao.SaveByList(listpn);
                    AddProductExtentName(caller);
                    return result;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("ProdNameExtendMgr-->SaveByList-->" + ex.Message, ex);
            }
        }

        /// <summary>
        /// 根據條件更新後綴結束時間
        /// </summary>
        /// <param name="pn">ProdNameExtendCustom對象</param>
        /// <returns>更新時間的sql語句</returns>
        public bool UpdateTime(List<ProdNameExtendCustom> prodNameExtends)
        {
            try
            {
                if (prodNameExtends.Count > 0)
                {
                    return peDao.UpdateTime(prodNameExtends);
                }
                else
                {
                    return true;
                }

            }
            catch (Exception ex)
            {
                throw new Exception("ProdNameExtendMgr-->UpdateTime-->" + ex.Message, ex);
            }
        }

        /// <summary>
        /// 調用儲存過程為price_master 裱中的name添加前後綴
        /// </summary>
        /// <param name="days">過期天數(超多少天的前後綴自動刪除)</param>
        /// <returns>受影響的行數</returns>
        public int ResetExtendName(Caller callId, uint product_id = 0)
        {
            try
            {
                IPriceMasterImplMgr pmMgr = new PriceMasterMgr(connectionStr);
                MySqlDao _mySqlDao = new MySqlDao(connectionStr);
                ArrayList sqlList = new ArrayList();
                string msg = string.Empty;
                List<ProdNameExtendCustom> list = new List<ProdNameExtendCustom>();
                if (product_id != 0)
                {
                    list = peDao.GetPastProductById(product_id);
                }
                else
                {
                    list = peDao.GetPastProduct();
                }
                foreach (var pM in list)
                {
                    //pM.Product_Name = pM.Product_Name.Replace(PriceMaster.L_HKH + pM.Product_Prefix + PriceMaster.R_HKH, "")//替換掉priceMaster中product_name的前綴
                    //.Replace(PriceMaster.L_HKH + pM.Product_Suffix + PriceMaster.R_HKH, "");//替換掉priceMaster中product_name的後綴
                    bool tempFlag = true;
                    int index = 0;///定義index防止錯誤導致下面的while無限循環
                    while(tempFlag)///循環刪除前後綴
                    {
                        index ++;
                        pM.Product_Name = ClearPreSuName(pM.Product_Name, PriceMaster.L_HKH.ToString(), PriceMaster.R_HKH.ToString());
                        if (pM.Product_Name.IndexOf(PriceMaster.L_HKH.ToString()) == -1 && pM.Product_Name.IndexOf(PriceMaster.R_HKH.ToString()) == -1||index>10)
                        {
                            tempFlag = false;
                        }
                    }
                    PriceMaster p = new PriceMaster();
                    p.price_master_id = pM.Price_Master_Id;
                    p.product_name = pM.Product_Name;
                    sqlList.Add(pmMgr.UpdateName(p)); //將去掉前後綴的名稱update到price_master表
                    pM.Flag = 3;//過期
                    pM.Event_End = pM.Event_End;
                    pM.Event_Start = pM.Event_Start;
                }

                if (_mySqlDao.ExcuteSqls(sqlList))
                {
                    peDao.Update(list.Select(p => (ProdNameExtend)p).ToList());
                }
                if (msg == "")
                {
                    int days = 30;//過期多少天刪除
                    return peDao.DeleteExtendName(days);
                }
                else
                {
                    return -1;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("ProdNameExtendMgr-->ResetExtendName-->" + ex.Message, ex);
            }
        }

        public bool AddProductExtentName(Caller caller)
        {
            try
            {
                string msg = string.Empty;
                var productExtent = peDao.QueryStart();
                IPriceMasterImplMgr _priceMasterMgr = new PriceMasterMgr(connectionStr);
                List<PriceMaster> priceMasters = _priceMasterMgr.Query(productExtent.Select(p => p.Price_Master_Id).ToArray());

                foreach (PriceMaster pm in priceMasters)
                {
                    var item = productExtent.Find(m => m.Price_Master_Id == pm.price_master_id);
                    pm.product_name = (item.Product_Prefix == "" ? "" : PriceMaster.L_HKH + item.Product_Prefix + PriceMaster.R_HKH)
                        + pm.product_name
                        + (item.Product_Suffix == "" ? "" : PriceMaster.L_HKH + item.Product_Suffix + PriceMaster.R_HKH);
                }
                if (_priceMasterMgr.UpdatePriceMasters(priceMasters, caller, out msg))
                {
                    productExtent.ForEach(p =>
                    {
                        p.Apply_id = priceMasters.Find(m => m.price_master_id == p.Price_Master_Id).apply_id;
                        p.Flag = 1;//待審核
                    });
                    return Update(productExtent);
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception("ProdNameExtendMgr.AddProductExtentName-->" + ex.Message, ex);
            }
        }

    }
}
