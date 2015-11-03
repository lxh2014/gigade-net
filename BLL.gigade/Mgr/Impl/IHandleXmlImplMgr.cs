using BLL.gigade.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr.Impl
{
    public interface IHandleXmlImplMgr
    {
        List<XmlModelCustom> GetXmlName(string fullPath);
        string GetXmlInfo(string fullPath);
        string GetStrCopy(string fullPath);
        bool SaveOrUpdate(string fullPath, XmlModelCustom xmc, int type);
        bool CreateFile(string fullPath, string nodeName);
        bool DeleteFile(string fullPath);
    }
}
