using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using System.Xml.Linq;
using BLL.gigade.Model;
using System.Data;

namespace BLL.gigade
{
    class ParametersrcXmlDao: IParametersrcImplDao
    {
        private string xmlPath;
        public ParametersrcXmlDao(string xmlPath)
        {
            this.xmlPath = xmlPath;
        }
        public List<Parametersrc> Query(Model.Parametersrc para)
        {
            try
            {
                XDocument xml = XDocument.Load(xmlPath);//加载xml
                return xml.Elements().Elements().
                    Select(x => new Parametersrc
                    {
                        ParameterType = x.Attribute("parameterType").Value,
                        ParameterProperty = x.Attribute("parameterProperty").Value,
                        parameterName = x.Attribute("parameterName").Value,
                        ParameterCode = x.Attribute("parameterCode").Value,
                        TopValue = x.Attribute("topValue").Value
                        //edit by zhuoqin0830w  2015/02/26  確保新類別中運行正確
                    }).Where(x => x.ParameterType == para.ParameterType).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("ParametersrcXmlDao-->Query(Model.Parametersrc para)-->" + ex.Message, ex);
            }
        }

        public List<Model.Parametersrc> QueryForTopValue(Model.Parametersrc para)
        {
            throw new NotImplementedException();
        }

        public string Save(Model.Parametersrc p)
        {
            throw new NotImplementedException();
        }

        public string Update(Model.Parametersrc para)
        {
            throw new NotImplementedException();
        }

        public string DeleteByType(Model.Parametersrc p)
        {
            throw new NotImplementedException();
        }

        public List<Model.Parametersrc> QueryType(string NotIn)
        {
            throw new NotImplementedException();
        }

        public System.Data.DataTable QueryProperty(Model.Parametersrc Pquery, Model.Parametersrc Cquery)
        {
            throw new NotImplementedException();
        }

        public List<Model.Parametersrc> GetParameterByCode(string code)
        {
            throw new NotImplementedException();
        }

        public List<Model.Parametersrc> GetParametersrcList(Model.Parametersrc store, out int totalCount)
        {
            throw new NotImplementedException();
        }

        public int ParametersrcSave(Model.Parametersrc para)
        {
            throw new NotImplementedException();
        }

        public int UpdateUsed(Model.Parametersrc store)
        {
            throw new NotImplementedException();
        }

        public List<Model.Parametersrc> PayforType(string parameterType)
        {
            throw new NotImplementedException();
        }

        public bool GetParametersrcMaxRowId(int id)
        {
            throw new NotImplementedException();
        }

        public List<Model.Parametersrc> QuerySinggleByID(Model.Parametersrc para)
        {
            throw new NotImplementedException();
        }

        public List<Model.Parametersrc> GetElementType(string types)
        {
            throw new NotImplementedException();
        }

        public List<Model.Parametersrc> GetAllKindType(string types)
        {
            throw new NotImplementedException();
        }
        public List<Model.Parametersrc> GetKindTypeByStatus(string types)
        {
            throw new NotImplementedException();
        }

        public bool Save(List<Parametersrc> saveList)
        {
            throw new NotImplementedException();
        }

        public bool Update(List<Parametersrc> updateList)
        {
            throw new NotImplementedException();
        }


        public List<Parametersrc> GetIialgParametersrcList(Parametersrc store, out int totalCount)
        {
            throw new NotImplementedException();
        }

        public int Delete(Parametersrc m)
        {
            throw new NotImplementedException();
        }

        public List<Parametersrc> GetParameter(Parametersrc p)
        {
            throw new NotImplementedException();
        }

        public int InsertTP(Parametersrc p)
        {
            throw new NotImplementedException();
        }

        public System.Data.DataTable GetParametercode(Parametersrc p)
        {
            throw new NotImplementedException();
        }

        public int UpdTP(Parametersrc p)
        {
            throw new NotImplementedException();
        }

        public string GetOrderStatus(int pc)
        {
            throw new NotImplementedException();
        }

        public List<Model.Parametersrc> GetDep()
        {
            throw new NotImplementedException();
        }

        public List<Model.Parametersrc> QueryParametersrcByTypes(params string[] types)
        {
            throw new NotImplementedException();
        }
        public DataTable GetTP(Parametersrc p)
        {
            throw new NotImplementedException();
        }


        string IParametersrcImplDao.Getmail(string p)
        {
            throw new NotImplementedException();
        }

        List<Parametersrc> IParametersrcImplDao.ReturnParametersrcList()
        {
            throw new NotImplementedException();
        }


        public string GetOrderPayment(int order_payment)
        {
            throw new NotImplementedException();
        }

        public string GetSlaveStatus(int slave_status)
        {
            throw new NotImplementedException();
        }

        public string GetProductMode(int product_mode)
        {
            throw new NotImplementedException();
        }
    }
}
