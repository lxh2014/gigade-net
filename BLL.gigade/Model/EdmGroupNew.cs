using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
 public  class EdmGroupNew:PageBase
    {
        public int group_id { get; set; }// 電子報群組代碼
        public string group_name { get; set; }//群組名稱
        public string description { get; set; }//群組描述文字
        public int is_member_edm { get; set; }//指定該群組是不是會員訂閱電子報，設定為True時，該群組將會顯示在會員中心電子報管理畫面中，讓會員自行選擇是否要訂閱。
        public int enabled { get; set; }//設定群組是否啟用。如果不啟用，即使is_member_edm為True，也不會顯示在會員中心電子報管理畫面；同時也不會顯示在電子報管理後台。
        public int sort_order { get; set; }//群組排序。當is_member_edm為True時，該群組會顯示在會員中心的電子報訂閱畫面，此時採用這個值來決定顯示的排序。
        public DateTime group_createdate { get; set; }
        public DateTime group_updatedate { get; set; }
        public int group_create_userid { get; set; }
        public int group_update_userid { get; set; }

        public EdmGroupNew()
        {
            group_id = 0;
            group_name = string.Empty;
            description = string.Empty;
            is_member_edm = 0;
            enabled = 0;
            sort_order = 0;
            group_createdate=DateTime.Now ;
            group_updatedate= DateTime.Now;
            group_create_userid=0;
            group_update_userid=0;
        }
    }

}
