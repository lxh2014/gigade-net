/* 
 * 武漢聯綿信息技術有限公司
 *  
 * 文件名称：IParticularsSrcImplDao 
 * 摘    要： 
 *  
 * 当前版本：1.0 
 * 作    者：zhuoqin0830w 
 * 完成日期：2015/05/19
 * 
 * 修改日期：2015/09/29
 * 修改原因：將使用 xml 資料庫 改為使用 db 數據庫
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;

namespace BLL.gigade.Dao.Impl
{
    public interface IParticularsSrcImplDao
    {
        // 獲取 ParticularsSrc 的信息
        List<ParticularsSrc> GetParticularsSrc();

        // 根據 保存期限 刪除 相關信息
        bool DeleteNode(string ParticularsName);

        // 根據 保存期限 添加 或 修改 相關信息
        bool SaveNode(List<ParticularsSrc> particularsSrc);
    }
}