using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBAccess;
using System.Data;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using BLL.gigade.Model.Custom;
using MySql.Data.MySqlClient;
using BLL.gigade.Common;
using System.Collections;
using BLL.gigade.Dao.Impl;
namespace BLL.gigade.Dao
{
    public class ProductItemMapDao : Impl.IProductItemMapImplDao
    {
        private IDBAccess _accessMySql;
        private string tempStr = string.Empty;
        private MySqlConnection Mycon;
        private string strConn;
        public ProductItemMapDao(string connectionString)
        {
            _accessMySql = DBFactory.getDBAccess(DBType.MySql, connectionString);
            Mycon = new MySqlConnection(connectionString);
            this.strConn = connectionString;
        }

        public List<ProductItemMap> QueryAll(ProductItemMap p)
        {
            p.Replace4MySQL();
            tempStr = string.Format(@"SELECT  rid,channel_id,channel_detail_id,item_id,product_name,product_cost,product_price,product_id,group_item_id,price_master_id  FROM product_item_map WHERE channel_id = {0} AND channel_detail_id = '{1}'", p.channel_id, p.channel_detail_id);
            return _accessMySql.getDataTableForObj<ProductItemMap>(tempStr);
        }
        //根據外站id和賣場商品ID查找是否存在信息
        public List<ProductItemMap> QueryAll(uint channelId, string condition)
        {
            //p.Replace4MySQL();
            tempStr = string.Format(@"select  rid,channel_id,channel_detail_id,item_id,product_name,product_cost,product_price,product_id,group_item_id,price_master_id  from product_item_map where channel_id = {0} and channel_detail_id in {1}", channelId, condition);
            return _accessMySql.getDataTableForObj<ProductItemMap>(tempStr);
        }
        //add by hufeng 2014/05/21 爲了在訂單匯入的時候判斷庫存
        public List<ProductItemMap> QueryChannel_detail_id(string id)
        {
            tempStr = string.Format(@"select channel_detail_id from product_item_map where item_id = {0} or group_item_id like '%{0}%'", id);
            return _accessMySql.getDataTableForObj<ProductItemMap>(tempStr);
        }
        public List<ProductItemMapCustom> QueryProductItemMap(ProductItemMapQuery p, out int totalCount)
        {
            p.Replace4MySQL();
            if (!string.IsNullOrEmpty(p.content))
            {
                switch (p.condition)
                {
                    case ProductItemMapQuery.conditionNo.product_id:
                        tempStr = string.Format("i.product_id like '%{0}%' or m.product_id like '%{0}%'", p.content);
                        break;
                    case ProductItemMapQuery.conditionNo.item_id:
                        tempStr = string.Format("m.item_id like '%{0}%' or m.group_item_id like '%{0}%'", p.content);
                        break;
                    case ProductItemMapQuery.conditionNo.user_id:
                        tempStr = string.Format("c.user_id like '%{0}%'", p.content);
                        break;
                    case ProductItemMapQuery.conditionNo.channel_name_full:
                        tempStr = string.Format("c.channel_name_full like '%{0}%'", p.content);
                        break;
                    case ProductItemMapQuery.conditionNo.product_name:
                        tempStr = string.Format("m.product_name like '%{0}%'", p.content);
                        break;
                    case ProductItemMapQuery.conditionNo.channel_detail_id:
                        tempStr = string.Format("m.channel_detail_id like '%{0}%'", p.content);
                        break;
                    case ProductItemMapQuery.conditionNo.channelid_detailid:
                        tempStr = string.Format("m.channel_detail_id = '{0}' and c.channel_id={1}", p.content, p.ChannelId);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                tempStr = "1=1";
            }

            StringBuilder stb = new StringBuilder();
            stb.Append("select m.rid,i.product_id,m.product_id as groupcombo_product_id,m.item_id,m.group_item_id,c.channel_name_full,m.product_name, m.channel_detail_id,m.product_cost,m.product_price");
            stb.Append(",s.site_name, p.user_level, p.user_level as user_level_name,u.user_email");
            stb.Append(" from product_item_map m left join product_item i on i.item_id = m.item_id inner join channel c on m.channel_id = c.channel_id ");
            //edit by xiangwang0413w 2014/07/10 增加三個欄位，站台、會員等級、會員email
            stb.Append(" left join price_master p on m.price_master_id=p.price_master_id left join site s on p.site_id=s.site_id");
            //left join t_parametersrc tp on p.user_level=tp.parameterCode and parameterType='UserLevel' 
            stb.Append(" left join users u on p.user_id=u.user_id");
            stb.Append(" where {0} limit {1},{2}");

            string _sql = string.Format(stb.ToString(), tempStr, p.Start, p.Limit);
            //m.rid,i.product_id,c.channel_name_full,m.item_id,m.product_name,m.channel_detail_id,m.product_cost,m.product_price
            totalCount = 0;
            DataTable dt = _accessMySql.getDataTable(string.Format("select count(m.rid) as total  from product_item_map m left join product_item i on i.item_id = m.item_id left join channel c on m.channel_id = c.channel_id  where {0} ", tempStr));
            if (dt != null && dt.Rows.Count > 0)
            {
                totalCount = Convert.ToInt32(dt.Rows[0]["total"]);
            }

            //edit by zhuoqin0830w  2015/05/18
            IParametersrcImplDao _parameterDao = new ParametersrcDao(strConn);
            List<Parametersrc> parameterList = _parameterDao.QueryParametersrcByTypes("UserLevel");
            List<ProductItemMapCustom> list = _accessMySql.getDataTableForObj<ProductItemMapCustom>(_sql);
            foreach (ProductItemMapCustom q in list)
            {
                var alist = parameterList.Find(m => m.ParameterType == "UserLevel" && m.ParameterCode == q.user_level.ToString());
                if (alist != null)
                {
                    q.user_level_name = alist.parameterName;
                }
            }

            return list;
            //return _accessMySql.getDataTableForObj<ProductItemMapCustom>(_sql);
        }

        public List<Model.Query.ProductCompare> QueryProductByNo(int cId, int pNo, int pmId, out int totalCount)
        {
            StringBuilder stb = new StringBuilder();

            stb.Append("select m.rid,p.product_id,m.channel_id,m.channel_detail_id,i.item_id, m.product_name,p.product_name as product_p_name,p.prod_sz,i.spec_id_1,i.spec_id_2,sp1.spec_name as spec_name_1,sp2.spec_name as spec_name_2,m.product_cost,m.product_price,s.cost,s.price");
            stb.Append(" from product p");
            stb.Append(" left join product_item i on p.product_id = i.product_id");
            //stb.Append(" left join price_master s on p.product_id=s.product_id and s.site_id=1 and s.user_level=1 and s.user_id=0");
            stb.AppendFormat(" left join price_master s on p.product_id=s.product_id and s.price_master_id={0}", pmId);//edit by xiangwang0413w 增加price_master_id
            stb.Append(" left join product_spec sp1 on sp1.spec_id = i.spec_id_1");
            stb.Append(" left join product_spec sp2 on sp2.spec_id = i.spec_id_2");
            stb.AppendFormat(" left join product_item_map m on i.item_id = m.item_id and m.channel_id = {0} and m.price_master_id={1}", cId, pmId);//edit by xiangwang0413w 增加price_master_id
            stb.AppendFormat(" where p.product_id = {0}", pNo);
            totalCount = 0;
            return _accessMySql.getDataTableForObj<Model.Query.ProductCompare>(stb.ToString());
        }


        //public int Save(ProductCompare p)
        //{
        //    tempStr = string.Format("insert into product_item_map (`channel_id`, `channel_detail_id`, `item_id`, `product_name`, `product_cost`, `product_price`) values ({0},'{1}',{2},'{3}',{4},{5});", p.channel_id, p.channel_detail_id, p.item_id, p.product_name, p.product_cost, p.product_price);
        //    return _accessMySql.execCommand(tempStr);
        //}

        /// <summary>
        /// 保存商品對照(單一)
        /// </summary>
        /// <param name="p">一條商品信息</param>
        /// <returns></returns>
        public int Save(ProductItemMap p)
        {
            p.Replace4MySQL();
            tempStr = string.Format("insert into product_item_map (`channel_id`, `channel_detail_id`, `item_id`, `product_name`, `product_cost`, `product_price`,`product_id`,`price_master_id`) values ({0},'{1}',{2},'{3}',{4},{5},{6},{7})", p.channel_id, p.channel_detail_id, p.item_id, p.product_name, p.product_cost, p.product_price, p.product_id, p.price_master_id);
            return _accessMySql.execCommand(tempStr);
        }

        public string saveString(ProductItemMap p)
        {
            p.Replace4MySQL();
            return string.Format("insert into product_item_map (`channel_id`, `channel_detail_id`, `item_id`, `product_name`, `product_cost`, `product_price`,`product_id`,`group_item_id`,`price_master_id`) values ({0},'{1}',{2},'{3}',{4},{5},{6},'{7}',{8});select @@identity;", p.channel_id, p.channel_detail_id, p.item_id, p.product_name, p.product_cost, p.product_price, p.product_id, p.group_item_id, p.price_master_id); //edit by xiangwang0413w 2014/07/20 增加站台價格
        }

        public void Save_Comb(ProductItemMapCustom p)
        {
            p.Replace4MySQL();
            MySqlCommand Mycmd = new MySqlCommand(string.Format("insert into product_item_map (`channel_id`, `channel_detail_id`, `group_item_id`, `product_name`, `product_cost`, `product_price`,product_id ,price_master_id) values ({0},'{1}','{2}','{3}',{4},{5},{6},{7});select @@identity;", p.channel_id, p.channel_detail_id, p.group_item_id, p.product_name, p.product_cost, p.product_price, p.product_id, p.price_master_id), Mycon);
            Mycon.Open();
            int rid = int.Parse(Mycmd.ExecuteScalar().ToString());
            Mycon.Close();

            if (p.MapChild.Count > 0)       //手動添加
            {
                for (int i = 0, j = p.MapChild.Count; i < j; i++)
                {
                    tempStr = string.Format("insert into product_map_set(map_rid,item_id,set_num)values({0},{1},{2})", rid, p.MapChild[i].item_id, p.MapChild[i].set_num);
                    _accessMySql.execCommand(tempStr);
                }
            }
            else        //匯入對照
            {
                for (int i = 0; i < p.group_item_id.Split(',').Length; i++)
                {
                    if (p.set_num == 0)
                    {
                        p.set_num = uint.Parse(_accessMySql.getDataTable(string.Format("select s_must_buy from product_combo where parent_id={0} and child_id =(select product_id from product_item where item_id={1})", p.product_id, p.group_item_id.Split(',')[i])).Rows[0][0].ToString());

                    }
                    tempStr = string.Format("insert into product_map_set(map_rid,item_id,set_num)values({0},{1},{2})", rid, uint.Parse(p.group_item_id.Split(',')[i]), p.set_num);
                    _accessMySql.execCommand(tempStr);
                }
            }

        }

        public bool comboSave(ArrayList delSqls, string itemMapSql, ArrayList mapSetSqls)
        {

            MySqlCommand mySqlCmd = new MySqlCommand();
            MySqlConnection mySqlCon = null;
            try
            {
                using (mySqlCon = new MySqlConnection(strConn))
                {
                    if (mySqlCon != null && mySqlCon.State == ConnectionState.Closed)
                    {
                        mySqlCon.Open();
                    }
                    mySqlCmd.Connection = mySqlCon;
                    mySqlCmd.Transaction = mySqlCon.BeginTransaction();
                    mySqlCmd.CommandType = System.Data.CommandType.Text;

                    //删除原有对照
                    if (delSqls != null && delSqls.Count > 0)
                    {
                        foreach (var item in delSqls)
                        {
                            mySqlCmd.CommandText = item.ToString();
                            mySqlCmd.ExecuteNonQuery();
                        }
                    }

                    //保存ProductItemMap并返回主键
                    mySqlCmd.CommandText = itemMapSql;
                    int rid = Convert.ToInt32(mySqlCmd.ExecuteScalar());

                    //保存ProductMapSet
                    if (mapSetSqls != null && mapSetSqls.Count > 0)
                    {
                        foreach (var item in mapSetSqls)
                        {
                            mySqlCmd.CommandText = string.Format(item.ToString(), rid);
                            mySqlCmd.ExecuteNonQuery();
                        }
                    }
                    mySqlCmd.Transaction.Commit();
                    return true;
                }
            }
            catch (Exception ex)
            {
                mySqlCmd.Transaction.Rollback();
                throw new Exception("ProductItemMapDao.comboSave(ArrayList delSqls, string itemMapSql, ArrayList mapSetSqls)-->" + ex.Message, ex);
            }
            finally
            {
                if (mySqlCon != null && mySqlCon.State == ConnectionState.Open)
                {
                    mySqlCon.Clone();
                }
            }
        }

        public int Delete(ProductItemMap p)
        {
            tempStr = string.Format("set sql_safe_updates = 0;delete from product_item_map where rid={0};set sql_safe_updates = 1;", p.rid);
            return _accessMySql.execCommand(tempStr);

        }

        public string deleteString(ProductItemMap p)
        {
            return string.Format("set sql_safe_updates = 0;delete from product_item_map where rid={0};set sql_safe_updates = 1;", p.rid);
        }

        public int Update(ProductCompare p)
        {
            p.Replace4MySQL();
            tempStr = string.Format("update product_item_map set channel_detail_id = '{0}',product_name='{1}',product_cost={2},product_price={3} where channel_id = {4} and item_id = {5} and rid={6} ", p.channel_detail_id, p.product_name, p.product_cost, p.product_price, p.channel_id, p.item_id, p.rid);
            return _accessMySql.execCommand(tempStr);
        }
        public int UpdatePIM(ProductItemMap p)
        {
            p.Replace4MySQL();
            tempStr = string.Format("update product_item_map set product_name='{1}',product_price={2} where channel_detail_id = '{0}' ", p.channel_detail_id, p.product_name, p.product_price);
            return _accessMySql.execCommand(tempStr);
        }
        /// <summary>
        /// 查詢商品對照是否存在(單一)
        /// </summary>
        /// <param name="p">一條商品信息</param>
        /// <returns></returns>
        public int Exist(ProductItemMapCustom p)
        {
            tempStr = string.Format("select count(rid) from product_item_map where channel_id={0} and item_id={1}", p.channel_id, p.item_id);
            if (p.channel_detail_id != ""&& p.channel_detail_id !=null)
            {
                tempStr += string.Format(" and channel_detail_id='{0}' ", p.channel_detail_id);
            }
            return int.Parse(_accessMySql.getDataTable(tempStr).Rows[0][0].ToString());
        }

        /// <summary>
        /// 查詢商品對照是否存在(組合)
        /// </summary>
        /// <param name="p">一條商品信息</param>
        /// <returns></returns>
        public int Comb_Exist(ProductItemMapCustom p)
        {
            p.Replace4MySQL();
            tempStr = string.Format("select count(rid) from product_item_map where channel_id={0} and  product_id={1} and group_item_id='{2}'", p.channel_id, p.product_id, p.group_item_id);
            if (p.channel_detail_id != "" && p.channel_detail_id != null)
            {
                tempStr += string.Format(" and channel_detail_id='{0}' ", p.channel_detail_id);
            }
            return int.Parse(_accessMySql.getDataTable(tempStr).Rows[0][0].ToString());
        }

        public int Comb_Compare(uint product_id, uint item_id)
        {
            tempStr = string.Format("select count(i.item_id) from product_combo c inner join product_item i on c.child_id = i.product_id where c.parent_id ={0} and i.item_id  ={1}", product_id, item_id);
            return int.Parse(_accessMySql.getDataTable(tempStr).Rows[0][0].ToString());
        }

        /// <summary>
        /// 根據itemmap表中的productid查找該商品是不是組合商品
        /// </summary>
        /// <param name="p">一條商品對照信息</param>
        /// <returns></returns>
        public List<ProductMapCustom> CombinationQuery(ProductItemMapCustom p)
        {
            StringBuilder sql = new StringBuilder("select p.combination,c.child_id,c.g_must_buy,c.pile_id,c.buy_limit,s.product_spec,p.product_name,p.prod_sz,m.cost,m.price,c.s_must_buy,p.price_type");
            sql.AppendFormat(" from product p left join product_combo c on p.product_id = c.parent_id");
            sql.AppendFormat(" left join product s on c.child_id = s.product_id");
            //sql.AppendFormat(" left join price_master m on p.product_id=m.product_id and m.site_id=1 and m.user_level=1 and m.user_id=0 and m.child_id=p.product_id");
            sql.AppendFormat(" left join price_master m on p.product_id=m.product_id and m.child_id=p.product_id and m.price_master_id={0}", p.price_master_id);
            //edit by xiangwang0413w 根據price_master_id來查詢價格
            sql.AppendFormat(" where p.product_id = {0} order by c.id", p.product_id);
            return _accessMySql.getDataTableForObj<ProductMapCustom>(sql.ToString());
        }

        public List<ProductComboMap> ProductComboItemQuery(int child_id, int parent_id)
        {
            StringBuilder sql = new StringBuilder("select i.item_id,p.product_name,s1.spec_name as spec_name_1,s2.spec_name as spec_name_2 ,c.s_must_buy as set_num,c.g_must_buy,c.pile_id");
            sql.Append(" from  product p inner join product_item i on p.product_id = i.product_id");
            sql.Append(" inner join product_combo c on c.child_id = p.product_id");
            sql.Append(" left join product_spec s1 on i.spec_id_1 = s1.spec_id left join product_spec s2 on i.spec_id_2 = s2.spec_id");
            sql.AppendFormat(" where p.product_id = {0} and c.parent_id = {1}", child_id, parent_id);
            return _accessMySql.getDataTableForObj<ProductComboMap>(sql.ToString());
        }

        /// <summary>
        /// 查詢固定組合商品的總信息
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public List<ProductItemMap> QueryProductItemMap(ProductItemMap p)
        {
            p.Replace4MySQL();
            tempStr = string.Format("select  rid,channel_id,channel_detail_id,item_id,product_name,product_cost,product_price,product_id,group_item_id,price_master_id  from product_item_map where channel_id={0} and product_id={1} and group_item_id='{2}' and price_master_id={3}", p.channel_id, p.product_id, p.group_item_id, p.price_master_id);//edit by xiangwang0413w 20714/07/02
            return _accessMySql.getDataTableForObj<ProductItemMap>(tempStr.ToString());
        }



        public List<ProductComboMap> QueryItemId(uint product_id)
        {
            StringBuilder sql = new StringBuilder("select p.product_spec,i.item_id");
            sql.AppendFormat(" from product_combo c left join product p on p.product_id=c.child_id");
            sql.AppendFormat(" left join product_item i on i.product_id = c.child_id");
            sql.AppendFormat(" where c.parent_id = {0}", product_id);
            return _accessMySql.getDataTableForObj<ProductComboMap>(sql.ToString());
        }


        public List<ProductComboMap> ComboItemQuery(ProductCombo query)
        {
            StringBuilder sql = new StringBuilder("select p.product_name,p.prod_sz,i.item_id from product_combo c inner join product p on c.child_id = p.product_id inner join product_item i on p.product_id = i.product_id");
            sql.AppendFormat(" where parent_id = {0}", query.Parent_Id);
            return _accessMySql.getDataTableForObj<ProductComboMap>(sql.ToString());
        }
        //查找建立過的對照信息
        public List<ProductItemMap> QueryOldItemMap(ProductItemMap Pip)
        {
            //edit by xiangwang0413w 2014/07/02 增加price_master_id條件查詢
            string sql = string.Format("select  rid,channel_id,channel_detail_id,item_id,product_name,product_cost,product_price,product_id,group_item_id,price_master_id  from product_item_map where product_id='{0}' and channel_id='{1}' and price_master_id={2}", Pip.product_id, Pip.channel_id, Pip.price_master_id);
            return _accessMySql.getDataTableForObj<ProductItemMap>(sql);
        }

        public DataTable ProductItemMapExc(ProductItemMap m)
        {
            tempStr = @"SELECT  channel_detail_id AS '外站商品編號',product_name AS '外站商品名稱',product_id AS '商品編號(5碼)',case item_id WHEN '0' THEN group_item_id ELSE item_id END AS '商品細項編號(6碼)','0' as '組合中之數量(0為照組合中之設定)',product_price AS '外站商品售價','1' as 'site_id','1' AS 'user_level','0' AS 'user_email' from product_item_map where 1=1 ";
            if (m.rid != 0)
            {
                tempStr += string.Format(" and rid >='{0}' ",m.rid);
            }
            if (m.channel_id != 0)
            {
                tempStr += string.Format(" and channel_id='{0}' ",m.channel_id);
            }
            return _accessMySql.getDataTable(tempStr);
        }
        public int Selrepeat(string cdid)
        {
            tempStr = string.Format("select count(rid) from product_item_map where channel_detail_id='{0}' ", cdid);
            return int.Parse(_accessMySql.getDataTable(tempStr).Rows[0][0].ToString());
        }
    }
}





