using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model;
using System.Data;
using DBAccess;

namespace BLL.gigade.Dao
{
    public class PayEasyDao : IPayEasyImplDao
    {
        private IDBAccess _accessMySql;

        public PayEasyDao(string connectionString)
        {
            _accessMySql = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }
        public List<PayEasyQuery> Query(PayEasyQuery query)
        {
            List<PayEasyQuery> list = new List<PayEasyQuery>();
            return list;
        }


        public DataTable QueryExcel(PayEasyQuery query)
        {
            StringBuilder sbSql = new StringBuilder();
            string sql = @"select 6727 AS '廠商流水號',
6037 AS '網站別流水號',
pim.channel_detail_id as '廠商原始商品碼',
pim.channel_detail_id AS 廠商康迅商品代碼,
pim.product_name as '商品名稱',
'0' as '成本價',
1 as '重量(公克)',
pim.product_price '現金價',
concat('http://www.gigade100.com/payeasy/product.php?mid=',pim.channel_detail_id) as '商品說明',
'1' as '體積 - 長(公分)','1' as '體積 - 寬(公分)','1' as '體積 - 高(公分)','0' as '付款期數','9999' as '庫存量',
date_format(date(now()),'%Y-%m-%d') as '上架日期',
date_format(date(date_add(now(), interval 10 year)),'%Y-%m-%d') as '下架日期',
concat('http://img.gigade100.com/product/280x280/',substr(product_image,1,2),'/',substr(product_image,3,2),'/',product_image) as '商品圖檔',
'' as '商品分類'
from product_category pc1,product_category pc2
inner join product_category_set pcs on pcs.category_id = pc2.category_id
left join product_item_map pim on pim.product_id=pcs.product_id
left join product p on p.product_id = pcs.product_id
where pc1.category_id=pc2.category_father_id  and (pc1.category_id={0}) and pim.channel_id={1} and pim.rid > {2}";

            //            string sql = @"select 
            //6727 AS '廠商流水號',
            //6037 AS '網站別流水號',
            //pim.channel_detail_id as '廠商原始商品碼',
            //pim.channel_detail_id AS 廠商康迅商品代碼,
            //pim.product_name as '商品名稱',
            //'0' as '成本價',
            //1 as '重量(公克)',
            //pim.product_price '現金價',
            //concat('http://www.gigade100.com/payeasy/product.php?mid=',pim.channel_detail_id) as '商品說明',
            //'1' as '體積 - 長(公分)','1' as '體積 - 寬(公分)','1' as '體積 - 高(公分)','0' as '付款期數','9999' as '庫存量',";

            //            sql += @"concat(" + "\"'\"" + ",date_format(date(now()),'%Y/%m/%d')) as '上架日期'," + "concat(" + "\"'\"" + ",date_format(date(date_add(now(), interval 10 year)),'%Y-%m-%d')) as '下架日期',";
            ////date_format(date(now()),'%Y/%m/%d') as '上架日期',
            ////date_format(date(date_add(now(), interval 10 year)),'%Y-%m-%d') as '下架日期',
            //            sql += @"concat('http://img.gigade100.com/product/',substr(product_image,1,2),'/',substr(product_image,3,4),product_image) as '商品圖檔',
            //'' as '商品分類'
            //from product_category pc1,product_category pc2
            //inner join product_category_set pcs on pcs.category_id = pc2.category_id
            //left join product_item_map pim on pim.product_id=pcs.product_id
            //left join product p on p.product_id = pcs.product_id
            //where pc1.category_id=pc2.category_father_id  and (pc1.category_id={0}) and pim.channel_id={1} and pim.rid > {2}
            //";

            sbSql.AppendFormat(sql, query.category_id, query.chnanel_id, query.rid);
            return _accessMySql.getDataTable(sbSql.ToString());
        }
    }
}
