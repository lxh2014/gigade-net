/*
jialei0706h 
datatime:20140922
*/
var CallidForm;
var pageSize = 28;
var boolPassword = true;//secretcopy
var info_type = "users";
var secret_info = "";
/**********************************************************************群組管理主頁面**************************************************************************************/
//群組管理Model
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
Ext.define('gigade.Fares', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "id", type: "int" },
        { name: "user_id", type: "int" },
        { name: "name", type: "string" },
        { name: "mail", type: "string" },
        { name: "user_name", type: "string" },
        { name: "user_gender", type: "string" },
        { name: "usname", type: "string" },//推薦人姓名
    //        { name: "Iuser_reg_date", type: "string" },
        { name: "user_birthday_year", type: "string" },
        { name: "user_birthday_month", type: "string" }, //月
        { name: "user_birthday_day", type: "string" }, //日
        { name: "user_ip", type: "string" },
        { name: "recommend_user_id", type: "string" },
    //        { name: "recommend_user_ip", type: "string" },
        { name: "createtime", type: "string" },
        { name: "user_zip", type: "string" }, //用戶地址
        { name: "user_email", type: "string" },
    //        { name: "user_status", type: "string" }, //用戶狀態
        { name: "suser_reg_date", type: "string" }, //註冊日期
    ////        {name: "sfirst_time", type: "string" }, //首次註冊時間
    //        {name: "slast_time", type: "string" }, //下次時間
    //        {name: "sbe4_last_time", type: "string" }, //下下次時間
        { name: "birthday", type: "string" }, //生日 
        { name: "user_address", type: "string" }, //用戶地址
        { name: "user_password", type: "string" }, //密碼
        { name: "send_sms_ad", type: "bool" }, //是否接收簡訊廣告
          { name: "paper_invoice", type: "bool" },
        { name: "user_reg_date", type: "string" }, //註冊日期
        { name: "adm_note", type: "string" }, //管理員備註
        { name: "mytype", type: "string" },
        { name: "user_phone", type: "string" },
        { name: "user_mobile", type: "string" },
        { name: "u_user_name", type: "string" }
    //        ,
    //        { name: "user_company_id", type: "string" },
    //        { name: "user_source", type: "string" },
    //        { name: "source_trace", type: "string" },
    //        { name: "s_id", type: "string" },
    //        { name: "source_trace_url", type: "string" }, //貌似可以不要
    //        {name: "redirect_name", type: "string" }
    //        ,
    //        { name: "redirect_url", type: "string" }
    ]
});

var FaresStore = Ext.create('Ext.data.Store', {
    autoDestroy: true, //自動消除
    pageSize: pageSize,
    model: 'gigade.Fares',
    proxy: {
        type: 'ajax',
        url: '/Member/Recommendlist',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'//總行數
        }
    }
});
var DDLStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
        { "txt": TEXPIRED, "value": "0" },
        { "txt": NOTPASTDUE, "value": "1" }
    ]
});

FaresStore.on('beforeload', function () {
    Ext.apply(FaresStore.proxy.extraParams, {
        ddlSel: Ext.getCmp('ddlSel').getValue(),
        ddlCon: Ext.getCmp('ddlCon').getValue(),
        start_date: Ext.getCmp('start_date').getValue(),
        end_date: Ext.getCmp('end_date').getValue()
    });
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
        { name: "paper_invoice", type: "bool" }
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
function Query(x) {
    FaresStore.removeAll();
    var ddlSel = Ext.getCmp('ddlSel').getValue();
    var ddlCon = Ext.getCmp('ddlCon').getValue();
    var start = Ext.getCmp('start_date').getValue();
    var end = Ext.getCmp('end_date').getValue();    
    if (ddlSel != null && ddlCon == "") {
        Ext.Msg.alert(INFORMATION, '請選擇查詢條件');
    }
    else if ((start == null && end == null) && (ddlSel == null || ddlCon == "")){
        Ext.Msg.alert(INFORMATION, '請選擇查詢條件');
    }
    else {
        Ext.getCmp("gdFgroup").store.loadPage(1, {
            params: {
                ddlSel: Ext.getCmp('ddlSel').getValue(),
                ddlCon: Ext.getCmp('ddlCon').getValue(),
                start_date: Ext.getCmp('start_date').getValue(),
                end_date: Ext.getCmp('end_date').getValue(),
                //add 添加查詢條件 全部/被推薦人已加入
                recommend: Ext.htmlEncode(Ext.getCmp("rd_recommend").getValue().rd_Recommend)
            }
        });
    }
}     

Ext.onReady(function () {
    var gdFgroup = Ext.create('Ext.grid.Panel', {
        id: 'gdFgroup',
        store: FaresStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
            {
                header: NAME, dataIndex: 'name', width: 150, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (record.data.user_id != 0) {
                        return "<a href='javascript:void(0);' onclick='SecretLogin(" + record.data.id + "," + record.data.user_id + ",false)'  >" + value + "</a>";
                    }
                    else {
                        return "<a href='javascript:void(0);' onclick='SecretLogin(" + record.data.id + ",0,false)'  >" + value + "</a>";
                    }

                }
            },
            {
                header: EMAIL, dataIndex: 'mail', width: 250, align: 'center'
            ,
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    return "<span onclick='SecretLogin(" + record.data.id + ",0,false)'  >" + value + "</span>";

                }
            },
            {
                header: REFERRERNAME, dataIndex: 'usname', width: 150, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    return "<span onclick='SecretLogin(" + record.data.id + ",0,true)'  >" + value + "</span>";

                }
            },
            { header: CREATETIME, dataIndex: 'createtime', flex: 1, align: 'center' }
        ],
        tbar: [
            {
                xtype: 'combobox',
                editable: false,
                fieldLabel: DDLSEL,
                labelWidth: 70,
                id: 'ddlSel',
                store: DDLStore,
                displayField: 'txt',
                valueField: 'value',
                // value: '0'
                emptyText: '請選擇...'
            },
            {
                xtype: 'textfield',
                id: 'ddlCon',
                name: 'ddlCon',
                fieldLabel: DDLCON,
                labelWidth: 80,
                listeners: {
                    focus: function () {
                        var searchType = Ext.getCmp("ddlSel").getValue();
                        if (searchType == null || searchType == '' || searchType == '0') {
                            Ext.Msg.alert("提示信息", "請先選則搜索類型");
                        }
                    },
                    specialkey: function (field, e) {
                        if (e.getKey() == e.ENTER) {
                            Query();
                        }
                    }
                }
            },
            {
                xtype: "datefield",
                fieldLabel: CREATETIME,
                id: 'start_date',
                name: 'start_date',
                format: 'Y-m-d',
                width: 250,
                submitValue: true,                              
                editable: false,
                allowBlank: true,
               // value:new Date(Tomorrow().setMonth(Tomorrow().getMonth() - 1)),
                listeners: {
                    select: function (a, b, c) {
                        var start = Ext.getCmp("start_date");
                        var end = Ext.getCmp("end_date");
                        if (end.getValue() == null) {
                            end.setValue(setNextMonth(start.getValue(), 1));
                        }
                        else if (end.getValue() < start.getValue()) {
                            Ext.Msg.alert(INFORMATION, DATA_TIP);
                            end.setValue(setNextMonth(start.getValue(), 1));
                        }
                        else if (end.getValue() > setNextMonth(start.getValue(), 1)) {
                            // Ext.Msg.alert(INFORMATION, DATE_LIMIT);
                            end.setValue(setNextMonth(start.getValue(), 1));
                        }
                    }
                }
            },
            { xtype: 'displayfield', value: "~", margin: '0' },
            {
                xtype: "datefield",
                id: 'end_date',
                name: 'end_date',
                format: 'Y-m-d',
                labelWidth: 55,
                submitValue: true,
                editable: false,
                allowBlank: true,
                //value: Tomorrow(),
                listeners: {
                    select: function (a, b, c) {
                        var start = Ext.getCmp("start_date");
                        var end = Ext.getCmp("end_date");
                        if (start.getValue() != "" && start.getValue() != null) {
                            if (end.getValue() < start.getValue()) {
                                Ext.Msg.alert(INFORMATION, DATA_TIP);
                                start.setValue(setNextMonth(end.getValue(), -1));
                            }
                            else if (end.getValue() > setNextMonth(start.getValue(), 1)) {
                                // Ext.Msg.alert(INFORMATION, DATE_LIMIT);
                                end.setValue(setNextMonth(start.getValue(), 1));
                            }
                        }
                        else {
                            start.setValue(setNextMonth(end.getValue(), -1));
                        }
                    }
                }
            },
            {
                xtype: 'radiogroup',
                id: 'rd_recommend',
                name: 'rd_recommend',
                colName: 'rd_recommend',
                width: 200,
                defaults: {
                    name: 'rd_Recommend'
                },
                columns: 3,
                vertical: true,
                items: [
                    { boxLabel: '全部', id: 'all', inputValue: '0', checked: true },
                    { boxLabel: '被推薦人已加入會員', id: 'recommend', inputValue: '1' }
                ]
            },
            {
                text: SEARCH,
                iconCls: 'icon-search',
                id: 'btnQuery',
                handler: Query
            },{
                text: '重置', id: 'reset', iconCls: 'ui-icon ui-icon-reset', handler: function () {
                    Ext.getCmp('ddlSel').reset();
                    Ext.getCmp('ddlCon').reset();
                    Ext.getCmp('start_date').reset();
                    Ext.getCmp('end_date').reset();
                    Ext.getCmp('rd_recommend').reset();
                }
            }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: FaresStore,
            pageSize: pageSize,
            displayInfo: true,
            displayMsg: NOW_DISPLAY_RECORD + ': {0} - {1}' + TOTAL + ': {2}',
            emptyMsg: NOTHING_DISPLAY
        }),
        listeners: {
            scrollershow: function (scroller) {
                if (scroller && scroller.scrollEl) {
                    scroller.clearManagedListeners();
                    scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                }
            }
        }
        //,
        //selModel: sm
    });

    Ext.create('Ext.container.Viewport', {
        layout: 'fit',
        items: [gdFgroup],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                gdFgroup.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    //FaresStore.load({ params: { start: 0, limit: 28 } });
});



function SecretLogin(rid, uid, isRecom) {//isRecom是否是推薦者的詳情
    var secret_type = "1";//參數表中的"會員查詢列表"

    var url = "/Member/RecommendMember";

    var ralated_id = rid;
    //點擊機敏信息先保存記錄在驗證密碼是否需要輸入
    boolPassword = SaveSecretLog(url, secret_type, ralated_id);//判斷5分鐘之內是否有輸入密碼
    if (boolPassword != "-1") {//不准查看
        if (boolPassword) {//超過5分鐘沒有輸入密碼
            //參數1：機敏頁面代碼，2：機敏資料主鍵，3：是否彈出驗證密碼框,4：是否直接顯示機敏信息6.驗證通過后是否打開編輯窗口
            //  function SecretLoginFun(type, relatedID, isLogin, isShow, editO, isEdit) {
            if (isRecom) {//推薦者調整推薦者詳情
                SecretLoginFun(secret_type, ralated_id, true, true, false, url, "", "", "");//先彈出驗證框，關閉時在彈出顯示框

            }
            else {//被推薦者是會員時調整被推薦者會員編輯頁面，否則顯示被推薦者詳情
                if (uid != 0)//調整編輯頁面
                {
                    SecretLoginFun(secret_type, uid, true, false, true, url, "", "", "");//先彈出驗證框，關閉時在彈出顯示框
                }
                else {//調轉顯示頁面
                    SecretLoginFun(secret_type, ralated_id, true, true, false, url, "", "", "");//先彈出驗證框，關閉時在彈出顯示框
                }
            }

        } else {
            if (isRecom) {
                SecretLoginFun(secret_type, ralated_id, false, true, false, url, "", "", "");//先彈出驗證框，關閉時在彈出顯示框

            }
            else {//被推薦者是會員時調整被推薦者會員編輯頁面，否則顯示被推薦者詳情
                if (uid != 0)//調整被推薦者編輯頁面
                {
                    editFunction(uid, edit_UserStore);
                }
                else {//調轉顯示頁面
                    SecretLoginFun(secret_type, ralated_id, false, true, false, url, "", "", "");//先彈出驗證框，關閉時在彈出顯示框
                }
            }
        }
    }
}


//修改會員信息
function UpdateUser() {
    var row = Ext.getCmp("gdFgroup").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        editFunction(row[0], FaresStore);
    }
}
var DDLStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
      //  { "txt": ALL, "value": "0" },
        { "txt": "被推薦人信箱", "value": "1" },
        { "txt": "被推薦人姓名", "value": "2" },
        { "txt": "推薦人姓名", "value": "3" },
        { "txt": "推薦人信箱", "value": "4" }
    ]
});
/********************************人員管理********************************/
onCallidClick = function () {
    var row = Ext.getCmp("gdFgroup").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        var groupId = Ext.getCmp('gdFgroup').getSelectionModel().getSelection()[0].data.rowid;
        Ext.Ajax.request({
            url: '/Fgroup/QueryCallidById',
            params: { groupId: groupId },
            success: function (response) {
                var a = response.responseText;
                var arr = a.split(",");
                if (!CallidForm) {
                    CallidForm = Ext.create('widget.window', {
                        title: TOOL_CALLID, closable: true,
                        closeAction: 'hide',
                        modal: true,
                        width: 500,
                        minWidth: 500,
                        height: document.documentElement.clientHeight * 300 / 783,
                        layout: 'fit',
                        bodyStyle: 'padding:5px;',
                        items: [{
                            xtype: 'itemselector',
                            name: 'itemselector',
                            id: 'itemselector-field',
                            anchor: '100%',
                            store: ManageUserStore,
                            displayField: 'name',
                            valueField: 'callid',
                            allowBlank: false,
                            msgTarget: 'side'
                        }, {
                            xtype: 'textfield',
                            name: 'groupId',
                            hidden: true
                        }],
                        fbar: [{
                            xtype: 'button',
                            text: RESET,
                            id: 'reset',
                            handler: function () {
                                Ext.getCmp("itemselector-field").reset();
                                return false;
                            }
                        },
                        {
                            xtype: 'button',
                            text: SAVE,
                            id: 'save',
                            handler: function () {
                                Ext.Ajax.request({
                                    url: '/Fgroup/AddCallid',
                                    params: { groupId: Ext.getCmp('gdFgroup').getSelectionModel().getSelection()[0].data.rowid, callid: Ext.getCmp("itemselector-field").getValue() },
                                    success: function (response, opts) {
                                        var result = eval("(" + response.responseText + ")");
                                        Ext.Msg.alert(INFORMATION, SUCCESS);
                                        CallidForm.hide();
                                    },
                                    failure: function (response) {
                                        var result = eval("(" + response.responseText + ")");
                                        Ext.Msg.alert(INFORMATION, FAILURE);
                                    }
                                });
                            }
                        }]
                    });
                }
                CallidForm.show();
                Ext.getCmp("itemselector-field").setValue(arr);
            },
            failure: function (response) {
                var resText = eval("(" + response.responseText + ")");
                alert(resText.rpackCode);
            }
        });
    }
}
function Tomorrow() {
    var d;
    d = new Date();                             // 创建 Date 对象。                               // 返回日期。
    d.setDate(d.getDate() + 1);
    return d;
}
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