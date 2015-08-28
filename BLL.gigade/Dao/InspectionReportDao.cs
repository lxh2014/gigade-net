using BLL.gigade.Model.Query;
using DBAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Query;
using System.Data;
using BLL.gigade.Common;
using System.Collections;
using BLL.gigade.Model;
using MySql.Data.MySqlClient;

namespace BLL.gigade.Dao
{
    public class InspectionReportDao
    {
        private IDBAccess _access;
        private string connStr;
        public InspectionReportDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
            this.connStr = connectionString;
        }
        #region 證書類型列表
        public List<CertificateCategoryQuery> GetCertificateCategoryList(CertificateCategoryQuery query, out int totalCount)
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder sbwhere = new StringBuilder();
            StringBuilder sbcount = new StringBuilder();
            totalCount = 0;
            try
            {
                sb.AppendFormat(@" SELECT cc.rowID,cc.certificate_categoryname AS certificate_category_childname,cc.certificate_categorycode AS certificate_category_childcode,cc.certificate_categoryfid as frowID ,cc2.certificate_categoryname,cc2.certificate_categorycode,u.user_username as k_user_tostring,cc.k_date,cc.`status` ");
                sbwhere.Append(" FROM certificate_category cc ");
                sbwhere.AppendFormat(" LEFT JOIN (SELECT cc1.rowID,cc1.certificate_categoryname,cc1.certificate_categorycode,cc1.certificate_categoryfid FROM certificate_category cc1 WHERE cc1.certificate_categoryfid=0 ) cc2 ON cc2.rowID=cc.certificate_categoryfid ");
                sbwhere.Append(" LEFT JOIN manage_user u ON u.user_id=cc.k_user  ");
                sbwhere.AppendFormat(" where cc.certificate_categoryfid<>0 ");
                sbcount.Append(" select count(cc.rowID) as totalCount ");
                if (!string.IsNullOrEmpty(query.searchcon))
                {
                    sbwhere.AppendFormat(" and (cc.certificate_categoryname like N'%{0}%' OR cc.certificate_categorycode like N'%{0}%' or cc2.certificate_categoryname like N'%{0}%' OR cc2.certificate_categorycode like N'%{0}%') ",query.searchcon);
                }
                if (query.IsPage)
                {
                    sbcount.Append(sbwhere.ToString());
                    DataTable dt = new DataTable();
                    dt = _access.getDataTable(sbcount.ToString());
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(dt.Rows[0]["totalCount"]);
                    }
                }
                sbwhere.AppendFormat(" limit {0},{1} ", query.Start, query.Limit);
                sb.Append(sbwhere.ToString());
                return _access.getDataTableForObj<CertificateCategoryQuery>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("InspectionReportDao-->GetCertificateCategoryList-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion
        #region 更新證書類型狀態
        public int UpdateActive(CertificateCategoryQuery query)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat("UPDATE certificate_category SET `status`='{0}' WHERE rowID='{1}'", query.status, query.rowID);
                return _access.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("InspectionReportDao-->UpdateActive-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion
        #region 刪除前判斷是否是該大類對應的唯一小類
        public List<CertificateCategoryQuery> CheckOnly(CertificateCategoryQuery query)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat(" SELECT rowID FROM certificate_category WHERE certificate_categoryfid='{0}'", query.frowID);
                return _access.getDataTableForObj<CertificateCategoryQuery>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("InspectionReportDao-->CheckOnly-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion
        #region 證書類型刪除
        public int DeleteCertificateCategory(CertificateCategoryQuery query)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat("DELETE FROM certificate_category WHERE rowID in ({0})", query.rowIDs);
                return _access.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("InspectionReportDao-->DeleteCertificateCategory-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion
        #region 事務刪除檔大類對應的小類全部被刪除.同時也刪除大類
        public int DeleteCCByTransaction(CertificateCategoryQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(" DELETE FROM certificate_category WHERE rowID in ({0}); ", query.frowIDs);//刪除大類
                MySqlConnection conn = new MySqlConnection(connStr);//在这里CONN_STRING也就是连接Mysql数据库的语句
                conn.Open();//打开
                MySqlCommand command = conn.CreateCommand();//在这里也可以写成MySqlCommand command=new MySqlCommand();

                MySqlTransaction transaction = null;
                transaction = conn.BeginTransaction(); //这两句也可以写成  MySqlTransaction transaction =new MySqlTransaction();
                command.Connection = conn;
                command.Transaction = transaction;
                int count = 0;

                try
                {
                    command.CommandText = sql.ToString();
                    count = command.ExecuteNonQuery();
                    transaction.Commit(); //进行执行
                }
                catch
                {

                    transaction.Rollback();//发生异常进行回滚
                }

                return count;
            }
            catch (Exception ex)
            {
                throw new Exception("InspectionReportDao-->DeleteCCByTransaction-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion
        #region 新增證書類型大類store
        public List<CertificateCategoryQuery> GetStore(CertificateCategoryQuery query)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat("SELECT cc.certificate_categoryname,cc.rowID AS frowID,cc.certificate_categorycode FROM certificate_category cc WHERE cc.certificate_categoryfid='{0}';", query.rowID);
                return _access.getDataTableForObj<CertificateCategoryQuery>(sb.ToString());
            }
            catch (Exception ex)  
            {
                throw new Exception("InspectionReportDao-->GetStore-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion
        #region 根據id搜索證書信息
        public List<CertificateCategoryQuery> GetCertificateCategoryInfo(CertificateCategoryQuery query)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat("SELECT cc.certificate_categoryname,cc.certificate_categorycode,cc.rowID FROM certificate_category cc WHERE cc.rowID='{0}'",query.rowID);
                //if (query.frowID > 0)
                //{
                //    sb.AppendFormat(" and cc.certificate_categoryfid='{0}' ",query.frowID);
                //}
                return _access.getDataTableForObj<CertificateCategoryQuery>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("InspectionReportDao-->GetCertificateCategoryInfo-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion
        #region 判斷證書大類名稱是否重複
        public List<CertificateCategoryQuery> CheckCertificateCategoryName(CertificateCategoryQuery query)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat("SELECT cc.certificate_categoryname FROM certificate_category cc WHERE cc.certificate_categoryfid=0 AND cc.certificate_categoryname='{0}'", query.certificate_categoryname);
                return _access.getDataTableForObj<CertificateCategoryQuery>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("InspectionReportDao-->CheckCertificateCategoryName-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion
        #region 新增證書-大類返回新增id
        public DataTable GetNewCertificateCategoryId(CertificateCategoryQuery query)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat("INSERT INTO certificate_category (certificate_categorycode,certificate_categoryfid,certificate_categoryname,k_user,k_date,status) VALUES('{0}','{1}','{2}','{3}','{4}','{5}');select @@identity", query.certificate_categorycode,query.frowID,query.certificate_categoryname,query.k_user,Common.CommonFunction.DateTimeToString(query.k_date),1);
                return _access.getDataTable(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("InspectionReportDao-->GetNewCertificateCategoryId-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion
        #region 證書大類更新
        public int UpdateCertificateCategory(CertificateCategoryQuery query)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat("UPDATE certificate_category SET certificate_categorycode='{0}' ", query.certificate_categorycode);
                if (!string.IsNullOrEmpty(query.certificate_categoryname))
                {
                    sb.AppendFormat(" ,certificate_categoryname='{0}' ", query.certificate_categoryname);
                }
                sb.AppendFormat(" WHERE rowID='{0}' ",query.frowID);
                return _access.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("InspectionReportDao-->UpdateCertificateCategory-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion
        #region 編輯保存
        public int Update(CertificateCategoryQuery query)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat("UPDATE certificate_category SET certificate_categorycode='{0}',certificate_categoryname='{1}',certificate_categoryfid='{2}' WHERE rowID='{3}'", query.certificate_category_childcode,query.certificate_category_childname,query.frowID,query.rowID);
                return _access.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("InspectionReportDao-->Update-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion
        #region 新增保存
        public int AddSave(CertificateCategoryQuery query)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat("INSERT INTO certificate_category (certificate_categoryname,certificate_categorycode,certificate_categoryfid,k_user,k_date,status) VALUES('{0}','{1}','{2}','{3}','{4}','{5}')", query.certificate_category_childname, query.certificate_category_childcode, query.frowID,query.k_user,Common.CommonFunction.DateTimeToString(query.k_date),1);
                return _access.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("InspectionReportDao-->AddSave-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion
        #region 證書小類名稱檢查
        public List<CertificateCategoryQuery> CheckChildName(CertificateCategoryQuery query)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat("SELECT cc.certificate_categoryname FROM certificate_category cc WHERE cc.certificate_categoryfid<>0 AND cc.certificate_categoryname='{0}'", query.certificate_category_childname);
                return _access.getDataTableForObj<CertificateCategoryQuery>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("InspectionReportDao-->CheckChildName-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion
        #region 非重複code驗證(大類小類通用,不包含二級聯動,只適用於新增/編輯)
        public List<CertificateCategoryQuery> CheckCode(CertificateCategoryQuery query)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.Append("SELECT cc.certificate_categoryname,cc.rowID FROM certificate_category cc WHERE  1=1");
                if (!string.IsNullOrEmpty(query.certificate_categorycode))
                {
                    sb.AppendFormat(" and cc.certificate_categorycode='{0}' ", query.certificate_categorycode);
                }
                else if (!string.IsNullOrEmpty(query.certificate_category_childcode))
                {
                    sb.AppendFormat(" and cc.certificate_categorycode='{0}' ", query.certificate_category_childcode);
                }
                return _access.getDataTableForObj<CertificateCategoryQuery>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("InspectionReportDao-->CheckCode-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion
        #region 根據商品id搜品牌id
        public ProductQuery GetBrandId(uint productid)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat("SELECT brand_id FROM product WHERE product_id='{0}'",productid);
                return _access.getSinggleObj<ProductQuery>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("InspectionReportDao-->GetBrandId-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion

        public List<InspectionReportQuery> InspectionReportList(InspectionReportQuery query, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlFrom = new StringBuilder();
            StringBuilder sqlWhere = new StringBuilder();
            StringBuilder sqlCount = new StringBuilder();
            query.Replace4MySQL();
            totalCount = 0;
            try
            {
                sql.Append(" select insr.rowID, p.brand_id ,vb.brand_name,p.product_id,p.product_name,insr.certificate_type1,cc1.certificate_categoryname as 'certificate_type1_name', insr.certificate_type2,cc2.certificate_categoryname as 'certificate_type2_name',insr.sort,   ");
                sql.Append(" insr.certificate_expdate,insr.certificate_desc,insr.certificate_filename,mu1.user_username as'create_user',insr.k_date,mu2.user_username as 'update_user' ,insr.m_date ");
                sqlFrom.Append(" from inspection_report insr  ");
                sqlFrom.Append(" LEFT JOIN product p on p.product_id=insr.product_id LEFT JOIN vendor_brand vb on vb.brand_id=p.brand_id  ");
                sqlFrom.Append(" LEFT JOIN manage_user mu1 on  insr.k_user  =mu1.user_id LEFT JOIN manage_user mu2 on insr.m_user=mu2.user_id  ");
                sqlFrom.Append(" LEFT JOIN certificate_category cc1 on cc1.rowID=insr.certificate_type1 LEFT JOIN certificate_category cc2 on cc2.rowID=insr.certificate_type2   ");
                sqlWhere.Append(" where 1=1 ");
                if (query.brand_id != 0)
                {
                    sqlWhere.AppendFormat(" and  p.brand_id='{0}'  ",query.brand_id);
                }
                if (query.name_code !="")
                {
                    sqlWhere.AppendFormat(" and ( p.product_id='{0}' or p.product_name  like N'%{0}%' )  ", query.name_code);
                }
                if (query.certificate_type1 != "")
                {
                    sqlWhere.AppendFormat(" and insr.certificate_type1='{0}'  ", query.certificate_type1);
                }
                if (query.certificate_type2 != "")
                {
                    sqlWhere.AppendFormat(" and insr.certificate_type2='{0}'  ", query.certificate_type2);
                }
                if (query.search_date != 0)
                {
                    sqlWhere.AppendFormat(" and  insr.certificate_expdate >='{0}' and  insr.certificate_expdate <='{1}'  ", CommonFunction.DateTimeToString(query.start_time), CommonFunction.DateTimeToString(query.end_time));
                }
                if (query.last_day != 0)
                {
                    sqlWhere.AppendFormat(" and  insr.certificate_expdate<='{0}' and  insr.certificate_expdate>='{1}' ", CommonFunction.DateTimeToString(DateTime.Now.AddDays(query.last_day)),DateTime.Now);
                }
            
                if (query.IsPage)
                {
                    sqlCount.Append(" select count( insr.rowID) as 'totalCount'  " + sqlFrom.ToString()+sqlWhere.ToString());
                    DataTable _dt = _access.getDataTable(sqlCount.ToString());
                    if (_dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0][0]);
                    }
                }
                sqlWhere.AppendFormat(" order by insr.rowID desc limit {0},{1};  ",query.Start,query.Limit);
                sql.Append(sqlFrom.ToString()+sqlWhere.ToString());
                return _access.getDataTableForObj<InspectionReportQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("InspectionReportDao-->InspectionReportList-->"+sql.ToString()+ex.Message,ex);
            }
        }

        public DataTable Export(InspectionReportQuery query)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlFrom = new StringBuilder();
            StringBuilder sqlWhere = new StringBuilder();
            StringBuilder sqlCount = new StringBuilder();
            query.Replace4MySQL();
            try
            {
                sql.Append(" select insr.rowID, p.brand_id ,vb.brand_name,p.product_id,p.product_name,insr.certificate_type1, cc1.certificate_categorycode as 'code1',cc1.certificate_categoryname as 'certificate_type1_name', insr.certificate_type2,cc2.certificate_categorycode as 'code2' ,cc2.certificate_categoryname as 'certificate_type2_name',   ");
                sql.Append(" insr.certificate_expdate,insr.certificate_desc,insr.certificate_filename,mu1.user_username as'create_user',insr.k_date,mu2.user_username as 'update_user' ,insr.m_date ");
                sqlFrom.Append(" from inspection_report insr  ");
                sqlFrom.Append(" LEFT JOIN product p on p.product_id=insr.product_id LEFT JOIN vendor_brand vb on vb.brand_id=p.brand_id  ");
                sqlFrom.Append(" LEFT JOIN manage_user mu1 on  insr.k_user  =mu1.user_id LEFT JOIN manage_user mu2 on insr.m_user=mu2.user_id  ");
                sqlFrom.Append(" LEFT JOIN certificate_category cc1 on cc1.rowID=insr.certificate_type1 LEFT JOIN certificate_category cc2 on cc2.rowID=insr.certificate_type2   ");
                sqlWhere.Append(" where 1=1 ");
                if (query.brand_id != 0)
                {
                    sqlWhere.AppendFormat(" and  p.brand_id='{0}'  ", query.brand_id);
                }
                if (query.name_code != "")
                {
                    sqlWhere.AppendFormat(" and ( p.product_id='{0}' or p.product_name  like N'%{0}%' )  ", query.name_code);
                }
                if (query.certificate_type1 != "")
                {
                    sqlWhere.AppendFormat(" and insr.certificate_type1='{0}'  ", query.certificate_type1);
                }
                if (query.certificate_type2 != "")
                {
                    sqlWhere.AppendFormat(" and insr.certificate_type2='{0}'  ", query.certificate_type2);
                }
                if (query.search_date != 0)
                {
                    sqlWhere.AppendFormat(" and  insr.certificate_expdate >='{0}' and  insr.certificate_expdate <='{1}'  ", CommonFunction.DateTimeToString(query.start_time), CommonFunction.DateTimeToString(query.end_time));
                }
                if (query.last_day != 0)
                {
                    sqlWhere.AppendFormat(" and  insr.certificate_expdate<='{0}' and  insr.certificate_expdate>='{1}' ", CommonFunction.DateTimeToString(DateTime.Now.AddDays(query.last_day)), DateTime.Now);
                }
                sqlWhere.AppendFormat(" order by insr.rowID desc; ");
                sql.Append(sqlFrom.ToString() + sqlWhere.ToString());
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("InspectionReportDao-->Export-->" + sql.ToString() + ex.Message, ex);
            }
        }

        public string InsertInspectionRe(InspectionReportQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                query.Replace4MySQL();

                sql.Append(" insert into inspection_report(product_id,certificate_type1,certificate_type2,certificate_expdate,certificate_desc,certificate_filename,k_user,k_date,m_user,m_date,sort)    ");
                sql.AppendFormat(" values('{0}','{1}','{2}','{3}','{4}',  ", query.product_id, query.certificate_type1, query.certificate_type2, CommonFunction.DateTimeToString(query.certificate_expdate), query.certificate_desc);
                sql.AppendFormat("'{0}','{1}','{2}','{3}','{4}','{5}');  ", query.certificate_filename, query.k_user,CommonFunction.DateTimeToString(query.k_date), query.m_user,CommonFunction.DateTimeToString(query.m_date),query.sort);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("InspectionReportDao-->InsertInspectionRe-->" + sql.ToString() + ex.Message, ex);
            }
        }

        public string UpInspectionRe(InspectionReportQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                query.Replace4MySQL();
                sql.AppendFormat(" update inspection_report set certificate_expdate='{0}',certificate_desc='{1}',certificate_filename='{2}',m_user='{3}',m_date='{4}',sort='{5}'  where rowID='{6}' ;  ", CommonFunction.DateTimeToString(query.certificate_expdate), query.certificate_desc, query.certificate_filename, query.m_user, CommonFunction.DateTimeToString(query.m_date), query.sort, query.rowID);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("InspectionReportDao-->InsertInspectionRe-->" + sql.ToString() + ex.Message, ex);
            }
        }

        public bool ExecSql(ArrayList arrayList)
        {
            try
            {
                MySqlDao myDao = new MySqlDao(connStr);
                return myDao.ExcuteSqls(arrayList);
            }
            catch (Exception ex)
            {
                throw new Exception("InspectionReportDao-->InsertInspectionRe-->" + arrayList + ex.Message, ex);
            }
        }

        public string DeleteInspectionRe(InspectionReportQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("delete from inspection_report where rowID='{0}';",query.rowID);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("InspectionReportDao-->DeleteInspectionRe-->" + sql.ToString() + ex.Message, ex);
            }
        }

        public string GetType1Folder(InspectionReportQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(" select   certificate_categorycode  from  certificate_category  where rowID='{0}';",query.certificate_type1);
                DataTable _dt=_access.getDataTable(sql.ToString());
                return _dt.Rows[0][0].ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("InspectionReportDao-->GetType1Folder-->" + sql.ToString() + ex.Message, ex);
            }
        }
        public string GetType2Folder(InspectionReportQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(" select   certificate_categorycode  from  certificate_category  where rowID='{0}';", query.certificate_type2);
                DataTable _dt = _access.getDataTable(sql.ToString());
                return _dt.Rows[0][0].ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("InspectionReportDao-->GetType2Folder-->" + sql.ToString() + ex.Message, ex);
            }
        }
        public InspectionReportQuery oldQuery(InspectionReportQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("select certificate_filename from inspection_report where rowID='{0}';",query.rowID);
                return _access.getSinggleObj<InspectionReportQuery>(sql.ToString());
            }
            catch (Exception ex)
            {

                throw new Exception("InspectionReportDao-->oldQuery-->" + sql.ToString() + ex.Message, ex);
            }
        }
        public DataTable IsExist(InspectionReportQuery query)
        {
            StringBuilder sql = new StringBuilder();
            DataTable _dt = new System.Data.DataTable();
            try
            {
                if (query.rowID == 0)
                {
                    sql.AppendFormat("select product_id from  inspection_report  where product_id='{0}'  and certificate_type1='{1}' and  certificate_type2='{2}';", query.product_id, query.certificate_type1, query.certificate_type2);
                  _dt=_access.getDataTable(sql.ToString());
                }
                else
                {
                    //sql.AppendFormat("select p.product_id from product p LEFT JOIN vendor_brand vb on vb.brand_id=p.brand_id  LEFT JOIN inspection_report insr on insr.product_id=p.product_id where p.product_id='{0}' and vb.brand_id='{1}' and insr.certificate_type1='{2}' and insr.certificate_type2='{3}' and insr.rowID !='{4}';", query.product_id, query.brand_id, query.certificate_type1, query.certificate_type2, query.rowID);
                }
                return _dt;
            }
            catch (Exception ex)
            {
                throw new Exception("InspectionReportDao-->IsExist-->"+sql.ToString()+ex.Message,ex);
            }
        }
        public DataTable GetBrandID(InspectionReportQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                if (query.product_id != 0)
                {
                    sql.AppendFormat(" select vb.brand_id,vb.brand_name from product p LEFT JOIN vendor_brand vb on vb.brand_id=p.brand_id where p.product_id='{0}';", query.product_id);
                }
                else  
                    //if(query.brand_name!="")
                {
                    sql.AppendFormat(" select vb.brand_id,vb.brand_name from  vendor_brand vb  where vb.brand_name='{0}' and brand_status='{1}'  ;", query.brand_name,query.brand_status);

                }
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("InspectionReportDao-->GetBrandID-->" + sql.ToString() + ex.Message, ex);
            }
        }
        public string GetType1(InspectionReportQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(" select certificate_type1 from inspection_report where rowID='{0}';",query.rowID);
                DataTable _dt = _access.getDataTable(sql.ToString());
                return _dt.Rows[0][0].ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("InspectionReportDao-->GetType1-->" + sql.ToString() + ex.Message, ex);
            }
        }
        public string GetType2(InspectionReportQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(" select certificate_type2 from inspection_report where rowID='{0}';", query.rowID);
                DataTable _dt = _access.getDataTable(sql.ToString());
                return _dt.Rows[0][0].ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("InspectionReportDao-->GetType2-->" + sql.ToString() + ex.Message, ex);
            }
        }
        #region  驗證用，隨時增刪
        public List<CertificateCategoryQuery> GetType1Store(CertificateCategoryQuery query)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.Append("SELECT cc.certificate_categoryname,cc.rowID AS frowID,cc.certificate_categorycode FROM certificate_category cc WHERE cc.certificate_categoryfid=0;");
                return _access.getDataTableForObj<CertificateCategoryQuery>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("InspectionReportDao-->GetType1Store-->" + ex.Message + sb.ToString(), ex);
            }
        }
        public List<CertificateCategoryQuery> GetType2Store(CertificateCategoryQuery query)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat("SELECT cc.certificate_categoryname,cc.rowID AS frowID,cc.certificate_categorycode FROM certificate_category cc WHERE cc.certificate_categoryfid='{0}';", query.rowID);
                return _access.getDataTableForObj<CertificateCategoryQuery>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("InspectionReportDao-->GetType1Store-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion
        #region 檢查報告匯入操作

        #region 判斷商品編號是否存在
        public List<ProductQuery> GetProductById(InspectionReportQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(" SELECT product_id FROM product WHERE product_id='{0}';", query.product_id);
                return _access.getDataTableForObj<ProductQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("InspectionReportDao-->GetProductById-->" + sql.ToString() + ex.Message, ex);
            }
        }
        #endregion
        #region 二級聯動,判斷大類下邊的小類是否存在
        public List<CertificateCategoryQuery> GetLsit(CertificateCategoryQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(" SELECT cc.rowID FROM certificate_category cc WHERE cc.certificate_categoryfid='{0}' AND cc.certificate_categorycode='{1}';", query.frowID,query.certificate_category_childcode);
                return _access.getDataTableForObj<CertificateCategoryQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("InspectionReportDao-->GetLsit-->" + sql.ToString() + ex.Message, ex);
            }
        }
        #endregion
        #region 匯入時判斷是否已存在
        public List<InspectionReportQuery> CheckInspectionReport(InspectionReportQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(" SELECT rowID FROM inspection_report WHERE product_id='{0}' AND certificate_type1='{1}' AND certificate_type2='{2}';", query.product_id,query.certificate_type1,query.certificate_type2);
                return _access.getDataTableForObj<InspectionReportQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("InspectionReportDao-->CheckInspectionReport-->" + sql.ToString() + ex.Message, ex);
            }
        }
        #endregion
        #region 插入數據
        public int InsertInspectionReport(InspectionReportQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append(" INSERT INTO inspection_report (product_id,certificate_type1,certificate_type2,certificate_expdate,certificate_desc,certificate_filename,k_user,k_date,m_user,m_date)");
                sql.AppendFormat(" values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}') ",query.product_id,query.certificate_type1,query.certificate_type2,Common.CommonFunction.DateTimeToString(query.certificate_expdate),query.certificate_desc,query.certificate_filename,query.k_user,Common.CommonFunction.DateTimeToString(query.k_date),query.m_user,Common.CommonFunction.DateTimeToString(query.m_date));
                return _access.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("InspectionReportDao-->InsertInspectionReport-->" + sql.ToString() + ex.Message, ex);
            }
        }
        #endregion
        #region 如果存在更新
        public int UpdateInspectionReport(InspectionReportQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append(" set sql_safe_updates=0;  ");
                sql.AppendFormat(" UPDATE inspection_report SET certificate_desc='{0}',certificate_expdate='{1}',certificate_filename='{2}',m_user='{3}',m_date='{4}'",query.certificate_desc,Common.CommonFunction.DateTimeToString(query.certificate_expdate),query.certificate_filename,query.m_user,Common.CommonFunction.DateTimeToString(query.m_date));
                sql.AppendFormat(" WHERE product_id='{0}' AND certificate_type1='{1}' AND certificate_type2='{2}' ;",query.product_id,query.certificate_type1,query.certificate_type2);
                sql.Append(" set sql_safe_updates=1;  ");
                return _access.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("InspectionReportDao-->UpdateInspectionReport-->" + sql.ToString() + ex.Message, ex);
            }
        }
        #endregion

        #endregion
        #region 如果大類小類已經被使用則不允許刪除
        public DataTable BeforeDelete(CertificateCategoryQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(" select rowID from inspection_report where certificate_type1='{0}' and certificate_type2='{1}';",query.frowID,query.rowID);
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("InspectionReportDao-->BeforeDelete-->" + sql.ToString() + ex.Message, ex);
            }
        }
        #endregion

        public int IsSortExist(InspectionReportQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(" select sort from inspection_report where product_id='{0}' and certificate_type1='{1}' and sort='{2}';", query.product_id, query.certificate_type1,query.sort);
                return _access.getDataTable(sql.ToString()).Rows.Count;
            }
            catch (Exception ex)
            {
                throw new Exception("InspectionReportDao-->IsSortExist-->" + sql.ToString() + ex.Message, ex);
            }

        }
    }
}
