using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using BLL.gigade;
using System.IO;
using BLL.gigade.Common;

namespace GigadeService
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“Service1”。
    
    public class TransImage : ITransImage
    {
        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }

        public string Trans(string sourceImgPath,string targetPath,string newFileName,int width,int height,string userName,string passWd)
        {
            string error = string.Empty;
            string targetImgPath = targetPath + newFileName;
            if (!Directory.Exists(targetPath))
                Directory.CreateDirectory(targetPath);
            ImageClass iC = new ImageClass(userName, passWd);
            iC.ImageMagick(sourceImgPath, targetImgPath, width, height, ref error);
            return error;
        }
    }
}
