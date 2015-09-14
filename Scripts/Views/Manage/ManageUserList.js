
Ext.Loader.setConfig({ enabled: true });
Ext.Loader.setPath('Ext.ux', '/Scripts/Ext4.0/ux');
Ext.require([
    'Ext.form.Panel',
    'Ext.ux.form.MultiSelect',
    'Ext.ux.form.ItemSelector'
]);
var pageSize = 25;
var boolPassword = true;//secretcopy
var info_type = "manager_user";
var secret_info = "";
 
Ext.define('gigade.ManageUser', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "user_id", type: "int" },
        { name: "user_username", type: "string" },
        { name: "user_email", type: "string" },
        { name: "user_delete_email", type: "string" },
        { name: "user_confirm_code", type: "int" },
        { name: "user_status", type: "int" },
        { name: "user_login_attempts", type: "int" },
        { name: "user_lastvisit", type: "string" },
        { name: "user_last_login", type: "string" },
        { name: "manage", type: "int" },
        { name: "user_createdate", type: "string" },
        { name: "user_updatedate", type: "string" },
        { name: "updtime", type: "string" },
        { name: "creattime", type: "string" },
        { name: "lastlogin", type: "string" },
        { name: "erp_id", type: "string" }
    ]
});
secret_info = "user_id;user_name;user_email";

var statusStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
          { 'txt': '全部', 'value': '-1' },
          { 'txt': '未啟用', 'value': '0' },
          { 'txt': '啟用', 'value': '1' },
          { 'txt': '停用', 'value': '2' },
          { 'txt': '刪除', 'value': '3' }
    ]
});
var ManageUserStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.ManageUser',
    proxy: {
        type: 'ajax',
        url: '/Manage/GetManageUserList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});
var edit_ManageUserStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.ManageUser',
    proxy: {
        type: 'ajax',
        url: '/Manage/GetManageUserList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});


var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp("ManageUserView").down('#edit').setDisabled(selections.length == 0);
            Ext.getCmp("ManageUserView").down('#update').setDisabled(selections.length == 0);
        }
    }
});

ManageUserStore.on('beforeload', function () {
    Ext.apply(ManageUserStore.proxy.extraParams, {
        s_mail: Ext.getCmp('s_mail').getValue(),
        s_name: Ext.getCmp('s_name').getValue(),
        search_status: Ext.getCmp('search_status').getValue(),
        login_sum: Ext.getCmp('login_sum').getValue(),
        relation_id: "",
        isSecret: true
    })
});


edit_ManageUserStore.on('beforeload', function () {
    Ext.apply(edit_ManageUserStore.proxy.extraParams, {
        s_mail: Ext.getCmp('s_mail').getValue(),
        s_name: Ext.getCmp('s_name').getValue(),
        search_status: Ext.getCmp('search_status').getValue(),
        login_sum: Ext.getCmp('login_sum').getValue(),
        relation_id: "",
        isSecret: false
    })
});
function Query() {
    if (Ext.getCmp('s_mail').getValue() != "" || Ext.getCmp('s_name').getValue() != "" || Ext.getCmp('search_status').getValue() != "-1" || (Ext.getCmp('login_sum').getValue() != null && Ext.getCmp('login_sum').getValue() != null)) {
        ManageUserStore.removeAll();
        ManageUserStore.loadPage(1, {
            params: {
                params: { start: 0, limit: pageSize }
            }

        });
    }
    else {
        Ext.Msg.alert(INFORMATION, SEARCH_LIMIT);
    }
}

Ext.onReady(function () {
    var frm = Ext.create('Ext.form.Panel', {
        id: 'frm',
        layout: 'anchor',
        height: 45,
        border: 0,
        bodyPadding: 10,
        width: document.documentElement.clientWidth,
        items: [
            {
                xtype: 'fieldcontainer',
                combineErrors: true,
                layout: 'hbox',
                items: [
                    {
                        xtype: 'textfield',
                        id: 's_mail',
                        name: 's_mail',
                        fieldLabel: '信箱',
                        labelWidth: 40,
                        margin: '0 5 0 2',
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
                        id: 's_name',
                        name: 's_name',
                        fieldLabel: '名稱',
                        labelWidth: 40,
                        margin: '0 5 0 2',
                        listeners: {
                            specialkey: function (field, e) {
                                if (e.getKey() == Ext.EventObject.ENTER) {
                                    Query();
                                }
                            }
                        }
                    },
                    {
                        xtype: 'combobox',
                        id: 'search_status',
                        name: 'search_status',
                        fieldLabel: '狀態',
                        displayField: 'txt',
                        valueField: 'value',
                        labelWidth: 40,
                        editable: false,
                        store: statusStore,
                        margin: '0 5 0 2',
                        value: -1
                    },
                      {
                          xtype: 'displayfield',
                          value: '登入錯誤次數>=',
                          margin: '0 0 0 5',
                          width: 100
                      },
                    {
                        xtype: 'numberfield',
                        id: 'login_sum',
                        name: 'login_sum',
                        margin: '0 0 0 2',
                        minValue: 0,
                        listeners: {
                            specialkey: function (field, e) {
                                if (e.getKey() == Ext.EventObject.ENTER) {
                                    Query();
                                }
                            }
                        }
                    },
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
                        listeners: {
                            click: function () {
                                this.up('form').getForm().reset();
                            }
                        }
                    }
                ]
            }
        ]
    });
    var ManageUserView = Ext.create('Ext.grid.Panel', {
        id: 'ManageUserView',
        store: ManageUserStore,
        flex: 8,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
            { header: "編號", dataIndex: 'user_id', width: 60, align: 'center' },
            {
                header: "名稱", dataIndex: 'user_username', width: 100, align: 'center'
            },
            {
                header: "信箱", dataIndex: 'user_email', align: 'center', flex: 1,
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {//secretcopy
                    return "<span onclick='SecretLogin(" + record.data.user_id + "," + record.data.user_id + ",\"" + info_type + "\")'  >" + value + "</span>";
                }
            },
            {
                header: "狀態", dataIndex: 'user_status', width: 150, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    switch (value) {
                        case 0:
                            return "未啟用";
                            break;
                        case 1:
                            return "啟用";
                            break;
                        case 2:
                            return "<font color='red'>停用</font>";
                            break;
                        case 3:
                            return "刪除";
                            break;
                        default:
                            break;
                    }
                }
            },
            { header: "登入錯誤次數", dataIndex: 'user_login_attempts', width: 150, align: 'center' },
            { header: "最後登入時間", dataIndex: 'lastlogin', width: 150, align: 'center' },
            {
                header: "登入錯誤解鎖", dataIndex: 'user_status', width: 150, align: 'center',//H鎖定
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (record.data.user_login_attempts >= 5 && value == "2") {
                        return "<a href='javascript:void(0);' onclick='Unlock(" + record.data.user_id + ")'><img hidValue='1' id='img" + record.data.user_id + "' src='../../../Content/img/icons/hmenu-lock.png'/></a>";
                    }
                }
            }
        ],
        tbar: [
         { xtype: 'button', text: ADD, id: 'add', hidden: false, iconCls: 'icon-user-add', handler: onAddClick },
         { xtype: 'button', text: EDIT, id: 'edit', hidden: false, iconCls: 'icon-user-edit', disabled: true, handler: onEditClick },
        { xtype: 'button', text: "修改密碼", id: 'update', iconCls: 'icon-edit', disabled: true, handler: onChangePwd }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: ManageUserStore,
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
        }, selModel: sm
    });
    Ext.create('Ext.container.Viewport', {
        layout: 'vbox',
        items: [frm, ManageUserView],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                ManageUserView.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
})
function SecretLogin(rid, info_id, info_type) {//secretcopy
    var secret_type = "8";//參數表中的"試用試吃活動"
    var url = "/Manager/ManageUser";
    var ralated_id = rid;
    //點擊機敏信息先保存記錄在驗證密碼是否需要輸入
    boolPassword = SaveSecretLog(url, secret_type, ralated_id);//判斷5分鐘之內是否有輸入密碼
    if (boolPassword != "-1") {//不准查看
        if (boolPassword) {//超過5分鐘沒有輸入密碼
            //參數1：機敏頁面代碼，2：機敏資料主鍵，3：是否彈出驗證密碼框,4：是否直接顯示機敏信息6.驗證通過后是否打開編輯窗口 7:客戶信息類型user:會員 order：訂單 vendor：供應商 8：客戶id9：要顯示的客戶信息
            //  function SecretLoginFun(type, relatedID, isLogin, isShow, editO, isEdit) {
            SecretLoginFun(secret_type, ralated_id, true, true, false, url, info_type, info_id, secret_info);//先彈出驗證框，關閉時在彈出顯示框
        } else {
            SecretLoginFun(secret_type, ralated_id, false, true, false, url, info_type, info_id, secret_info);//先彈出驗證框，關閉時在彈出顯示框
        }
    }
}

function Unlock(id) {
    var activeValue = $("#img" + id).attr("hidValue");
    Ext.MessageBox.confirm(CONFIRM, "是否啟用?", function (btn) {
        if (btn == "yes") {
            $.ajax({
                url: "/Manage/UpdateStatus",
                data: {
                    "id": id,
                    "active": activeValue
                },
                type: "post",
                type: 'text',
                success: function (msg) {
                    ManageUserStore.load();
                    Ext.Msg.alert(INFORMATION, "解鎖成功");
                },
                error: function (msg) {
                    Ext.Msg.alert(INFORMATION, "解鎖失敗");
                }
            });
        }
        else {
            return false;
        }


    });
}

onAddClick = function () {
    AddFunction(null, ManageUserStore);
}

onChangePwd = function () {
    var row = Ext.getCmp("ManageUserView").getSelectionModel().getSelection();
    if (row[0].data.user_status != 3) {
        if (row.length == 0) {
            Ext.Msg.alert(INFORMATION, NO_SELECTION);
        }
        else if (row.length > 1) {
            Ext.Msg.alert(INFORMATION, ONE_SELECTION);
        } else {
            var secret_type = "8";//參數表中的"會員查詢列表"
            var url = "/Manage/ManageUser/changePwd ";
            var ralated_id = row[0].data.user_id;
            var info_id = row[0].data.user_id;
            boolPassword = SaveSecretLog(url, secret_type, ralated_id);//判斷5分鐘之內是否有輸入密碼
            if (boolPassword != "-1") {
                if (boolPassword) {//驗證
                    SecretLoginFun(secret_type, ralated_id, true, false, true, url, info_type, info_id, secret_info);//先彈出驗證框，關閉時在彈出顯示框

                } else {
                    ChangePwdFunction(ralated_id);
                }
            }
        }
    }
    else {
        Ext.getCmp("ManageUserView").down('#edit').setDisabled(true);
        Ext.getCmp("ManageUserView").down('#update').setDisabled(true);
        Ext.Msg.alert("提示信息", "該條數據狀態無法操作!");
    }
}

onEditClick = function () {
    var row = Ext.getCmp("ManageUserView").getSelectionModel().getSelection();
    if (row[0].data.user_status != 3) {
        if (row[0].data.user_status == 2 && row[0].data.user_login_attempts >= 5) {
            Ext.Msg.alert(INFORMATION, "此賬號狀態為停用狀態,請先解鎖賬號!");
        }
        else {
            if (row.length == 0) {
                Ext.Msg.alert(INFORMATION, NO_SELECTION);
            }
            else if (row.length > 1) {
                Ext.Msg.alert(INFORMATION, ONE_SELECTION);
            } else {
                var secret_type = "8";//參數表中的"會員查詢列表"
                var url = "/Manage/ManageUser/Edit ";
                var ralated_id = row[0].data.user_id;
                var info_id = row[0].data.user_id;
                boolPassword = SaveSecretLog(url, secret_type, ralated_id);//判斷5分鐘之內是否有輸入密碼
                if (boolPassword != "-1") {
                    if (boolPassword) {//驗證
                        SecretLoginFun(secret_type, ralated_id, true, false, true, url, info_type, info_id, secret_info);//先彈出驗證框，關閉時在彈出顯示框
                    } else {
                        editFunction(ralated_id, ManageUserStore);
                    }
                }
                //editFunction(row[0], ManageUserStore);
            }
        }
    }
    else {
        Ext.getCmp("ManageUserView").down('#edit').setDisabled(true);
        Ext.getCmp("ManageUserView").down('#update').setDisabled(true);
        Ext.Msg.alert("提示信息", "該條數據狀態無法操作!");
    }
}

//RemoveClick = function () {
//    var row = Ext.getCmp("ManageUserView").getSelectionModel().getSelection();
//    if (row.length <= 0) {
//        Ext.Msg.alert(INFORMATION, NO_SELECTION);
//    } else {
//        Ext.Msg.confirm(CONFIRM, Ext.String.format(DELETE_INFO, row.length), function (btn) {
//            if (btn == 'yes') {
//                var rowIDs = '';
//                for (var i = 0; i < row.length; i++) {
//                    rowIDs += row[i].data.map_id + ',';
//                }
//                Ext.Ajax.request({
//                    url: '/MarketCategory/DeleteMarketProductMap',
//                    method: 'post',
//                    params: {
//                        rowId: rowIDs
//                    },
//                    success: function (form, action) {
//                        Ext.Msg.alert(INFORMATION, SUCCESS);
//                        ManageUserStore.load();

//                    },
//                    failure: function () {
//                        Ext.Msg.alert(INFORMATION, FAILURE);
//                        ManageUserStore.load();
//                    }
//                });
//            }
//        });
//    }
//}