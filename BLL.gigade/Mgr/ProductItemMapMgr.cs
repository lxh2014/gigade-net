using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using BLL.gigade.Model.Custom;
using BLL.gigade.Dao;
using System.Collections;
using BLL.gigade.Mgr.Impl;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using System.Data;
namespace BLL.gigade.Mgr
{
    public class ProductItemMapMgr : Impl.IProductItemMapImplMgr
    {
        private IProductItemMapImplDao _ProductItemMapDao;
        private IChannelImplDao _channelDao;
        private MySqlDao _mySqlDao;
        private IProductMapSetImplDao _mapSetDao;
        private IChannelOrderImplDao _channelorderDao;
        private IPriceMasterImplDao _pricemasterDao;
        private string connectionString;
        public ProductItemMapMgr(string connectionString)
        {
            this.connectionString = connectionString;
            _channelDao = new BLL.gigade.Dao.ChannelDao(connectionString);
            _ProductItemMapDao = new Dao.ProductItemMapDao(connectionString);
            _mySqlDao = new MySqlDao(connectionString);
            _mapSetDao = new ProductMapSetDao(connectionString);
            _channelorderDao = new ChannelOrderDao(connectionString);
            _pricemasterDao = new PriceMasterDao(connectionString);
        }

        public List<ProductItemMap> QueryAll(ProductItemMap p)
        {

            try
            {
                return _ProductItemMapDao.QueryAll(p);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductItemMapMgr-->QueryAll-->" + ex.Message, ex);
            }
        }
        //add by hufeng 2014/05/21 爲了在訂單匯入的時候判斷庫存
        public List<ProductItemMap> QueryChannel_detail_id(string id)
        {
            try
            {
                return _ProductItemMapDao.QueryChannel_detail_id(id);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductItemMapMgr-->QueryAll-->" + ex.Message, ex);
            }
        }
        //根據外站id和賣場商品ID查找是否存在信息
        public List<ProductItemMap> QueryAll(uint channelId, string condition)
        {
            return _ProductItemMapDao.QueryAll(channelId, condition);
        }

        public List<ProductItemMapCustom> QueryProductItemMap(ProductItemMapQuery p, out int totalCount)
        {

            try
            {
                return _ProductItemMapDao.QueryProductItemMap(p, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductItemMapMgr-->QueryProductItemMap-->" + ex.Message, ex);
            }
        }


        public List<Model.Query.ProductCompare> QueryProductByNo(int cId, int pNo,int pmId, out int totalCount)
        {

            try
            {
                return _ProductItemMapDao.QueryProductByNo(cId, pNo,pmId, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductItemMapMgr-->QueryProductByNo-->" + ex.Message, ex);
            }
        }
        public List<Model.Channel> QueryOutSite()
        {

            try
            {
                return _channelDao.QueryCooperationSite(1);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductItemMapMgr-->QueryOutSite-->" + ex.Message, ex);
            }
        }
        //add by xiangwang0413w 2014/07/02
        public List<PriceMasterCustom> QuerySitePriceOption(uint productId, uint channelId)
        {
            try 
            {
                return _pricemasterDao.QuerySitePriceOption(productId,channelId);               
            }
            catch (Exception ex)
            {
                throw new Exception("ProductItemMapMgr-->QueryOutSite-->" + ex.Message, ex);
            }
        }

        public bool SingleCompareSave(List<ProductItemMap> pList)
        {
            ArrayList saveList = new ArrayList();
            try
            {
                foreach (ProductItemMap item in pList)
                {
                    if (item.rid != 0)
                    {
                        saveList.Add(_ProductItemMapDao.deleteString(item));
                    }
                    saveList.Add(_ProductItemMapDao.saveString(item));
                }
                return _mySqlDao.ExcuteSqls(saveList);

            }
            catch (Exception ex)
            {
                throw new Exception("ProductItemMapMgr-->SingleCompareSave-->" + ex.Message, ex);
            }
        }

        public bool ComboCompareSave(List<ProductItemMapCustom> saveList)
        {
            bool result = true;
            if (saveList != null && saveList.Count > 0)
            {
                foreach (var item in saveList)
                {
                    ArrayList delList = new ArrayList();
                    string itemMapStr = string.Empty;
                    ArrayList mapSetList = new ArrayList();
                    if (item.rid != 0)
                    {
                        delList.Add(_mapSetDao.Delete(new ProductMapSet { map_rid = item.rid }));
                        delList.Add(_ProductItemMapDao.deleteString(item));
                    }

                    itemMapStr = _ProductItemMapDao.saveString(item);

                    foreach (var mapset in item.MapChild)
                    {
                        mapSetList.Add(_mapSetDao.Save(mapset));
                    }

                    if (!_ProductItemMapDao.comboSave(delList, itemMapStr, mapSetList))
                    {
                        result = false;
                    }

                }
            }
            return result;

            //foreach (var item in saveList)
            //{
            //    ArrayList arry = new ArrayList();
            //    if (item.rid != 0)
            //    {
            //        arry.Add(_mapSetDao.Delete(new ProductMapSet { map_rid = item.rid }));
            //        arry.Add(_ProductItemMapDao.deleteString(item));
            //        _mySqlDao.ExcuteSqls(arry);
            //    }
            //    _ProductItemMapDao.Save_Comb(item);
            //}
        }

        public int Delete(ProductItemMap p)
        {
            try
            {
                List<ChannelOrder> result = _channelorderDao.Query(new ChannelOrder { Channel_Detail_Id = p.channel_detail_id, Channel_Id = int.Parse(p.channel_id.ToString()) });
                if (result.Count() > 0)
                {
                    return -1;
                }
                else
                {
                    return _ProductItemMapDao.Delete(p);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("ProductItemMapMgr-->Delete-->" + ex.Message, ex);
            }
        }

        public bool ComboDelete(ProductItemMap delete)
        {
            ArrayList delArry = new ArrayList();
            delArry.Add(_mapSetDao.Delete(new ProductMapSet { map_rid = delete.rid }));
            delArry.Add(_ProductItemMapDao.deleteString(delete));
            return _mySqlDao.ExcuteSqls(delArry);
        }

        public List<ProductMapCustom> CombinationQuery(ProductItemMapCustom p)
        {
            return _ProductItemMapDao.CombinationQuery(p);
        }

        public List<ProductComboMap> ProductComboItemQuery(int child_id, int parent_id)
        {
            return _ProductItemMapDao.ProductComboItemQuery(child_id, parent_id);
        }



        public List<ProductComboMap> ComboItemQuery(ProductCombo query)
        {
            return _ProductItemMapDao.ComboItemQuery(query);
        }

        //查找建立過的對照信息
        public List<ProductItemMap> QueryOldItemMap(ProductItemMap Pip)
        {
            return _ProductItemMapDao.QueryOldItemMap(Pip);
        }

        /// <summary>
        /// 匯出商品對照檔
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public MemoryStream ExportProductItemMap(QueryVerifyCondition query)
        {
            IProductImplMgr  _prodMgr = new ProductMgr(connectionString);
            List<ProductItemMapCustom> pims = new List<ProductItemMapCustom>();
            List<Model.Custom.PriceMasterCustom> pmcs = _prodMgr.Query(query);
            foreach (var pmc in pmcs)
            {
                switch (pmc.combination)
                {
                    case 1://單一商品
                        var single = SingleProductItemMapExc(pmc);
                        pims.AddRange(single);
                        break;
                    case 2://固定組合
                        var fixedCombo = FixedComboProductItemMapExc(pmc);
                        pims.AddRange(fixedCombo);
                        break;
                    case 3:
                    case 4:
                        ProductItemMapCustom item = new ProductItemMapCustom();
                        item.channel_detail_id = Resource.CoreMessage.GetResource("MANUAL_OPERATING");
                        item.product_name = pmc.product_name;
                        item.product_id = pmc.product_id;
                        item.site_id = pmc.site_id;
                        item.user_level = pmc.user_level;
                        item.user_id = pmc.user_id;
                        pims.Add(item);
                        break;
                    default:
                        break;
                }
            }
            //創建excel並填入數據
            MemoryStream ms = new MemoryStream();
            IWorkbook workBook = new HSSFWorkbook();
            ISheet sheet = workBook.CreateSheet();
            IRow headerRow = sheet.CreateRow(0);
            headerRow.CreateCell(0).SetCellValue(Resource.CoreMessage.GetResource("OUTSITE_PRODUCT_ID"));
            headerRow.CreateCell(1).SetCellValue(Resource.CoreMessage.GetResource("OUTSITE_PRODUCT_NAME"));
            headerRow.CreateCell(2).SetCellValue(Resource.CoreMessage.GetResource("PRODUCT_ID"));
            headerRow.CreateCell(3).SetCellValue(Resource.CoreMessage.GetResource("ITEM_ID"));
            headerRow.CreateCell(4).SetCellValue(Resource.CoreMessage.GetResource("COMBO_NUM_SET"));
            headerRow.CreateCell(5).SetCellValue(Resource.CoreMessage.GetResource("OUTSITE_PRODUCT_PRICE"));
            headerRow.CreateCell(6).SetCellValue("site_id");
            headerRow.CreateCell(7).SetCellValue("user_level");
            headerRow.CreateCell(8).SetCellValue("user_email");

            int rowIndex=1;
            foreach (var item in pims)
            {
                IRow dataRow = sheet.CreateRow(rowIndex);
                dataRow.CreateCell(0).SetCellValue(item.channel_detail_id);
                dataRow.CreateCell(1).SetCellValue(item.product_name);
                dataRow.CreateCell(2).SetCellValue(item.product_id);
                dataRow.CreateCell(3).SetCellValue(item.group_item_id);
                dataRow.CreateCell(4).SetCellValue(0);//組合中之數量(0為照組合中之設定)
                //dataRow.CreateCell(5).SetCellValue(item.product_cost);//外站商品成本
                dataRow.CreateCell(5).SetCellValue(item.product_price);
                dataRow.CreateCell(6).SetCellValue(item.site_id);
                dataRow.CreateCell(7).SetCellValue(item.user_level);
                dataRow.CreateCell(8).SetCellValue(item.user_id);

                rowIndex++;
            }
            workBook.Write(ms);
            ms.Flush();
            ms.Position = 0;
            workBook = null;
            return ms;
        }

        //單一商品
        public List<ProductItemMapCustom> SingleProductItemMapExc(PriceMasterCustom pmc)
        {
            IProductItemImplMgr _proItem = new ProductItemMgr(connectionString);
            List<ProductItem> resultList = _proItem.Query(pmc);
            List<ProductItemMapCustom> pimcs = new List<ProductItemMapCustom>();
            int i=1;
            foreach (var item in resultList)
            {
                var p = new ProductItemMapCustom();
                p.channel_detail_id = pmc.product_id + ((resultList.Count > 1) ? ("-" + i) : "");//如果該商品只有一個子項,則不拼接序號
                p.product_name = pmc.product_name + ((item.Spec_Name_1 == "" && item.Spec_Name_2 == "") ? "" : ("-" + item.Spec_Name_1 + item.Spec_Name_2));
                p.product_id = pmc.product_id;
                p.group_item_id = item.Item_Id.ToString() ;
                p.product_price = pmc.price;
                p.product_cost = pmc.cost;

                p.site_id = pmc.site_id;
                p.user_level = pmc.user_level;
                p.user_id = pmc.user_id;
                pimcs.Add(p);
                i++;
            }
            return pimcs;
        }

        //組合商品
        public List<ProductItemMapCustom> FixedComboProductItemMapExc(PriceMasterCustom pmc)
        {
            ProductItemMapMgr _mapMgr = new ProductItemMapMgr(connectionString);
            List<ProductItemMapCustom> pimcs = new List<ProductItemMapCustom>();
            List<ProductMapCustom> resultList = _ProductItemMapDao.CombinationQuery(
                new ProductItemMapCustom { product_id = pmc.product_id }
                );

            if (resultList.Count() > 0)
            {
                List<List<ProductComboMap>> itemAll = new List<List<ProductComboMap>>();
                for (int i = 0, j = resultList.Count(); i < j; i++)
                {
                    List<ProductComboMap> itemList = _mapMgr.ProductComboItemQuery(resultList[i].child_id, (int)pmc.product_id);
                    if (itemList.Count > 0)//當子商品有規格時才可以建立對照
                    {
                        itemAll.Add(itemList);
                    }
                }
                if (itemAll.Count == 0) return pimcs;
                string s = "";
                getCombo(0, itemAll, "", ref s);
                s = s.Remove(s.Length - 1);
                string[] itemstr = s.Split('&');
                for (int Si = 0, Sj = itemstr.Length; Si < Sj; Si++)
                {
                    ProductItemMapCustom _productitemmap = new ProductItemMapCustom();
                    _productitemmap.channel_detail_id = pmc.product_id + ((Sj > 1) ? ("-" + (Si + 1)) : "");//如果組合種類為多種,則加序號
                    _productitemmap.product_name = pmc.product_name;
                    _productitemmap.product_id = pmc.product_id;
                    _productitemmap.group_item_id = itemstr[Si];
                    _productitemmap.product_price = pmc.price;
                    _productitemmap.product_cost = pmc.cost;
                    _productitemmap.site_id = pmc.site_id;
                    _productitemmap.user_level = pmc.user_level;
                    _productitemmap.user_id = pmc.user_id;
                    pimcs.Add(_productitemmap);
                }
            }
            return pimcs;
        }

        /// <summary>
        /// 遞歸組合固定/群組商品
        /// </summary>
        /// <param name="count"></param>
        /// <param name="itemAll"></param>
        /// <param name="result"></param>
        /// <param name="r"></param>
        public void getCombo(int count, List<List<ProductComboMap>> itemAll, string result, ref string r)
        {
            if (count >= itemAll.Count)
            {
                r += result + "&";
                return;
            }
            if (count > 0)
            {
                result += ",";
            }
            foreach (ProductComboMap s in itemAll[count])
            {
                getCombo(count + 1, itemAll, result + s.item_id, ref r);
            }
        }
        //匯出商品對照信息-20140805
        public DataTable ProductItemMapExc(ProductItemMap Pip)
        {
            return _ProductItemMapDao.ProductItemMapExc(Pip);
        }
        public int Selrepeat(string cdid)
        {
            return _ProductItemMapDao.Selrepeat(cdid);
        }
        public int UpdatePIM(ProductItemMap p)
        {
            return _ProductItemMapDao.UpdatePIM(p);
        }
        public int Save(ProductItemMap p)
        {
            return _ProductItemMapDao.Save(p);
        }
    }
}
