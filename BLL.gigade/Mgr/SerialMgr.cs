/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：SerialMgr 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/8/23 9:43:17 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Dao;

namespace BLL.gigade.Mgr
{
    public class SerialMgr : ISerialImplMgr
    {
        private ISerialImplDao _serialDao;
        public SerialMgr(string connectionStr)
        {
            _serialDao = new SerialDao(connectionStr);
        }

        public Serial GetSerialById(int serialId)
        {
            try
            {
                return _serialDao.GetSerialById(serialId);
            }
            catch (Exception ex)
            {

                throw new Exception("SerialMgr-->GetSerialById-->" + ex.Message, ex);
            }

        }

        public int Update(Serial serial)
        {
            try
            {
                return _serialDao.Update(serial);
            }
            catch (Exception ex)
            {

                throw new Exception("SerialMgr-->Update(Serial serial)-->" + ex.Message, ex);
            }

        }

        public string Update(int seriaId)
        {
            try
            {
                return _serialDao.Update(seriaId);
            }
            catch (Exception ex)
            {

                throw new Exception("SerialMgr--> Update(int seriaId)" + ex.Message, ex);
            }

        }

        public ulong NextSerial(int serialId)
        {
            try
            {
                Serial serial = GetSerialById(serialId);
                if (serial != null)
                {
                    serial.Serial_Value = serial.Serial_Value + 1;
                    if (Update(serial) > 0)
                    {
                        return serial.Serial_Value;
                    }
                }
            }
            catch(Exception ex)
            {
                throw new Exception("SerialMgr-->NextSerial-->" + ex.Message, ex);
            }
            return 0;
        }


        public int Insert(Serial serial)
        {
            try
            {
                return _serialDao.Insert(serial);
            }
            catch (Exception ex)
            {

                throw new Exception("SerialMgr--> Insert" + ex.Message, ex);
            }
        }
    }
}
