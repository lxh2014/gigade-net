/* 
 * 武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：IOrderPaymentHncbImplMgr 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作    者：zhuoqin0830w 
 * 完成日期：2015/05/13
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;

namespace BLL.gigade.Mgr.Impl
{
    public interface IOrderPaymentHncbImplMgr
    {
        // 華南賬戶（虛擬帳號）  add by zhuoqin0830w  2015/05/13
        string AddPaymentHncb(OrderPaymentHncb orderPayment);
    }
}