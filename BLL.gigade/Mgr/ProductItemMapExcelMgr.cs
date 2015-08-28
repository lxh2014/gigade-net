using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Mgr;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.Text.RegularExpressions;
using NPOI.XSSF.UserModel;
using BLL.gigade.Common;
using System.Data;
using DBAccess;
using BLL.gigade.Model.Custom;
using BLL.gigade.Model.Query;
namespace BLL.gigade.Mgr
{
    public class ProductItemMapExcelMgr : IProductItemMapExcelImplMgr
    {
        private IProductItemMapImplMgr _productItemMapMgr;
        private IProductItemImplMgr _productItemMgr;
        private IPriceMasterImplMgr _priceMasterMgr;
        private IProductItemMapImplDao _productItemMapDao;
        private ProductItemDao _productItemDao;
        private ICallerImplMgr _callerMgr;
        private IDBAccess _access;
        public ProductItemMapExcelMgr(string connectionString)
        {
            _productItemMapMgr = new ProductItemMapMgr(connectionString);
            _productItemMapDao = new ProductItemMapDao(connectionString);
            _productItemMgr = new ProductItemMgr(connectionString);
            _productItemDao = new ProductItemDao(connectionString);
            _callerMgr = new CallerMgr(connectionString);
            _priceMasterMgr = new PriceMasterMgr(connectionString);
            _access = DBAccess.DBFactory.getDBAccess(DBType.MySql, connectionString);
        }

        #region 讀取Excel文件 +List<ProductItemMapCustom> ReadFile(string filePath, int channel_id, string fExtension, ref bool isHeaderError, List<ProductItemMapCustom> errorPm)
        /// <summary>
        /// 讀取Excel文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="channel_id"></param>
        /// <param name="fExtension"></param>
        /// <param name="isHeaderError"></param>
        /// <param name="errorPm"></param>
        /// <returns></returns>
        public List<ProductItemMapCustom> ReadFile(string filePath, int channel_id, string fExtension, ref bool isHeaderError, List<ProductItemMapCustom> errorPm)
        {
            try
            {
                NPOI4ExcelHelper fm = new NPOI4ExcelHelper(filePath);
                DataTable dt = fm.SheetData();
                if (dt == null) { return null; }

                List<ProductItemMapCustom> result = new List<ProductItemMapCustom>();

                Regex RegxProductId = new Regex("^\\d{5}$");
                Regex RegxItemId = new Regex("^\\d{6}$");
                Regex RegxMoney = new Regex("^\\d{1,9}$");

                for (int i = 0, j = dt.Rows.Count; i < j; i++)
                {
                    bool bl = true;
                    bool el = true;
                    bool isNull = false;
                    bool Pro_id = false;//記錄商品編號是否為空
                    bool Itm_id = false;//記錄商品細項編號是否為空
                    PriceMaster prm = new PriceMaster();
                    ProductItemMapCustom pm = new ProductItemMapCustom();
                    pm.channel_id = Convert.ToUInt32(channel_id);
                    for (int m = 0, n = dt.Columns.Count; m < n; m++)
                    {
                        string valStr = dt.Rows[i][m].ToString();
                        switch (dt.Columns[m].ToString().Trim())
                        {
                            case "商品編號(5碼)":
                                if (!string.IsNullOrEmpty(valStr))
                                {
                                    uint _ProductId = 0;
                                    uint.TryParse(valStr ?? "0", out _ProductId);
                                    pm.product_id = _ProductId;
                                    prm.product_id = _ProductId;
                                    if (!RegxProductId.IsMatch(pm.product_id.ToString()))
                                    {
                                        bl = false;
                                    }
                                }
                                else
                                {
                                    Pro_id = true;
                                    isNull = true;
                                    break;
                                }
                                break;
                            case "商品細項編號(6碼)":
                                if (!string.IsNullOrEmpty(valStr))
                                {
                                    if (valStr.Split(',').Length > 1)
                                    {
                                        for (int k = 0; k < valStr.Split(',').Length; k++)
                                        {
                                            if (!RegxItemId.IsMatch(valStr.Split(',')[k]))
                                            {
                                                bl = false;
                                            }
                                        }
                                        pm.group_item_id = CommonFunction.Rank_ItemId(valStr);
                                    }
                                    else
                                    {
                                        uint _itemId = 0;
                                        uint.TryParse(valStr ?? "0", out _itemId);
                                        pm.item_id = _itemId;
                                        pm.group_item_id = _itemId.ToString();
                                        if (!RegxItemId.IsMatch(pm.item_id.ToString()))
                                        {
                                            bl = false;
                                        }
                                    }
                                }
                                else
                                {
                                    Itm_id = true;
                                    isNull = true;
                                    break;
                                    //bl = false;
                                };
                                break;
                            case "外站商品名稱":
                                if (!string.IsNullOrEmpty(valStr))
                                {
                                    pm.product_name = valStr;
                                }
                                else
                                {
                                    bl = false;
                                };
                                break;
                            case "外站商品編號":
                                if (!string.IsNullOrEmpty(valStr))
                                {
                                    pm.channel_detail_id = valStr;
                                }
                                else
                                {
                                    bl = false;
                                }; break;
                            case "外站商品成本":
                                if (!string.IsNullOrEmpty(valStr))
                                {
                                    if (RegxMoney.IsMatch(valStr))
                                    {
                                        pm.product_cost = int.Parse(valStr);
                                    }
                                    else
                                    {
                                        bl = false;
                                    }
                                }
                                else
                                {
                                    bl = false;
                                }
                                ; break;
                            case "外站商品售價":
                                if (!string.IsNullOrEmpty(valStr))
                                {
                                    if (RegxMoney.IsMatch(valStr))
                                    {
                                        pm.product_price = int.Parse(valStr);
                                    }
                                    else
                                    {
                                        bl = false;
                                    }
                                }
                                else
                                {
                                    bl = false;
                                }
                                ; break;
                            case "組合中之數量(0為照組合中之設定)":
                                if (!string.IsNullOrEmpty(valStr))
                                {
                                    if (RegxMoney.IsMatch(valStr))
                                    {
                                        pm.set_num = uint.Parse(valStr);
                                    }
                                    else
                                    {
                                        bl = false;
                                    }
                                }
                                else
                                {
                                    bl = false;
                                } break;
                            case "user_email"://郵箱
                                if (!string.IsNullOrEmpty(valStr))
                                {
                                    if (_callerMgr.Login(valStr) != null)
                                    {
                                        prm.user_id = uint.Parse(_callerMgr.Login(valStr).user_id.ToString());
                                    }
                                }
                                break;
                            case "site_id"://站臺
                                if (!string.IsNullOrEmpty(valStr))
                                {
                                    if (RegxMoney.IsMatch(valStr))
                                    {
                                        prm.site_id = uint.Parse(valStr);
                                    }
                                    else
                                    {
                                        bl = false;
                                    }
                                }
                                else
                                {
                                    bl = false;
                                }
                                break;
                            case "user_level"://站臺等級
                                if (!string.IsNullOrEmpty(valStr))
                                {
                                    if (RegxMoney.IsMatch(valStr))
                                    {
                                        prm.user_level = uint.Parse(valStr);
                                    }
                                    else
                                    {
                                        bl = false;
                                    }
                                }
                                else
                                {
                                    bl = false;
                                }
                                break;
                            default:
                                el = false;
                                break;
                        }
                        //if (isNull)
                        //{
                        //    break;
                        //}
                    }

                    //判斷Excel表頭格式
                    if (!el)
                    {
                        isHeaderError = true;
                        break;
                    }
                    else
                    {
                        isHeaderError = false;
                    }

                    //若單一商品可以無商品編號,若固定組合可以無商品細項編號。其餘類型出現為空數據,則此行記錄無效,不做處理
                    if (Itm_id)
                    {
                        if (_productItemMapDao.CombinationQuery(pm).FirstOrDefault() != null && _productItemMapDao.CombinationQuery(pm).FirstOrDefault().combination == 2)
                        {
                            List<ProductComboMap> pMc = _productItemMapDao.QueryItemId(pm.product_id);
                            if (pMc != null && pMc.Where(e => e.product_spec == 0).Count() == pMc.Count())
                            {
                                string strItem = "";
                                for (int Itemi = 0, Itemj = pMc.Count(); Itemi < Itemj; Itemi++)
                                {
                                    if (Itemi > 0)
                                    {
                                        strItem += ",";
                                    }
                                    strItem += pMc[Itemi].item_id;
                                }
                                
                                pm.group_item_id = CommonFunction.Rank_ItemId(strItem);
                                isNull = false;
                            }
                        }
                    }
                    //單一商品邏輯 add by hufeng0813w 2013/12/23
                    if (Pro_id)
                    {
                        ProductItem pIm = new ProductItem();
                        pIm.Item_Id = uint.Parse(pm.item_id.ToString());
                        //pm.product_id = _productItemDao.Query(pIm).FirstOrDefault().Product_Id;
                        ProductItem itemResult = _productItemDao.Query(pIm).FirstOrDefault();
                        if (itemResult != null)
                        {
                            prm.product_id = pm.product_id = itemResult.Product_Id;
                            ProductMapCustom mapcusResult = _productItemMapDao.CombinationQuery(pm).FirstOrDefault();
                            if (mapcusResult != null && mapcusResult.combination == 1)
                            {
                                isNull = false;
                            }
                        }

                    }
                    if (isNull)
                    {
                        continue;
                    }
                    //獲取price_master_id
                    if (prm.product_id != 0)
                    {
                        pm.price_master_id = _priceMasterMgr.QueryPriceMasterId(prm);
                    }
                    else
                    {
                        bl = false;
                    }

                    //判斷內容格式
                    if (!bl)
                    {
                        pm.msg = Resource.CoreMessage.GetResource("ERROR_FORMAT");
                    }
                    else
                    {
                        ProductMapCustom map = _productItemMapDao.CombinationQuery(pm).FirstOrDefault();
                        if (map != null)
                        {
                            uint Combination = map.combination;
                            if (Combination != 2)
                            {
                                ProductItem pi = new ProductItem();
                                pi.Item_Id = uint.Parse(pm.item_id.ToString());
                                if (_productItemMgr.Query(pi).Count == 0)
                                {
                                    pm.msg = Resource.CoreMessage.GetResource("ITEMID_ID_NOT_EXISTS");
                                    bl = false;
                                }
                                else
                                {
                                    if (_productItemMapDao.Exist(pm) > 0)
                                    {
                                        pm.msg = Resource.CoreMessage.GetResource("COMPARE_EXISTS");
                                        bl = false;
                                    }
                                    else
                                    {
                                        ProductItemMapCustom existItem = result.Where(m => m.item_id == pm.item_id).FirstOrDefault();
                                        if (existItem != null)
                                        {
                                            pm.msg = Resource.CoreMessage.GetResource("COMPARE_EXISTS");
                                            bl = false;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                #region
                                for (int l = 0; l < pm.group_item_id.Split(',').Length; l++)
                                {
                                    if (_productItemMapDao.Comb_Compare(pm.product_id, uint.Parse(pm.group_item_id.Split(',')[l])) == 0)
                                    {
                                        pm.msg = Resource.CoreMessage.GetResource("ITEMID_ID_NOT_COMBINATION");
                                        bl = false;
                                    }
                                    else
                                    {
                                        #region
                                        if (l == 0)
                                        {
                                            if (_productItemMapDao.Comb_Exist(pm) > 0)
                                            {
                                                pm.msg = Resource.CoreMessage.GetResource("COMPARE_EXISTS");
                                                bl = false;
                                            }
                                            else
                                            {
                                                ProductItemMapCustom existItem = result.Where(m => m.group_item_id == pm.group_item_id).FirstOrDefault();
                                                if (existItem != null)
                                                {
                                                    pm.msg = Resource.CoreMessage.GetResource("COMPARE_EXISTS");
                                                    bl = false;
                                                }
                                            }
                                        }
                                        #endregion
                                    }
                                }
                                #endregion
                            }
                        }
                        else
                        {
                            bl = false;
                            pm.msg = Resource.CoreMessage.GetResource("PRODUCT_NOT_EXIST");
                        }
                    }

                    if (!bl)
                    {
                        errorPm.Add(pm);
                    }
                    else
                    {
                        result.Add(pm);
                    }
                }
                return result; // _access.getObjByTable<ProductItemMap>(dt);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductItemMapExcelMgr-->ReadFile-->" + ex.Message, ex);
            }
        } 
        #endregion

        #region 商品對照保存到數據庫 +int SaveToDB(List<ProductItemMapCustom> pm)
        /// <summary>
        /// 商品對照保存到數據庫
        /// </summary>
        /// <param name="pm">數據列表</param>
        /// <returns>返回保存成功的數量</returns>
        public int SaveToDB(List<ProductItemMapCustom> pm)
        {
            foreach (ProductItemMapCustom p in pm)
            {
                try
                {
                    if (_productItemMapDao.CombinationQuery(p).FirstOrDefault() != null && _productItemMapDao.CombinationQuery(p).FirstOrDefault().combination != 2)
                    {
                        _productItemMapDao.Save(p);
                    }
                    else
                    {
                        _productItemMapDao.Save_Comb(p);
                    }
                }
                catch (Exception ex)
                {
                    return 0;
                }
            }
            return pm.Count;
        } 
        #endregion

        #region 创建Excel模版 +MemoryStream CreateExcelTable()
        /// <summary>
        /// 创建Excel模版
        /// </summary>
        /// <returns></returns>
        public MemoryStream CreateExcelTable()
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                IWorkbook workBook = new HSSFWorkbook();
                ISheet sheet = workBook.CreateSheet();
                IRow headerRow = sheet.CreateRow(0);
                headerRow.CreateCell(0).SetCellValue(Resource.CoreMessage.GetResource("OUTSITE_PRODUCT_ID"));
                headerRow.CreateCell(1).SetCellValue(Resource.CoreMessage.GetResource("OUTSITE_PRODUCT_NAME"));
                headerRow.CreateCell(2).SetCellValue(Resource.CoreMessage.GetResource("PRODUCT_ID"));
                headerRow.CreateCell(3).SetCellValue(Resource.CoreMessage.GetResource("ITEM_ID"));
                headerRow.CreateCell(4).SetCellValue(Resource.CoreMessage.GetResource("COMBO_NUM_SET"));
                headerRow.CreateCell(5).SetCellValue(Resource.CoreMessage.GetResource("OUTSITE_PRODUCT_COST"));
                headerRow.CreateCell(6).SetCellValue(Resource.CoreMessage.GetResource("OUTSITE_PRODUCT_PRICE"));
                //add by hufeng0813w 2014/07/07
                headerRow.CreateCell(7).SetCellValue("site_id");
                headerRow.CreateCell(8).SetCellValue("user_level");
                headerRow.CreateCell(9).SetCellValue("user_email");
                //end by hufeng0813w
                workBook.Write(ms);
                ms.Flush();
                ms.Position = 0;
                workBook = null;
                return ms;
            }
            catch (Exception ex)
            {
                throw new Exception("ProductItemMapExcelMgr-->MemoryStream-->" + ex.Message, ex);
            }

        } 
        #endregion

        /// <summary>
        /// 查詢product_item_map表中是否有數據
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public List<ProductItemMap> QueryProductItemMap(ProductItemMap p)
        {
            return _productItemMapDao.QueryProductItemMap(p);
        }
    }
}
