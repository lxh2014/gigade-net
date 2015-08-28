using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
namespace BLL.gigade.Mgr
{
    public class PageAreaMgr : IPageAreaImplMgr
    {
        private IPageAreaImplDao _beradao;

        public PageAreaMgr(string connectionString)
        {
            _beradao = new PageAreaDao(connectionString);
        }
        public List<Model.Query.PageAreaQuery> QueryAll(Model.Query.PageAreaQuery query, out int totalCount)
        {
            return _beradao.QueryAll(query, out totalCount);
        }

        public int Update(Model.PageArea model)
        {
            return _beradao.Update(model);
        }


        public Model.PageArea GetModel(Model.PageArea model)
        {
            return _beradao.GetModel(model);
        }


        public List<Model.PageArea> GetArea()
        {
            try
            {
                return _beradao.GetArea();
            }
            catch (Exception ex)
            {
                throw new Exception("BannerAreaMgr.GetArea-->" + ex.Message, ex);
            }
        }
        /// <summary>
        /// 保存新增區域數據
        /// </summary>
        /// <param name="ba"></param>
        /// <returns></returns>
        public int AreaSave(PageArea ba)
        {
            try
            {
                return _beradao.AreaSave(ba);
            }
            catch (Exception ex)
            {

                throw new Exception("BannerAreaMgr-->AreaSave-->" + ex.Message, ex);
            }
        }
        /// <summary>
        /// 查詢一條區域信息
        /// </summary>
        /// <param name="areaId"></param>
        /// <returns></returns>
        public PageAreaQuery GetBannerByAreaId(int areaId)
        {
            try
            {
                return _beradao.GetBannerByAreaId(areaId);
            }
            catch (Exception ex)
            {

                throw new Exception("BannerAreaMgr-->AreaSave-->" + ex.Message, ex);
            }
        }


        public int UpPageAreaStatus(PageArea model)
        {
            try
            {
                return _beradao.UpPageAreaStatus(model);
            }
            catch (Exception ex)
            {
                throw new Exception("PageAreaMgr-->UpPageAreaStatus-->" + ex.Message, ex);
            }
          
        }
    }
}
