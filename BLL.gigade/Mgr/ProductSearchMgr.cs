using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr
{
    public class ProductSearchMgr
    {
        string connectionstring = string.Empty;
        ProductSearchDao dao;
        public ProductSearchMgr(string _connectionstring)
        {
            this.connectionstring = _connectionstring;
            dao = new ProductSearchDao(connectionstring);
        }
        public ProductSearchMgr(string _connectionstring, string sphinxHost, string sphinxPort)
        {
            this.connectionstring = _connectionstring;
            dao = new ProductSearchDao(connectionstring,sphinxHost,sphinxPort);
        }
        public List<ProductSearchQuery> GetProductSearchList(ProductSearchQuery query, out int totalCount)
        {
            List<int> searchList = new List<int>();
            List<int> flagList=new List<int>();
            List<int> idList = new List<int>();
            List<string> keyList = new List<string>();
            try
            {
                if (!string.IsNullOrEmpty(query.searchKey))
                {
                    //根據關鍵字查詢出來的商品編號
                    searchList = dao.GetIdList(query.searchKey, query.MaxMatches);
                }
                if (query.flag == "1")
                {
                    keyList = dao.GetKeyList(query);
                    flagList = dao.GetFlagList(keyList, query.MaxMatches);

                    List<int> sphinxIdList = dao.GetSphinxExcludeIdList();

                    #region 處理出最後的productID列表
                    if (string.IsNullOrEmpty(query.searchKey))
                    {
                        idList = flagList;
                    }
                    else if (!string.IsNullOrEmpty(query.searchKey))
                    {
                        foreach (int item in searchList)
                        {
                            if (flagList.Contains(item))
                            {
                                idList.Add(item);
                            }
                        }
                    }
                    foreach (int sphinxId in sphinxIdList)
                    {
                        if (idList.Contains(sphinxId))
                        {
                            idList.Remove(sphinxId);
                        }
                    }
                    #endregion
                }
                
                else
                {
                    //如果選擇的是否
                    idList = dao.GetIdList(query.searchKey, query.MaxMatches);
                }
                
                //獲取最後的keylist
                if (!string.IsNullOrEmpty(query.searchKey))
                {
                    if (!keyList.Contains(query.searchKey))
                    {
                        keyList.Add(query.searchKey);
                    }
                }
                
                List<ProductSearchQuery> oldStore = dao.GetListDetail( query,idList,out totalCount);

                return dao.GetListResult(oldStore,keyList);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductSearchMgr->GetProductSearchList:" + ex.Message);
            }
        }

    }
}
