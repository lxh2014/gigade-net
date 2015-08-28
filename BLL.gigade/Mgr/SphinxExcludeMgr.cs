using BLL.gigade.Dao;
using BLL.gigade.Model.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr
{
    public class SphinxExcludeMgr
    {
        SphinxExcludeDao dao;
        public SphinxExcludeMgr(string connectionString)
        {
            dao = new SphinxExcludeDao(connectionString);
        }
        public int InsertModel(SphinxExcludeQuery model)
        {
            try
            {
                return dao.InsertModel(model);
            }
            catch (Exception ex)
            {
                throw new Exception("SphinxExcludeMgr-->InsertModel-->" + ex.Message, ex);
            }
        }
        public List<SphinxExcludeQuery> GetList(SphinxExcludeQuery model, out int total)
        {
            try
            {
                return dao.GetList(model, out total);
            }
            catch (Exception ex)
            {
                throw new Exception("SphinxExcludeMgr-->GetList-->" + ex.Message, ex);
            }
        }
        public int UpdateModel(SphinxExcludeQuery model)
        {
            try
            {
                return dao.UpdateModel(model);
            }
            catch (Exception ex)
            {
                throw new Exception("SphinxExcludeMgr-->UpdateModel-->" + ex.Message, ex);
            }
        }

        public int DeleteModel(SphinxExcludeQuery model)
        {
            try
            {
                return dao.DeleteModel(model);
            }
            catch (Exception ex)
            {
                throw new Exception("SphinxExcludeMgr-->DeleteModel-->" + ex.Message, ex);
            }
        }
    }
}
