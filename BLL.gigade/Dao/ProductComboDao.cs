using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Custom;
using DBAccess;
using BLL.gigade.Model;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Dao;
namespace BLL.gigade.Dao
{
    public class ProductComboDao : BLL.gigade.Dao.Impl.IProductComboImplDao
    {
        private IDBAccess _access;
        private IPriceMasterImplDao pmDao;
        public ProductComboDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
            pmDao = new PriceMasterDao(connectionString);
        }

        public List<ProductComboCustom> combQuery(ProductComboCustom query)
        {
            StringBuilder sql = new StringBuilder("select c.id,c.parent_id,child_id,product_name,p.sale_status,p.prod_sz,p.brand_id,s_must_buy,g_must_buy,pile_id,buy_limit from product p inner join product_combo c  on p.product_id = c.child_id where 1=1"); //add by 
            if (query.Parent_Id != 0)
            {
                sql.AppendFormat(" and c.parent_id = {0}", query.Parent_Id);
            }
            if (query.Pile_Id != 0)
            {
                sql.AppendFormat(" and c.pile_id = {0}", query.Pile_Id);
            }
            if (query.Child_Id!="0")//edit 2014/09/24
            {
                sql.AppendFormat(" and c.child_id = {0}", query.Child_Id);//XXL
            }   

            return _access.getDataTableForObj<ProductComboCustom>(sql.ToString());
        }


        public List<ProductComboCustom> combNoPriceQuery(ProductComboCustom query)
        {
            StringBuilder sql = new StringBuilder("select c.id,c.parent_id,c.child_id,p.product_name,p.prod_sz,s_must_buy,g_must_buy,pile_id,buy_limit");
            sql.Append(" from product p");
            sql.Append(" inner join product_combo c  on p.product_id = c.child_id");
            sql.Append(" where 1=1");
            if (query.Parent_Id != 0)
            {
                sql.AppendFormat(" and c.parent_id = {0}", query.Parent_Id);
            }
            if (query.Pile_Id != 0)
            {
                sql.AppendFormat(" and c.pile_id = {0}", query.Pile_Id);
            }
            if (query.Child_Id != "0")//edit 2014/09/24
            {
                sql.AppendFormat(" and c.child_id={0}", query.Child_Id);
            }

            return _access.getDataTableForObj<ProductComboCustom>(sql.ToString());
        }

        public List<ProductCombo> groupNumQuery(ProductCombo query)
        {
            string sql = string.Format("select pile_id,g_must_buy,buy_limit from product_combo where parent_id = {0} group by pile_id,g_must_buy", query.Parent_Id);
            return _access.getDataTableForObj<ProductCombo>(sql);
        }

        public List<ProductComboCustom> sameSpecQuery(ProductComboCustom query)
        {
            StringBuilder sql = new StringBuilder("select c.id,c.parent_id,c.child_id,m.product_name as Product_Name,p.product_name,p.prod_sz,s_must_buy,g_must_buy,pile_id,buy_limit,m.price_master_id,m.price as item_money,m.event_price as event_money");
            sql.Append(",m.cost as item_cost,m.event_cost from product p");
            sql.AppendFormat(" inner join product_combo c on p.product_id = c.child_id and c.parent_id = {0}",query.Parent_Id);
            sql.AppendFormat(" left join price_master m on c.child_id = m.child_id and c.parent_id = m.product_id and m.user_id = {0} and m.user_level = {1} and m.site_id = {2}", query.user_id, query.user_level, query.site_id);
            return _access.getDataTableForObj<ProductComboCustom>(sql.ToString());
        }


        public List<ProductComboCustom> getChildren(ProductComboCustom query)
        {
            StringBuilder sql = new StringBuilder("select c.id,c.parent_id,c.child_id,m.product_name,s_must_buy,g_must_buy,pile_id,buy_limit,m.price_master_id,m.price as item_money,m.event_price as event_money,m.cost as item_cost,m.event_cost");
            if (query.price_type == 1)
            {
                sql.Append(" from product_combo c");
                sql.AppendFormat(" left join price_master m on m.product_id = c.child_id where c.parent_id={0}", query.Parent_Id);
            }
            else
            { 
               sql.AppendFormat(" from product_combo c inner join product p on p.product_id = c.child_id and c.parent_id = {0}",query.Parent_Id);
               sql.AppendFormat(" left join price_master m on c.child_id = m.child_id and c.parent_id = m.product_id   and m.child_id<>{0}", query.Parent_Id);
            }
            return _access.getDataTableForObj<ProductComboCustom>(sql.ToString());
        }


        public List<MakePriceCustom> differentSpecQuery(ProductComboCustom query)
        {
            StringBuilder stb = new StringBuilder("select a.product_id as child_id,a.item_id,b.product_name,b.prod_sz,c.spec_name as spec_1,d.spec_name as spec_2,e.s_must_buy,e.g_must_buy,  ");
            stb.Append("e.parent_id,e.pile_id,g.item_money,g.item_cost,g.event_money,g.event_cost,g.item_price_id,f.price_master_id ");
            stb.Append(" from product_item a ");
            stb.Append("left join product b on a.product_id=b.product_id ");
            stb.Append("left join product_spec c on a.spec_id_1=c.spec_id ");
            stb.Append("left join product_spec d on a.spec_id_2=d.spec_id ");
            stb.AppendFormat("left join (select pile_id,s_must_buy,g_must_buy,child_id,parent_id from product_combo where parent_id={0}) e on a.product_id=e.child_id ", query.Parent_Id);
            stb.AppendFormat(" left join price_master f on f.product_id={0} and f.child_id<>{0} and a.product_id=f.child_id ", query.Parent_Id);
            stb.AppendFormat(" left join item_price g on g.price_master_id=f.price_master_id and a.item_id=g.item_id ");
            stb.AppendFormat("where a.product_id in (select child_id from product_combo where parent_id={0})", query.Parent_Id);
            stb.AppendFormat(" and f.site_id={0} and f.user_id={1} and f.user_level={2}",query.site_id,query.user_id,query.user_level);
            stb.AppendFormat(" and e.parent_id={0}", query.Parent_Id );
            return _access.getDataTableForObj<MakePriceCustom>(stb.ToString());

        }

        public List<MakePriceCustom> differentNoPriceSpecQuery(ProductComboCustom query)
        {
            StringBuilder stb = new StringBuilder("select a.product_id as child_id,a.item_id,b.product_name,b.prod_sz,c.spec_name as spec_1,d.spec_name as spec_2,e.s_must_buy,e.g_must_buy,  ");
            stb.Append("e.parent_id,e.pile_id ");
            stb.Append(" from product_item a ");
            stb.Append("left join product b on a.product_id=b.product_id ");
            stb.Append("left join product_spec c on a.spec_id_1=c.spec_id ");
            stb.Append("left join product_spec d on a.spec_id_2=d.spec_id ");
            stb.AppendFormat("left join (select pile_id,s_must_buy,g_must_buy,child_id,parent_id from product_combo where parent_id={0}) e on a.product_id=e.child_id ", query.Parent_Id);
            stb.AppendFormat("where a.product_id in (select child_id from product_combo where parent_id={0})", query.Parent_Id);
            stb.AppendFormat(" and e.parent_id={0}", query.Parent_Id );
            return _access.getDataTableForObj<MakePriceCustom>(stb.ToString()); 
        }

        public string Save(ProductCombo combo)
        {
            StringBuilder stb = new StringBuilder("insert into product_combo(`parent_id`,`child_id`,`s_must_buy`,`g_must_buy`,`pile_id`,`buy_limit`) values({0},");
            stb.AppendFormat("{0},{1},{2},{3},{4})", combo.Child_Id, combo.S_Must_Buy, combo.G_Must_Buy, combo.Pile_Id, combo.Buy_Limit);
            return stb.ToString();
        }

        public string Delete(int parent_Id)
        {
            StringBuilder strSql = new StringBuilder("set sql_safe_updates=0;delete from product_combo ");
            strSql.AppendFormat("where parent_id={0};set sql_safe_updates=1;", parent_Id);
            return strSql.ToString();
        }

        public List<ProductCombo> GetParentList(int product_id)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat(@"SELECT parent_id FROM product_combo WHERE child_id = {0}", product_id);
                return _access.getDataTableForObj<ProductCombo>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ProductComboDao-->GetParentList" + ex.Message,ex);
            }
        }
    }
}
