using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Admin.gigade.CustomError
{
    public class CustomHandleErrorAttribute : HandleErrorAttribute
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public override void OnException(ExceptionContext Context)
        {
            base.OnException(Context);
            Exception ex = Context.Exception;
            if (!Context.ExceptionHandled)
                return;
            Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
            logMessage.Content = string.Format("TargetSite:{0},Name:{1},Message:{2}", ex.TargetSite.ReflectedType.FullName, ex.TargetSite.Name, ex.Message);
            logMessage.MethodName = Context.HttpContext.Request.Path;
            log.Error(logMessage);
            if (Context.HttpContext.Request.IsAjaxRequest())
            {
                Context.HttpContext.Response.Clear();
                Context.HttpContext.Response.Write("");
                Context.HttpContext.Response.End();
                Context.HttpContext.Response.Flush();
                Context.HttpContext.Response.Close();
            }
            else
            {
                Context.HttpContext.Response.Redirect("/Error");
            }
        }
    }
}
