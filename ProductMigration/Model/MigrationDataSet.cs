using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Mgr;
using System.Text.RegularExpressions;
using BLL.gigade.Common;

namespace ProductMigration.Model
{
    public class MigrationDataSet
    {
        static string mysqlConnectionStr = System.Configuration.ConfigurationSettings.AppSettings["MySqlConnectionString"];
        IVendorBrandImplMgr _vbrandMgr = new VendorBrandMgr(mysqlConnectionStr);
        IParametersrcImplMgr _paramMgr = new ParameterMgr(mysqlConnectionStr);
        IProductImplMgr _proMgr = new ProductMgr(mysqlConnectionStr);
        IProductItemImplMgr _pItemMgr = new ProductItemMgr(mysqlConnectionStr);
        ISiteImplMgr _siteMgr = new SiteMgr(mysqlConnectionStr);
        IProductMigrationImplMgr _prodMig = new ProductMigrationMgr(mysqlConnectionStr);
        IProductCategoryImplMgr _cateMgr = new ProductCategoryMgr(mysqlConnectionStr);

        Product query = new Product();

        /// <summary>
        /// 錯誤信息
        /// </summary>
        private string outMessage = string.Empty;

        public string OutMessage
        {
            get { return outMessage; }
            set { outMessage = value; }
        }
        /// <summary>
        /// 品牌名稱
        /// </summary>
        private string brand_name = string.Empty;

        public string Brand_name
        {
            get { return brand_name; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    this.outMessage += "brand_name不能為空;";
                    brand_name = value;
                }
                else
                {
                    VendorBrand vb = _vbrandMgr.GetProductBrand(new VendorBrand { Brand_Name = value });
                    if (vb == null)
                    {
                        this.outMessage += "brand_name不存在;";
                        brand_name = value;
                    }
                    else
                    {
                        brand_name = vb.Brand_Id.ToString();
                    }
                }
            }
        }


        /// <summary>
        /// 現有商品
        /// </summary>
        private string is_exist = string.Empty;
        public string Is_exist
        {
            get { return is_exist; }
            set
            {
                is_exist = CheckIsNullOrEmpty(value, "is_exist不能為空;");
            }
        }
        /// <summary>
        /// 商品編號
        /// </summary>
        private string product_id = string.Empty;

        public string Product_id
        {
            get { return product_id; }
            set
            {
                if (Is_exist.Equals("old"))
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        this.outMessage += "product_id不能為空;";
                    }
                    else
                    {
                        if (!value.Equals("0"))
                        {
                            query = _proMgr.Query(new Product { Product_Id = uint.Parse(value) }).FirstOrDefault();

                            List<ProductItem> pItemList = _pItemMgr.Query(new ProductItem { Product_Id = uint.Parse(value) });
                            if (query == null)
                            {
                                this.outMessage += "product_id不存在;";
                            }
                            if (pItemList == null || pItemList.Count == 0)
                            {
                                this.outMessage += "此舊商品數據錯誤,沒有細項記錄;";
                            }
                        }
                        else
                        {
                            if (Is_exist.Equals("old"))
                            {
                                this.OutMessage += "product_id不能為零";
                            }
                        }
                    }
                }
                product_id = value;
            }
        }
        /// <summary>
        /// 暫時編號
        /// </summary>
        private string temp_id = string.Empty;

        public string Temp_id
        {
            get { return temp_id; }
            set
            {
                if (Is_exist.Equals("new"))
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        this.OutMessage += "temp_id不能為空";
                    }
                    else
                    {
                        ProductMigrationMap prodMigra = _prodMig.GetSingle(new ProductMigrationMap { temp_id = value });
                        if (prodMigra != null)
                        {
                            this.outMessage += "此記錄暫時編號(temp_id)已經存在;";
                        }
                    }
                }
                temp_id = value;
            }
        }


        /// <summary>
        /// 站臺
        /// </summary>
        private string site = string.Empty;

        public string Site
        {
            get { return site; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    this.outMessage += "site不能為空;";
                    site = value;
                }
                else
                {
                    BLL.gigade.Model.Site s = _siteMgr.Query(new BLL.gigade.Model.Site { Site_Name = value }).FirstOrDefault();
                    if (s == null)
                    {
                        this.outMessage += "site不存在";
                        site = value;
                    }
                    else
                    {
                        site = s.Site_Id.ToString();
                    }
                }
            }
        }

        /// <summary>
        /// 商品名稱
        /// </summary>
        private string product_name = string.Empty;

        public string Product_name
        {
            get { return product_name; }
            set { product_name = CheckIsNullOrEmpty(value, "product_name不能為空;"); }
        }
        /// <summary> 
        /// 商品類型
        /// </summary>
        private string combination = string.Empty;

        public string Combination
        {
            get { return combination; }
            set
            {
                combination = CheckIsNullOrEmpty(value, "combination不能為空;");
            }
        }
        /// <summary>
        /// 原料編號新舊
        /// </summary>
        private string new_old = string.Empty;

        public string New_old
        {
            get { return new_old; }
            set { new_old = value; }
        }

        /// <summary>
        /// 原料編號
        /// </summary>
        private string formula = string.Empty;

        public string Formula
        {
            get { return formula; }
            set { formula = value; }
        }

        /// <summary>
        /// 商品狀態
        /// </summary>
        private string status = string.Empty;

        public string Status
        {
            get { return status; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    if (Is_exist == "new")
                        this.outMessage += "status不能為空;";
                    status = value;
                }
                else
                {
                    value = value.ToLower();
                    if (!value.Equals("same") && !value.Equals("on") && !value.Equals("off"))
                    {
                        this.outMessage += "status不存在;";
                        status = value;
                    }
                    else
                    {
                        if (value.Equals("on"))//上架 
                        {
                            status = "5";
                        }
                        else if (value.Equals("off"))
                        {
                            status = "6";
                        }
                        else if (value.Equals("new"))
                        {
                            status = "0";
                        }
                        else if (value.Equals("same"))
                        {
                            if (query != null)
                            {
                                status = query.Product_Status.ToString();
                            }
                            else
                            {
                                status = value;
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 出貨方式
        /// </summary>
        private string product_mode = string.Empty;

        public string Product_mode
        {
            get { return product_mode; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    if (Is_exist == "new")
                        this.outMessage += "product_mode不能為空;";
                    product_mode = value;
                }
                else
                {
                    Parametersrc query = _paramMgr.QueryUsed(new Parametersrc { ParameterType = "product_mode", parameterName = value, Used = 1 }).FirstOrDefault();
                    if (query == null)
                    {
                        this.outMessage += "product_mode不存在;";
                        product_mode = value;
                    }
                    else
                    {
                        product_mode = query.ParameterCode.ToString();
                    }
                }
            }
        }
        /// <summary>
        /// 運送模式
        /// </summary>
        private string freight_set = string.Empty;

        public string Freight_set
        {
            get { return freight_set; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    if (Is_exist == "new")
                        this.outMessage += "freight_set不能為空;";
                    freight_set = value;
                }
                else
                {
                    Parametersrc query = _paramMgr.QueryUsed(new Parametersrc { ParameterType = "product_freight", parameterName = value, Used = 1 }).FirstOrDefault();
                    if (query == null)
                    {
                        this.outMessage += "freight_set不存在;";
                        freight_set = value;
                    }
                    else
                    {
                        freight_set = query.ParameterCode.ToString();
                    }
                }
            }
        }

        /// <summary>
        /// 營業稅
        /// </summary>
        private string tax = string.Empty;

        public string Tax
        {
            get { return tax; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    if (Is_exist == "new")
                        this.outMessage += "taxb不能為空;";
                    tax = value;
                }
                else
                {
                    if (!value.Equals("應稅") && !value.Equals("免稅"))
                    {
                        this.outMessage += "tax不存在;";
                        tax = value;
                    }
                    else
                    {
                        tax = value.Equals("應稅") ? "1" : "3";
                    }
                }

            }
        }


        /// <summary>
        /// 庫存為0時是否還能販售
        /// </summary>
        private string ignore_stock = string.Empty;

        public string Ignore_stock
        {
            get { return ignore_stock; }
            set
            {
                #region 原邏輯
                //if (string.IsNullOrEmpty(value))
                //{
                //    if (Is_exist == "new")
                //        this.outMessage = "ignore_stock不能為空;";
                //    ignore_stock = value;
                //}
                //else
                //{
                //    if (!value.Equals("N") && !value.Equals("Y"))
                //    {
                //        this.outMessage = "ignore_stock不存在,需為N或Y;";
                //        ignore_stock = value;
                //    }
                //    else
                //    {
                //        ignore_stock = value.Equals("N") ? "0" : "1";
                //    }
                //} 
                #endregion
                ignore_stock = "0";
            }
        }

        /// <summary>
        /// 補貨中停止販售
        /// </summary>
        private string shortage = string.Empty;

        public string Shortage
        {
            get { return shortage; }
            set
            {
                #region 原邏輯
                //if (string.IsNullOrEmpty(value))
                //{
                //    if (Is_exist == "new")
                //        this.outMessage = "shortage不能為空;";
                //    shortage = value;
                //}
                //else
                //{
                //    if (!value.Equals("N") && !value.Equals("Y"))
                //    {
                //        this.outMessage = "shortage不存在,需為N或Y;";
                //        shortage = value;
                //    }
                //    else
                //    {
                //        shortage = value.Equals("N") ? "0" : "1";
                //    }
                //} 
                #endregion
                shortage = "0";
            }
        }

        /// <summary>
        /// 庫存  
        /// </summary>
        private string stock = string.Empty;

        public string Stock
        {
            get { return stock; }
            set { stock = CheckIsNullOrEmpty(value, "stock不能為空;"); }
        }

        /// <summary>
        /// 販售價格   price_master_price
        /// </summary>
        private string price = string.Empty;

        public string Price
        {
            get { return price; }
            set
            {
                if (this.Combination != "199")
                {
                    price = CheckIsNullOrEmpty(value, "price不能為空;");
                }
                else
                {
                    price = value;
                }

            }
        }

        /// <summary>
        /// 成本  price_master.cost
        /// </summary>
        private string cost = string.Empty;

        public string Cost
        {
            get { return cost; }
            set
            {
                if (this.Combination != "199")
                {
                    cost = CheckIsNullOrEmpty(value, "cost不能為空;");
                }
                else
                {
                    cost = value;
                }
            }
        }

        /// <summary>
        /// 品類分類
        /// </summary>
        private string cate_id = string.Empty;

        public string Cate_id
        {
            get { return cate_id; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    if (Is_exist == "new")
                        this.outMessage += "cate_id不能為空;";
                }
                cate_id = value;
            }
        }

        /// <summary>
        /// 前台分類
        /// </summary>
        private string display = string.Empty;
        public string Display
        {
            get { return display; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    Regex regx = new Regex("^[0-9]*$");
                    string[] displayArry = value.Split(',');
                    foreach (string item in displayArry)
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            if (regx.IsMatch(item))
                            {
                                ProductCategory query = _cateMgr.QueryAll(new ProductCategory { category_id = uint.Parse(item) }).FirstOrDefault();
                                if (query == null || item.Equals("0"))
                                {
                                    this.OutMessage += "display:" + item + " 不存在;";
                                }
                            }
                            else
                            {
                                this.OutMessage += "display:" + item + " 格式不正確;";
                            }
                        }
                    }
                    display = value;
                }
            }
        }

        /// <summary>
        /// 寄倉費
        /// </summary>
        private string service_fee = string.Empty;
        public string Service_Fee
        {
            get { return service_fee; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    if (!DataCheck.IsNumeric(value))
                    {
                        this.OutMessage += "寄倉費格式錯誤;";
                    }
                }
                service_fee = value;
            }
        }

        public MigrationDataSet()
        {
        }
        public string CheckIsNullOrEmpty(string value, string Msg)
        {
            if (string.IsNullOrEmpty(value) && is_exist == "new")
            {
                this.OutMessage += Msg;
            }
            return value;
        }

    }
}
