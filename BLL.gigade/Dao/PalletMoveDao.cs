using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using DBAccess;
using System.Data;
using BLL.gigade.Dao.Impl;
using MySql.Data.MySqlClient;
using System.Collections;
using BLL.gigade.Common;

namespace BLL.gigade.Dao
{
    public class PalletMoveDao : IPalletMoveImplDao
    {
        private IDBAccess _access;
        private string connStr;
        public PalletMoveDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
            this.connStr = connectionString;
        }

        #region 返回查询的料位+List<IinvdQuery> GetPalletList(IinvdQuery Iinvd)
        /// <summary>
        /// 通过商品编号和料位编号查询
        /// </summary>
        /// <param name="Iinvd">实体</param>
        /// <returns>因为商品编号和料位是固定的，只能查询出一条，应该返回一个实体，在次返回集合</returns>
        public List<IinvdQuery> GetPalletList(IinvdQuery Iinvd, string invposcat = "")
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendLine(@"select p.product_name,i.row_id,i.lic_plt_id,i.dc_id,i.whse_id,i.po_id,i.plas_id,i.prod_qty,i.rcpt_id,i.lot_no,i.hgt_used,");
                sb.AppendLine(@" i.create_user,i.create_dtim,i.change_user,i.change_dtim,i.cde_dt,i.ista_id as ista_id,i.receipt_dtim,i.stor_ti,i.stor_hi");
                sb.AppendLine(@" ,i.inv_pos_cat,i.qity_id,i.plas_loc_id,i.item_id,i.plas_prdd_id,i.made_date,pe.cde_dt_incr,pe.cde_dt_var,pe.pwy_dte_ctl,ioc.lsta_id as iloc_ista ,ioc.lcat_id,concat(IFNULL(ps1.spec_name,''),IFNULL(ps2.spec_name,'')) as prod_sz,vb.vendor_id from iinvd i ");//把料位锁先映射到库存锁上。主，副料位
                sb.AppendLine(@"  left join product_item pi on i.item_id=pi.item_id ");
                sb.AppendLine(@" left join product p on p.product_id=pi.product_id ");
                sb.AppendLine(@" LEFT JOIN product_spec ps1 ON pi.spec_id_1 = ps1.spec_id ");
                sb.AppendLine(@" LEFT JOIN product_spec ps2 ON pi.spec_id_2 = ps2.spec_id ");
                sb.AppendLine(@" left join iloc ioc on ioc.loc_id=i.plas_loc_id ");
                sb.AppendLine(@" left join vendor_brand vb on p.brand_id=vb.brand_id ");
                sb.AppendLine(@" left join product_ext pe on pe.item_id=i.item_id ");
                if (Iinvd.item_id != 0)
                {
                    sb.AppendFormat(@"WHERE 1=1 and i.item_id='{0}' ", Iinvd.item_id);
                }
                else
                {
                    sb.AppendLine(@"left JOIN iupc upc on i.item_id=upc.item_id");
                    sb.AppendFormat(@"WHERE 1=1 and upc.upc_id='{0}' ", Iinvd.upc_id);
                }
                sb.AppendFormat(@"and  (i.plas_loc_id='{0}' or ioc.hash_loc_id='{0}') ", Iinvd.plas_loc_id);

                if (!string.IsNullOrEmpty(invposcat))//如果无条件就查询所有。如果有条件就查询副料位的,主料位 S，副料位R
                {
                    sb.AppendFormat(" and ioc.lcat_id='{0}' ", invposcat);
                }
                if (!string.IsNullOrEmpty(Iinvd.ista_id))//如果无条件就查询所有。如果有条件就查询副料位的,主料位 S，副料位R
                {
                    sb.AppendFormat(" and i.ista_id='{0}' ", Iinvd.ista_id);
                }
                sb.AppendFormat(" order by cde_dt");

                return _access.getDataTableForObj<IinvdQuery>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PalletMoveDao-->GetPalletList-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion
        #region Ajax请求更新时间
        /// <summary>
        /// 捕获到主料位，及时更新时间
        /// </summary>
        /// <param name="Iinvd"></param>
        /// <returns></returns>
        public int UpPalletTime(IinvdQuery Iinvd)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat(@"update iinvd set cde_dt='{0}' where  iinvd.row_id='{1}';", Iinvd.cde_dt.ToString("yyyy-MM-dd"), Iinvd.row_id);
                return _access.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PalletMoveDao-->UpPalletTime-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion


        #region 修改商品料位
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Iinvd">通過實體查詢源料位的列表信息</param>
        /// <param name="newinvd">同夥參數查詢目標料位的列表信息</param>
        /// <param name="num">截取要轉移的數量</param>
        /// <param name="userId">新增-修改人</param>
        /// <returns></returns>
        public string UpPallet(IinvdQuery Iinvd, string newinvd, string num,int userId)
        {
            StringBuilder sb = new StringBuilder();
            MySqlCommand mySqlCmd = new MySqlCommand();
            MySqlConnection mySqlConn = new MySqlConnection(connStr);
            int result = 0;
            newinvd = newinvd.ToUpper();
            try
            {
                if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Closed)
                {
                    mySqlConn.Open();
                }
                mySqlCmd.Connection = mySqlConn;
                mySqlCmd.Transaction = mySqlConn.BeginTransaction();
                mySqlCmd.CommandType = System.Data.CommandType.Text;
                ///先通过实体查询副料位
                List<IinvdQuery> SIinvd = new List<IinvdQuery>();//副料位
                SIinvd = new PalletMoveDao(connStr).GetPalletList(Iinvd);
             
                sb.Clear();
                #region 再获取修改数量的数组

                int[] sort = new int[SIinvd.Count];
                if (string.IsNullOrEmpty(num))//免得再次加載時直接點轉移
                {
                    return "2";
                }
                // string arr = "Request.Params[]";//获取的row_id/number,row_id/number
                string[] nums = num.Split(',');//拆分row_id/number
                for (int m = 0; m < SIinvd.Count; m++)
                {
                    sort[m] = 0;
                    for (int n = 0; n < nums.Length; n++)
                    {
                        string[] xiu = nums[n].Split('/');
                        string id = xiu[0];//row_Id
                        string shu = xiu[1];//number
                        if (SIinvd[m].row_id == int.Parse(id))
                        {
                            sort[m] = int.Parse(xiu[1]);
                        }
                    }
                }
                #endregion

                List<IinvdQuery> PIinvd = new List<IinvdQuery>();//目的料位

                #region 测试
                mySqlCmd.CommandText = string.Format("select lsta_id  from iloc where loc_id='{0}';", newinvd);
                object serchlsta = mySqlCmd.ExecuteScalar();//目标料位--锁定状态
                sb.Clear();

                mySqlCmd.CommandText = string.Format("select lcat_id  from iloc where loc_id='{0}';", newinvd);
                object serchlcat = mySqlCmd.ExecuteScalar();//目标料位--主副料位
                sb.Clear();

                if (string.IsNullOrEmpty(newinvd) || serchlsta == null || serchlcat==null)//查看目标料位是否存在
                    return "7";//目标料位不存在
                Iinvd.plas_loc_id = newinvd;
                PIinvd = new PalletMoveDao(connStr).GetPalletList(Iinvd);
              
                if (SIinvd[0].plas_loc_id == newinvd)//原料位和目的料位相同
                {
                    return "5";//源料位和副料位相同
                }
                if (SIinvd[0].iloc_ista == "H")//如果源料位的料位锁住，不可补货到目标料位
                {
                    return "6";//源料位被锁
                }
              
                if ( serchlsta.ToString() == "H")//如果目标料锁住，不可收货
                {
                    return "8";//目标料位被锁
                }
                mySqlCmd.CommandText = string.Format("select count(*)from iinvd INNER JOIN iloc on iinvd.plas_loc_id=iloc.loc_id where iinvd.plas_loc_id='{0}';", newinvd);//目标料位是否被释放
                int IsEmpty = int.Parse(mySqlCmd.ExecuteScalar().ToString());
                sb.Clear();
                uint itemId = 0;
                if ( serchlcat.ToString().Equals("S") && serchlsta.ToString() == "A" )//如果目標料位是主料位
                {
                    mySqlCmd.CommandText = string.Format("select iplas.item_id from iloc left join  iplas on iloc.loc_id=iplas.loc_id where iloc.loc_id='{0}' ;", newinvd);//如果目标料位是主料位，查询主料位所指定的商品
                    itemId = uint.Parse(mySqlCmd.ExecuteScalar().ToString());
                    sb.Clear();
                }
                if (serchlcat.ToString().Equals("S") && SIinvd[0].item_id != itemId)//目标主料位，非商品主料位
                {
                    return "4";//目标料位非商品主料位
                }
                //到副料位《被释放的》---//目标料位本来就有商品--//到主料位《是这个商品的主料位》
                if ( serchlcat.ToString().Equals("R") && IsEmpty <= 0 || PIinvd.Count > 0 || itemId == SIinvd[0].item_id)
                {
                    for (int m = 0; m < sort.Length; m++)//先循环副料位表，
                    {
                        int a = SIinvd[m].cde_dt.CompareTo(DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd")));
                        if (sort[m] == 0)//如果数据为零，不修改
                            continue;
                        bool blag = true;

                        #region 循環目的料位，找到時間相同 的就進行修改

                        for (int n = 0; n < PIinvd.Count; n++)//循环目的料位进行比较
                        {
                            if (SIinvd[m].cde_dt.ToString("yyyy-MM-dd") == PIinvd[n].cde_dt.ToString("yyyy-MM-dd"))//时间相同 修改
                            {
                                if (SIinvd[m].prod_qty < sort[m])//判断转移的数量是否符合
                                {
                                    return "0";
                                }
                                blag = false;
                                SIinvd[m].prod_qty = SIinvd[m].prod_qty - sort[m];
                                PIinvd[n].prod_qty = PIinvd[n].prod_qty + sort[m];
                                //change_dtim,create_user,change_user
                                sb.AppendFormat("update iinvd set prod_qty='{0}' ,change_dtim='{1}', change_user='{2}' ", SIinvd[m].prod_qty, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), userId);//修改原料位数量 减
                                sb.AppendFormat("  where  iinvd.row_id='{0}';", SIinvd[m].row_id);
                                mySqlCmd.CommandText = sb.ToString();
                                result += mySqlCmd.ExecuteNonQuery();
                                sb.Clear();
                                sb.AppendFormat("update iinvd set prod_qty='{0}' ,change_dtim='{1}', change_user='{2}' ", PIinvd[n].prod_qty, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), userId);//修改目标料位 加 
                                if (SIinvd[m].ista_id == "H")//如果源料位上锁，修改到目标料位时，加上上锁原因。
                                {
                                    sb.AppendFormat(" ,ista_id='{0}',qity_id='{1}' ", SIinvd[m].ista_id, SIinvd[m].qity_id);
                                }
                                sb.AppendFormat(" where  iinvd.row_id='{0}';", PIinvd[n].row_id);
                                mySqlCmd.CommandText = sb.ToString();
                                result += mySqlCmd.ExecuteNonQuery();
                                sb.Clear();
                                break;//如果目标料位两个时间相同，那就捕货到第一个符合的料位,到第二个跳出此次循环
                            }
                        }
                        #endregion

                        #region 目的料位找不到的進行新增
                        if (blag)//时间不同的话，新增至目标料位
                        {
                            if (SIinvd[m].prod_qty < sort[m])//判断转移的数量是否符合
                            {
                                return "0";
                            }
                            uint id = SIinvd[m].item_id;
                            SIinvd[m].prod_qty = SIinvd[m].prod_qty - sort[m];

                            sb.Append("set sql_safe_updates = 0;");
                            sb.AppendFormat("update iinvd set prod_qty='{0}' ,change_dtim='{1}' ,change_user='{2}' ", SIinvd[m].prod_qty, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), userId);//修改原料位数量 减
                            sb.AppendFormat("  where  row_id='{0}';", SIinvd[m].row_id);
                            sb.Append("set sql_safe_updates = 1;");
                            mySqlCmd.CommandText = sb.ToString();
                            result += mySqlCmd.ExecuteNonQuery();
                            sb.Clear();
                            sb.AppendLine(@"insert into iinvd (lic_plt_id,dc_id,whse_id,po_id,plas_id,prod_qty,");
                            sb.AppendLine(@"rcpt_id,lot_no,hgt_used,create_user,create_dtim,");
                            sb.AppendLine(@"change_user,change_dtim,cde_dt,ista_id,receipt_dtim,");
                            sb.AppendLine(@"stor_ti,stor_hi,inv_pos_cat,qity_id,");
                            sb.AppendLine(@"plas_loc_id,item_id,plas_prdd_id,made_date) VALUES (");
                            sb.AppendFormat(@"'{0}','{1}','{2}','{3}','{4}','{5}',", SIinvd[m].lic_plt_id, SIinvd[m].dc_id, SIinvd[m].whse_id, SIinvd[m].po_id,SIinvd[m].plas_id, sort[m]);
                            sb.AppendFormat(@"'{0}','{1}','{2}','{3}','{4}',", SIinvd[m].rcpt_id, SIinvd[m].lot_no, SIinvd[m].hgt_used, userId, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                            sb.AppendFormat(@"'{0}','{1}','{2}','{3}','{4}',", userId,DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), SIinvd[m].cde_dt.ToString("yyyy-MM-dd"), SIinvd[m].ista_id, SIinvd[m].receipt_dtim.ToString("yyyy-MM-dd HH:mm:ss")); 
                            sb.AppendFormat(@"'{0}','{1}','{2}','{3}',", SIinvd[m].stor_ti, SIinvd[m].stor_hi, SIinvd[m].inv_pos_cat, SIinvd[m].qity_id);
                            sb.AppendFormat(@"'{0}','{1}','{2}','{3}');", newinvd, SIinvd[m].item_id, SIinvd[m].plas_prdd_id,SIinvd[m].made_date.ToString("yyyy-MM-dd"));
                            mySqlCmd.CommandText = sb.ToString();
                            result += mySqlCmd.ExecuteNonQuery();
                            sb.Clear();
                        }
                        #endregion
                    }
                    if (PIinvd.Count == 0 && serchlcat.Equals("R"))//这个副料位没有被使用过,,主料位不需要更改
                    {
                        sb.Append("set sql_safe_updates = 0;");
                        sb.AppendFormat("update iloc set lsta_id='A' where loc_id='{0}';", newinvd);//释放过的副料位，增加别的商品，要把状态改为已启用
                        sb.Append("set sql_safe_updates = 1;");
                        mySqlCmd.CommandText = sb.ToString();
                        result += mySqlCmd.ExecuteNonQuery();
                        sb.Clear();
                    }
                    sb.Append("set sql_safe_updates = 0;");
                    sb.AppendFormat("DELETE from iinvd where  prod_qty='{0}' and plas_loc_id='{1}';", 0, SIinvd[0].plas_loc_id.ToString().ToUpper());//刪除源料位 库存为零的信息
                    sb.Append("set sql_safe_updates = 1;");
                    mySqlCmd.CommandText = sb.ToString();
                    result += mySqlCmd.ExecuteNonQuery();
                    sb.Clear();
                    sb.AppendFormat("select count(*)from iinvd where  plas_loc_id='{0}' ;", SIinvd[0].plas_loc_id.ToUpper());
                    mySqlCmd.CommandText = sb.ToString();
                    if (int.Parse(mySqlCmd.ExecuteScalar().ToString()) == 0 && SIinvd[0].lcat_id == "R")//副料位的库存都空了，修改料位已占用为可以使用。F 可以使用，H锁住，A已占用
                    {
                        sb.Append("set sql_safe_updates = 0;");
                        sb.AppendFormat("update iloc set lsta_id='F' where loc_id='{0}';", SIinvd[0].plas_loc_id);// 库存为零的,释放源料位
                        sb.Append("set sql_safe_updates = 1;");
                        mySqlCmd.CommandText = sb.ToString();
                        result += mySqlCmd.ExecuteNonQuery();
                        sb.Clear();
                    }
                }
                else
                {
                    return "1";
                }
                if (result == 0)//代表没有输入数量，所有sql都不会执行
                    return "2";

                mySqlCmd.Transaction.Commit();
                return "3";

                #endregion
            }
            catch (Exception ex)
            {
                mySqlCmd.Transaction.Rollback();
                throw new Exception("PalletMoveDao-->UpPallet-->" + ex.Message + sb.ToString(), ex);
            }
            finally
            {
                if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Open)
                {
                    mySqlConn.Close();
                }
            }
        }
        #endregion
        /// <summary>
        /// 根據商品編號或條碼來獲取商品名稱 並且判斷是否存在主料位 如果不存在用寄倉YY999999或者ZZ999999
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public DataTable GetProdInfo(string pid)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine(@"select pi.item_id,pi.product_id,vb.vendor_id,CONCAT(vb.brand_name,'-',p.product_name) as product_name,p.product_mode,ips.loc_id from product_item pi ");
            sql.AppendLine(@" LEFT JOIN product p on pi.product_id=p.product_id ");
            sql.AppendLine(@" left join vendor_brand vb on p.brand_id=vb.brand_id ");
            sql.AppendFormat(@" LEFT JOIN iplas ips on ips.item_id=pi.item_id  ");
            if (pid.Length == 6)
            {
                sql.AppendFormat(@" WHERE pi.item_id='{0}'", pid);
            }
            else
            {
               sql.AppendFormat(@" LEFT JOIN  iupc i on pi.item_id=i.item_id where i.upc_id='{0}'", pid);
            }
            try
            {
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PalletMoveDao-->GetProdInfo-->" + ex.Message + sql.ToString(), ex);
            }

        }


        public int updatemadedate(IinvdQuery Iinvd)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat(" update iinvd set made_date='{0}',cde_dt='{1}',change_user='{2}',change_dtim='{3}' where row_id='{4}' ", Common.CommonFunction.DateTimeToString(Iinvd.made_date),Common.CommonFunction.DateTimeToString(Iinvd.cde_dt),Iinvd.change_user,Common.CommonFunction.DateTimeToString(DateTime.Now), Iinvd.row_id);
            try
            {
                return _access.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PalletMoveDao-->updatemadedate-->" + ex.Message + sql.ToString(), ex);
            }
        }


        public List<ProductExt> selectproductexttime(string item_id)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat(@"select * from product_ext where item_id='{0}' and pwy_dte_ctl='Y' ", item_id);
            try
            {
                return _access.getDataTableForObj<ProductExt>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PalletMoveDao-->selectproductexttime-->" + ex.Message + sql.ToString(), ex);
            }

        }

        public int UpdateordeleteIinvd(IinvdQuery invd, IinvdQuery newinvd)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("  update iinvd set change_user='{0}',change_dtim='{1}',prod_qty='{2}' where row_id='{3}'; ", newinvd.change_user, CommonFunction.DateTimeToString(newinvd.change_dtim), newinvd.prod_qty, newinvd.row_id);
            sb.AppendFormat(" delete from iinvd where row_id='{0}'; ", invd.row_id);
            int result = 0;

            MySqlCommand mySqlCmd = new MySqlCommand();
            MySqlConnection mySqlConn = new MySqlConnection(connStr); 
            try
            {
                if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Closed)
                {
                    mySqlConn.Open();
                }
                mySqlCmd.Connection = mySqlConn;
                mySqlCmd.Transaction = mySqlConn.BeginTransaction();
                mySqlCmd.CommandType = System.Data.CommandType.Text;

                mySqlCmd.CommandText = sb.ToString();
                result = mySqlCmd.ExecuteNonQuery();
                mySqlCmd.Transaction.Commit();
            }
            catch (Exception ex)
            {
                mySqlCmd.Transaction.Rollback();
                throw new Exception("IialgDao.UpdateordeleteIinvd-->" + ex.Message + sb.ToString(), ex);
            }
            finally
            {
                if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Open)
                {
                    mySqlConn.Close();
                }
            }
            return result;
        }


        public int selectcount(IinvdQuery invd)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat(@"select count(*) from iinvd where item_id='{0}' and plas_loc_id='{1}' and made_date='{2}' and cde_dt='{3}' and row_id <> '{4}' ", invd.item_id, invd.plas_loc_id, CommonFunction.DateTimeToString(invd.made_date), CommonFunction.DateTimeToString(invd.cde_dt),invd.row_id);
            try
            {
                return Convert.ToInt32(_access.getDataTable(sql.ToString()).Rows[0][0]);
            }
            catch (Exception ex)
            {
                throw new Exception("PalletMoveDao-->selectcount-->" + ex.Message + sql.ToString(), ex);
            }
        }

        public DataTable selectrow_id(IinvdQuery invd)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat(@"select row_id,prod_qty from iinvd where item_id='{0}' and plas_loc_id='{1}' and made_date='{2}' and cde_dt='{3}' ", invd.item_id, invd.plas_loc_id, CommonFunction.DateTimeToString(invd.made_date), CommonFunction.DateTimeToString(invd.cde_dt));
            try
            {
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PalletMoveDao-->selectrow_id-->" + ex.Message + sql.ToString(), ex);
            }
        }


        public DataTable GetProductMsgByLocId(string loc_id)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine(@"select pi.item_id,pi.product_id,CONCAT(vb.brand_name,'-',p.product_name) as product_name,p.product_mode,ips.loc_id from product_item pi ");
            sql.AppendLine(@" LEFT JOIN product p on pi.product_id=p.product_id ");
            sql.AppendLine(@" left join vendor_brand vb on p.brand_id=vb.brand_id ");
            sql.AppendFormat(@" LEFT JOIN iplas ips on ips.item_id=pi.item_id  ");
            if (!string.IsNullOrEmpty(loc_id))
            {
                sql.AppendFormat(@" WHERE ips.loc_id='{0}'",loc_id);
            }
            try
            {
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PalletMoveDao-->GetProductMsgByLocId-->" + ex.Message + sql.ToString(), ex);
            }
        }
    }
}
