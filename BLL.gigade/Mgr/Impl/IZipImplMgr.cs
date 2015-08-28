using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr.Impl
{
    public interface IZipImplMgr
    {
        string QueryBig(string strTopValue);
        /// <summary>
        /// 查詢市級
        /// </summary>
        /// <param name="strTopValue">上一級區域的code，如：北區</param>
        /// <returns></returns>
        string QueryMiddle(string strTopValue);

        /// <summary>
        /// 查詢地區
        /// </summary>
        /// <param name="strTopValue">上一級市級的code，如：基隆市</param>
        /// <returns></returns>
        string QuerySmall(string strTopValue,string topText);
        Zip QueryCityAndZip(string zipcode);
        DataTable ZipTable(Zip zip, String appendSql);
        DataTable GetZip();
        string Getaddress(int zipcode);
    }
}
