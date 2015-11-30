/*
* 文件名稱 :EDM
* 文件功能描述 :EDM條件篩選條件表功能
* 版權宣告 :
* 開發人員 : jialei,jiaohe
* 版本資訊 : 1.0
* 創建日期 : 2015/07/21
* 修改人員 :
* 版本資訊 : 
* 修改日期 : 2015/07/21
* 修改備註 : 
	*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBAccess;
using BLL.gigade.Model.Query;
using BLL.gigade.Common;
using BLL.gigade.Model;

namespace BLL.gigade.Dao
{
    public class EdmListConditoinSubDao
    {
        private IDBAccess _dbAccess;
        private string connStr;

        public EdmListConditoinSubDao(string connectionStr)
        {
            _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectionStr);
            this.connStr = connectionStr;
        }

        /// <summary>
        /// 儲存篩選條件內容
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public int SaveListInfoCondition(EdmListConditoinSubQuery query)
        {
            query.Replace4MySQL();
            StringBuilder sql = new StringBuilder();
            try
            {
                if (query.chkGender == true)
                {
                    sql.AppendFormat(@"INSERT INTO edm_list_conditoin_sub (elcm_id,elcs_key,elcs_value1) VALUES({0},'gender','{1}');", query.elcm_id, query.genderCondition);
                }

                if (query.ChkBuy == true)
                {
                    if (query.buyTimeMin != DateTime.MinValue && query.buyTimeMax != DateTime.MinValue)
                    {
                        sql.AppendFormat(@"INSERT INTO edm_list_conditoin_sub (elcm_id,elcs_key,elcs_value1,elcs_value2,elcs_value3,elcs_value4) VALUES({0},'buy_times','{1}','{2}','{3}','{4}');", query.elcm_id, query.buyCondition, query.buyTimes, CommonFunction.DateTimeToString(query.buyTimeMin), CommonFunction.DateTimeToString(query.buyTimeMax));
                    }
                    else
                    {
                        sql.AppendFormat(@"INSERT INTO edm_list_conditoin_sub (elcm_id,elcs_key,elcs_value1,elcs_value2) VALUES({0},'buy_times','{1}','{2}');", query.elcm_id, query.buyCondition, query.buyTimes);
                    }
                }

                if (query.ChkAge == true)
                {
                    sql.AppendFormat(@"INSERT INTO edm_list_conditoin_sub (elcm_id,elcs_key,elcs_value1,elcs_value2) VALUES({0},'age','{1}','{2}');", query.elcm_id, query.ageMin, query.ageMax);
                }

                if (query.ChkCancel == true)
                {
                    if (query.cancelTimeMin != DateTime.MinValue && query.cancelTimeMax != DateTime.MinValue)
                    {
                        sql.AppendFormat(@"INSERT INTO edm_list_conditoin_sub (elcm_id,elcs_key,elcs_value1,elcs_value2,elcs_value3,elcs_value4) VALUES({0},'cancel_times','{1}','{2}','{3}','{4}');", query.elcm_id, query.cancelCondition, query.cancelTimes, CommonFunction.DateTimeToString(query.cancelTimeMin), CommonFunction.DateTimeToString(query.cancelTimeMax));
                    }
                    else
                    {
                        sql.AppendFormat(@"INSERT INTO edm_list_conditoin_sub (elcm_id,elcs_key,elcs_value1,elcs_value2) VALUES({0},'cancel_times','{1}','{2}');", query.elcm_id, query.cancelCondition, query.cancelTimes);
                    }
                }

                if (query.ChkRegisterTime == true)
                {
                    if (query.registerTimeMin != DateTime.MinValue && query.registerTimeMax != DateTime.MinValue)
                    {
                        sql.AppendFormat(@"INSERT INTO edm_list_conditoin_sub (elcm_id,elcs_key,elcs_value1,elcs_value2) VALUES({0},'register_time','{1}','{2}');", query.elcm_id, CommonFunction.DateTimeToString(query.registerTimeMin), CommonFunction.DateTimeToString(query.registerTimeMax));
                    }
                    else
                    {
                        sql.AppendFormat(@"INSERT INTO edm_list_conditoin_sub (elcm_id,elcs_key,elcs_value1,elcs_value2) VALUES({0},'register_time','','');", query.elcm_id);
                    }
                }

                if (query.ChkReturn == true)
                {
                    if (query.returnTimeMin != DateTime.MinValue)
                    {
                        sql.AppendFormat(@"INSERT INTO edm_list_conditoin_sub (elcm_id,elcs_key,elcs_value1,elcs_value2,elcs_value3,elcs_value4) VALUES({0},'return_times','{1}','{2}','{3}','{4}');", query.elcm_id, query.returnCondition, query.returnTimes, CommonFunction.DateTimeToString(query.returnTimeMin), CommonFunction.DateTimeToString(query.returnTimeMax));
                    }
                    else
                    {
                        sql.AppendFormat(@"INSERT INTO edm_list_conditoin_sub (elcm_id,elcs_key,elcs_value1,elcs_value2) VALUES({0},'return_times','{1}','{2}');", query.elcm_id, query.returnCondition, query.returnTimes);
                    }
                }

                if (query.ChkLastOrder == true)
                {
                    if (query.lastOrderMin != DateTime.MinValue && query.lastOrderMax != DateTime.MinValue)
                    {
                        sql.AppendFormat(@"INSERT INTO edm_list_conditoin_sub (elcm_id,elcs_key,elcs_value1,elcs_value2) VALUES({0},'last_order','{1}','{2}');", query.elcm_id, CommonFunction.DateTimeToString(query.lastOrderMin), CommonFunction.DateTimeToString(query.lastOrderMax));
                    }
                    else 
                    {
                        sql.AppendFormat(@"INSERT INTO edm_list_conditoin_sub (elcm_id,elcs_key,elcs_value1,elcs_value2) VALUES({0},'last_order','','');", query.elcm_id);
                    }
                }

                if (query.ChkNotice == true)
                {
                    sql.AppendFormat(@"INSERT INTO edm_list_conditoin_sub (elcm_id,elcs_key,elcs_value1,elcs_value2) VALUES({0},'replenishment_info','{1}','{2}');", query.elcm_id, query.noticeCondition, query.noticeTimes);
                }

                if (query.ChkLastLogin == true)
                {
                    if (query.lastLoginMin != DateTime.MinValue && query.lastLoginMax != DateTime.MinValue)
                    {
                        sql.AppendFormat(@"INSERT INTO edm_list_conditoin_sub (elcm_id,elcs_key,elcs_value1,elcs_value2) VALUES({0},'last_login','{1}','{2}');", query.elcm_id, CommonFunction.DateTimeToString(query.lastLoginMin), CommonFunction.DateTimeToString(query.lastLoginMax));
                    }
                    else
                    {
                        sql.AppendFormat(@"INSERT INTO edm_list_conditoin_sub (elcm_id,elcs_key,elcs_value1,elcs_value2) VALUES({0},'last_login','','');", query.elcm_id);

                    }
                }

                if (query.ChkTotalConsumption == true)
                {
                    if (query.totalConsumptionMin != 0 && query.totalConsumptionMax != 0)
                    {
                        sql.AppendFormat(@"INSERT INTO edm_list_conditoin_sub (elcm_id,elcs_key,elcs_value1,elcs_value2) VALUES({0},'total_consumption','{1}','{2}');", query.elcm_id, query.totalConsumptionMin, query.totalConsumptionMax);
                    }
                    else
                    {
                        sql.AppendFormat(@"INSERT INTO edm_list_conditoin_sub (elcm_id,elcs_key,elcs_value1,elcs_value2) VALUES({0},'total_consumption','','');", query.elcm_id);                     
                    }
                }
                if (query.ChkBlackList == true)
                {
                    sql.AppendFormat(@"INSERT INTO edm_list_conditoin_sub (elcm_id,elcs_key,elcs_value1) VALUES({0},'black_list','{1}');", query.elcm_id, query.ChkBlackList);
                }
                if (query.ChkPhone == true)
                {
                    sql.AppendFormat(@"INSERT INTO edm_list_conditoin_sub (elcm_id,elcs_key,elcs_value1) VALUES({0},'phone','{1}');", query.elcm_id, query.ChkPhone);
                }
                return _dbAccess.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(" EdmListConditoinSubDao-->SaveListInfoCondition " + ex.Message, ex);
            }
        }

        public List<EdmListConditoinSub> LoadCondition(EdmListConditoinSubQuery query)
        {
            query.Replace4MySQL();
            StringBuilder sql = new StringBuilder();
            List<EdmListConditoinSub> store = new List<EdmListConditoinSub>();
            try
            {
                sql.AppendFormat(@"SELECT elcs_id,elcm_id,elcs_key,elcs_value1,elcs_value2,elcs_value3,elcs_value4 from edm_list_conditoin_sub WHERE elcm_id={0};", query.elcm_id);
                store = _dbAccess.getDataTableForObj<EdmListConditoinSub>(sql.ToString());
                return store;
            }
            catch (Exception ex)
            {
                throw new Exception(" EdmListConditoinSubDao-->LoadCondition " + ex.Message, ex);
            }
        }

        public int DeleteInfo(EdmListConditionMain query)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.AppendFormat(@"DELETE from edm_list_conditoin_sub WHERE elcm_id={0}; ", query.elcm_id);               
                return _dbAccess.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(" EdmListConditionMainDao-->DeleteListInfo " + ex.Message, ex);
            }
        }
    }
}
