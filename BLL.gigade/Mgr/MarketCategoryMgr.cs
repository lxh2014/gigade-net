using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using DBAccess;

namespace BLL.gigade.Mgr
{
    public class MarketCategoryMgr : IMarketCategoryImplMgr
    {
        private IMarketCategoryImplDao _marketCategoryDao;
        private IDBAccess _access;
        private MarketProductMapDao _marketProductMapDao;
        public MarketCategoryMgr(string connectionStr)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionStr);
            _marketCategoryDao = new MarketCategoryDao(connectionStr);
            _marketProductMapDao = new MarketProductMapDao(connectionStr);
        }
        public int MarketCategoryImport(DataRow[] dr)
        {
            try
            {
                int res = 0;
                StringBuilder sql = new StringBuilder();
                List<string> cateIds = new List<string>();

                for (int j = 0; j < dr.Length; j++)
                {
                    uint fatherID = 0;//保存上層market_category_id

                    string[] arryCate = dr[j][1].ToString().Split('/');
                    for (int i = 0; i < arryCate.Length; i++)//循環每一層類別
                    {
                        int firstLeft = arryCate[i].IndexOf('(');//首個左括號的位置
                        int firstRight = arryCate[i].IndexOf(')', firstLeft);//首個右括號的位置
                        string cateId = arryCate[i].Substring(firstLeft + 1, firstRight - firstLeft - 1);
                        string cateName = arryCate[i].Substring(firstRight + 1).Replace('+', '&');
                        if (!cateIds.Contains(cateId))//判斷是否與本次匯入重複
                        {
                            cateIds.Add(cateId);

                            List<MarketCategoryQuery> store = _marketCategoryDao.GetMarketCategoryList(new MarketCategory { market_category_code = cateId });
                            if (store.Count == 0)//判斷是否與已有的類別重複
                            {
                                MarketCategory model = new MarketCategory();
                                model.attribute = dr[j][1].ToString();
                                model.created = DateTime.Now;
                                model.kuser = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
                                model.market_category_code = cateId;
                                if (i == 0)//匯入時每行第一個類別父層為0
                                {
                                    model.market_category_father_id = 0;
                                }
                                else//非首層類別父層為首層的market_category_id
                                {

                                    model.market_category_father_id = (int)fatherID;
                                }
                                model.market_category_name = cateName;
                                model.market_category_sort = 0;
                                model.market_category_status = 1;
                                model.modified = model.created;
                                model.muser = model.kuser;
                                sql.Append(_marketCategoryDao.InsertMarketCategory(model));
                            }
                            else
                            {//已經存在則進行修改
                                MarketCategory model = store.FirstOrDefault();
                                model.attribute = dr[j][1].ToString();
                                model.modified = DateTime.Now;
                                model.muser = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
                                model.market_category_code = cateId;
                                if (i == 0)//匯入時每行第一個類別父層為0
                                {
                                    model.market_category_father_id = 0;
                                }
                                else//非首層類別父層為首層的market_category_id
                                {

                                    model.market_category_father_id = (int)fatherID;
                                }
                                model.market_category_name = cateName;
                                model.market_category_sort = 0;
                                model.market_category_status = 1;

                                sql.Append(_marketCategoryDao.UpdateMarketCategory(model));
                            }
                            if (i == 0)//執行首條數據
                            {
                                res = res + _access.execCommand(sql.ToString());
                                sql.Clear();
                                fatherID = _marketCategoryDao.GetMarketCategoryList(new MarketCategory { market_category_code = cateId }).FirstOrDefault().market_category_id;
                            }
                        }
                        else//
                        {
                            if (i == 0)//執行首條數據
                            {
                                if (!string.IsNullOrEmpty(sql.ToString()))
                                {
                                    res = res + _access.execCommand(sql.ToString());
                                    sql.Clear();
                                }
                                fatherID = _marketCategoryDao.GetMarketCategoryList(new MarketCategory { market_category_code = cateId }).FirstOrDefault().market_category_id;
                            }
                        }

                    }

                    if (!string.IsNullOrEmpty(sql.ToString()))
                    {
                        res = res + _access.execCommand(sql.ToString());
                        sql.Clear();
                    }
                }

                return res;

            }
            catch (Exception ex)
            {
                throw new Exception("MarketCategoryMgr-->MarketCategoryImport-->" + ex.Message, ex);
            }
        }

        public List<MarketCategoryQuery> GetMarketCategoryList(MarketCategory model, out int totalCount)
        {
            try
            {
                return _marketCategoryDao.GetMarketCategoryList(model, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("MarketCategoryMgr-->GetMarketCategoryList-->" + ex.Message, ex);
            }
        }
        public List<MarketCategoryQuery> GetMarketCategoryList(MarketCategory model)
        {
            try
            {
                return _marketCategoryDao.GetMarketCategoryList(model);
            }
            catch (Exception ex)
            {
                throw new Exception("MarketCategoryMgr-->GetMarketCategoryList-->" + ex.Message, ex);
            }
        }
        public int InsertMarketCategory(MarketCategory model)
        {
            try
            {
                string sql = _marketCategoryDao.InsertMarketCategory(model);
                return _access.execCommand(sql);
            }
            catch (Exception ex)
            {
                throw new Exception("MarketCategoryMgr-->InsertMarketCategory-->" + ex.Message, ex);
            }
        }

        public int UpdateMarketCategory(MarketCategory model)
        {
            try
            {
                string sql = _marketCategoryDao.UpdateMarketCategory(model);
                return _access.execCommand(sql);
            }
            catch (Exception ex)
            {
                throw new Exception("MarketCategoryMgr-->UpdateMarketCategory-->" + ex.Message, ex);
            }
        }
        public int DeleteMarketCategory(string cids)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                foreach (string item in cids.Split('|'))
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        sql.Append(_marketCategoryDao.DeleteMarketCategory(Convert.ToInt32(item)));
                        sql.Append(_marketProductMapDao.DeleteMarketProductMap(new MarketProductMapQuery { market_category_id = Convert.ToInt32(item) }));
                    }
                }

                return _access.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("MarketCategoryMgr-->DeleteMarketCategory-->" + ex.Message, ex);
            }
        }

    }
}
