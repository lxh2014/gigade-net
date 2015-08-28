using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Custom;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;
using DBAccess;
using System.IO;
using System.Xml.Linq;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using BLL.gigade.Model;
using System.Collections;
using BLL.gigade.Common;
using gigadeExcel.Comment;


namespace BLL.gigade.Mgr
{
    public class ProductExtMgr:IProductExtImplMgr
    {
        private IProductExtImplDao pei;
        private string conStr = "";
        public ProductExtMgr(string connectionStr)
        {
            pei = new ProductExtDao(connectionStr);
            conStr = connectionStr;
        }
        /// <summary>
        /// 查詢product_ext
        /// </summary>
        /// <param name="pe">查詢條件</param>
        /// <returns>符合條件的集合</returns>
        public List<ProductExtCustom> Query(ProductExtCustom.Condition condition, params int[] ids)
        {
            try
            {
                if (ids.Length == 0) //add by wwei0216w 如果ids為空,則返回空
                    return null;
                return pei.Query(ids, condition);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductExtMgr-->Query-->" + ex.Message, ex);
            }
        }

        /// <summary>
        /// 操作商品細項
        /// </summary>
        /// <param name="pe">需要操作的商品條件</param>
        /// <returns>操作是否成功</returns>
        public bool UpdateProductExt(List<ProductExtCustom> lists , Caller _caller,string controlId)
        {
            bool flag = true;
            IFunctionImplMgr _functionMgr = new FunctionMgr(conStr); //用於找到控件的function信息
            ITableHistoryImplMgr _tableHistoryMgr; //用於歷史記錄保存的類
            IProductExtImplMgr _productExtMgr = new ProductExtMgr(conStr);
            try
            {
                Function fun = _functionMgr.QueryFunction(controlId, "/ProductParticulars/Index");
                ArrayList aList = new ArrayList();
                _tableHistoryMgr = new TableHistoryMgr(conStr);//實例化歷史記錄的類
                HistoryBatch batch = new HistoryBatch { functionid = fun.RowId };
                Int64 n_Time = CommonFunction.GetPHPTime();
                batch.kuser = _caller.user_email;
                foreach (ProductExtCustom item in lists)
                {
                    aList.Add(pei.UpdateProductExt(item));
                    batch.batchno = n_Time + "_" + "2" + "_" + item.Product_id;//add by wwei0216w 2015/7/6 註:將item_id改為product_id 統一使用product_id查詢
                    flag = _tableHistoryMgr.SaveHistory<ProductExtCustom>(item, batch, aList);
                    if(flag == false)
                    {
                        throw new Exception("ProductExtMgr-->UpdateProductExt Exception!" );
                    }
                }

                return flag;
            }
            catch (Exception ex)
            {
                throw new Exception("ProductExtMgr-->UpdateProductExt-->" + ex.Message, ex);
            }
        }

        /// <summary>
        /// 刪除結果
        /// </summary>
        /// <param name="pe">刪除條件</param>
        /// <returns>受影響的行數</returns>
        public int DeleteProductExtByCondition(ProductExtCustom pe)
        {
            try
            {
                return pei.DeleteProductExtByCondition(pe);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductExtMgr-->DeleteProductExtByCondition-->" + ex.Message, ex);
            }
        }

        public bool UpdatePendDel(uint proudctId, bool penDel)
        {
            try
            {
                return pei.UpdatePendDel(proudctId, penDel) > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("ProductExtMgr-->UpdatePendDel-->" + ex.Message, ex);
            }
        }

        public List<ProductExtCustom> QueryHistoryInfo(Int64 Update_start, Int64 Update_end, int Brand_id = 0, string Item_ids = null, string Product_ids = null)
        {
            try
            {
                DateTime updateStart = Common.CommonFunction.GetNetTime(Update_start);
                DateTime updateEnd = Update_end == 0 ? DateTime.MaxValue : Common.CommonFunction.GetNetTime(Update_end);
                List<ProductExtCustom> peList = pei.QueryHistoryInfo(updateStart, updateEnd, Brand_id, Item_ids, Product_ids);
                return peList;
            }
            catch (Exception ex)
            {
                throw new Exception("ProductExtMgr-->QueryHistoryInfo" + ex.Message,ex);
            }
        }

        public MemoryStream OutToExcel(string fileName, Int64 Update_start, Int64 Update_end, int Brand_id = 0, string Item_ids = null, string Product_ids = null)
        {
            XDocument xml = XDocument.Load(fileName);//加载xml 
            try
            {
                Dictionary<string, string> columns = xml.Elements().Elements().ToDictionary(p => p.Attribute("key").Value, p => p.Value);//將xml轉換成Dictionary
                List<ProductExtCustom> pExtList = QueryHistoryInfo(Update_start, Update_end, Brand_id, Item_ids, Product_ids);
                return ExcelHelperXhf.ExportExcel<ProductExtCustom>(pExtList, columns, null, "商品細項記錄");  
            }
            catch (Exception ex)
            {
                throw new Exception("ProductExtMgr-->OutToExcel" + ex.Message,ex);
            }
        }
    }
}
