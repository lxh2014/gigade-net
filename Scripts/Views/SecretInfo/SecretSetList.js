var pageSize = 25;
//密碼Model
Ext.define('gigade.SecretAccountSet', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "id", type: "string" },
        { name: "user_id", type: "string" },
        { name: "user_username", type: "string" },
        { name: "user_email", type: "string" },
        { name: "createdate", type: "string" },
        { name: "updatedate", type: "string" },
        { name: "status", type: "string" },
         { name: "pwd_status", type: "int" },
        { name: "ipfrom", type: "string" },
        { name: "user_login_attempts", type: "int" },
           { name: "secret_limit", type: "int" },
        { name: "secret_count", type: "int" }
    ]
});

var SecretAccountSetStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.SecretAccountSet',
    proxy: {
        type: 'ajax',
        url: '/SecretInfo/GetSecretSetList',
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
            Ext.getCmp("gdSecretAccountSet").down('#edit').setDisabled(selections.length == 0);
            Ext.getCmp("gdSecretAccountSet").down('#reset').setDisabled(selections.length == 0);
        }
    }
});
SecretAccountSetStore.on('beforeload', function () {
    Ext.apply(SecretAccountSetStore.proxy.extraParams, {
        search_content: Ext.getCmp('search_content') == null ? "" : Ext.getCmp('search_content').getValue()
    });
});
Ext.onReady(function () {
    Ext.tip.QuickTipManager.init();
    var gdSecretAccountSet = Ext.create('Ext.grid.Panel', {
        id: 'gdSecretAccountSet',
        store: SecretAccountSetStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
            { header: "流水號", dataIndex: 'id', width: 80, align: 'center' },
            {
                header: "用戶編號", dataIndex: 'user_id', width: 150, align: 'center'
            },
            {
                header: "用戶名稱", dataIndex: 'user_username', width: 150, align: 'center'
            },
            { header: "創建時間", dataIndex: 'createdate', flex: 1, align: 'center' },
            { header: "來源IP", dataIndex: 'ipfrom', flex: 1, align: 'center' },
            { header: "5分鐘內查詢次數限制", dataIndex: 'secret_limit', width: 150, align: 'center' },
              { header: "5分鐘內查詢次數總計", dataIndex: 'secret_count', width: 150, align: 'center' },
            { header: "登入查詢錯誤次數", dataIndex: 'user_login_attempts', width: 150, align: 'center' },
             {
                 header: "狀態", dataIndex: 'status', width: 100, align: 'center',
                 renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                     if (value == "1") {
                         return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.id + ")'><img hidValue='0' id='img" + record.data.id + "' src='../../../Content/img/icons/accept.gif'/></a>";
                     } else {
                         if (value == "0") {
                             if (record.data.user_login_attempts < 5 && record.data.secret_count != record.data.secret_limit) {
                                 return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.id + ")'><img hidValue='1' id='img" + record.data.id + "' src='../../../Content/img/icons/drop-no.gif'/></a>";
                             }
                         }
                     }
                 }
             },
                     {
                         header: "登陸錯誤解鎖", dataIndex: 'status', width: 150, align: 'center',//H鎖定
                         renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                             if (record.data.user_login_attempts >= 5 && value == "0") {
                                 return "<a href='javascript:void(0);' onclick='Unlock(" + record.data.id + ")'><img hidValue='1' id='img" + record.data.id + "' src='../../../Content/img/icons/hmenu-lock.png'/></a>";
                             }
                         }
                     },
        {
            header: "查詢達到極限解鎖", dataIndex: 'status', width: 150, align: 'center',//H鎖定
            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                if (record.data.secret_count >= record.data.secret_limit && value == "0") {
                    return "<a href='javascript:void(0);' onclick='UpdateCount(" + record.data.id + ")'><img hidValue='1' id='img" + record.data.id + "' src='../../../Content/img/icons/hmenu-lock.png'/></a>";
                }
            }
        }
        ],
        tbar: [
    { xtype: 'button', text: "新增", id: 'add', hidden: false, iconCls: 'icon-user-add', handler: onAddClick },
         { xtype: 'button', text: "編輯", id: 'edit', hidden: false, iconCls: 'icon-user-edit', disabled: true, handler: onEditClick },
          { xtype: 'button', text: "重置密碼", id: 'reset', hidden: true, iconCls: 'ui-icon ui-icon-reset uui-icon-reset-align', disabled: true, handler: ResetPwd },
          '->',
         {
             xtype: 'textfield', fieldLabel: "用戶編號/名稱/郵箱/IP", labelWidth: 150, id: 'search_content', listeners: {
                 specialkey: function (field, e) {
                     if (e.getKey() == e.ENTER) {
                         Query(1);
                     }
                 }
             }
         },
         {
             text: SEARCH,
             iconCls: 'icon-search',
             id: 'btnQuery',
             handler: Query
         }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: SecretAccountSetStore,
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
        layout: 'fit',
        items: [gdSecretAccountSet],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                gdSecretAccountSet.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    //  SecretAccountSetStore.load({ params: { start: 0, limit: 25 } });
});
/*************************************************************************************查詢*************************************************************************************************/
function Query() {

    SecretAccountSetStore.removeAll();
    Ext.getCmp("gdSecretAccountSet").store.loadPage(1, {
        params: { start: 0, limit: 25 }
    });

}
/*************************************************************************************新增*************************************************************************************************/
onAddClick = function () {
    editFunction(null, SecretAccountSetStore);
}

/*************************************************************************************編輯*************************************************************************************************/
onEditClick = function () {
    var row = Ext.getCmp("gdSecretAccountSet").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        editFunction(row[0], SecretAccountSetStore);
    }
}
ResetPwd = function () {
    var row = Ext.getCmp("gdSecretAccountSet").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        Ext.MessageBox.confirm(CONFIRM, "是否確認強制更換密碼？", function (btn) {
            if (btn == "yes") {
                ResetPwdFunction(row[0], SecretAccountSetStore);
            } else {
                return false;
            }
        })
    }
}

//更改狀態(啟用或者禁用)
function UpdateActive(id) {
    var activeValue = $("#img" + id).attr("hidValue");
    $.ajax({
        url: "/SecretInfo/UpdateActive",
        data: {
            "id": id,
            "active": activeValue
        },
        type: "POST",
        dataType: "json",
        success: function (msg) {
            if (activeValue == 1) {
                $("#img" + id).attr("hidValue", 0);
                $("#img" + id).attr("src", "../../../Content/img/icons/accept.gif");
                SecretAccountSetStore.load();
            }
            else {
                $("#img" + id).attr("hidValue", 1);
                $("#img" + id).attr("src", "../../../Content/img/icons/drop-no.gif");
                SecretAccountSetStore.load();
            }
        },
        error: function (msg) {
            Ext.Msg.alert(INFORMATION, FAILURE);
            SecretAccountSetStore.load();
        }
    });
}


function Unlock(id) {
    var activeValue = $("#img" + id).attr("hidValue");
    Ext.MessageBox.confirm(CONFIRM, IS_UNLOCK, function (btn) {
        if (btn == "yes") {
            $.ajax({
                url: "/SecretInfo/UnlockAndReset",
                data: {
                    "id": id,
                    "active": activeValue
                },
                type: "POST",
                dataType: "json",
                success: function (msg) {
                    SecretAccountSetStore.load();
                    Ext.Msg.alert(INFORMATION, SUCCESS);
                },
                error: function (msg) {
                    Ext.Msg.alert(INFORMATION, EDITERROR);
                }
            });
        }
        else {
            return false;
        }


    });
}
function UpdateCount(id) {
    var activeValue = $("#img" + id).attr("hidValue");
    $.ajax({
        url: "/SecretInfo/UpdateCount",
        data: {
            "id": id,
            "active": activeValue
        },
        type: "POST",
        dataType: "json",
        success: function (msg) {
            SecretAccountSetStore.load();
            Ext.Msg.alert(INFORMATION, SUCCESS);
        },
        error: function (msg) {
            Ext.Msg.alert(INFORMATION, EDITERROR);
        }
    })
}
