using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using log4net.Layout.Pattern;
using System.IO;
using log4net.Core;
namespace Vendor.Log4NetCustom
{
    public class MyMessagePatternConverter : PatternLayoutConverter
    {
        protected override void Convert(System.IO.TextWriter writer, log4net.Core.LoggingEvent loggingEvent)
        {
            if (Option != null)
            {
                // Write the value for the specified key
                WriteObject(writer, loggingEvent.Repository, LookupProperty(Option, loggingEvent));
            }
            else
            {
                // Write all the key value pairs
                WriteDictionary(writer, loggingEvent.Repository, loggingEvent.GetProperties());
            }
            //if (Option != null)
            //{
            //    // Write the value for the specified key
            //    WriteObject(writer, loggingEvent.Repository, loggingEvent.LookupProperty(Option));
            //}
            //else
            //{
            //    // Write all the key value pairs
            //    WriteDictionary(writer, loggingEvent.Repository, loggingEvent.GetProperties());
            //}
        }
        /// <summary>
        /// 通过反射获取传入的日志对象的某个属性的值
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        private object LookupProperty(string property, log4net.Core.LoggingEvent loggingEvent)
        {
            object propertyValue = string.Empty;
            System.Reflection.PropertyInfo propertyInfo = loggingEvent.MessageObject.GetType().GetProperty(property);
            if (propertyInfo != null)
                propertyValue = propertyInfo.GetValue(loggingEvent.MessageObject, null);
            return propertyValue;
        }

    }
}