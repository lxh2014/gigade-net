using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace GigadeService
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的接口名“IService1”。
    [ServiceContract]
    public interface ITransImage
    {

        [OperationContract]
        string GetData(int value);

        [OperationContract]
        CompositeType GetDataUsingDataContract(CompositeType composite);

        // TODO: 在此添加您的服务操作
        /// <summary>
        /// 壓縮圖片大小
        /// </summary>
        /// <param name="sourceImgPath">原圖路徑</param>
        /// <param name="targetPath">目標路勁(無圖片名稱)</param>
        /// <param name="newFileName">新圖片名稱</param>
        /// <param name="width">新寬度</param>
        /// <param name="height">新高度</param>
        /// <param name="userName">用戶名</param>
        /// <param name="passWd">密碼</param>
        /// <returns></returns>
        [OperationContract]
        string Trans(string sourceImgPath, string targetPath, string newFileName, int width, int height, string userName, string passWd);
    }


    // 使用下面示例中说明的数据约定将复合类型添加到服务操作。
    [DataContract]
    public class CompositeType
    {
        bool boolValue = true;
        string stringValue = "Hello ";

        [DataMember]
        public bool BoolValue
        {
            get { return boolValue; }
            set { boolValue = value; }
        }

        [DataMember]
        public string StringValue
        {
            get { return stringValue; }
            set { stringValue = value; }
        }
    }
}
