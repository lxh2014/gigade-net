using BLL.gigade.Dao;
using BLL.gigade.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr
{
   public class BrandLogoSortMgr
    {
       private BrandLogoSortDao _brandLogoSort;
       private MySqlDao _mysqlDao;
       public BrandLogoSortMgr(string connectionString)
       {
           _brandLogoSort = new BrandLogoSortDao(connectionString);
           _mysqlDao = new MySqlDao(connectionString);
       }
       /// <summary>
       /// 列表頁
       /// </summary>
       /// <param name="query"></param>
       /// <param name="totalCount"></param>
       /// <returns></returns>
       public List<BrandLogoSort> GetBLSList(BrandLogoSort query, out int totalCount)
       {
           try
           {
               return _brandLogoSort.GetBLSList(query, out totalCount);
           }
           catch (Exception ex)
           {
             throw new Exception("BrandLogoSortDao-->GetBLSList-->" +ex.Message,ex);
           }
       }

       /// <summary>
       /// 保存
       /// </summary>
       /// <param name="query"></param>
       /// <returns></returns>
       public string SaveBLS(BrandLogoSort query)
       {
           string json = string.Empty;
           try
           {
               if (query.blo_id == 0)//add
               {
                   //當前品牌館下有多少筆數據了
                   DataTable _dataCount = _brandLogoSort.GetCountByCat(Convert.ToInt32(query.category_id));
                   if (_dataCount.Rows.Count < 10)
                   {
                       //數據不能重複
                       DataTable _existData = _brandLogoSort.NoExistData(query);
                       if (_existData == null || _existData.Rows.Count == 0)
                       {
                           //排序不能重複
                           DataTable _existSort = _brandLogoSort.NoExistSort(query);
                           if (_existSort == null || _existSort.Rows.Count == 0)
                           {
                               if (_brandLogoSort.InsertBLS(query) > 0)
                               {
                                   json = "{success:'true',re:'true'}";
                               }
                               else
                               {
                                   json = "{success:'false'}";
                               }
                           }
                           else
                           {
                               json = "{success:'true',repeatSort:'true'}";
                           }
                       }
                       else
                       {
                           json = "{success:'true',repeatData:'true'}";
                       }
                   }
                   else
                   {
                       json = "{success:'true',maxCount:'true'}";
                   }
               }
               else//edit
               {

                   //编辑时不檢查當前品牌館有多少條數據了
                   //數據是否重複
                      DataTable _existData = _brandLogoSort.NoExistData(query);
                      if (_existData == null || _existData.Rows.Count == 0)
                      {
                          //排序是否重複
                          DataTable _existSort = _brandLogoSort.NoExistSort(query);
                          if (_existSort == null || _existSort.Rows.Count == 0)
                          {
                              if (_brandLogoSort.UpdateBLS(query) > 0)
                              {
                                  json = "{success:'true',re:'true'}";
                              }
                              else
                              {
                                  json = "{success:'false'}";
                              }
                          }
                          else
                          {
                              json = "{success:'true',repeatSort:'true'}";
                          }
                      }
                      else
                      {
                          json = "{success:'true',repeatData:'true'}";
                      }
               }
               return json;
           }
           catch (Exception ex)
           {
               throw new Exception("BrandLogoSortDao-->SaveBLS-->" + ex.Message, ex);
           }
       }


       /// <summary>
       /// 刪除
       /// </summary>
       /// <param name="list"></param>
       /// <returns></returns>
       public string DeleteBLS(List<BrandLogoSort> list)
       {
           string json = string.Empty;
           ArrayList arrList = new ArrayList();
           try
           {
               for (int i = 0; i < list.Count; i++)
               {
                   arrList.Add(_brandLogoSort.DeleteBLS(list[i]));
               }
               if (arrList.Count > 0)
               {
                   if (_mysqlDao.ExcuteSqlsThrowException(arrList))
                   {
                       json = "{success:true}";
                   }
                   else
                   {
                       json = "{success:false}";
                   }
               }
               else
               {
                   json = "{success:true}";
               }
               return json;
           }
               
           catch (Exception ex)
           {
               throw new Exception("BrandLogoSortDao-->DeleteBLS-->" + ex.Message, ex);
           }
       }

       public DataTable CategoryStore()
       {
           DataTable _categoryDt = new DataTable();
           try
           {
               DataTable _dt = _brandLogoSort.CategoryId();
               if (_dt != null && _dt.Rows.Count > 0)
               {
                   _categoryDt = _brandLogoSort.CategoryStore(Convert.ToInt32( _dt.Rows[0][0]));
               }
               return _categoryDt;
           }
           catch (Exception ex)
           {
               throw new Exception("BrandLogoSortDao-->CategoryStore-->" + ex.Message, ex);
           }
       }

       public DataTable BrandStore(BrandLogoSort query)
       {
           try
           {
               return _brandLogoSort.BrandStore(query);
           }
           catch (Exception  ex)
           {
               throw new Exception("BrandLogoSortDao-->BrandStore-->" + ex.Message, ex);
           }
       }

       public int MaxSort(BrandLogoSort query)
       {
           int sort = 0;
           try
           {
               DataTable _dt = _brandLogoSort.MaxSort(Convert.ToInt32(query.category_id));
               int n=0;
               if (int.TryParse(_dt.Rows[0][0].ToString(), out n))
               {
                   sort = Convert.ToInt32(_dt.Rows[0][0]) + 1;
               }
               else
               {
                   sort = 1;
               }
            
               return sort;
           }
           catch (Exception ex)
           {
               throw new Exception("BrandLogoSortDao-->MaxSort-->" + ex.Message, ex);
           }
       }





    }
}
