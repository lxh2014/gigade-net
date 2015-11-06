using BLL.gigade.Common;
using BLL.gigade.Model;
using DBAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao
{
    public class BrandLogoSortDao
    {
        private IDBAccess _access;
        public BrandLogoSortDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }
        /// <summary>
        /// 列表頁
        /// </summary>
        /// <param name="query"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<BrandLogoSort> GetBLSList(BrandLogoSort query, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlFrom = new StringBuilder();
            StringBuilder sqlWhere = new StringBuilder();
            StringBuilder sqlCount = new StringBuilder();
            totalCount = 0;
            try
            {
                sqlCount.Append("select count(blo.blo_id) as 'totalCount'   ");
                sql.Append(" select blo.blo_id,blo.brand_id,vb.brand_name, vb.brand_logo,  blo.blo_sort, pc.category_id, pc.category_name,mu.user_username,blo.blo_mdate  ");
                sqlFrom.Append(" from brand_logo_sort blo LEFT JOIN vendor_brand vb on blo.brand_id=vb.brand_id LEFT JOIN product_category pc on blo.category_id=pc.category_id LEFT JOIN manage_user mu on blo.blo_muser=mu.user_id  ");
                sqlWhere.AppendFormat(" where 1=1   ");
                if (query.category_id != 0 && query.brand_id != 0)
                {
                    sqlWhere.AppendFormat("and pc.category_id='{0}' and blo.brand_id='{1}'", query.category_id, query.brand_id);
                }
                else if (query.category_id != 0 && query.brand_id == 0)
                {
                    sqlWhere.AppendFormat("and pc.category_id='{0}'  ", query.category_id);
                }
                DataTable _dt = _access.getDataTable(sqlCount.ToString() + sqlFrom.ToString()+sqlWhere.ToString());
                if (_dt != null && _dt.Rows.Count > 0)
                {
                    totalCount = Convert.ToInt32(_dt.Rows[0][0]);
                }
                sqlWhere.AppendFormat(" order by  blo.blo_id desc limit {0},{1}; ", query.Start, query.Limit);
                return _access.getDataTableForObj<BrandLogoSort>(sql.ToString() + sqlFrom.ToString() + sqlWhere.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("BrandLogoSortDao-->GetBLSList-->" + sql.ToString() + sqlFrom.ToString() + sqlWhere.ToString() + ex.Message, ex);
            }
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public int InsertBLS(BrandLogoSort query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("insert into brand_logo_sort(category_id,brand_id,blo_sort,blo_kuser,blo_kdate,blo_muser,blo_mdate) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}');", query.category_id, query.brand_id, query.blo_sort, query.blo_kuser, CommonFunction.DateTimeToString(DateTime.Now), query.blo_muser, CommonFunction.DateTimeToString(DateTime.Now));
                return _access.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("BrandLogoSortDao-->InsertBLS-->" + sql.ToString() + ex.Message, ex);
            }
        }

        /// <summary>
        /// 編輯
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public int UpdateBLS(BrandLogoSort query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(" update brand_logo_sort  set category_id='{0}',brand_id='{1}',blo_sort='{2}', blo_muser='{3}',blo_mdate='{4}' where blo_id='{5}';  ", query.category_id, query.brand_id, query.blo_sort, query.blo_muser, CommonFunction.DateTimeToString(DateTime.Now), query.blo_id);
                return _access.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("BrandLogoSortDao-->UpdateBLS-->" + sql.ToString() + ex.Message, ex);
            }
        }

        /// <summary>
        /// 刪除
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public string DeleteBLS(BrandLogoSort query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("delete from brand_logo_sort where blo_id='{0}';", query.blo_id);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("BrandLogoSortDao-->DeleteBLS-->" + sql.ToString() + ex.Message, ex);
            }
        }

        /// <summary>
        /// 品牌store
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public DataTable BrandStore(BrandLogoSort query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("SELECT DISTINCT  p.brand_id, v.brand_name FROM product_category_brand AS p LEFT JOIN vendor_brand AS v ON p.brand_id = v.brand_id WHERE p.category_id IN (SELECT t_parametersrc.parameterCode FROM t_parametersrc  WHERE t_parametersrc.parameterType = 'BrandCategoryRelation' AND t_parametersrc.topValue = '{0}');", query.category_id);
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("BrandLogoSortDao-->BrandStore-->" + sql.ToString(), ex);
            }
        }

        /// <summary>
        /// 品牌館store
        /// </summary>
        /// <param name="category_id"></param>
        /// <returns></returns>
        public DataTable CategoryStore(uint category_id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(" select  category_id,category_name  from  product_category where category_father_id='{0}';", category_id);
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("BrandLogoSortDao-->CategoryStore-->" + sql.ToString(), ex);
            }
        }

        /// <summary>
        /// 是否有品牌館
        /// </summary>
        /// <returns></returns>
        public DataTable CategoryId()
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(" select category_id from product_category where category_father_id=(select  parameterCode from t_parametersrc where parameterType='brand_logo' and parameterName='new') and category_name=(select  parameterCode from t_parametersrc where parameterType='brand_logo' and parameterName='brand');");
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("BrandLogoSortDao-->CategoryId-->" + sql.ToString(), ex);
            }
        }

        /// <summary>
        /// 當前品牌館下有多少筆數據了
        /// </summary>
        /// <param name="category_id"></param>
        /// <returns></returns>
        public DataTable GetCountByCat(uint category_id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(" select  blo_id from  brand_logo_sort where category_id='{0}';", category_id);
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("BrandLogoSortDao-->GetCountByCat-->" + sql.ToString(), ex);
            }
        }

        /// <summary>
        /// 當前品牌館分類下最大的排序
        /// </summary>
        /// <param name="category_id"></param>
        /// <returns></returns>
        public DataTable MaxSort(uint category_id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(" select max(blo_sort) from  brand_logo_sort where category_id='{0}';", category_id);
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("BrandLogoSortDao-->GetCountByCat-->" + sql.ToString(), ex);
            }

        }


        /// <summary>
        /// 排序不能重複
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public DataTable NoExistSort(BrandLogoSort query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(" select  blo_id from  brand_logo_sort where  category_id='{0}' and  blo_sort='{1}'  ", query.category_id, query.blo_sort);
                if (query.blo_id != 0)
                {
                    sql.AppendFormat(" and  brand_id!='{0}'; ", query.brand_id);
                }
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("BrandLogoSortDao-->GetCountByCat-->" + sql.ToString(), ex);
            }

        }

        /// <summary>
        /// 數據不能重複
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public DataTable NoExistData(BrandLogoSort query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                if (query.old_brand_id != query.brand_id)
                {
                    sql.AppendFormat(" select  blo_id from  brand_logo_sort where  category_id='{0}' and brand_id='{1}';", query.category_id, query.brand_id);
                    return _access.getDataTable(sql.ToString());
                }
                else
                {
                    return new DataTable();
                }

            }
            catch (Exception ex)
            {
                throw new Exception("BrandLogoSortDao-->GetCountByCat-->" + sql.ToString(), ex);
            }

        }

        public int MaxLimit()
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                int n = 10;
                sql.Append("select  parameterCode from t_parametersrc where parameterType='brand_logo' and parameterName='limit';");
                DataTable _dt = _access.getDataTable(sql.ToString());
                if (_dt.Rows.Count > 0 && _dt != null)
                {
                    if (int.TryParse(_dt.Rows[0][0].ToString(), out n))
                    {
                        return Convert.ToInt32(_dt.Rows[0][0]);
                    }
                    else
                    {
                        return n;
                    }
                }
                else
                {
                    return n;
                }

            }
            catch (Exception ex)
            {
                throw new Exception("BrandLogoSortDao-->MaxLimit-->" + sql.ToString(), ex);
            }
        }

    }
}
