using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
public class ActionFilter : IHttpModule
{
    public ActionFilter()
    {

    }
    public void Init(HttpApplication context)
    {
        context.AcquireRequestState += new EventHandler(context_AcquireRequestState);
    }
    void context_AcquireRequestState(object sender, EventArgs e)
    {
        HttpApplication application = (HttpApplication)sender;
        HttpContext context = application.Context;

        //try
        //{
        //    if (context.Request.Cookies["lang"] != null)
        //    {
        //        System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.CreateSpecificCulture(context.Request.Cookies["lang"].Value.ToString());
        //        System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(context.Request.Cookies["lang"].Value.ToString());
        //    }
        //}
        //catch (Exception)
        //{ }
        if (context.Request.Url.LocalPath == "/" || context.Request.Url.LocalPath.ToLower().IndexOf("/login") != -1 || context.Request.Url.LocalPath.ToLower().IndexOf("/error") != -1
|| context.Request.Url.LocalPath.ToLower().IndexOf("/notification") != -1)
        {
            return;
        }

        if (context.Request.Url.LocalPath.IndexOf(".") == -1 && (context.Session == null || context.Session["vendor"] == null))
        {
            //AJAX请求
            if (context.Request.Headers["x-requested-with"] != null
                        && context.Request.Headers["x-requested-with"].Equals("XMLHttpRequest"))
            {
                context.Response.AddHeader("sessionstatus", "timeout");
            }
            else
            {//普通请求
                application.CompleteRequest();
                context.Response.Write("<script language='javascript'>" +
                              "alert('" + Resources.Login.NOTICE_LOGIN_OUT + "');" +
                              "window.top.location.href = \"http://\" + window.top.location.host;" +
                              "</script>");
                return;
            }
        }

        //if (context.Request.Url.LocalPath == "/" || context.Request.Url.LocalPath.ToLower().IndexOf("/Home/") != -1 || context.Request.Url.LocalPath.ToLower().IndexOf("/error") != -1
        //    || context.Request.Url.LocalPath.ToLower().IndexOf("/checkstockalarm") != -1 || context.Request.Url.LocalPath.ToLower().IndexOf("/productupdatenotice") != -1 || context.Request.Url.LocalPath.ToLower().IndexOf("/getxmlinfo") != -1
        //    || context.Request.Url.LocalPath.ToLower().IndexOf("/productitemupdatenotice") != -1)
        //{
        //    return;
        //}
        //if (context.Request.Url.LocalPath.IndexOf(".") == -1 && (context.Session == null || context.Session["vendor"] == null))
        //{
        //    application.CompleteRequest();
        //    context.Response.Write("<script language='javascript'>" +
        //                  "alert('" + "登錄過期"+ "');" +
        //                  "window.top.location.href = \"http://\" + window.top.location.host;" +
        //                  "</script>");
        //    return;
        //}
        if (context.Request.Url.LocalPath.IndexOf(".js") != -1 || context.Request.Url.LocalPath.IndexOf(".css") != -1)
        {
            context.Response.Cache.SetCacheability(HttpCacheability.Public);
            context.Response.Cache.SetExpires(DateTime.Now.AddHours(1.0));
        }
        if (context.Request.Url.LocalPath.IndexOf(".jpg") != -1 || context.Request.Url.LocalPath.IndexOf(".png") != -1 || context.Request.Url.LocalPath.IndexOf(".gif") != -1)
        {
            context.Response.Cache.SetCacheability(HttpCacheability.Public);
            context.Response.Cache.SetExpires(DateTime.Now.AddHours(5.0));
        }

    }
    public void Dispose()
    {

    }
}
