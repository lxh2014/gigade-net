using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Web;

namespace GigadeService
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的接口名“IBackDoor”。
    [ServiceContract]
    public interface IBackDoor
    {
        /// <summary>
        /// 批量修改ERP_ID
        /// Author: Castle
        /// Date:    2014/06/26   
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        string UpdateERP_ID();
    }
}
