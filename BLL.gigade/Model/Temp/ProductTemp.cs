#region 文件信息
/* 
* Copyright (c) 2014，武漢聯綿信息技術有限公司鄭州分公司 
* All rights reserved. 
* 
* 文件名称：ProductTemp.cs      
* 摘 要：                                                                               
* 商品暫存表，存貯商品新增完成前的信息，和供應商審核前的信息
* 当前版本：v1.1                                                                 
* 作 者：   未知                                         
* 完成日期：未知
* 修改歷史：                                                                     
*         v1.1修改日期：2014/8/18
*         v1.1修改人員：shuangshuang0420j     
*         v1.1修改内容：新增Temp_status欄位
*/

#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class ProductTemp : Product
    {
        public new string Product_Id { get; set; }
        public int Writer_Id { get; set; }
        public int Combo_Type { get; set; }
        public int Temp_Status { get; set; }
        public int Rid { get; set; }
       

        public ProductTemp()
        {
            Product_Id = "0";
            Writer_Id = 0;
            Combo_Type = 0;
            Temp_Status = 11;//狀態，11-新增中，12-供應商新建
            Rid = 0;//product_temp的自增主鍵
        }
    }
}
