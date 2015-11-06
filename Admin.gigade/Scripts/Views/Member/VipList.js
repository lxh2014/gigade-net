

Ext.Loader.setConfig({ enabled: true });
Ext.Loader.setPath('Ext.ux', '/Scripts/Ext4.0/ux');
Ext.require([
    'Ext.form.Panel',
    'Ext.ux.form.MultiSelect',
    'Ext.ux.form.ItemSelector'
]);
var pageSize = 25;
//聲明grid
Ext.apply(Ext.form.field.VTypes, {
    daterange: function (val, field) {
        var date = field.parseDate(val);

        if (!date) {
            return false;
        }
        this.dateRangeMax = null;
        this.dateRangeMin = null;
        if (field.startDateField && (!this.dateRangeMax || (date.getTime() != this.dateRangeMax.getTime()))) {
            var start = field.up('form').down('#' + field.startDateField);
            start.setMaxValue(date);
            //start.validate();
            this.dateRangeMax = date;
        } else if (field.endDateField && (!this.dateRangeMin || (date.getTime() != this.dateRangeMin.getTime()))) {
            var end = field.up('form').down('#' + field.endDateField);
            end.setMinValue(date);
            //end.validate();
            this.dateRangeMin = date;
        }
        /*  
         * Always return true since we're only using this vtype to set the  
         * min/max allowed values (these are tested for after the vtype test)  
         */
        return true;
    },

    daterangeText: '開始時間必須小於結束時間'
});
Ext.define('GIGADE.VipList', {
    extend: 'Ext.data.Model',
    fields: [
                { name: "user_id", type: "string" },
                { name: "user_name", type: "string" },
                { name: "user_email", type: "string" },
                { name: "user_address", type: "string" },
                { name: "birthday", type: "string" },
                { name: "user_phone", type: "string" },
                { name: "user_mobile", type: "string" },
                { name: "mytype", type: "string" }, //1：網絡會員，2：電話會員
                { name: "send_sms_ad", type: "bool" },
                { name: "adm_note", type: "string" },
                { name: "user_gender", type: "string" }, //性別 
                { name: "vip", type: "string" },
                { name: "user_password", type: "string" },
                { name: "user_birthday_year", type: "uint" }, //年
                { name: "reg_date", type: "string" }, //註冊時間
                { name: "create_date", type: "string" }, //最近歸檔日
                { name: "sum_amount", type: "decimal" }, //購買金額
                { name: "cou", type: "int" }, //購買次數	<td><a href='/order/order_search.php?condition=3&content={{$value.user_email}}&datetime=1&date_start={{$s_date_start}}&date_end={{$s_date_end}}' target="_blank">{{$value.cou}}</a></td>
                { name: "aver_amount", type: "decimal" }, //客單價$value.sum_amount/$value.cou
                { name: "sum_bonus", type: "decimal" }, //購物金使用
                { name: "normal_product", type: "decimal" }, //常溫商品總額
                { name: "freight_normal", type: "decimal" }, //常溫商品運費
                { name: "low_product", type: "decimal" }, //低溫商品總額
                { name: "freight_low", type: "string" }, //低溫商品運費
                { name: "ct", type: "decimal" }, //中信折抵金額
                { name: "happygo", type: "decimal" }, //HG折抵
                { name: "ht", type: "decimal" } //台新折抵
                ,
                { name: "user_zip", type: "int" },
                   { name: "paper_invoice", type: "bool" },
                    { name: "ml_code", type: "string" },
                    { name: "order_product_subtotal", type: "string" }

    ]
});
//獲取grid中的數據
var VipListStore = Ext.create('Ext.data.Store', {
    pageSize: pageSize,
    model: 'GIGADE.VipList',
    proxy: {
        type: 'ajax',
        url: '/Member/GetVipList',
        timeout: 900000,
        actionMethods: 'post',
        reader: {
            type: 'json',
            totalProperty: 'totalCount',
            root: 'data'
        }
    }
});

//加載前先獲取ddl的值
VipListStore.on('beforeload', function () {
    if (Ext.getCmp('dateOne').getValue() == null && Ext.getCmp('userID').getValue()=='') {
        Ext.Msg.alert('提示信息', '請輸入查詢條件');
        return false;
    }
    Ext.apply(VipListStore.proxy.extraParams, {
        dateOne: Ext.getCmp('dateOne').getValue(),
        dateTwo: Ext.getCmp('dateTwo').getValue(),
        user_id: Ext.getCmp('userID').getValue()
    })

});
//群組管理Model 
Ext.define('gigade.Users', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "user_id", type: "int" }, //用戶編號     上面的是編輯的時候關係到的
        { name: "user_email", type: "string" }, //用戶郵箱
        { name: "user_name", type: "string" }, //用戶名
        { name: "user_password", type: "string" }, //密碼
        { name: "user_gender", type: "string" }, //性別
        { name: "user_birthday_year", type: "string" }, //年
        { name: "user_birthday_month", type: "string" }, //月
        { name: "user_birthday_day", type: "string" }, //日
        { name: "birthday", type: "string" }, //生日 
        { name: "user_zip", type: "string" }, //用戶地址
        { name: "user_address", type: "string" }, //用戶地址
        { name: "user_mobile", type: "string" },
        { name: "user_phone", type: "string" }, //行動電話
        { name: "reg_date", type: "string" }, //註冊日期 
        { name: "mytype", type: "string" },//會員類別
        { name: "send_sms_ad", type: "bool" }, //是否接收簡訊廣告 
        { name: "adm_note", type: "string" }, //管理員備註   上面這些編輯時要帶入的值
        { name: "user_type", type: "string" }, //用戶類別   下面的這些結合上面的會顯示在列表頁
        { name: "user_status", type: "string" }, //用戶狀態
        { name: "sfirst_time", type: "string" }, //首次註冊時間
        { name: "slast_time", type: "string" }, //下次時間
        { name: "sbe4_last_time", type: "string" }, //下下次時間
        { name: "user_company_id", type: "string" },
        { name: "user_source", type: "string" },
        { name: "source_trace", type: "string" },
        { name: "s_id", type: "string" },
        { name: "source_trace_url", type: "string" },
        { name: "redirect_name", type: "string" },
        { name: "redirect_url", type: "string" },
        { name: "paper_invoice", type: "bool" },
        { name: "ml_code", type: "string" },
        { name: "order_product_subtotal", type: "string" }
    ]
});

//用作編輯時獲得數據包含機敏信息
var edit_UserStore = Ext.create('Ext.data.Store', {
    //  autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.Users',
    proxy: {
        type: 'ajax',
        url: '/Member/UsersList',
        reader: {
            type: 'json',
            root: 'data',//在執行成功后。顯示數據。所以record.data.用戶字段可以直接讀取
            totalProperty: 'totalCount'
        }
    }
});


Ext.onReady(function () {

    //頁面加載時創建grid
    var VipListGrid = Ext.create('Ext.grid.Panel', {
        id: 'VipListGrid',
        store: VipListStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [

            { header: USERID, dataIndex: 'user_id', width: 60, align: 'center' },
            {
                header: USERNAME, dataIndex: 'user_name', width: 100, align: 'center'
                ,
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    //return "<a href='javascript:void(0);'  onclick='UpdateUser()'>" + value + "</a>";
                    return "<a href='javascript:void(0);' onclick='SecretLogin(" + record.data.user_id + ")'  >" + value + "</a>";
                }
            },
            {
                header: USERGENDER, dataIndex: 'user_gender', width: 80, align: 'center',
                renderer: function (val) {
                    return val == "1" ? MAN : WOMAN;
                }
            },
            { header: "vip", dataIndex: 'vip', width: 50, align: 'center' },
            { header: "會員等級", dataIndex: 'ml_code', width: 60, align: 'center' },
            //{ header: "等級購買金額", dataIndex: 'order_product_subtotal', width: 100, align: 'center' },
            {
                header: BIRTHDAY, dataIndex: 'user_birthday_year', width: 100, align: 'center',
                renderer: function (val) {
                    var year = new Date().getFullYear();
                    return val == "" ? "" : (year - val).toString();
                }
            },

            { header: REGDATE, dataIndex: 'reg_date', width: 150, align: 'center' },
            { header: CREATEDATE, dataIndex: 'create_date', width: 150, align: 'center' },
            { header: SUMAMOUNT, dataIndex: 'sum_amount', width: 60, align: 'center' },
            {
                header: COU, dataIndex: 'cou', width: 60, align: 'center'
                //,
                //                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                //                    //href='/order/order_search.php?condition=3&content={{$value.user_email}}&datetime=1&date_start={{$s_date_start}}&date_end={{$s_date_end}}'鏈接至訂單查詢
                //                   // return Ext.String.format('<a href="/OrderSearch/Index?selectType=3&searchCon={0}&timeOne=1&dateOne={1}&dateTwo={2}" target="_self">{3}</a>', record.data.user_email, Ext.getCmp('dateOne').getValue(), Ext.getCmp('dateTwo').getValue(), value);
                //                }
            },
            {
                header: AVERAMOUNT, dataIndex: 'aver_amount', width: 60, align: 'center'

            },
            { header: SUMBONUS, dataIndex: 'sum_bonus', width: 80, align: 'center' },
            { header: NORMALPROD, dataIndex: 'normal_product', width: 80, align: 'center' },
            { header: FREIGHTNORMAL, dataIndex: 'freight_normal', width: 80, align: 'center' },
            { header: LOWPROD, dataIndex: 'low_product', width: 80, align: 'center' },
            { header: FREIGTLOW, dataIndex: 'freight_low', width: 80, align: 'center' },
            { header: CT, dataIndex: 'ct', width: 80, align: 'center' },
            { header: HAPPYGO, dataIndex: 'happygo', width: 80, align: 'center' },
            { header: HT, dataIndex: 'ht', width: 80, align: 'center' }
        ],
        tbar: [
            {
                text: '匯出會員購買記錄排行CSV',
                handler: ExportCsv,
                iconCls: 'icon-excel',
            },
           '->',
           {
               xtype: 'textfield',
               fieldLabel: '會員編號',
               id: 'userID',
               name: 'userID',
               regex: /^[0-9]*[1-9][0-9]*$/,
               regexText: '請輸入數字類型的字符',
               allowBlank: true,
              
               emptyText: '請輸入會員編號(可以為空)',
               listeners: {
                   specialkey: function (field, e) {
                       if (e.getKey() == e.ENTER) {
                           Query();
                       }
                   },
                   change: function () {
                       if (!(/^[0-9]*[1-9][0-9]*$/).test(this.getValue())) {
                           if (this.getValue().trim() == '') {
                               Ext.getCmp('btnQuery').setDisabled(false);
                           }
                           else {
                               Ext.getCmp('btnQuery').setDisabled(true);
                           }                           
                       }
                       else {                          
                           Ext.getCmp('btnQuery').setDisabled(false);
                       }                                            
                   }
               }
           },
          {
              xtype: "datefield",
              fieldLabel: "歸檔訂單創建日期",
              id: 'dateOne',
              name: 'dateOne',
              format: 'Y-m-d',
              allowBlank: true,
              editable: false,            
              submitValue: true,
              //value: new Date(Tomorrow().setMonth(Tomorrow().getMonth() - 1)),
              listeners: {
                  select: function (a, b, c) {
                      var start = Ext.getCmp("dateOne");
                      var end = Ext.getCmp("dateTwo");
                      if (end.getValue() == null) {
                          end.setValue(setNextMonth(start.getValue(), 1));
                      }
                      else if (end.getValue() < start.getValue()) {
                          Ext.Msg.alert(INFORMATION, DATA_TIP);
                          end.setValue(setNextMonth(start.getValue(), 1));
                      }
                      //else if (end.getValue() > setNextMonth(start.getValue(), 1)) {
                      //    // Ext.Msg.alert(INFORMATION, DATE_LIMIT);
                      //    end.setValue(setNextMonth(start.getValue(), 1));
                      //}
                  }
              }
          }, '~', {
              xtype: "datefield",
              format: 'Y-m-d',
              id: 'dateTwo',
              name: 'dateTwo',
              allowBlank: true,
              editable: false,
              submitValue: true,
              //value: Tomorrow(),
              listeners: {
                  select: function (a, b, c) {
                      var start = Ext.getCmp("dateOne");
                      var end = Ext.getCmp("dateTwo");
                      if (start.getValue() != "" && start.getValue() != null) {
                          if (end.getValue() < start.getValue()) {
                              Ext.Msg.alert(INFORMATION, DATA_TIP);
                              start.setValue(setNextMonth(end.getValue(), -1));
                          }
                          //else if (end.getValue() > setNextMonth(start.getValue(), 1)) {
                          //    // Ext.Msg.alert(INFORMATION, DATE_LIMIT);
                          //    start.setValue(setNextMonth(end.getValue(), -1));
                          //}
                      }
                      else {
                          start.setValue(setNextMonth(end.getValue(), -1));
                      }
                  }
              }
          },
            {
                text: SEARCH,
                iconCls: 'icon-search',
                id: 'btnQuery',
                disabled: false,
                handler: Query
            },
            {
                text: '重置',
                iconCls: 'ui-icon ui-icon-reset',
                id: 'btnReset',
                disabled: false,
                handler: 
                    function () {
                        Ext.getCmp('userID').setValue('');
                        Ext.getCmp('dateOne').reset();
                        Ext.getCmp('dateTwo').reset();
                    }
            }
        ],
        listeners: {
            scrollershow: function (scroller) {
                if (scroller && scroller.scrollEl) {
                    scroller.clearManagedListeners();
                    scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                }
            }
        },
        bbar: Ext.create('Ext.PagingToolbar', {
            store: VipListStore,
            pageSize: pageSize,
            displayInfo: true,
            displayMsg: NOW_DISPLAY_RECORD + ': {0} - {1}' + TOTAL + ': {2}',
            emptyMsg: NOTHING_DISPLAY
        })
    });


    Ext.create('Ext.container.Viewport', {
        layout: 'fit',
        items: [VipListGrid],
        renderTo: Ext.getBody(),
        autoScroll: false,
        listeners: {
            resize: function () {
                VipListGrid.width = document.documentElement.clientWidth;
                this.doLayout();

            }
        }
    });
    ToolAuthority();
   // VipListStore.load({ params: { start: 0, limit: 25 } });

})


function SecretLogin(rid) {//secretcopy
    var secret_type = "1";//參數表中的"會員查詢列表"
    var url = "/Member/VipList ";
    var ralated_id = rid;
    //點擊機敏信息先保存記錄在驗證密碼是否需要輸入
    boolPassword = SaveSecretLog(url, secret_type, ralated_id);//判斷5分鐘之內是否有輸入密碼
    if (boolPassword != "-1") {//不准查看
        if (boolPassword) {//超過5分鐘沒有輸入密碼
            //參數1：機敏頁面代碼，2：機敏資料主鍵，3：是否彈出驗證密碼框,4：是否直接顯示機敏信息6.驗證通過后是否打開編輯窗口
            //  function SecretLoginFun(type, relatedID, isLogin, isShow, editO, isEdit) {
            SecretLoginFun(secret_type, ralated_id, true, false, true, url, "", "", "");//先彈出驗證框，關閉時在彈出顯示框

        } else {
            editFunction(ralated_id, edit_UserStore);
        }
    }
}
//function UpdateUser() {

//    var row = Ext.getCmp("VipListGrid").getSelectionModel().getSelection();
//    if (row.length == 0) {
//        Ext.Msg.alert(INFORMATION, NO_SELECTION);
//    }
//    else if (row.length > 1) {
//        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
//    } else if (row.length == 1) {
//        editFunction(row[0].data.user_id);
//    }

//}
setNextMonth = function (source, n) {
    var s = new Date(source);
    s.setMonth(s.getMonth() + n);
    if (n < 0) {
        s.setHours(0, 0, 0);
    }
    else if (n > 0) {
        s.setHours(23, 59, 59);
    }
    return s;
}
function Tomorrow() {
    var d;
    d = new Date();                             // 创建 Date 对象。                               // 返回日期。
    d.setDate(d.getDate() + 1);
    return d;
}

//查询
Query = function () {
    if (!(/^[0-9]*[1-9][0-9]*$/).test(Ext.getCmp('userID').getValue())) {
        if (Ext.getCmp('userID').getValue().trim() != '') {
            Ext.Msg.alert('提示信息','請輸入正確格式的會員編號！');
            return false;
        }        
    }
    if (Ext.getCmp('userID').getValue().trim() == '' && Ext.getCmp('dateOne').getValue() == null)
    {
        Ext.Msg.alert('提示信息', '請輸入查詢條件');
        return false;
    } 
    VipListStore.removeAll();
    Ext.getCmp("VipListGrid").store.loadPage(1, {
        params: {
            user_id: Ext.getCmp('userID').getValue(),
            dateOne: Ext.getCmp('dateOne').getValue(),
            dateTwo: Ext.getCmp('dateTwo').getValue()
        }
    });
}
function ExportCsv() {
    Ext.MessageBox.show({
        msg: '正在匯出，請稍後....',
        width: 300,
        wait: true
    });
    Ext.Ajax.request({
        url: '/Member/ExportVipListCsv',
        timeout: 600000,
        params: {
            dateOne: Ext.getCmp('dateOne').getValue(),
            dateTwo: Ext.getCmp('dateTwo').getValue(),
            user_id: Ext.getCmp('userID').getValue()
        },
        success: function (response) {
            Ext.MessageBox.hide();
            var result = eval("(" + response.responseText + ")");
            if (result.success == "true") {
                var url = Ext.String.format('<a href=../../ImportUserIOExcel/{0}>點此下載</a>', result.filename);
                Ext.MessageBox.alert("下載提示", "" + url + "");
            }
            else {
                Ext.Msg.alert("提示信息", "下載出錯");
            }
        }
    });
}
