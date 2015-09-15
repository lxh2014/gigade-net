var pageSize = 25;
Ext.define('gigade.UserCondition', {
    extend: 'Ext.data.Model',
    fields: [
    { name: 'condition_id', type: 'string' },
    { name: 'condition_name', type: 'string' },
    { name: 'level_id', type: 'string' },
      { name: 'ml_name', type: 'string' },
    { name: 'first_buy_time', type: 'string' },
    { name: 'reg_start', type: 'string' },
    { name: 'reg_end', type: 'string' },
    { name: 'buy_times_min', type: 'string' },
    { name: 'buy_times_max', type: 'string' },
    { name: 'buy_amount_min', type: 'string' },
    { name: 'buy_amount_max', type: 'string' },
    { name: 'group_id', type: 'string' },
    { name: 'group_name', type: 'string' },
    ]
});
var UserConditionStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.UserCondition',
    proxy: {
        type: 'ajax',
        url: '/EventPromo/GetUserConditionList',
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
            Ext.getCmp("ucList").down('#edit').setDisabled(selections.length == 0);
            Ext.getCmp("ucList").down('#delete').setDisabled(selections.length == 0);
        }
    }
});
UserConditionStore.on('beforeload', function () {
    Ext.apply(UserConditionStore.proxy.extraParams,
    {
        text: Ext.getCmp('keyWord').getValue()
    });
});
Ext.onReady(function () {
    var ucList = Ext.create('Ext.grid.Panel', {
        id: 'ucList',
        store: UserConditionStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        flex: 1,
        columns: [
            { header: "流水號", dataIndex: 'condition_id', width: 100, align: 'center', hidden: true },
            { header: "會員條件名稱", dataIndex: 'condition_name', width: 100, align: 'center' },
            { header: "會員等級", dataIndex: 'ml_name', width: 150, align: 'center' },
            {
                header: "會員群組", dataIndex: 'group_name', width: 150, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (record.data.group_id == 0) {
                        return "不分";
                    }
                    else {
                        return value;
                    }
                }
            },
            {
                header: "首次購買時間", dataIndex: 'first_buy_time', width: 150, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value == "1970-01-01 08:00:00") {
                        return "";
                    }
                    else {
                        return value;
                    }
                }
            },
            {
                header: "會員註冊時間起", dataIndex: 'reg_start', width: 150, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value == "1970-01-01 08:00:00") {
                        return "";
                    } else {
                        return value;
                    }
                }
            },
            {
                header: "會員註冊時間迄", dataIndex: 'reg_end', width: 150, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value == "1970-01-01 08:00:00") {
                        return "";
                    } else {
                        return value;
                    }
                }
            },
            { header: "最低消費次數", dataIndex: 'buy_times_min', width: 100, align: 'center' },
            { header: "最高消費次數", dataIndex: 'buy_times_max', width: 150, align: 'center' },
            { header: "最低消費總金額", dataIndex: 'buy_amount_min', width: 150, align: 'center' },
            { header: "最高消費總金額", dataIndex: 'buy_amount_max', width: 150, align: 'center' }
        ],
        tbar: [
        { xtype: 'button', text: ADD, id: 'add', iconCls: 'icon-user-add', handler: addClick },
        { xtype: 'button', text: "編輯", id: 'edit', iconCls: 'ui-icon ui-icon-user-edit', disabled: true, handler: editClick },
        { xtype: 'button', text: "刪除", id: 'delete', iconCls: 'icon-user-edit', disabled: true, handler: deleteClick },
        '->',
        {
            xtype: 'textfield',
            fieldLabel: '會員條件名稱',
            labelWidth: 90,
            id: 'keyWord',
            name: 'keyWord',
            listeners: {
                specialkey: function (field, e) {
                    if (e.getKey() == e.ENTER) {
                        Query();
                    }
                }
            }
        }
         ,
        {
            xtype: 'button',
            iconCls: 'ui-icon ui-icon-search-2',
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
                    Ext.getCmp('keyWord').setValue('');
                }
            }
        }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: UserConditionStore,
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
        items: [ucList],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                ucList.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
})
var addClick = function () {
    editFunction(null, UserConditionStore);
}
var editClick = function () {
    var row = Ext.getCmp("ucList").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert("提示信息", "沒有選擇一行！");
    }
    else if (row.length > 1) {
        Ext.Msg.alert("提示信息", "只能選擇一行！");
    }
    else {
        editFunction(row[0], UserConditionStore);
    }
}
var deleteClick = function () {
    var row = Ext.getCmp("ucList").getSelectionModel().getSelection();
    if (row.length <= 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else {
        Ext.Msg.confirm(CONFIRM, Ext.String.format("刪除選中" + row.length + "條數據?", row.length), function (btn) {
            if (btn == 'yes') {
                var rowIDs = '';
                for (var i = 0; i < row.length; i++) {
                    rowIDs += row[i].data.condition_id + ',';
                }
                Ext.Ajax.request({
                    url: '/EventPromo/DeleteUserCondition',//執行方法
                    method: 'post',
                    params: {
                        ids: rowIDs
                    },
                    success: function (form, action) {
                        var result = Ext.decode(form.responseText);
                        if (result.success) {
                            Ext.Msg.alert(INFORMATION, "刪除成功!");
                            UserConditionStore.loadPage(1);
                        }
                        else {
                            Ext.Msg.alert(INFORMATION, "操作失敗!");
                            UserConditionStore.loadPage(1);
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
function Query(x) {
    UserConditionStore.removeAll();
    var text = Ext.getCmp('keyWord').getValue();
    if (text.trim() == '') {
        Ext.Msg.alert(INFORMATION, '請選擇查詢條件');
    }
    else {
        Ext.getCmp("ucList").store.loadPage(1, {
            params: {
                text: Ext.getCmp('keyWord').getValue()
            }
        });
    }
}

setNextMonth = function (source, n) {
    var s = new Date(source);
    s.setMonth(s.getMonth() + n);
    if (n < 0) {
        s.setHours(0, 0, 0);
    } else if (n > 0) {
        s.setHours(23, 59, 59);
    }
    return s;
}
/*********************啟用/禁用**********************/
//function UpdateActive(id) {
//    var activeValue = $("#img" + id).attr("hidValue");
//    $.ajax({
//        url: "/DispatchingSystem/UpdateStats",
//        data: {
//            "id": id,
//            "active": activeValue
//        },
//        type: "post",
//        type: 'text',
//        success: function (msg) {
//            //Ext.Msg.alert(INFORMATION, "修改成功!");
//            UserConditionStore.load();
//            if (activeValue == 1) {
//                $("#img" + id).attr("hidValue", 0);
//                $("#img" + id).attr("src", "../../../Content/img/icons/accept.gif");
//            } else {
//                $("#img" + id).attr("hidValue", 1);
//                $("#img" + id).attr("src", "../../../Content/img/icons/drop-no.gif");
//            }
//        },
//        error: function (msg) {
//            Ext.Msg.alert(INFORMATION, FAILURE);
//        }
//    });
//}