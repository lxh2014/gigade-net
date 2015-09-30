/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：SerialDao 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/8/23 9:42:07 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model;
using DBAccess;
using System.Data;

namespace BLL.gigade.Dao
{
    public class SerialDao : ISerialImplDao
    {
        private IDBAccess _dbAccess;
        public SerialDao(string connectionStr)
        {
            _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectionStr);
        }

        public Serial GetSerialById(int serialId)
        {
            StringBuilder strSql = new StringBuilder("select serial_id,serial_value,serial_description from serial");
            strSql.AppendFormat(" where serial_id={0} ;", serialId);
            return _dbAccess.getSinggleObj<Serial>(strSql.ToString());
        }

        public int Update(Serial serial)
        {
            StringBuilder strSql = new StringBuilder("update serial set ");
            strSql.AppendFormat("serial_value={0} where serial_id={1} ;", serial.Serial_Value, serial.Serial_id);
            return _dbAccess.execCommand(strSql.ToString());
        }

        public string Update(int seriaId)
        {
            return string.Format("update serial set serial_value=serial_value+1 where serial_id={0};select serial_value from serial where serial_id={0};", seriaId);
        }

        /// <summary>
        /// 用於返回事物所用到的sql語句
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public string UpdateAutoIncreament(Serial serial)
        {
            StringBuilder strSql = new StringBuilder("update serial set ");
            strSql.AppendFormat("serial_value={0} where serial_id={1};", serial.Serial_Value, serial.Serial_id);
            return strSql.ToString();
        }
        public int Insert(Serial serial)
        {

            StringBuilder strSql = new StringBuilder();
            try
            {
                strSql.AppendFormat("insert into serial(serial_value,serial_id)values('{0}','{1}') ;", serial.Serial_Value, serial.Serial_id);
                return _dbAccess.execCommand(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("SerialDao.Insert-->" + ex.Message + strSql.ToString(), ex);
            }
        }
        public string InsertStr(Serial serial)
        {

            StringBuilder strSql = new StringBuilder();
            try
            {
                strSql.AppendFormat("insert into serial(serial_value,serial_id)values('{0}','{1}');", serial.Serial_Value, serial.Serial_id);
                return strSql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("SerialDao.InsertStr-->" + ex.Message + strSql.ToString(), ex);
            }
        }
    }
}
