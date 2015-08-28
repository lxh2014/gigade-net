Ext.Loader.setConfig({ enabled: true });
Ext.Loader.setPath('Ext.ux', '/Scripts/Ext4.0/ux');
Ext.require([
    'Ext.form.Panel',
    'Ext.ux.form.MultiSelect',
    'Ext.ux.form.ItemSelector'
]);

var pageSize = 25;
/**********************************************************************群組管理主頁面**************************************************************************************/
//購物金活動Model
Ext.define('gigade.PromotionsAccumulateBonusModel', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "id", type: "int" },
        { name: "name", type: "string" },
        { name: "group_name", type: "string" },
        { name: "startTime", type: "string" },
        { name: "end", type: "string" },
        { name: "bonus_rate", type: "int" },
        { name: "extra_point", type: "int" },
        { name: "bonus_expire_day", type: "int" }, 
        { name: "new_user", type: "string" },
        { name: "repeat", type: "string" },
        { name: "present_time", type: "string" },
        { name: "created", type: "string" },
        { name: "modified", type: "string" },
        { name: "active", type: "string" },
        { name: "muser", type: "string" },
        { name: "event_desc", type: "string" },
        { name: "event_type", type: "string" },
        { name: "condition_id", type: "string" },
        { name: "device", type: "string" },
        { name: "payment_code", type: "string" },
        { name: "payment_name", type: "string" },
        { name: "kuser", type: "string" },
        { name: "new_user_date", type: "string" },
        { name: "status", type: "string" },
        { name: 'muser', type: 'int' },
        { name: 'user_username', type: 'string' }
    ]
});
//購物金活動Store
var PromotionsAccumulateBonusStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.PromotionsAccumulateBonusModel',
    proxy: {
        type: 'ajax',
        url: '/PromotionsAccumulateBonus/List',
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
            Ext.getCmp("gdFgroup").down('#edit').setDisabled(selections.length == 0);
            Ext.getCmp("gdFgroup").down('#remove').setDisabled(selections.length == 0);
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
PromotionsAccumulateBonusStore.on('beforeload', function () {
    Ext.apply(PromotionsAccumulateBonusStore.proxy.extraParams, {
        ddlSel: Ext.getCmp('ddlSel').getValue()
    });
});
function Query() {
    PromotionsAccumulateBonusStore.removeAll();
    Ext.getCmp("gdFgroup").store.loadPage(1, {
        params: {
            ddlSel: Ext.getCmp('ddlSel').getValue()
        }
    });
}

//頁面載入
Ext.onReady(function () {
    var gdFgroup = Ext.create('Ext.grid.Panel', {
        id: 'gdFgroup',
        store: PromotionsAccumulateBonusStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        viewConfig: {
            enableTextSelection: true,
            stripeRows: false,
            getRowClass: function (record, rowIndex, rowParams, store) {
                return "x-selectable";
            }
        },
        columns: [
            { header: PID, dataIndex: 'id', width: 60, align: 'center', align: 'center' },
            { header: ACTIVENAME, dataIndex: 'name', width: 130, align: 'center' },
            {
                header: VIPGROUP,
                dataIndex: 'group_name',
                width: 100,
                align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (record.data.condition_id != "0") {
                        return VIPCONDITION;
                    }
                    else if (value == "") {
                        return BUFEN;
                    }
                    else {
                        return value;
                    }
                }
            },
            { header: BONUSRATE, dataIndex: 'bonus_rate', width: 120, align: 'center' },
            { header: BONUSEXPIREDAY, dataIndex: 'bonus_expire_day', width: 100, align: 'center' },
            { header: EXTRABONUS, dataIndex: 'extra_point', width: 130, align: 'center' },
            { header: "付款方式", dataIndex: 'payment_name', width: 130, align: 'center' },
            { header: NEWVIP, dataIndex: 'new_user', width: 130, align: 'center', renderer: NewUserShow },
            { header: REPEAT, dataIndex: 'repeat', width: 130, align: 'center', renderer: RepeatShow },
            { header: BEGINTIME, dataIndex: 'startTime', width: 130, align: 'center' },
            { header: ENDTIME, dataIndex: 'end', width: 130, align: 'center' },
             { header: 'muser', dataIndex: 'muser', width: 30, align: 'center', hidden: true },
             { header: QuanXianMuser, dataIndex: 'user_username', width: 80, align: 'center' },
            { header: '是否啟用',
                dataIndex: 'active',
                id: 'controlactive',
                hidden: true,
                align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value == "true") {
                        return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.id + "," + record.data.muser + ")'><img hidValue='0' id='img" + record.data.id + "' src='../../../Content/img/icons/accept.gif'/></a>";
                    } else if (value=="false") {
                        return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.id + "," + record.data.muser + ")'><img hidValue='1' id='img" + record.data.id + "' src='../../../Content/img/icons/drop-no.gif'/></a>";
                    }
                }
            }
        ],
        tbar: [
            { xtype: 'button', text: ADD, id: 'add', hidden: false, iconCls: 'icon-user-add', handler: onAddClick },
            { xtype: 'button', text: EDIT, id: 'edit', hidden: true, iconCls: 'icon-user-edit', disabled: true, handler: onEditClick },
            { xtype: 'button', text: REMOVE, id: 'remove', hidden: true, iconCls: 'icon-user-remove', disabled: true, handler: onRemoveClick },
            '->',
            {
                xtype: 'combobox',
                editable: false,
                fieldLabel: OPTION,
                labelWidth: 60,
                name: 'ddlSel',
                id: 'ddlSel',
                store: DDLStore,
                displayField: 'txt',
                valueField: 'value',
                value: '1'
            },
            {
                text: SEARCH,
                iconCls: 'icon-search',
                id: 'btnQuery',
                handler: Query
            }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: PromotionsAccumulateBonusStore,
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
   // PromotionsAccumulateBonusStore.load({ params: { start: 0, limit: 25} });
});

//可多次贈送
function RepeatShow(val) {
    switch (val) {
        case "0":
            return NO;
            break;
        default:
            return YES;
            break;
    }
}
//新會員限定
function NewUserShow(val) {
    switch (val) {
        case "true":
            return YES;
            break;
        case "false":
            return NO;
            break;
    }
}


//添加
onAddClick = function () {
    //addWin.show();
    editFunction(null, PromotionsAccumulateBonusStore);
}
//修改
onEditClick = function () {
    var row = Ext.getCmp("gdFgroup").getSelectionModel().getSelection();
    //alert(row[0]);
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        editFunction(row[0], PromotionsAccumulateBonusStore);
    }
}
//刪除
onRemoveClick = function () {
    var row = Ext.getCmp("gdFgroup").getSelectionModel().getSelection();
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
                    url: '/PromotionsAccumulateBonus/DeletePromotionsAccumulateBonus',
                    method: 'post',
                    params: { rowID: rowIDs },
                    success: function (form, action) {
                        var result = Ext.decode(form.responseText);
                        Ext.Msg.alert(INFORMATION, SUCCESS);
                        if (result.success) {
                            PromotionsAccumulateBonusStore.load(1);
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

//更改活動狀態(設置活動可用與不可用)
function UpdateActive(id,muser) {
    var activeValue = $("#img" + id).attr("hidValue");
    $.ajax({
        url: "/PromotionsAccumulateBonus/UpdateActive",
        data: {
            "id": id,
            "muser": muser,
            "active": activeValue
        },
        type: "POST",
        dataType: "json",
        success: function (msg) {
            if (msg.success == "stop") {
                Ext.Msg.alert("提示信息", QuanXianInfo);
            }
            else if (msg.success == "true") {
                PromotionsAccumulateBonusStore.removeAll();
                PromotionsAccumulateBonusStore.load();
                if (activeValue == 1) {
                    $("#img" + id).attr("hidValue", 0);
                    $("#img" + id).attr("src", "../../../Content/img/icons/accept.gif");
                } else {
                    $("#img" + id).attr("hidValue", 1);
                    $("#img" + id).attr("src", "../../../Content/img/icons/drop-no.gif");
                }
            }
            else {
                Ext.Msg.alert("提示信息", "更改失敗");
            }
        },
        error: function (msg) {
            alert("修改失敗,請稍后再試!");
        }
    });
}
