using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Dao;
using BLL.gigade.Model.Query;
using BLL.gigade.Mgr.Impl;
using System.Data;

namespace BLL.gigade.Mgr
{
    public class IupcMgr : IiupcImplMgr
    {
        private IiupcImplDao _iupcdao;

        public IupcMgr(string connectionString)
        {
            _iupcdao = new IupcDao(connectionString);

        }
        public int Delete(Iupc iupc)
        {
            try
            {
                return _iupcdao.Delete(iupc);
            }
            catch (Exception ex)
            {
                throw new Exception("IupcMgr-->Insert-->" + ex.Message, ex);
            }
        }
        public int Insert(Iupc iupc)
        {
            try
            {
                return _iupcdao.Insert(iupc); 
            }
            catch (Exception ex)
            {
                throw new Exception("IupcMgr-->Insert-->" + ex.Message, ex);
            }
        }
        public string  IsExist(Iupc iupc)
        {
         try
            {
                return _iupcdao.IsExist(iupc); 
            }
            catch (Exception ex)
            {
                throw new Exception("IupcMgr-->IsExist-->" + ex.Message, ex);
            }
         
        }
        public int Update(Iupc iupc)
        {
            try
            {
                return _iupcdao.Update(iupc);
            }
            catch (Exception ex)
            {
                throw new Exception("IupcMgr-->Update-->" + ex.Message, ex);
            }
        }
        public List<IupcQuery> GetIupcList(IupcQuery iupc, out int totalCount)
        {
            try
            {
                return _iupcdao.GetIupcList(iupc, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("IupcMgr-->GetIupcList-->" + ex.Message, ex);
            }
        }
        public DataTable upcid(Iupc m)
        {
            try
            {
               return  _iupcdao.upcid(m);
            }
            catch (Exception ex)
            {
                throw new Exception("IupcMgr-->upcid-->" + ex.Message, ex);
            }
        }
        public int Yesornoexist(int i, string j)
        {
            try
            {
                return _iupcdao.Yesornoexist(i, j);
            }
            catch (Exception ex)
            {
                throw new Exception("IupcMgr-->Yesornoexist-->" + ex.Message, ex);
            }
        }
        public int ExcelImportIupc(string condition)
        {
            try
            {
                return _iupcdao.ExcelImportIupc(condition);
            }
            catch (Exception ex)
            {
                throw new Exception("IupcMgr-->ExcelImportIupc-->" + ex.Message, ex);
            }
        }


        #region 條碼維護匯出信息
        public List<IupcQuery> GetIupcExportList(IupcQuery iupc)
        {
            try
            {
                return _iupcdao.GetIupcExportList(iupc);
            }
            catch (Exception ex)
            {
                throw new Exception("IupcMgr-->GetIupcExportList-->" + ex.Message, ex);
            }
        }
        #endregion
        public int upc_num(int m)
        {
            try
            {
                return _iupcdao.upc_num(m);
            }
            catch (Exception ex)
            {
                throw new Exception("IupcMgr-->upc_num-->" + ex.Message, ex);
            }
        }
        public string Getupc(string item_id, string type)
        {
            try
            {
                return _iupcdao.Getupc(item_id,type);
            }
            catch (Exception ex)
            {
                throw new Exception("IupcMgr-->Getupc-->" + ex.Message, ex);
            }
        }
        public List<IupcQuery> GetIupcByItemID(IupcQuery query)
        {
            try
            {
                return _iupcdao.GetIupcByItemID(query);
            }
            catch (Exception ex)
            {
                throw new Exception("IupcMgr-->GetIupcByItemID-->" + ex.Message, ex);
            }
        }
        public List<IupcQuery> GetIupcByType(IupcQuery query)
        {
            try
            {
                return _iupcdao.GetIupcByType(query);
            }
            catch (Exception ex)
            {
                throw new Exception("IupcMgr-->GetIupcByType-->" + ex.Message, ex);
            }
        }
    }
}
