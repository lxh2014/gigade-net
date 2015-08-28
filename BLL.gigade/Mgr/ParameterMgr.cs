using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;
using System.Data;

namespace BLL.gigade.Mgr
{
    public class ParameterMgr : IParametersrcImplMgr
    {
        private IParametersrcImplDao _paraDao;
        public ParameterMgr(string connectionString, ParaSourceType sourceType = ParaSourceType.MYSQL)
        {
            switch (sourceType)
            {
                case ParaSourceType.MYSQL:
                    _paraDao = new ParametersrcDao(connectionString);
                    break;
                case ParaSourceType.XML:
                    _paraDao = new ParametersrcXmlDao(connectionString);
                    break;
            }
        }

        public List<Parametersrc> ReturnParametersrcList()
        {
            try
            {
                return _paraDao.ReturnParametersrcList();
            }
            catch (Exception ex)
            {
                throw new Exception(" ParameterMgr-->ReturnParametersrcList()-->" + ex.Message, ex);
            }
        }
        #region IParametersrcImplMgr 成员

        public string Query(string strParaType, int used = 1)
        {
            try
            {
                List<Parametersrc> paraResult = _paraDao.Query(new Parametersrc { ParameterType = strParaType, Used = used });
                StringBuilder stb = new StringBuilder();

                stb.Append("{");
                stb.Append(string.Format("success:true,items:["));
                if (strParaType == "payment")
                {
                    stb.Append("{");
                    stb.Append(string.Format("\"rowid\":\"{0}\",\"parameterType\":\"{1}\",\"parameterCode\":\"{2}\",\"parameterName\":\"{3}\",\"sort\":\"{4}\"", 0, strParaType, 0, "不分", 0));
                    stb.Append("}");
                }
                foreach (Parametersrc para in paraResult)
                {
                    stb.Append("{");
                    stb.Append(string.Format("\"rowid\":\"{0}\",\"parameterType\":\"{1}\",\"parameterCode\":\"{2}\",\"parameterName\":\"{3}\",\"sort\":\"{4}\",\"remark\":\"{5}\"", para.Rowid, para.ParameterType, para.ParameterCode, para.parameterName, para.Sort, para.remark));
                    stb.Append("}");
                }
                stb.Append("]}");
                return stb.ToString().Replace("}{", "},{");
            }
            catch (Exception ex)
            {
                throw new Exception("ParameterMgr-->Query-->" + ex.Message, ex);
            }
        }
        /// <summary>
        /// 根據code獲得rowid,再查詢出以該rowid為topvalue的json
        /// </summary>
        /// <param name="strParaCode"></param>
        /// <param name="used"></param>
        /// <returns></returns>
        public string QueryByCode(string strParaCode, int used = 1)
        {
            try
            {
                Parametersrc paraResult = _paraDao.Query(new Parametersrc { ParameterCode = strParaCode, Used = used }).FirstOrDefault();
                List<Parametersrc> paraModel = _paraDao.Query(new Parametersrc { TopValue = paraResult.Rowid.ToString(), Used = used });
                StringBuilder stb = new StringBuilder();

                stb.Append("{");
                stb.Append(string.Format("success:true,items:["));

                foreach (Parametersrc para in paraModel)
                {
                    stb.Append("{");
                    stb.Append(string.Format("\"rowid\":\"{0}\",\"parameterType\":\"{1}\",\"parameterCode\":\"{2}\",\"parameterName\":\"{3}\",\"remark\":\"{4}\"", para.Rowid, para.ParameterType, para.ParameterCode, para.parameterName, para.remark));
                    stb.Append("}");
                }
                stb.Append("]}");
                return stb.ToString().Replace("}{", "},{");
            }
            catch (Exception ex)
            {
                throw new Exception("ParameterMgr-->QueryByCode-->" + ex.Message, ex);
            }
        }




        public List<Parametersrc> QueryForTopValue(Parametersrc para)
        {
            return _paraDao.QueryForTopValue(para);
        }

        public string Save(Parametersrc p)
        {
            return _paraDao.Save(p);
        }

        public bool Save(List<Parametersrc> saveList)
        {
            return _paraDao.Save(saveList);
        }

        public string Update(Parametersrc p)
        {
            return _paraDao.Update(p);
        }

        public bool Update(List<Parametersrc> updateList)
        {
            //將 Save 改為 Update  以便獲取最新的修改數據  edit by zhuoqin0830w  2015/03/04
            return _paraDao.Update(updateList);
        }

        public string DeleteByType(Parametersrc p)
        {
            return _paraDao.DeleteByType(p);
        }

        public List<Parametersrc> QueryType(string NotIn)
        {
            return _paraDao.QueryType(NotIn);
        }

        public List<Parametersrc> QueryUsed(Parametersrc para)
        {
            try
            {
                return _paraDao.Query(para);
            }
            catch (Exception ex)
            {
                throw new Exception("ParameterMgr-->QueryUsed-->" + ex.Message, ex);
            }
        }

        public string QueryBindData(Parametersrc para)
        {
            try
            {
                List<Parametersrc> paraResult = _paraDao.Query(para);

                StringBuilder stb = new StringBuilder();

                for (int i = 1; i <= paraResult.Count; i++)
                {
                    if (i == 1 && para.ParameterType != "product_mode")
                    {
                        stb.AppendFormat("<input type='radio' id='{0}{1}' name='{0}' checked='checked' style='margin-right:2px' value='{2}' /><label for = {0}{1} style='margin-right:5px'>{3}</label>", para.ParameterType, i, paraResult[i - 1].ParameterCode, paraResult[i - 1].parameterName);
                    }
                    else if (i == 2 && para.ParameterType == "product_mode")//組合新建的時候默認出貨方式改為第二個
                    {
                        stb.AppendFormat("<input type='radio' id='{0}{1}' name='{0}' checked='checked' style='margin-right:2px' value='{2}' /><label for = {0}{1} style='margin-right:5px'>{3}</label>", para.ParameterType, i, paraResult[i - 1].ParameterCode, paraResult[i - 1].parameterName);
                    }
                    else
                    {
                        stb.AppendFormat("<input type='radio' id='{0}{1}' name='{0}' style='margin-right:2px' value='{2}' /><label for = {0}{1} style='margin-right:5px'>{3}</label>", para.ParameterType, i, paraResult[i - 1].ParameterCode, paraResult[i - 1].parameterName);
                    }
                }

                return stb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("ParameterMgr-->QueryBindData-->" + ex.Message, ex);
            }
        }

        public DataTable QueryProperty(Parametersrc Pquery, Parametersrc Cquery)
        {
            try
            {
                return _paraDao.QueryProperty(Pquery, Cquery);
            }
            catch (Exception ex)
            {
                throw new Exception("ParameterMgr-->QueryProperty-->" + ex.Message, ex);
            }
        }

        /// <summary>
        /// 根據code得到相關parameter信息
        /// </summary>
        /// add by wangwei
        /// <returns></returns>
        public List<Parametersrc> GetParameterByCode(string code)
        {
            return _paraDao.GetParameterByCode(code);
        }
        #region 參數表新增修改保存方法+int ParametersrcSave(Parametersrc para)
        /// <summary>
        /// 參數表新增修改保存方法
        /// 2014/10/20號zhejiangj新增
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public int ParametersrcSave(Parametersrc para)
        {
            try
            {
                return _paraDao.ParametersrcSave(para);
            }
            catch (Exception ex)
            {

                throw new Exception("ParameterMgr-->ParametersrcSave-->" + ex.Message, ex);
            }
        }
        #endregion


        #region 查詢一條數據根據RowId+List<Parametersrc> QuerySinggleByID(Parametersrc para)
        /// <summary>
        /// 查詢一條數據根據RowId
        /// 2014/10/20號zhejiangj新增
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public List<Parametersrc> QuerySinggleByID(Parametersrc para)
        {
            try
            {
                return _paraDao.QuerySinggleByID(para);
            }
            catch (Exception ex)
            {

                throw new Exception("ParameterMgr-->QuerySinggleByID-->" + ex.Message, ex);
            }
        }
        #endregion
        #endregion

        #region 參數表列表頁顯示+List<Parametersrc> GetParametersrcList(Parametersrc store, out int totalCount)
        public List<Parametersrc> GetParametersrcList(Parametersrc store, out int totalCount)
        {
            try
            {
                return _paraDao.GetParametersrcList(store, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception(" ParameterMgr-->GetParametersrcList-->" + ex.Message, ex);
            }
        }

        #endregion

        #region 更改參數表狀態+int UpdateUsed(Parametersrc store)


        public int UpdateUsed(Parametersrc store)
        {
            try
            {
                return _paraDao.UpdateUsed(store);
            }
            catch (Exception ex)
            {

                throw new Exception("ParameterMgr-->UpdateUsed-->" + ex.Message, ex);
            }
        }

        #endregion

        #region 付款單狀態下拉列表+List<Parametersrc> PayforType(string parameterType)


        public List<Parametersrc> PayforType(string parameterType)
        {
            try
            {
                return _paraDao.PayforType(parameterType);
            }
            catch (Exception ex)
            {

                throw new Exception("ParameterMgr-->PayforType-->" + ex.Message, ex);
            }
        }

        #endregion


        public List<Parametersrc> GetElementType(string types)
        {
            return _paraDao.GetElementType(types);
        }


        public List<Parametersrc> GetAllKindType(string types)
        {
            return _paraDao.GetAllKindType(types);
        }

        public List<Parametersrc> GetIialgParametersrcList(Parametersrc store, out int totalCount)
        {
            try
            {
                return _paraDao.GetIialgParametersrcList(store, out totalCount);
            }
            catch (Exception ex)
            {

                throw new Exception(" ParameterMgr-->GetIialgParametersrcList-->" + ex.Message, ex);
            }
        }

        public int Delete(Parametersrc m)
        {
            try
            {
                return _paraDao.Delete(m);
            }
            catch (Exception ex)
            {
                throw new Exception(" ParameterMgr-->Delete-->" + ex.Message, ex);
            }
        }

        public List<Parametersrc> GetParameter(Parametersrc p)
        {
            try
            {
                return _paraDao.GetParameter(p);
            }
            catch (Exception ex)
            {
                throw new Exception(" ParameterMgr-->GetParameter-->" + ex.Message, ex);
            }
        }

        public int InsertTP(Parametersrc p)
        {
            try
            {
                return _paraDao.InsertTP(p);
            }
            catch (Exception ex)
            {
                throw new Exception(" ParameterMgr-->InsertTP-->" + ex.Message, ex);
            }
        }

        public DataTable GetParametercode(Parametersrc p)
        {
            try
            {
                return _paraDao.GetParametercode(p);
            }
            catch (Exception ex)
            {
                throw new Exception(" ParameterMgr-->GetParametercode-->" + ex.Message, ex);
            }
        }

        public int UpdTP(Parametersrc p)
        {
            try
            {
                return _paraDao.UpdTP(p);
            }
            catch (Exception ex)
            {
                throw new Exception(" ParameterMgr-->UpdTP-->" + ex.Message, ex);
            }
        }


        public string GetOrderStatus(int pc)
        {
            try
            {
                return _paraDao.GetOrderStatus(pc);
            }
            catch (Exception ex)
            {
                throw new Exception(" ParameterMgr-->GetOrderStatus-->" + ex.Message, ex);
            }
        }
        public DataTable GetTP(Parametersrc p)
        {
            try
            {
                return _paraDao.GetTP(p);
            }
            catch (Exception ex)
            {
                throw new Exception(" ParameterMgr-->GetTP-->" + ex.Message, ex);
            }
        }
        public string Getmail(string p)
        {
            try
            {
                return _paraDao.Getmail(p);
            }
            catch (Exception ex)
            {
                throw new Exception(" ParameterMgr-->Getmail-->" + ex.Message, ex);
            }
        }
    }
    public enum ParaSourceType
    {
        MYSQL,
        XML
    }

   

}
