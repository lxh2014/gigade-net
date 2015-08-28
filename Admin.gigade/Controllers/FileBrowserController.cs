using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebDav.Client;

namespace Admin.gigade.Controllers
{
    public class FileBrowserController : Controller
    {
        protected string BassAddress { get { return T("webDavBaseAddress"); } }
        protected virtual string RootConfigName { get { return "webDavFile"; } }
        protected virtual string WebDavRoot { get { return BassAddress + "/" + T(RootConfigName); } }
        protected WebDavSession session = null;
        protected string folderPath;
        protected string resourcePath;
        public FileBrowserController()
        {
            session = WebDavSession.Create(BassAddress, T(RootConfigName), T("webDavUserName"), T("webDavPwd"));
        }
        protected string T(string c)
        {
            return ConfigurationManager.AppSettings[c];
        }
        public JsonResult Read(string path)
        {
            var folder = session.OpenFolder(WebDavRoot + "/" + path);
            var items = folder.GetChildren();
            long tempSize = 0;
            var result = items.Select(item =>
                new
                {
                    Name = item.DisplayName.Replace("/", ""),
                    EntryType = item.ItemType == ItemType.Resource ? 0 : 1,
                    Size = item.Properties.Count() > 2 ? long.TryParse(item.Properties[2].StringValue, out tempSize) ? tempSize : long.Parse(item.Properties[1].StringValue) : 0,
                }).ToList();
            return Json(result);
        }
        public ActionResult Create(string path, FileBrowserEntry entry)
        {
            if (entry.EntryType == FileBrowserEntryType.Directory)
            {
                BuildPath(path, out folderPath, out resourcePath);
                var folder = session.OpenFolder(folderPath + "/");
                var cFolder = folder.CreateFolder(entry.Name);
            }
            return Json(new { Name = entry.Name, Size = entry.Size, EntryType = (int)entry.EntryType });
        }
        public ActionResult Destroy(string path, FileBrowserEntry entry)
        {
            BuildPath(path, out folderPath, out resourcePath);
            if (entry.EntryType == FileBrowserEntryType.Directory)
            {
                var target = folderPath + "/" + entry.Name + "/";
                var folder = session.OpenFolder(target + "/");
                (folder as WebDavFolder).SetHref(target, new Uri(target));
                folder.Delete();
            }
            else if (entry.EntryType == FileBrowserEntryType.File)
            {
                var folder = session.OpenFolder(folderPath + "/");
                var resource = folder.GetResource(resourcePath + entry.Name);
                resource.Delete();
            }
            return Json("");
        }
        public ActionResult Upload(string path, HttpPostedFileBase file)
        {
            BuildPath(path, out folderPath, out resourcePath);
            var folder = session.OpenFolder(folderPath + "/");
            var bytes = new byte[file.ContentLength];
            file.InputStream.Read(bytes, 0, bytes.Length);
            var resource = folder.CreateResource(file.FileName, bytes);
            return Json(new { Name = file.FileName, Size = file.ContentLength, EntryType = FileBrowserEntryType.File });
        }


        protected void BuildPath(string path, out string folderPath, out string resourcePath)
        {
            folderPath = WebDavRoot + "/";
            resourcePath = "/" + path;
            if (path.Contains("/"))
            {
                var index = path.LastIndexOf("/");
                folderPath += path.Substring(0, index);
                resourcePath = "/" + path.Substring(index + 1);
            }
        }
    }
}
