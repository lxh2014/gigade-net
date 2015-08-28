﻿/* 
* Copyright (c) 2013，武漢聯綿信息技術有限公司鄭州分公司 
* All rights reserved. 
* 
* 文件名称：PromotionsBonusSerialDao.cs
* 摘 要：
* 序號兌換
* 当前版本：v1.1
* 作 者：dongya0410j    
* 完成日期：2014/6/20 
* 修改歷史:
*         v1.1修改日期：2014/8/15 
*         v1.1修改人員：dongya0410j
*         v1.1修改内容：在抛出異常的時候將Sql語句抛出，合并代碼，添加注釋 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using DBAccess;
using BLL.gigade.Model;
using System.Data;

namespace BLL.gigade.Dao
{
    public class PromotionsBonusSerialDao : IPromotionsBonusSerialImplDao
    {
        private IDBAccess _access;
        string strSql = string.Empty;
        public PromotionsBonusSerialDao(string connectionString)
        {
            // TODO: Complete member initialization  this
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }
        #region QueryById +List<Model.PromotionsBonusSerial> QueryById(int id)
        public List<Model.PromotionsBonusSerial> QueryById(int id)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat("SELECT `PromotionBonusSerial`.`id`, `PromotionBonusSerial`.`promotion_id`, `PromotionBonusSerial`.`serial`, `PromotionBonusSerial`.`active` FROM `promotions_bonus_serial` AS `PromotionBonusSerial` WHERE `PromotionBonusSerial`.`promotion_id`='{0}'", id);
                return _access.getDataTableForObj<PromotionsBonusSerial>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsBonusSerialDao-->QueryById-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion

        #region 保存 +int Save(string serials, int id)
        public int Save(string serials, int id)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat("insert into promotions_bonus_serial(serial,active,promotion_id)values('{0}','{1}','{2}')", serials, 1, id);//這裡先默認一個值
                return _access.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsBonusSerialDao-->Save-->" + ex.Message + sb.ToString(), ex);
            }
        }
        #endregion
        /// <summary>
        /// 檢查該序號是否已經存在
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public List<Model.PromotionsBonusSerial> YesOrNoExist(string str)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat("select count(1),serial from promotions_bonus_serial where promotion_id='{0}'", str);
                return _access.getDataTableForObj<PromotionsBonusSerial>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PromotionsBonusSerialDao-->YesOrNoExist-->" + ex.Message + sb.ToString(), ex);
            }
        }
    }
}
