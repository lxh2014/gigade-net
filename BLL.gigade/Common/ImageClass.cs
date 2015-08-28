using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

/// <summary>
///ImageClass 的摘要说明
/// </summary>
/// 

namespace BLL.gigade.Common
{
    public class ImageClass
    {
        public Image ResourceImage;
        private int ImageWidth;
        private int ImageHeight;
        private string _imageFileName;
        private string _userName;
        private string _passwd;

        public ImageClass(string userName,string passwd) {
            _userName = userName;
            _passwd = passwd;
        }
        public ImageClass(string ImageFileName)
        {
            ResourceImage = Image.FromFile(ImageFileName,true);
            _imageFileName = ImageFileName;
        }

        public bool ThumbnailCallback()
        {
            return false;
        }

        public Image getReducedImage(int width, int height)
        {
            Image ReducedImage = null;
            try
            {
                Image.GetThumbnailImageAbort call = new Image.GetThumbnailImageAbort(ThumbnailCallback);

                ReducedImage = ResourceImage.GetThumbnailImage(width, height, call, IntPtr.Zero);

                return ReducedImage;
            }
            catch
            {
                return null;
            }
            finally
            {
                ReducedImage.Dispose();
                ResourceImage.Dispose();
            }
        }
        /// <summary>
        /// 通过调用ImageMagick来缩放图片（传说这样的保真度高一些）
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="desPath"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="msg"></param>
        public void ImageMagick(string sourcePath,string desPath,int width,int height,ref string msg) {
            string cmd = string.Format("convert -resize \"{0}\" {1} {2}", width.ToString() + "x" + height.ToString(), sourcePath, desPath);
            //string cmd = string.Format("copy {0} {1}", sourcePath, desPath);
            MyProcess p = new MyProcess(_userName,_passwd);
            p.Start();
            p.ExecCMD(cmd);
            p.Exit();
            //string result = p.GetResult();
            p.Close();
        }

        public void MakeThumbnail( string thumbnailPath, int width, int height,ref string msg)
        {
            int buffer_count = 2048;
            byte[] buffer = new byte[buffer_count];
            //缩略图画布宽高  
            int towidth = width;
            int toheight = height;
            //原始图片写入画布坐标和宽高(用来设置裁减溢出部分)  
            int x = 0;
            int y = 0;
            int ow = ResourceImage.Width;
            int oh = ResourceImage.Height;
            if (ow == width && oh == height)
            {
                msg = "exists";
                return;
            }
            //原始图片画布,设置写入缩略图画布坐标和宽高(用来原始图片整体宽高缩放)  
            int bg_x = 0;
            int bg_y = 0;
            int bg_w = towidth;
            int bg_h = toheight;
            //倍数变量  
            double multiple = 0;
            //获取宽长的或是高长与缩略图的倍数  
            if (ResourceImage.Width >= ResourceImage.Height)
                multiple = (double)ResourceImage.Width / (double)width;
            else
                multiple = (double)ResourceImage.Height / (double)height;
            //上传的图片的宽和高小等于缩略图  
            if (ow <= width && oh <= height)
            {
                //缩略图按原始宽高  
                bg_w = ResourceImage.Width;
                bg_h = ResourceImage.Height;
                //空白部分用背景色填充  
                bg_x = Convert.ToInt32(((double)towidth - (double)ow) / 2);
                bg_y = Convert.ToInt32(((double)toheight - (double)oh) / 2);
            }
                //上传的图片的宽和高大于缩略图  
            else
            {
                //宽高按比例缩放  
                bg_w = Convert.ToInt32((double)ResourceImage.Width / multiple);
                bg_h = Convert.ToInt32((double)ResourceImage.Height / multiple);
                //空白部分用背景色填充  
                bg_y = Convert.ToInt32(((double)height - (double)bg_h) / 2);
                bg_x = Convert.ToInt32(((double)width - (double)bg_w) / 2);
            }
            //新建一个bmp图片,并设置缩略图大小.  
            System.Drawing.Image bitmap = new System.Drawing.Bitmap(towidth, toheight);
            //新建一个画板  
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap);

            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            g.Clear(Color.Transparent);
            //在指定位置并且按指定大小绘制原图片的指定部分  
            //第一个System.Drawing.Rectangle是原图片的画布坐标和宽高,第二个是原图片写在画布上的坐标和宽高,最后一个参数是指定数值单位为像素  
            g.DrawImage(ResourceImage, new System.Drawing.Rectangle(bg_x, bg_y, bg_w, bg_h), new System.Drawing.Rectangle(x, y, ow, oh), System.Drawing.GraphicsUnit.Pixel);
            //g.DrawImage(ResourceImage, x, y, bg_w, bg_h);
            try
            {
                ImageCodecInfo ici = ImageCodecInfo.GetImageEncoders().FirstOrDefault(p => p.FormatID == ResourceImage.RawFormat.Guid);
                //EncoderParameters paras = ResourceImage.GetEncoderParameterList(ici.Clsid);
                System.Drawing.Imaging.EncoderParameters ep = new System.Drawing.Imaging.EncoderParameters(1);
                ep.Param[0] = new System.Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.Quality, (long)100);

                //获取图片类型
                string fileExtension = System.IO.Path.GetExtension(_imageFileName).ToLower();
                //按原图片类型保存缩略图片,不按原格式图片会出现模糊,锯齿等问题.  
                foreach (PropertyItem i in ResourceImage.PropertyItems)
                {
                    bitmap.SetPropertyItem(i);
                }
                //switch (fileExtension)
                //{
                //    case ".gif": bitmap.Save(thumbnailPath, System.Drawing.Imaging.ImageFormat.Gif); break;
                //    case ".jpg": bitmap.Save(thumbnailPath,ici,ep); break;//, System.Drawing.Imaging.ImageFormat.Jpeg
                //    case ".bmp": bitmap.Save(thumbnailPath, System.Drawing.Imaging.ImageFormat.Bmp); break;
                //    case ".png": bitmap.Save(thumbnailPath, System.Drawing.Imaging.ImageFormat.Png); break;
                //}
                bitmap.Save(thumbnailPath, ici, ep);
            } catch (System.Exception e)
            {
                throw e;
            } finally
            {
                ResourceImage.Dispose();
                bitmap.Dispose();
                g.Dispose();
            }
        }

        public bool getReducedImage(int width, int height, string targetFilePath)
        {
            Image ReducedImage = null;
            try
            {
                Image.GetThumbnailImageAbort call = new Image.GetThumbnailImageAbort(ThumbnailCallback);

                ReducedImage = ResourceImage.GetThumbnailImage(width, height, call, IntPtr.Zero);
                ReducedImage.Save(targetFilePath);

                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                ReducedImage.Dispose();
                ResourceImage.Dispose();
            }
        }
    }
}