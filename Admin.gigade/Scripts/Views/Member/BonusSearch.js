Ext.Loader.setConfig({ enabled: true });
Ext.Loader.setPath('Ext.ux', '/Scripts/Ext4.0/ux');
Ext.require([
    'Ext.form.Panel',
    'Ext.ux.form.MultiSelect',
    'Ext.ux.form.ItemSelector'
]);
var CallidForm;
var pageSize = 25;
var info_type = "users";
var secret_info = "user_id;user_name;user_email";
Ext.define('gigade.Gwj', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "user_id", type: "int" },
        { name: "master_id", type: "string" },
        { name: "now_time", type: "int" },
        { name: "user_name", type: "string" },
        { name: "user_email", type: "string" },
        { name: "type_description", type: "string" },
        { name: "master_total", type: "int" },
        { name: "master_balance", type: "int" },
        { name: "smaster_start", type: "string" },
        { name: "smaster_end", type: "string" },
        { name: "smaster_createtime", type: "string" },
        { name: "master_start", type: "int" },
        { name: "master_end", type: "int" },
        { name: "master_createtime", type: "int" },
        { name: "bonus_type", type: "string" },
        { name: "master_note", type: "string" }
    ]
});

var BonusStore = Ext.create('Ext.data.Store', {
    pageSize: pageSize,
    model: 'gigade.Gwj',
    proxy: {
        type: 'ajax',
        url: '/Member/BonusSearchList',
        reader: {
            type: 'json',
            root: 'data', //在執行成功后。顯示數據。所以record.data.用戶字段可以直接讀取
            totalProperty: 'totalCount'
        }
    }
});

//頁面加載時判斷是否有數據
BonusStore.on('load', function (store, records, options) {
    var uid = document.getElementById("userid").value;
    if (uid != 0) {
        var totalcount = records.length;
        if (totalcount == 0) {
            Ext.MessageBox.alert(INFORMATION, "～搜尋不到資料～");
        }
    }
});

var edit_BonusStore = Ext.create('Ext.data.Store', {
    pageSize: pageSize,
    autoDestroy: true,
    model: 'gigade.Gwj',
    proxy: {
        type: 'ajax',
        url: '/Member/BonusSearchList',
        reader: {
            type: 'json',
            root: 'data', //在執行成功后。顯示數據。所以record.data.用戶字段可以直接讀取
            totalProperty: 'totalCount'
        }
    }
});


edit_BonusStore.on('beforeload', function () {
    var start_time = Ext.getCmp('time_start_create').getValue();
    var end_time = Ext.getCmp('time_end_create').getValue();
    if (start_time == null && end_time != null) {
        Ext.Msg.alert("提示", "請把發放日期補充完整");
        return false;
    }
    if (start_time != null && end_time == null) {
        Ext.Msg.alert("提示", "請把發放日期補充完整");
        return false;
    }
    Ext.apply(edit_BonusStore.proxy.extraParams, {
        uid: Ext.getCmp('user_id_list').getValue(),
        userNameMail: Ext.getCmp('mailName').getValue(),
        timestart: Ext.getCmp('time_start_create').getValue(),
        timeend: Ext.getCmp('time_end_create').getValue(),
        bonus_type: Ext.getCmp('ddlSel').getValue(),
        type_id: Ext.getCmp('ddlSelsend').getValue(),
        use: Ext.getCmp('use').getValue(),
        using: Ext.getCmp('using').getValue(),
        used: Ext.getCmp('used').getValue(),
        usings: Ext.getCmp('usings').getValue(),
        useds: Ext.getCmp('useds').getValue(),
        relation_id: "",
        isSecret: false
    });
});

//發送類型Model
Ext.define('gigade.sendType', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "type_id", type: "int" },
        { name: "type_description", type: "string" }
    ]
});
var TypeStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
        { "txt": "全部", "value": "0" },
        { "txt": "購物金", "value": "1" },
        { "txt": "抵用券", "value": "2" }
    ]
});

var sendTypeStore = Ext.create('Ext.data.Store', {
    model: 'gigade.sendType',
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: '/Member/BonusTypeList',
        reader: {
            type: 'json',
            root: 'data',
        }
    }
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
            Ext.getCmp("gdGwj").down('#edit').setDisabled(selections.length == 0);
        }
    }
});

BonusStore.on('beforeload', function () {
    var start_time = Ext.getCmp('time_start_create').getValue();
    var end_time = Ext.getCmp('time_end_create').getValue();
    if (start_time == null && end_time != null) {
        Ext.Msg.alert("提示", "請把發放日期補充完整");
        return false;
    }
    if (start_time != null && end_time == null) {
        Ext.Msg.alert("提示", "請把發放日期補充完整");
        return false;
    }
    Ext.apply(BonusStore.proxy.extraParams, {
        uid: Ext.getCmp('user_id_list').getValue(),
        userNameMail: Ext.getCmp('mailName').getValue(),
        timestart: Ext.getCmp('time_start_create').getValue(),
        timeend: Ext.getCmp('time_end_create').getValue(),
        bonus_type: Ext.getCmp('ddlSel').getValue(),
        type_id: Ext.getCmp('ddlSelsend').getValue(),
        use: Ext.getCmp('use').getValue(),
        using: Ext.getCmp('using').getValue(),
        used: Ext.getCmp('used').getValue(),
        usings: Ext.getCmp('usings').getValue(),
        useds: Ext.getCmp('useds').getValue(),
        relation_id: "",
        isSecret: true
    });
});


Ext.onReady(function () {
    var frm = Ext.create('Ext.form.Panel', {
        id: 'frm',
        layout: 'anchor',//anchor固定
        height: 170,
        border: 0,
        bodyPadding: 0,
        width: document.documentElement.clientWidth,
        items: [
                {
                    xtype: 'fieldcontainer',
                    combineErrors: true,
                    layout: 'hbox',
                    items: [
                        {
                            xtype: 'label',
                            forId: 'myFieldId',
                            text: '狀態:',
                            margin: '6 0 0 5'
                        },
                        {
                            xtype: 'checkbox',
                            boxLabel: "尚未開通",
                            id: 'use',
                            name: 'use',
                            margin: '5 0 0 5'
                        },
                        {
                            xtype: 'checkbox',
                            boxLabel: "未使用",
                            id: 'using',
                            name: 'using',
                            margin: '5 0 0 5'
                        },
                        {
                            xtype: 'checkbox',
                            boxLabel: "已過期",
                            id: 'used',
                            name: 'used'
                            , margin: '5 0 0 5'
                        },
                         {
                             xtype: 'checkbox',
                             boxLabel: "尚餘點數",
                             id: 'usings',
                             name: 'using',
                             margin: '5 0 0 5'
                         },
                        {
                            xtype: 'checkbox',
                            boxLabel: "已用完",
                            id: 'useds',
                            name: 'useds'
                            , margin: '5 0 0 5'
                        }
                    ]
                },

                {
                    xtype: 'fieldcontainer',
                    combineErrors: true,
                    layout: 'hbox',
                    margin: '0 0 0 0',
                    items: [
                        {
                            xtype: 'datetimefield',
                            fieldLabel: "發放日期",
                            labelWidth: 60,
                            //width: 220,
                            id: 'time_start_create',
                            name: 'time_start_create',
                            margin: '5 0 0 5',
                            format: 'Y-m-d  H:i:s',
                            time: { hour: 00, min: 00, sec: 00 },
                            editable: false,
                            listeners: {
                                select: function () {
                                    var start = Ext.getCmp("time_start_create");
                                    var end = Ext.getCmp("time_end_create");
                                    if (end.getValue() == null) {
                                        end.setValue(setNextMonth(start.getValue(), 1));
                                    }
                                    else if (start.getValue() > end.getValue()) {
                                        Ext.Msg.alert(INFORMATION, DATA_TIP);
                                        end.setValue(setNextMonth(start.getValue(), 1));
                                    }
                                }
                                  , specialkey: function (field, e) {
                                      if (e.getKey() == Ext.EventObject.ENTER) {
                                          Query();
                                      }
                                  }
                            }
                        },
                        {
                            xtype: 'label',
                            forId: 'myFieldId',
                            text: '~',
                            margin: '5 0 0 5'
                        },
                        {
                            xtype: 'datetimefield',
                            width: 150,
                            id: 'time_end_create',
                            name: 'time_end_create',
                            margin: '5 0 0 6',
                            format: 'Y-m-d H:i:s',
                            time: { hour: 23, min: 59, sec: 59 },
                            editable: false,
                            listeners: {
                                select: function () {
                                    var start = Ext.getCmp("time_start_create");
                                    var end = Ext.getCmp("time_end_create");
                                    if (start.getValue() != "" && start.getValue() != null) {
                                        if (end.getValue() < start.getValue()) {
                                            Ext.Msg.alert(INFORMATION, DATA_TIP);
                                            start.setValue(setNextMonth(end.getValue(), -1));
                                        }
                                    }
                                    else {
                                        start.setValue(setNextMonth(end.getValue(), -1));
                                    }
                                }
                                , specialkey: function (field, e) {
                                    if (e.getKey() == Ext.EventObject.ENTER) {
                                        Query();
                                    }
                                }
                            }
                        }
                    ]
                },
                 {
                     xtype: 'fieldcontainer',
                     combineErrors: true,
                     layout: 'hbox',
                     items: [
                     {
                         xtype: 'numberfield',
                         fieldLabel: "會員編號",
                         name: 'user_id_list',
                         id: 'user_id_list',
                         minValue: 0,
                         labelWidth: 60,
                         maxValue: 2147483647,
                         hideTrigger: true,
                         allowDecimals: false,
                         margin: '5 0 0 5',
                         listeners: {
                             specialkey: function (field, e) {
                                 if (e.getKey() == Ext.EventObject.ENTER) {
                                     Query();
                                 }
                             }
                         }
                     },
                     {
                         xtype: 'textfield',
                         allowBlank: true,
                         fieldLabel: "會員姓名/會員Mail",
                         margin: '5 0 0 5',
                         labelWidth: 110,
                         width: 270,
                         id: 'mailName',
                         name: 'mailName',
                         listeners: {
                             specialkey: function (field, e) {
                                 if (e.getKey() == Ext.EventObject.ENTER) {
                                     Query();
                                 }
                             }
                         }
                     }
                     ]
                 },
                {
                    xtype: 'fieldcontainer',
                    combineErrors: true,
                    layout: 'hbox',
                    margin: '0 0 0 0',
                    items: [
                        {
                            xtype: 'combobox',
                            fieldLabel: "類型",
                            margin: '5 0 0 5',
                            editable: false,//阻止直接在表单项的文本框中输入字符
                            id: 'ddlSel',
                            name: 'ddlSel',
                            store: TypeStore,
                            displayField: 'txt',
                            valueField: 'value',
                            labelWidth: 50,
                            width: 130,
                            value: "0",
                            queryMode: 'local',//請求數據模式，local：读取本地数据 remote：读取远程数据
                            lastQuery: ''//匹配字符串的值用于过滤store。删除这个属性来强制重新查询。
                            , listeners: {
                                specialkey: function (field, e) {
                                    if (e.getKey() == Ext.EventObject.ENTER) {
                                        Query();
                                    }
                                }
                            }
                        },
                            {
                                xtype: 'combobox',
                                fieldLabel: "發放類型",
                                margin: '5 0 0 5',
                                editable: true,//阻止直接在表单项的文本框中输入字符
                                id: 'ddlSelsend',
                                name: 'ddlSelsend',
                                store: sendTypeStore,
                                displayField: 'type_description',
                                valueField: 'type_id',
                                typeAhead: true,
                                forceSelection: true,//时，所选择的值限制在一个列表中的值，false时，允许用户设置任意的文本字段。
                                labelWidth: 60,
                                width: 355,
                                value: "0",
                                queryMode: 'local',//請求數據模式，local：读取本地数据 remote：读取远程数据   ,
                                listeners: {
                                    specialkey: function (field, e) {
                                        if (e.getKey() == Ext.EventObject.ENTER) {
                                            Query();
                                        }
                                    }
                                }
                            }
                    ]
                },

                {
                    xtype: 'fieldcontainer',
                    combineErrors: true,//如果设置为 true, 则 field 容器自动将其包含的所有属性域的校验错误组合为单个错误信息, 并显示到 配置的 msgTarget 上. 默认值 false.
                    layout: 'hbox',
                    margin: '10 0 0 0',
                    items:
                     [
                        {
                            xtype: 'button',
                            margin: '0 10 0 10',
                            iconCls: 'icon-search',
                            text: "查詢",
                            handler: Query
                        },
                        {
                            xtype: 'button',
                            text: '重置',
                            id: 'btn_reset',
                            iconCls: 'ui-icon ui-icon-reset',
                            listeners:
                            {
                                click: function () {
                                    frm.getForm().reset();
                                    if (document.getElementById("userid").value != 0) {
                                        Ext.getCmp('user_id_list').setValue(document.getElementById("userid").value);
                                        Ext.getCmp('ddlSel').setValue(document.getElementById("bonusType").value);
                                    }
                                    var datetime1 = new Date();
                                    datetime1.setFullYear(2000, 1, 1);
                                    var datetime2 = new Date();
                                    datetime2.setFullYear(2100, 1, 1);
                                    Ext.getCmp("time_end_create").setMinValue(datetime1);
                                    Ext.getCmp("time_end_create").setMaxValue(datetime2);
                                    Ext.getCmp("time_start_create").setMinValue(datetime1);
                                    Ext.getCmp("time_start_create").setMaxValue(datetime2);
                                }
                            }
                        }
                     ]
                }
        ]

    });

    var gdGwj = Ext.create('Ext.grid.Panel', {
        id: 'gdGwj',
        store: BonusStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        flex: 1.8,
        frame: true,
        columns: [
            { header: "購物金編號", dataIndex: 'master_id', width: 80, align: 'center' },
            { header: "會員編號", dataIndex: 'user_id', width: 70, align: 'center' },
            {
                header: "會員姓名", dataIndex: 'user_name', width: 70, align: 'center', hidden: false,
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    return "<span onclick='SecretLogin(" + record.data.master_id + "," + record.data.user_id + ",\"" + info_type + "\")'  >" + value + "</span>";
                }
            },
            {
                header: "會員Mail", dataIndex: 'user_email', width: 200, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    return "<span onclick='SecretLogin(" + record.data.master_id + "," + record.data.user_id + ",\"" + info_type + "\")'  >" + value + "</span>";
                }
            },
            { header: "發放類型", dataIndex: 'type_description', width: 120, align: 'center' },
            { header: "總額", dataIndex: 'master_total', width: 70, align: 'center' },
            { header: "結餘", dataIndex: 'master_balance', width: 70, align: 'center' },
            { header: "開始日期", dataIndex: 'smaster_start', width: 120, align: 'center' },
            { header: "結束日期", dataIndex: 'smaster_end', width: 120, align: 'center' },
            { header: "發放日期", dataIndex: 'smaster_createtime', width: 120, align: 'center' },
            { header: "狀態 ", dataIndex: 'now_time', width: 100, align: 'center', renderer: showbonus_status },
            { header: "類型", dataIndex: 'bonus_type', width: 100, align: 'center', renderer: showbonus_type },
            { header: "備註 ", dataIndex: 'master_note', width: 100, align: 'center' }
        ],
        tbar: [
            { xtype: 'button', text: EDIT, id: 'edit', iconCls: 'icon-user-edit', handler: onEditClick, hidden: true, disabled: true },
            //'->',
            {
                xtype: "button",
                text: "返回會員查詢列表",
                id: "goback",
                handler: function () {
                    history.back();
                }
            }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            id: "PagingToolbar",
            store: BonusStore,
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

    Ext.create('Ext.container.Viewport', {
        layout: 'vbox',
        items: [frm, gdGwj],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                gdGwj.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });

    if (document.getElementById("userid").value != 0) {
        Ext.getCmp('user_id_list').setValue(document.getElementById("userid").value);
        Ext.getCmp('ddlSel').setValue(document.getElementById("bonusType").value);
    }
    else {
        Ext.getCmp('goback').hide();
    }
    if (document.getElementById("userid").value != 0) {
        BonusStore.load({ params: { start: 0, limit: 25 } });
    }
    ToolAuthority();
});
function showbonus_type(val) {
    switch (val) {
        case "1":
            return "購物金";
            break;
        case "2":
            return "抵用券";
            break;
        default:
            return "數據異常";
            break;
    }
}

function showbonus_status(value, cellmeta, record, rowIndex, columnIndex, store) {
    if (value > record.data.master_end && record.data.master_balance > 0) {
        return "已過期";
    }
    if (value < record.data.master_start) {
        return "尚未開通";
    }
    if (record.data.master_balance > 0 && record.data.master_total > record.data.master_balance && record.data.master_start < value && value < record.data.master_end) {
        return "尚餘點數";
    }
    if (record.data.master_total <= record.data.master_balance && record.data.master_start < value && value < record.data.master_end) {
        return "未使用";
    }
    if (record.data.master_balance == 0) {
        return "已用完";
    }
}

function SecretLogin(rid, info_id, info_type) {//secretcopy
    var secret_type = "1";//參數表中的"會員查詢列表"
    var url = "/Member/BonusSearch";
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

/*************************************************************************************編輯*************************************************************************************************/
onEditClick = function () {
    var row = Ext.getCmp("gdGwj").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        // editFunction(row[0], BonusStore);
        var secret_type = "1";//參數表中的"會員查詢列表"
        var url = "/Member/BonusSearch/Edit ";
        var ralated_id = row[0].data.master_id;
        var info_id = row[0].data.user_id;
        boolPassword = SaveSecretLog(url, secret_type, ralated_id);//判斷5分鐘之內是否有輸入密碼
        if (boolPassword != "-1") {
            if (boolPassword) {//驗證
                SecretLoginFun(secret_type, ralated_id, true, false, true, url, info_type, info_id, secret_info);//先彈出驗證框，關閉時在彈出顯示框
            } else {
                editFunction(ralated_id, BonusStore, row[0].data.bonus_type);
            }
        }
    }
}

/*************************************************************************************刪除*************************************************************************************************/
onRemoveClick = function () {
    var row = Ext.getCmp("gdGwj").getSelectionModel().getSelection();
    if (row.length < 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else {
        Ext.Msg.confirm(CONFIRM, Ext.String.format(DELETE_INFO, row.length), function (btn) {
            if (btn == 'yes') {
                var rowIDs = '';
                for (var i = 0; i < row.length; i++) {
                    rowIDs += row[i].data.id + '|';
                }
                Ext.Ajax.request({
                    url: '/Promotions/DeletePromotionsAccumulateRate',
                    method: 'post',
                    params: { rowID: rowIDs },
                    success: function (form, action) {
                        var result = Ext.decode(form.responseText);
                        Ext.Msg.alert(INFORMATION, SUCCESS);
                        if (result.success) {
                            BonusStore.load();
                        }
                    },
                    failure: function () {
                        Ext.Msg.alert(INFORMATION, FAILURE);
                    }
                });
            }
        });
    }
}
function Query() {
    var start_time = Ext.getCmp('time_start_create').getValue();
    var end_time = Ext.getCmp('time_end_create').getValue();
    if (start_time == null && end_time != null) {
        Ext.Msg.alert("提示", "請把發放日期補充完整");
        return false;
    }
    if (start_time != null && end_time == null) {
        Ext.Msg.alert("提示", "請把發放日期補充完整");
        return false;
    }
    var falg = 0;
    if (Ext.getCmp('use').getValue()) { falg++; }
    if (Ext.getCmp('using').getValue()) { falg++; }
    if (Ext.getCmp('used').getValue()) { falg++; }
    if (Ext.getCmp('usings').getValue()) { falg++; }
    if (Ext.getCmp('useds').getValue()) { falg++; }
    if (start_time != null && end_time != null) { falg++; }
    if (Ext.getCmp('user_id_list').getValue() != null) { falg++; }
    if (Ext.getCmp('mailName').getValue().trim() != "") { falg++; }
    if (Ext.getCmp('ddlSel').getValue() != 0) { falg++; }
    if (Ext.getCmp('ddlSelsend').getValue() != 0) { falg++; }
    if (falg == 0) {
        Ext.Msg.alert("提示", "請輸入或選擇查詢條件");
        return false;
    }
    var form = Ext.getCmp('frm').getForm();
    if (form.isValid()) {
        BonusStore.removeAll();
        Ext.getCmp("gdGwj").store.loadPage(1,
             {
                 params: {
                     uid: Ext.getCmp('user_id_list').getValue(),
                     userNameMail: Ext.getCmp('mailName').getValue(),
                     timestart: Ext.getCmp('time_start_create').getValue(),
                     timeend: Ext.getCmp('time_end_create').getValue(),
                     bonus_type: Ext.getCmp('ddlSel').getValue(),
                     type_id: Ext.getCmp('ddlSelsend').getValue(),
                     use: Ext.getCmp('use').getValue(),
                     using: Ext.getCmp('using').getValue(),
                     used: Ext.getCmp('used').getValue(),
                     usings: Ext.getCmp('usings').getValue(),
                     useds: Ext.getCmp('useds').getValue()
                 }
             });
    }
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