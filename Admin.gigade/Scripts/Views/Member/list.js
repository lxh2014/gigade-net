Ext.Loader.setConfig({ enabled: true });
Ext.Loader.setPath('Ext.ux', '/Scripts/Ext4.0/ux');
Ext.require([
'Ext.form.Panel',
'Ext.ux.form.MultiSelect',
'Ext.ux.form.ItemSelector'
]);
var CallidForm;
var pageSize = 25;
var boolPassword = true;//secretcopy
var info_type = "users";
var secret_info = "";

/**********************************************************************群組管理主頁面**************************************************************************************/
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
    { name: "user_actkey", type: "string" },


    //Edit Start Modify by Gaoyuwei 2015-12-07
    //{ name: "user_mobile", type: "string" },
    //{ name: "user_phone", type: "string" }, //行動電話
    { name: "user_mobile", type: "string" },//行動電話
    { name: "user_phone", type: "string" }, //市話
    //Edti End Modify by Gaoyuwei 2015-12-07


    { name: "reg_date", type: "string" }, //註冊日期 
    { name: "mytype", type: "string" },//會員類別
    { name: "send_sms_ad", type: "bool" }, //是否接收簡訊廣告 
    { name: "adm_note", type: "string" }, //管理員備註   上面這些編輯時要帶入的值
    { name: "user_type", type: "string" }, //用戶類別   下面的這些結合上面的會顯示在列表頁
    { name: "user_status", type: "string" }, //用戶狀態
    { name: "user_level", type: "int" }, //會員等級
    { name: "userLevel", type: "string" }, //會員等級
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
    { name: "bonus_typename", type: "string" }, 
    { name: "bonus_typenamequan", type: "string" },
    { name: "bonus_type", type: "string" },
    { name: "bonus_type1", type: "string" }, 
    { name: "user_url", type: "string" }
    ]
});

secret_info = "user_id;user_email;user_name;user_mobile";
var UserStore = Ext.create('Ext.data.Store', {
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



UserStore.on('beforeload', function () {
    Ext.apply(UserStore.proxy.extraParams, {
        serchs: Ext.getCmp('serchs').getValue(),
        timestart: Ext.getCmp('timestart').getValue(),
        timeend: Ext.getCmp('timeend').getValue(),
        serchcontent: Ext.getCmp('serchcontent').getValue(),
        bonus_type: Ext.htmlEncode(Ext.getCmp("bonus_type").getValue().Tax_Type),
        checkbox1: Ext.getCmp('checkbox1').getValue(),
        relation_id: "",
        isSecret: true
    });
});
//用作編輯時獲得數據包含機敏信息
var edit_UserStore = Ext.create('Ext.data.Store', {
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
edit_UserStore.on('beforeload', function () {
    Ext.apply(edit_UserStore.proxy.extraParams, {
        serchs: Ext.getCmp('serchs').getValue(),
        timestart: Ext.getCmp('timestart').getValue(),
        timeend: Ext.getCmp('timeend').getValue(),
        serchcontent: Ext.getCmp('serchcontent').getValue(),
        bonus_type: Ext.htmlEncode(Ext.getCmp("bonus_type").getValue().Tax_Type),
        checkbox1: Ext.getCmp('checkbox1').getValue(),
        relation_id: "",
        isSecret: false
    });
});
//使用者Model
Ext.define('gigade.ManageUser', {
    extend: 'Ext.data.Model',
    fields: [
    { name: "name", type: "string" },
    { name: "callid", type: "string" }]
});

var ManageUserStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    model: 'gigade.ManageUser',
    proxy: {
        type: 'ajax',
        url: '/Fgroup/QueryCallid',
        reader: {
            type: 'json',
            root: 'items'
        }
    }
});

var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp("gdUser").down('#edit').setDisabled(selections.length == 0);
            Ext.getCmp("gdUser").down('#cancel').setDisabled(selections.length == 0);
          //  Ext.getCmp("gdUser").down('#editEmail').setDisabled(selections.length == 0);
        }
    }
});
var DDLStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
    { "txt": "電子信箱", "value": "1" },
    { "txt": "會員姓名", "value": "2" },


    //Edti Start Modify by Gaoyuwei 2015-12-07
    //{ "txt": "手機號碼", "value": "3" },
    { "txt": "行動電話", "value": "3" },
    //Edit End Modify by Gaoyuwei 2015-12-07


    { "txt": "會員編號", "value": "4" },
    { "txt": "市話", "value": "5" },
    { "txt": "地址", "value": "6" },
    //Edit Start
    //Add by yuwei1015j 2015-12-02
    { "txt": "會員等級", "value": "7" }
    //Edit End
    ]
});

function Query(x) {
    var bonus_type = Ext.getCmp("bonus_type").getValue().Tax_Type;
    var phoneCheck = Ext.getCmp("checkbox1").getValue();
    var serchs = Ext.getCmp("serchs").getValue();
    var serchcontent = Ext.getCmp("serchcontent").getValue();
    var timestart = Ext.getCmp("timestart").getValue();
    var timeend = Ext.getCmp("timeend").getValue();
    if (serchs != null && serchcontent == "") {
        Ext.Msg.alert(INFORMATION, SEARCH_LIMIT);
    }
    else if ((timestart == null && timeend == null) && (serchs == null || serchcontent == "") && bonus_type == "" && phoneCheck == false) {
        Ext.Msg.alert(INFORMATION, SEARCH_LIMIT);
    }
    else {
        UserStore.removeAll();
        Ext.getCmp("gdUser").store.loadPage(1, {
            params: {
                serchs: Ext.getCmp('serchs').getValue(),
                timestart: Ext.getCmp('timestart').getValue(),
                timeend: Ext.getCmp('timeend').getValue(),
                serchcontent: Ext.getCmp('serchcontent').getValue(),
                bonus_type: Ext.htmlEncode(Ext.getCmp("bonus_type").getValue().Tax_Type),
                checkbox1: Ext.getCmp('checkbox1').getValue(),
                relation_id: "",
                isSecret: true
            }
        });
    }   
}

/************匯入到Excel**ATM************/
function Export() {
    var bonus_type = Ext.getCmp("bonus_type").getValue().Tax_Type;
    var phoneCheck = Ext.getCmp("checkbox1").getValue();
    var serchs = Ext.getCmp("serchs").getValue();
    var serchcontent = Ext.getCmp("serchcontent").getValue();
    var timestart = Ext.getCmp("timestart").getValue();
    var timeend = Ext.getCmp("timeend").getValue();
    if (serchs != null && serchcontent == "") {
        Ext.Msg.alert(INFORMATION, SEARCH_LIMIT);
    }
    else if ((timestart == null && timeend == null) && (serchs == null || serchcontent == "") && bonus_type == "" && phoneCheck == false) {
        Ext.Msg.alert(INFORMATION, SEARCH_LIMIT);
    }
    else {

        Ext.MessageBox.show({
            msg: '正在匯出，請稍後....',
            width: 300,
            wait: true
        });
        Ext.Ajax.request({
            url: "/Member/UserslistExport",
            timeout: 900000,
            params: {
                serchs: Ext.getCmp('serchs').getValue(),
                timestart: Ext.getCmp('timestart').getValue(),
                timeend: Ext.getCmp('timeend').getValue(),
                serchcontent: Ext.getCmp('serchcontent').getValue(),
                bonus_type: Ext.htmlEncode(Ext.getCmp("bonus_type").getValue().Tax_Type),
                checkbox1: Ext.getCmp('checkbox1').getValue()
            },
            success: function (form, action) {
                var result = Ext.decode(form.responseText);
                if (result.success) {
                    window.location = '../../ImportUserIOExcel/' + result.fileName;
                    Ext.MessageBox.hide();
                } else {
                    Ext.MessageBox.hide();
                    Ext.Msg.alert(INFORMATION, "匯出失敗或沒有數據,請先搜索查看!");
                }
            }
        });
    }
}

var channelTpl = new Ext.XTemplate(
'<a href="/{redirect_url}">' + ['{source_trace}'] + '{rediret_name}' + '</a>'
);

var channelTpl2 = new Ext.XTemplate(//Gwjserch
'<a href="/Member/BonusSearch?uid={user_id}&bonus_type={bonus_type}">' + "{bonus_typename}" + '</a>'
);

var channelTpl3 = new Ext.XTemplate(//Gwjserch
'<a href="/Member/BonusSearch?uid={user_id}&bonus_type={bonus_type1}">' + "{bonus_typenamequan}" + '</a>'
);

Ext.onReady(function () {
    var gdUser = Ext.create('Ext.grid.Panel', {
        id: 'gdUser',
        store: UserStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
        {header: "編號", dataIndex: 'user_id', width: 50, align: 'center' },
            
       
        {
            header: "姓名", dataIndex: 'user_name', width: 60, align: 'center'
        ,
            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {//secretcopy
                return "<span onclick='SecretLogin(" + record.data.user_id + "," + record.data.user_id + ",\"" + info_type + "\")'  >" + value + "</span>";
            }
        },
        { header: "會員等級", dataIndex: 'ml_code', width: 55, align: 'center' },
        {
            header: "信箱",
            dataIndex: 'user_email',
            flex: 2,
            align: 'center',
            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                return "<span onclick='SecretLogin(" + record.data.user_id + "," + record.data.user_id + ",\"" + info_type + "\")'  >" + value + "</span>";
            }
        },
        {
            header: "行動電話", dataIndex: 'user_mobile', width: 85, align: 'center',
            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {//secretcopy
                return "<span onclick='SecretLogin(" + record.data.user_id + "," + record.data.user_id + ",\"" + info_type + "\")'  >" + value + "</span>";
            }
        },
        { header: "狀態", dataIndex: 'user_status', width: 50, align: 'center', renderer: showstatus },
        { header: "註冊日期", dataIndex: 'reg_date', width: 120, align: 'center' },
        { header: "驗證碼", dataIndex: 'user_actkey', width: 80, align: 'center' },
        { header: "r_id", dataIndex: 's_id', width: 25, align: 'center', hidden: true },
        { header: "source_trace", dataIndex: 'source_trace', width: 100, align: 'center', hidden: true },
        { header: "redirect_url", dataIndex: 'redirect_url', width: 100, align: 'center', hidden: true },
        { header: "redirect_name", dataIndex: 'redirect_name', width: 100, align: 'center', hidden: true },
        { header: "是否是的", dataIndex: 'send_sms_ad', width: 100, align: 'center', hidden: true },
        { header: "來源", dataIndex: 'user_company_id', width: 100, align: 'center' },
        { header: "外網來源", dataIndex: 'user_source', width: 70, align: 'center' }, //有電話會員等
        {
            header: "站內連結",
            dataIndex: 'source_trace',
            width: 150,
            align: 'center',
            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {//搞定
                if (value == "0") {
                    return "";
                } else {
                    return Ext.String.format("<a id='znlj" + record.data.id + "' href='{0}' target='_blank'>[{1}]{2}</a>", record.data.redirect_url, record.data.source_trace, record.data.redirect_name);
                }
            }
        },
        { header: "購物金", xtype: 'templatecolumn', width: 140, tpl: channelTpl2, align: 'center' },
        { header: "抵用券", xtype: 'templatecolumn', width: 140, tpl: channelTpl3, align: 'center' },
        { header: "帳號開通", dataIndex: 'user_status', width: 100, align: 'center', renderer: show_user_status }, //基本搞定
        {
            header: "操作", dataIndex: 'user_id', width: 100, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    return "<a href='javascript:void(0);' onclick='TranToDetial(" + record.data.user_id + ")'>點擊跳轉會員登陸頁面</a>";//<span style='color:Red;'></span>
                }
        }, //基本搞定
        //{//經東亞與阿鴻確認,此功能已棄用,數據庫表已刪除
        //    header: "銷售賬號",
        //    dataIndex: 's_id',
        //    width: 60,
        //    align: 'center',
        //    renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
        //        switch (value) {
        //            case "0":
        //                return Ext.String.format("<a alt='0' id='add" + record.data.id + "' href='javascript:void(0);' onclick='onaddedit({0})' >新增</a>", record.data.user_id);
        //                break;
        //            default:

        //                return Ext.String.format("<a alt='1' id='add" + record.data.id + "' href='/SalesUser/Index?user_id={0}'>編輯</a>", record.data.user_id);
        //                break;
        //        }
        //    }
        //},
        {
            header: "客服代下單", id: 'clicks', dataIndex: 'user_id', width: 100, align: 'center', hidden: true
        ,
            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {//secretcopy
                return "<span onclick='SecretLogin(" + record.data.user_id + "," + record.data.user_id + ",\"" + info_type + "\")'  > *** </span>";
            }
        },
        {
            header: "首購時間",
            dataIndex: 'sfirst_time',
            width: 120,
            align: 'center',
            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                if (value == "1970-01-01 08:00:00") {
                    return "N/A";
                } else {
                    return value;
                }
            }
        },
        {
            header: "上次購買時間",
            dataIndex: 'slast_time',
            width: 120,
            align: 'center',
            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                if (value == "1970-01-01 08:00:00") {
                    return "N/A";
                } else {
                    return value;
                }
            }
        },
        {
            header: "上上次購買時間",
            dataIndex: 'sbe4_last_time',
            width: 120,
            align: 'center',
            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                if (value == "1970-01-01 08:00:00") {
                    return "N/A";
                } else {
                    return value;
                }
            }
        }
        ],
        tbar: [
        { xtype: 'button', text: EDIT, id: 'edit', iconCls: 'icon-user-edit', disabled: true, handler: onEditClick },
        { xtype: 'button', text: "禁用", id: 'cancel', hidden: false, iconCls: 'icon-user-auth', disabled: true, handler: onCancelClick },
      //  { xtype: 'button', text: "修改Email", id: 'editEmail', hidden: false, iconCls: 'icon-user-edit', disabled: true, handler: EditEmail }
        ],
        dockedItems: [
        {   //類似于tbar
            xtype: 'toolbar',
            dock: 'top',
            width: document.documentElement.clientWidth,
            items: [
            {
                xtype: 'combobox',
                editable: false,
                fieldLabel: "查詢條件",
                labelWidth: 60,
                id: 'serchs',
                store: DDLStore,
                displayField: 'txt',
                valueField: 'value',
                emptyText: '請選擇...'
            },
            {
                xtype: 'textfield', fieldLabel: "查詢內容", id: 'serchcontent', labelWidth: 60,
                listeners: {
                    focus: function () {
                        var searchType = Ext.getCmp("serchs").getValue();
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
                xtype: 'datetimefield',
                fieldLabel: "註冊日期",
                id: 'timestart',
                format: 'Y-m-d  H:i:s',
                time: { hour: 00, min: 00, sec: 00 },
                labelWidth: 60,
                editable: false,
                listeners: {
                    select: function (a, b, c) {
                        var start = Ext.getCmp("timestart");
                        var end = Ext.getCmp("timeend");                                          
                        if (end.getValue() == null) {
                            end.setValue(setNextMonth(start.getValue(), 1));
                        }
                        else if (start.getValue() > end.getValue()) {
                            Ext.Msg.alert(INFORMATION, DATA_TIP);
                            end.setValue(setNextMonth(start.getValue(), 1));
                        }
                        //else if (end.getValue() > setNextMonth(start.getValue(), 1)) {
                        //    end.setValue(setNextMonth(start.getValue(), 1));
                        //}
                    },
                    listeners: {
                        specialkey: function (field, e) {
                            if (e.getKey() == e.ENTER) {
                                Query();
                            }
                        }
                    }
                }
            },
            { xtype: 'displayfield', value: "~", margin: '0' },
            {
                xtype: 'datetimefield',
                id: 'timeend',
                format: 'Y-m-d  H:i:s',
                time: { hour: 23, min: 59, sec: 59 },
                editable: false,
                listeners: {
                    select: function (a, b, c) {
                        var start = Ext.getCmp("timestart");
                        var end = Ext.getCmp("timeend");
                        if (start.getValue() != "" && start.getValue() != null) {
                            if (end.getValue() < start.getValue()) {
                                Ext.Msg.alert(INFORMATION, DATA_TIP);
                                start.setValue(setNextMonth(end.getValue(), -1));
                            }
                            //else if (end.getValue() > setNextMonth(start.getValue(), 1)) {
                            //    start.setValue(setNextMonth(end.getValue(), -1));
                            //}
                        }
                        else {
                            start.setValue(setNextMonth(end.getValue(), -1));
                        }
                    }, listeners: {
                        specialkey: function (field, e) {
                            if (e.getKey() == e.ENTER) {
                                Query();
                            }
                        }
                    }
                }
            },
            {
                xtype: 'radiogroup',
                id: 'bonus_type',
                name: 'bonus_type',
                fieldLabel: "",
                colName: 'bonus_type',
                defaults: {
                    name: 'Tax_Type'
                },
                width: 380,
                columns: 5,
                items: [
                { id: 'stateid1', boxLabel: "所有狀態", inputValue: '', checked: true },
                { id: 'stateid2', boxLabel: "未啟用", inputValue: '0' },
                { id: 'stateid3', boxLabel: "已啟用", inputValue: '1' },
                { id: 'stateid4', boxLabel: "停用", inputValue: '2' },
                { id: 'stateid5', boxLabel: "簡易會員", inputValue: '5' }
                ]
            },
            {
                xtype: 'checkboxfield',
                id: 'checkbox1',
                name: 'checkbox1',
                boxLabel: '電話會員',
                width: 70,
                margin: "0 0 0 30",
                value: 2
            },
            '->',
            {
                text: SEARCH,
                iconCls: 'icon-search',
                id: 'btnQuery',
                handler: function () {
                    Query();
                }
            }, {
                text: '重置', id: 'reset', iconCls: 'ui-icon ui-icon-reset', handler: function () {
                    Ext.getCmp('serchs').reset();
                    Ext.getCmp('timestart').reset();
                    Ext.getCmp('timeend').reset();
                    Ext.getCmp('serchcontent').setValue('');
                    Ext.getCmp('bonus_type').reset();
                    Ext.getCmp('checkbox1').reset();
                }
            },
            {
                text: '匯出',
                margin: '0 0 10 5',
                iconCls: 'icon-excel',
                id: 'btnExcel',
                handler: Export
            }
            ]
        }],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: UserStore,
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
        },
        selModel: sm
    });
    if (document.getElementById('UserEmail').value != "") {
        Ext.getCmp("serchs").setValue("1");
        Ext.getCmp("serchcontent").setValue(document.getElementById('UserEmail').value);
    } else {
        if (document.getElementById('UserMobile').value != "") {
            Ext.getCmp("serchs").setValue("3");
            Ext.getCmp("serchcontent").setValue(document.getElementById('UserMobile').value);
        }
    }

    Ext.create('Ext.container.Viewport', {
        layout: 'fit',
        items: [gdUser],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                gdUser.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
});

function SecretLogin(rid, info_id, info_type) {//secretcopy
    var secret_type = "1";//參數表中的"會員查詢列表"
    var url = "/Member/UsersListIndex";
    var ralated_id = rid;
    //點擊機敏信息先保存記錄在驗證密碼是否需要輸入
    boolPassword = SaveSecretLog(url, secret_type, ralated_id);//判斷5分鐘之內是否有輸入密碼
    if (boolPassword != "-1") {//不准查看
        if (boolPassword) {//超過5分鐘沒有輸入密碼
            //參數1：機敏頁面代碼，2：機敏資料主鍵，3：是否彈出驗證密碼框,4：是否直接顯示機敏信息6.驗證通過后是否打開編輯窗口
            //  function SecretLoginFun(type, relatedID, isLogin, isShow, editO, isEdit) {
            SecretLoginFun(secret_type, ralated_id, true, true, false, url, info_type, info_id, secret_info);//先彈出驗證框，關閉時在彈出顯示框

        } else {
            SecretLoginFun(secret_type, ralated_id, false, true, false, url, info_type, info_id, secret_info);//直接彈出顯示框
        }
    }
}
function show_user_status(value, cellmeta, record, rowIndex, columnIndex, store) {

    switch (value) {
        case "0":
            return Ext.String.format("<a  id='sendmail" + record.data.id + "' href='javascript:void(0);' onclick='sendmail({0})' >寄送認證信</a>", record.data.user_id);
            break;
    }
}
function showstatus(val) {
    switch (val) {
        case "0":
            return "未啟用";
            break;
        case "1":
            return "已啟用";
            break;
        case "2":
            return "停用";
            break;
        case "5":
            return "簡易會員";
            break;
    }
}


/************************************編輯**********************************/
onEditClick = function () {

    var row = Ext.getCmp("gdUser").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        var secret_type = "1";//參數表中的"會員查詢列表"
        var url = "/Member/UsersListIndex/Edit ";
        var ralated_id = row[0].data.user_id;
        var info_id = row[0].data.user_id;
        boolPassword = SaveSecretLog(url, secret_type, ralated_id);//判斷5分鐘之內是否有輸入密碼
        if (boolPassword != "-1") {
            if (boolPassword) {//驗證
                SecretLoginFun(secret_type, ralated_id, true, false, true, url, info_type, info_id, secret_info);//先彈出驗證框，關閉時在彈出顯示框
            } else {
                editFunction(ralated_id);
            }
        }
    }
}
/********************禁用**********************/
onCancelClick = function () {
    var row = Ext.getCmp("gdUser").getSelectionModel().getSelection();
    if (row.length < 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else if (row.length == 1) {

        if (row[0].data.user_status == 2) {
            Ext.Msg.alert(INFORMATION, "該用戶已被禁用!");
        }
        else {
            edit_UserStore.load({
                params: { relation_id: row[0].data.user_id },
                callback: function () {
                    recordRow = edit_UserStore.getAt(0);
                    Ext.Msg.confirm(CONFIRM, "是否禁用該用戶?", function (btn) {
                        if (btn == 'yes') {
                            Ext.Ajax.request({
                                url: '/Member/UserCancel',
                                method: 'post',
                                params: {
                                    rowID: recordRow.data.user_id,
                                    email: recordRow.data.user_email
                                },
                                success: function (form, action) {
                                    var result = Ext.decode(form.responseText);
                                    Ext.Msg.alert(INFORMATION, SUCCESS);
                                    if (result.success) {
                                        UserStore.load();
                                    }
                                },
                                failure: function () {
                                    Ext.Msg.alert(INFORMATION, FAILURE);
                                }
                            });
                        }
                    });
                }
            });

        }
    } else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    }
}

function sendmail(id) {
    Ext.Ajax.request({
        url: '/Member/sendemail',//執行方法
        method: 'post',
        params: { "id": id },
        success: function (form, action) {
            var result = Ext.decode(form.responseText);
            if (result.success) {
                Ext.Msg.alert(INFORMATION, "郵件發送成功!<br/>" + "寄送時間:" + result.sendtime + "</br>" + "會員編號:" + result.userid + "</br>" + "會員名稱:" + result.username + "</br>" + "寄送地址:" + result.sendto + "</br>" + "認證號碼:" + result.authenid + "</br>");
                UserStore.load();
            }
            else {
                Ext.Msg.alert(INFORMATION, "郵件發送失敗!<br/>" + "寄送時間:" + result.sendtime + "</br>" + "會員編號:" + result.userid + "</br>" + "會員名稱:" + result.username + "</br>" + "寄送地址:" + result.sendto + "</br>" + "認證號碼:" + result.authenid + "</br>");
                UserStore.load();
            }
        },
        failure: function () {
            Ext.Msg.alert(INFORMATION, FAILURE);
        }
    })
};

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

EditEmail = function () {
    var row = Ext.getCmp("gdUser").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        var secret_type = "1";//參數表中的"會員查詢列表"
        var url = "/Member/UsersListIndex/EditEmail";
        var ralated_id = row[0].data.user_id;
        var info_id = row[0].data.user_id;
        boolPassword = SaveSecretLog(url, secret_type, ralated_id);//判斷5分鐘之內是否有輸入密碼
        if (boolPassword != "-1") {
            if (boolPassword) {//驗證
                SecretLoginFun(secret_type, ralated_id, true, false, true, url, info_type, info_id, secret_info);//先彈出驗證框，關閉時在彈出顯示框
            } else {
                editEmailFunction(ralated_id, edit_UserStore);
            }
        }
    }
}

function TranToDetial(us) {
    var row = Ext.getCmp("gdUser").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        var secret_type = '20';
        var url = row[0].data.user_url + "?uid=" + row[0].data.user_id;
       
        var ralated_id = row[0].data.user_id;
        var info_id = row[0].data.user_id;
        boolPassword = SaveSecretLog(url, secret_type, ralated_id);//判斷5分鐘之內是否有輸入密碼
        if (boolPassword != "-1") {
            if (boolPassword) {
                SecretLoginFun(secret_type, ralated_id, true, false, true, url);//先彈出驗證框，關閉時在彈出顯示框
                //SecretLoginFun(secret_type, ralated_id, false, true, false, url);//直接彈出顯示框
            }
            else {
                // productId = 15382;//product_id
                //if (winDetail == undefined) {
                    window.open(url);
               // }
            }
        }
    }
}