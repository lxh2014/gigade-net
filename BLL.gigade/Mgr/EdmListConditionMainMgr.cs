using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Dao;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr
{
    public class EdmListConditionMainMgr
    {
        private EdmListConditionMainDao _edmlistmainDao;
        private EdmListConditoinSubDao _edmlistsubDao;
        public EdmListConditionMainMgr(string connectionString)
        {
            _edmlistmainDao = new EdmListConditionMainDao(connectionString);
            _edmlistsubDao = new EdmListConditoinSubDao(connectionString);
        }
        public List<EdmListConditionMain> GetConditionList()
        {
            try
            {
                return _edmlistmainDao.GetConditionList();
            }
            catch (Exception ex)
            {
                throw new Exception(" EdmListConditionMainMgr-->GetConditionList " + ex.Message, ex);
            }
        }
        public int DeleteListInfo(EdmListConditionMain query)
        {
            EdmListConditionMain model = new EdmListConditionMain();
            try
            {
                model = _edmlistmainDao.SelectElcmIDByConditionName(query.elcm_name);
                query.elcm_id = model.elcm_id;
                return _edmlistmainDao.DeleteListInfo(query);
            }
            catch (Exception ex)
            {
                throw new Exception(" EdmListConditionMainMgr-->DeleteListInfo " + ex.Message, ex);
            }
        }
        public int SaveListInfoName(EdmListConditionMain query, out int id,out int msg)
        {
            try
            {
                EdmListConditionMain idModel = new EdmListConditionMain();       
                EdmListConditionMain model = new EdmListConditionMain();
                idModel = _edmlistmainDao.SelectElcmIDByConditionName(query.elcm_name);
                msg = 0;
                id = 0;
                if (idModel == null)
                {
                    query.elcm_created = DateTime.Now;
                    int i = _edmlistmainDao.SaveListInfoName(query);
                    if (i > 0)
                    {
                        model = _edmlistmainDao.SelectElcmIDByConditionName(query.elcm_name);
                        id = model.elcm_id;
                    }
                    return i;
                }
                else
                {
                    msg = 1; //篩選條件名已存在
                    return 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(" EdmListConditionMainMgr-->SaveListInfoName " + ex.Message, ex);
            }
        }
        public int UpdateCondition(EdmListConditoinSubQuery query)
        {
            int i = 0;
            try
            {                
                EdmListConditionMain model = new EdmListConditionMain();
                model = _edmlistmainDao.SelectElcmIDByConditionName(query.elcm_name);
                if (model != null)
                {
                    query.elcm_id = model.elcm_id;
                    _edmlistsubDao.DeleteInfo(query);
                     i = _edmlistsubDao.SaveListInfoCondition(query);
                }
                return i;
            }
            catch (Exception ex)
            {
                throw new Exception(" EdmListConditionMainMgr-->UpdateCondition " + ex.Message, ex);
            }
        }
        public DataTable GetUserNum(EdmListConditoinSubQuery q)
        {
            try
            {
                return _edmlistmainDao.GetUserNum(q);
            }
            catch (Exception ex)
            {
                throw new Exception(" EdmListConditionMainMgr-->GetUserNum " + ex.Message, ex);
            }
        }
    }
}
