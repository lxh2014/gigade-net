/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：PageBase 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/8/1 16:06:19 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Web;

namespace BLL.gigade.Model
{
    [Serializable]
    public class PageBase
    {
        public bool IsPage { get; set; }
        public int Start { get; set; }
        public int Limit { get; set; }
        public float deduct_happygo_convert { set; get; }
        public PageBase()
        {
            IsPage = true;
            Start = 0;
            Limit = 25;
            deduct_happygo_convert = 0;
        }

        /// <summary>
        /// 將字符串'替換為'',以免拼接的sql語句被截斷
        /// </summary>
        public void Replace4MySQL()
        {
            Type entityType = this.GetType();
            foreach (PropertyInfo propertyInfo in entityType.GetProperties())
            {
                if (IsType(propertyInfo.PropertyType, "System.String"))
                {
                    object value = propertyInfo.GetValue(this, null);
                    string htmlendocevalue = string.Empty;
                    if (value != null)
                    {
                        htmlendocevalue = Common.CommonFunction.StringTransfer(value.ToString());
                    }
                    if (propertyInfo.GetSetMethod() != null)
                        propertyInfo.SetValue(this, htmlendocevalue, null);
                }
            }
        }

        /// <summary>
        /// 將對象進行Url編碼
        /// </summary>
        public void UrlEncode()
        {
            Type entityType = this.GetType();
            foreach (PropertyInfo propertyInfo in entityType.GetProperties())
            {
                if (IsType(propertyInfo.PropertyType, "System.String"))
                {
                    object value = propertyInfo.GetValue(this, null);
                    string htmlendocevalue = string.Empty;
                    if (value != null)
                    {
                        htmlendocevalue = HttpUtility.UrlEncode(value.ToString());
                    }
                    propertyInfo.SetValue(this, htmlendocevalue, null);
                }
            }
        }

        /// <summary>
        /// 檢查對象類型是否匹配
        /// </summary>
        /// <param name="type">該對象類型</param>
        /// <param name="typeName">需要匹配類型</param>
        /// <returns>bool</returns>
        public static bool IsType(Type type, string typeName)
        {
            if (type.ToString() == typeName)
                return true;
            if (type.ToString() == "System.Object")
                return false;
            return IsType(type.BaseType, typeName);
        }
    }
}
