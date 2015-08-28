using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Admin.gigade.Controllers
{
    public class ImageBrowserController : FileBrowserController
    {
        protected override string RootConfigName
        {
            get
            {
                return "webDavImage";
            }
        }

        public ImageBrowserController()
            : base()
        {
        }
        public ActionResult Thumbnail(string path)
        {
            var myCallback =
                new Image.GetThumbnailImageAbort(ThumbnailCallback);
            var paths = new List<string>(2);
            BuildPath(path, out folderPath, out resourcePath);
            var folder = session.OpenFolder(folderPath + "/");
            var resource = folder.GetResource(resourcePath + "/");
            var sourceStream = resource.GetReadStream();
            Bitmap bitmap = null;
            try
            {
                bitmap = new Bitmap(sourceStream);
            }
            catch (Exception)
            {
                var fs = new FileStream(Server.MapPath("~/Content/kendo/2014.2.716/Bootstrap/imagebrowser.png"), FileMode.Open);
                var tempBs = new byte[fs.Length];
                fs.Read(tempBs, 0, tempBs.Length);
                return new FileContentResult(tempBs, "image/jpeg");
            }
            var myThumbnail = bitmap.GetThumbnailImage(84, 70, myCallback, IntPtr.Zero);
            var ms = new MemoryStream();
            var myEncoderParameters = new EncoderParameters(1);
            var myEncoderParameter = new EncoderParameter(Encoder.Quality, 25L);
            myEncoderParameters.Param[0] = myEncoderParameter;
            myThumbnail.Save(ms, GetEncoderInfo("image/jpeg"), myEncoderParameters);
            ms.Position = 0;
            var bytes = ms.ToArray();
            return new FileContentResult(bytes, "image/jpeg");
        }
        private bool ThumbnailCallback()
        {
            return false;
        }
        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }
    }
}
