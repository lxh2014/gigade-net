﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
namespace BLL.gigade.Dao.Impl
{
   public interface IAreaPacketImplDao
    {
       List<AreaPacket> QueryAll(AreaPacket query, out int totalCount);
       int AreaPacketSave(AreaPacket ap);
       AreaPacket GetModelById(AreaPacket model);
       int UpAreaPacketStatus(AreaPacket model);
       List<AreaPacket> GetPacket(int element_type);
       bool SelectCount(int packetId);
    }
}
