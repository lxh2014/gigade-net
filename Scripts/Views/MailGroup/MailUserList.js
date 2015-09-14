var CallidForm;
var pageSize = 25;

//供应商出货单Model
Ext.define('gigade.MailUser', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "row_id", type: "string" },
        { name: "user_mail", type: "string" },
        { name: "user_name", type: "string" },
        { name: "status", type: "int" },
        { name: "user_pwd", type: "string" },
        { name: "create_time", type: "string" },
        { name: "create_user", type: "string" },
        { name: "update_time", type: "string" },
        { name: "update_user", type: "int" },
        { name: "create_user_name", type: "string" },
        { name: "update_user_name", type: "string" },
        { name: "key", type: "string" } //用於登錄到供應商後臺而被加密的key
    ]
});

//
//用戶Store 
var UserMailStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.MailUser',
    proxy: {
        type: 'ajax',
        url: '/MailGroup/MailUserList',
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
            Ext.getCmp("MailView").down('#edit').setDisabled(selections.length == 0);
            Ext.getCmp("MailView").down('#delete').setDisabled(selections.length == 0);
        }
    }
});
var edit_UserMailStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.MailUser',
    proxy: {
        type: 'ajax',
        url: '/MailGroup/MailUserList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});
edit_UserMailStore.on('beforeload', function () {
    Ext.apply(edit_UserMailStore.proxy.extraParams, {
        relation_id: "",
        isSecret: false
    });
});
UserMailStore.on('beforeload', function () {
    Ext.apply(UserMailStore.proxy.extraParams, {
        relation_id: "",
        isSecret: true
    });
});
function Query(x) {
    UserMailStore.removeAll();
    Ext.getCmp("MailView").store.loadPage(1, {
        params: {
            user_name: Ext.getCmp('searchname').getValue(),
            user_mail: Ext.getCmp('searchmail').getValue(),
            relation_id: "",
            isSecret: true
        }
    });
}

Ext.onReady(function () {

    var MailView = Ext.create('Ext.grid.Panel', {
        id: 'MailView',
        store: UserMailStore,
        // flex: 8.8,
        flex: 10,
        //height: 780,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
            { header: "用戶編號", dataIndex: 'row_id', width: 60, align: 'center' },
            {
                header: "用戶姓名", dataIndex: 'user_name', width: 180, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    return "<span onclick='SecretLogin(" + record.data.row_id + ")'  >" + value + "</span>";
                }
            },
            {
                header: "郵箱地址", dataIndex: 'user_mail', flex: 1, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    return "<span onclick='SecretLogin(" + record.data.row_id + ")'  >" + value + "</span>";
                }
            },
            {
                header: "狀態", dataIndex: 'status', width: 60, align: 'center', renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value == 1) {
                        return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.row_id + ")'><img hidValue='0' id='img" + record.data.row_id + "' src='../../../Content/img/icons/accept.gif'/></a>";
                    } else {
                        return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.row_id + ")'><img hidValue='1' id='img" + record.data.row_id + "' src='../../../Content/img/icons/drop-no.gif'/></a>";
                    }
                }
            },
            { header: "創建時間", dataIndex: 'create_time', width: 200, align: 'center' },
            { header: "創建人", dataIndex: 'create_user_name', width: 180, align: 'center' },
            { header: "修改時間", dataIndex: 'update_time', width: 200, align: 'center' },
              { header: "修改人", dataIndex: 'update_user_name', width: 180, align: 'center' },
        ],
        tbar: [
             { xtype: 'button', text: "新增", id: 'add', hidden: false, iconCls: 'icon-user-add', handler: onAddClick },
             { xtype: 'button', text: "編輯", id: 'edit', hidden: false, iconCls: 'icon-user-edit', disabled: true, handler: onEditClick },
             { xtype: 'button', text: '刪除', id: 'delete', hidden: true, iconCls: 'icon-remove', disabled: true, handler: RemoveClick },
                '->',
            {
                id: 'searchname',
                xtype: 'textfield',
                fieldLabel: "姓名",
                width: 200,
                // margin:' 0,0,0,0',
                labelWidth: 40,
                name: 'searchcontent',
                allowBlank: true
            },
             {
                 id: 'searchmail',
                 xtype: 'textfield',
                 fieldLabel: "郵箱",
                 width: 200,
                 labelWidth: 40,
                 name: 'searchcontent',
                 allowBlank: true
             },
            {
                xtype: 'button',
                text: SEARCH,
                iconCls: 'icon-search',
                margin: '0 0 0 0',
                id: 'btnQuery',
                handler: Query
            },
                    {
                        xtype: 'button',
                        text: RESET,
                        id: 'btn_reset',
                        listeners: {
                            click: function () {
                                Ext.getCmp("searchname").setValue('');
                                Ext.getCmp("searchmail").setValue('');
                            }
                        }
                    }
        ],
        //dockedItems: [
        //{   //類似于tbar
        //    xtype: 'toolbar',
        //    dock: 'top',
        //    items: [

        //    ]
        //}],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: UserMailStore,
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
        , selModel: sm
    });

    Ext.create('Ext.container.Viewport', {
        layout: 'vbox',
        items: [MailView],//frm
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                MailView.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    UserMailStore.load({ params: { start: 0, limit: 25 } });
});

function SecretLogin(rid) {//secretcopy
    var secret_type = "9";//參數表中的"郵件群組"
    var url = "/MailGroup/MailUser ";
    var ralated_id = rid;
    //點擊機敏信息先保存記錄在驗證密碼是否需要輸入
    boolPassword = SaveSecretLog(url, secret_type, ralated_id);//判斷5分鐘之內是否有輸入密碼
    if (boolPassword != "-1") {//不准查看
        if (boolPassword) {//超過5分鐘沒有輸入密碼
            //參數1：機敏頁面代碼，2：機敏資料主鍵，3：是否彈出驗證密碼框,4：是否直接顯示機敏信息6.驗證通過后是否打開編輯窗口
            //  function SecretLoginFun(type, relatedID, isLogin, isShow, editO, isEdit) {
            SecretLoginFun(secret_type, ralated_id, true, true, false, url);//先彈出驗證框，關閉時在彈出顯示框

        } else {
            SecretLoginFun(secret_type, ralated_id, false, true, false, url);//先彈出驗證框，關閉時在彈出顯示框
        }
    }
}
/*************************************************************************************新增*************************************************************************************************/
onAddClick = function () {
    //addWin.show();
    editFunction(null);
}

/*************************************************************************************編輯*************************************************************************************************/
onEditClick = function () {
    var row = Ext.getCmp("MailView").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        var secret_type = "9";//參數表中的"郵件群組"
        var url = "/MailGroup/MailUser/Edit ";
        var ralated_id = row[0].data.row_id;
        boolPassword = SaveSecretLog(url, secret_type, ralated_id);//判斷5分鐘之內是否有輸入密碼
        if (boolPassword != "-1") {
            if (boolPassword) {//驗證
                SecretLoginFun(secret_type, ralated_id, true, false, true, url);//先彈出驗證框，關閉時在彈出顯示框
            } else {
                editFunction(ralated_id);
            }
        }
    }
}
/*************************************************************************************刪除**************************************************************************************************/
RemoveClick = function () {
    var row = Ext.getCmp("MailView").getSelectionModel().getSelection();
    if (row.length <= 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else {
        Ext.Msg.confirm(CONFIRM, Ext.String.format(DELETE_INFO, row.length), function (btn) {
            if (btn == 'yes') {
                var rowIDs = '';
                for (var i = 0; i < row.length; i++) {


                    rowIDs += row[i].data.row_id + ',';
                }

                Ext.Ajax.request({
                    url: '/MailGroup/DeleteMailUser',
                    method: 'post',
                    params: {
                        rowId: rowIDs
                    },
                    success: function (form, action) {
                        Ext.Msg.alert("提示信息", "刪除成功！");
                        UserMailStore.load();
                        //var result = Ext.decode(form.responseText);
                        //Ext.Msg.alert(INFORMATION, SUCCESS);
                        //SearchActivy();
                    },
                    failure: function () {
                        Ext.Msg.alert(INFORMATION, FAILURE);
                    }
                });
            }
        });
    }
}

//更改狀態(啟用或者禁用)
function UpdateActive(id) {
    var activeValue = $("#img" + id).attr("hidValue");
    $.ajax({
        url: "/MailGroup/UpdateActive",
        data: {
            "id": id,
            "active": activeValue
        },
        type: "POST",
        dataType: "json",
        success: function (msg) {
            UserMailStore.remove();
            if (activeValue == 1) {
                $("#img" + id).attr("hidValue", 0);
                $("#img" + id).attr("src", "../../../Content/img/icons/accept.gif");
                UserMailStore.load();
            } else {
                $("#img" + id).attr("hidValue", 1);
                $("#img" + id).attr("src", "../../../Content/img/icons/drop-no.gif");
                UserMailStore.load();
            }
        },
        error: function (msg) {
            Ext.Msg.alert(INFORMATION, FAILURE);
        }
    });


}
