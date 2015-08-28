using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Mgr;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Dao;
using BLL.gigade.Model;
namespace BLL.gigade.Mgr
{
    public class AreaPacketMgr : IAreaPactetImplMgr
    {
        private IAreaPacketImplDao _areaPacketDao;
        public AreaPacketMgr(string connectionString)
        {
            _areaPacketDao = new AreaPacketDao(connectionString);
        }
        public List<Model.AreaPacket> QueryAll(Model.AreaPacket query, out int totalCount)
        {
            return _areaPacketDao.QueryAll(query, out totalCount);
        }
        public int AreaPacketSave(AreaPacket ap)
        {
            return _areaPacketDao.AreaPacketSave(ap);
        }
        public AreaPacket GetModelById(AreaPacket model)
        {
            return _areaPacketDao.GetModelById(model);
        }
        public int UpAreaPacketStatus(AreaPacket model)
        {
            return _areaPacketDao.UpAreaPacketStatus(model);
        }


        public List<AreaPacket> GetPacket(int element_type)
        {
            try
            {
                return _areaPacketDao.GetPacket(element_type);
            }
            catch (Exception ex)
            {
                throw new Exception("AreaPacketMgr.GetPacket-->" + ex.Message, ex);
            }
        }


        public bool SelectCount(int packetId)
        {
            try
            {
                return _areaPacketDao.SelectCount(packetId);
            }
            catch (Exception ex)
            {
                throw new Exception("AreaPacketMgr-->SelectCount-->" + ex.Message, ex);
            }
        }
    }
}
