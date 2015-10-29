var pageSize = 25;
var boolPassword = true;//secretcopy
var info_type = "edm_group_email";
var secret_info = "user_id;user_name;user_email";

var EdmEmailStatusStore = Ext.create('Ext.data.Store', {
    fields: ["txt", "value"],
    data: [
    { "txt": "全部狀態", "value": "3" },
    { "txt": "Y", "value": "1" },
    { "txt": "N", "value": "0" },
    ]
});
var EdmSearchStore = Ext.create('Ext.data.Store', {
    fields: ["txt", "value"],
    data: [
    { "txt": "電子郵件", "value": "0" },
    { "txt": "姓名", "value": "1" },
    ]
});
Ext.define('gigade.EdmGroupEmail', {
    extend: 'Ext.data.Model',
    fields: [
    { name: "email_id", type: "int" },
    { name: "email_name", type: "string" },
    { name: "email_address", type: "string" },
    { name: "group_id", type: "int" },
    { name: "group_name", type: "string" },
    { name: "email_status", type: "int" },
    { name: "email_createdate_tostring", type: "datetime" },
    { name: "email_updatedate_tostring", type: "datetime" }

    ]
});
var EdmGroupEmailStore = Ext.create('Ext.data.Store', {
    model: 'gigade.EdmGroupEmail',
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: '/Edm/GetEdmGroupEmailList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});
EdmGroupEmailStore.on('beforeload', function () {
    Ext.apply(EdmGroupEmailStore.proxy.extraParams,
        {
            email_status: Ext.getCmp('email_status_search').getValue(),
            selectType: Ext.getCmp('selectType').getValue(),
            search_con: Ext.getCmp('search_con').getValue(),
            group_id: document.getElementById("group_id").value, relation_id: "",
            isSecret: true
        });
});

var Edit_EdmGroupEmailStore = Ext.create('Ext.data.Store', {
    model: 'gigade.EdmGroupEmail',
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: '/Edm/GetEdmGroupEmailList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});
Edit_EdmGroupEmailStore.on('beforeload', function () {
    Ext.apply(Edit_EdmGroupEmailStore.proxy.extraParams,
        {
            email_status: Ext.getCmp('email_status_search').getValue(),
            selectType: Ext.getCmp('selectType').getValue(),
            search_con: Ext.getCmp('search_con').getValue(),
            group_id: document.getElementById("group_id").value,
            relation_id: "",
            isSecret: false
        });
});
var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            var row = Ext.getCmp("EdmGroupEmailGrid").getSelectionModel().getSelection();
            Ext.getCmp("EdmGroupEmailGrid").down('#edit').setDisabled(selections.length == 0);
            Ext.getCmp("EdmGroupEmailGrid").down('#remove').setDisabled(selections.length == 0);
        }
    }
});



Ext.onReady(function () {

    var searchFrm = Ext.create('Ext.form.Panel', {
        id: 'searchFrm',
        bodyPadding: '15',
        border: 0,
        width: document.documentElement.clientWidth,
        items: [
            {
                xtype: 'fieldcontainer',
                layout: 'hbox',
                items: [
                    {
                        xtype: 'displayfield',
                        id: 'groupid',
                        fieldLabel: '群組編號'
                    }
                ]
            },
            {
                xtype: 'fieldcontainer',
                layout: 'hbox',
                items: [
                    {
                        xtype: 'displayfield',
                        id: 'groupname',
                        fieldLabel: '群組名稱',
                        width: 400
                    }
                ]
            }, {
                xtype: 'fieldcontainer',
                layout: 'hbox',
                items: [
                    {
                        xtype: 'displayfield',
                        id: 'groupcount',
                        fieldLabel: '群組名單數',
                        width: 300
                    }
                ]
            }
        ]
    });


    var EdmGroupEmailGrid = Ext.create('Ext.grid.Panel', {
        id: 'EdmGroupEmailGrid',
        store: EdmGroupEmailStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        flex: 8.1,
        columns: [
        { header: "郵件編號", dataIndex: 'email_id', width: 150, align: 'center' },
        {
            header: "電子郵件", dataIndex: 'email_address', width: 150, align: 'center',
            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {//secretcopy
                return "<span onclick='SecretLogin(" + record.data.email_id + "," + record.data.group_id + ",\"" + info_type + "\")'  >" + value + "</span>";
            }
        },
        {
            header: "姓名", dataIndex: 'email_name', width: 150, align: 'center',
            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {//secretcopy
                return "<span onclick='SecretLogin(" + record.data.email_id + "," + record.data.group_id + ",\"" + info_type + "\")'  >" + value + "</span>";
            }
        },
        {
            header: "訂閱狀態", dataIndex: 'email_status', width: 120, align: 'center',
            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                if (value == 1) {
                    return "Y";
                }
                else {
                    return "<font color='red'>N</font>";
                }
            }
        },
        { header: "建立時間", dataIndex: 'email_createdate_tostring', width: 120, align: 'center' },
        { header: "更新時間", dataIndex: 'email_updatedate_tostring', width: 120, align: 'center' }
        ],
        tbar: [
        { xtype: 'button', text: "新增", id: 'add', iconCls: 'ui-icon ui-icon-user-add', handler: onAddClick },
        { xtype: 'button', text: "編輯", id: 'edit', iconCls: 'ui-icon ui-icon-user-edit', disabled: true, handler: onEditClick },
        { xtype: 'button', text: "刪除", id: 'remove', iconCls: 'ui-icon ui-icon-user-delete', disabled: true, handler: onRemoveClick },
        '->',
        {
            xtype: 'combobox', fieldLabel: '訂閱狀態', labelWidth: 60, width: 180, id: 'email_status_search', editable: false,
            displayField: 'txt', valueField: 'value', emptyText: '請選擇', value: '3', store: EdmEmailStatusStore,
            listeners: {
                specialkey: function (field, e)
                {
                    if (e.getKey() == Ext.EventObject.ENTER)
                    {
                        onQuery();
                    }
                }
            }
        },
        {
            xtype: 'combobox', fieldLabel: '類型', margin: '0 0 0 10', labelWidth: 45, width: 160, id: 'selectType', editable: false,
            displayField: 'txt', valueField: 'value', emptyText: '請選擇', store: EdmSearchStore,
            listeners: {
                specialkey: function (field, e)
                {
                    if (e.getKey() == Ext.EventObject.ENTER)
                    {
                        onQuery();
                    }
                }
            }
        },
        {
            xtype: 'textfield',
            //fieldLabel: KEY,
            labelWidth: 60,
            id: 'search_con',
            name: 'search_con',
            width: 120,
            listeners: {
                specialkey: function (field, e)
                {
                    if (e.getKey() == Ext.EventObject.ENTER)
                    {
                        onQuery();
                    }
                }
            }
        },
        { xtype: 'button', text: '查詢', iconCls: 'icon-search', handler: onQuery },
        {
            xtype: 'button',
            text: '重置',
            iconCls: 'ui-icon ui-icon-reset',
            handler: function ()
            {
                Ext.getCmp('email_status_search').setValue('3');
                Ext.getCmp('selectType').setValue(null);
                Ext.getCmp('search_con').setValue('');
            }
        }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: EdmGroupEmailStore,
            pageSize: pageSize,
            displayInfo: true,
            displayMsg: "當前顯示記錄" + ': {0} - {1}' + "總計" + ': {2}',
            emptyMsg: "沒有記錄可以顯示"
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
        items: [searchFrm, EdmGroupEmailGrid],
        renderTo: Ext.getBody(),
        listeners: {
            resize: function () {
                EdmGroupEmailGrid.width = document.documentElement.clientWidth;
                load();
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    EdmGroupEmailStore.load();
});

function load() {
    var group_id = document.getElementById("group_id").value;
    Ext.Ajax.request({
        url: '/Edm/Load',
        params: {
            group_id: group_id
        },
        success: function (form, action) {
            var result = Ext.decode(form.responseText);
            if (result.success) {
                Ext.getCmp("groupid").setValue(result.group_id);
                Ext.getCmp("groupname").setValue(result.group_name);
                Ext.getCmp("groupcount").setValue(result.group_count);
            }
            else {
                if (result.msg == 1) {
                    Ext.Msg.alert(INFORMATION, "無改群組信息!");
                }
            }
        },
        failure: function () {

            Ext.Msg.alert(INFORMATION, FAILURE);

        }
    });
}

//*********新增********//
onAddClick = function () {
    var groupid = Ext.getCmp("groupid").getValue();
    var groupname = Ext.getCmp("groupname").getValue();
    editFunction(null, EdmGroupEmailStore, groupid, groupname);
} 

//*********編輯********//
onEditClick = function () {
    var row = Ext.getCmp("EdmGroupEmailGrid").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert("提示信息", "沒有選擇一行！");
    }
    else if (row.length > 1) {
        Ext.Msg.alert("提示信息", "只能選擇一行！");
    }
    else if (row.length == 1) {
        // editFunction(row[0], EdmGroupEmailStore, row[0].data.group_id, row[0].data.group_name);

        var secret_type = "15";//參數表中的"會員查詢列表"
        var url = "/edm/edmGroupMail?group_id=" + row[0].data.group_id + " && email_id=" + row[0].data.email_id + "/Edit";
        var ralated_id = row[0].data.email_id;
        var info_id = row[0].data.group_id;
        boolPassword = SaveSecretLog(url, secret_type, ralated_id);//判斷5分鐘之內是否有輸入密碼
        if (boolPassword != "-1") {
            if (boolPassword) {//驗證
                SecretLoginFun(secret_type, ralated_id, true, false, true, url, info_type, info_id, secret_info);//先彈出驗證框，關閉時在彈出顯示框
            } else {
                editFunction(ralated_id, Edit_EdmGroupEmailStore, info_id, row[0].data.group_name);
            }
        }
    }
}

//*********刪除********//
onRemoveClick = function () {
    var row = Ext.getCmp("EdmGroupEmailGrid").getSelectionModel().getSelection();
    if (row.length < 0) {
        Ext.Msg.alert("提示信息", "沒有選擇一行！");
    }
    else {
        var myMask = new Ext.LoadMask(Ext.getBody(), { msg: "Please wait..." });
        myMask.show();
        Ext.Msg.confirm("確認信息", Ext.String.format("刪除選中 {0} 條數據？", row.length), function (btn) {
            if (btn == 'yes') {
                var groupid = document.getElementById("group_id").value;
                var rowIDs = '';
                for (var i = 0; i < row.length; i++) {
                    rowIDs = rowIDs + row[i].data.email_id + ",";
                }
                Ext.Ajax.request({
                    url: '/Edm/DelEdmGroupEmail',
                    method: 'post',
                    params: { group_id: groupid, email_ids: rowIDs },
                    success: function (form, action) {
                        myMask.hide();
                        var result = Ext.decode(form.responseText);
                        if (result.success) {
                            if (result.msg == 1) {
                                Ext.Msg.alert("提示信息", "刪除成功！");
                            }
                            else {
                                Ext.Msg.alert("提示信息", "刪除失敗！");
                            }
                        }
                        else {
                            Ext.Msg.alert("提示信息", "刪除失敗！");
                        }
                        load()
                        EdmGroupEmailStore.load();
                    },
                    failure: function () {
                        myMask.hide();
                        Ext.Msg.alert("提示信息", "刪除失敗！");
                        EdmGroupEmailStore.load();
                    }
                });
            }
            else {
                myMask.hide();
            }
        });
    }
}
function NextMonth() {
    var d;
    d = new Date();                             // 创建 Date 对象。
    d.setDate(d.getMonth() + 1);
    return d;
}
function SecretLogin(rid, info_id, info_type) {//secretcopy
    var secret_type = "15";//參數表中的"會員查詢列表"
    var url = "/edm/edmGroupMail?group_id=" + info_id + " && email_id=" + rid;
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

function onQuery()
{
    if (Ext.getCmp('selectType').getValue() != ("" || null))
    {
        if (Ext.getCmp('search_con').getValue() == ("" || null))
        {
            Ext.Msg.alert(INFORMATION, "請輸入查詢條件");
            return;
        }
    }
    Ext.getCmp('EdmGroupEmailGrid').store.loadPage(1, {
        params: {
            selectType: Ext.getCmp('selectType').getValue(),
            search_con: Ext.getCmp('search_con').getValue(),
            email_status: Ext.getCmp('email_status_search').getValue()
        }
    });
}