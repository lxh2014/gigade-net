
Ext.Loader.setConfig({ enabled: true });
Ext.Loader.setPath('Ext.ux', '/Scripts/Ext4.0/ux');
Ext.require([
    'Ext.form.Panel',
    'Ext.ux.form.MultiSelect',
    'Ext.ux.form.ItemSelector'
]);

var pageSize = 15;
/**********************************************************************群組管理主頁面**************************************************************************************/
//群組管理Model
Ext.define('gigade.Fares', {
    extend: 'Ext.data.Model',
    fields: [
    { name: "id", type: "int" },
        { name: "name", type: "string" },
        { name: "group_name", type: "string" },
        { name: "amount", type: "int" },
        { name: "bonus_type", type: "string" },
        { name: "points", type: "string" },
        { name: "rate", type: "string" },
        { name: "startdate", type: "string" },
        { name: "condition_id", type: "int" },
        { name: "end", type: "string" },
        { name: "group_id", type: "int" },
        { name: "dollar", type: "int" },
        { name: "point", type: "int" },
        { name: "active", type: "bool" },
        { name: 'muser', type: 'int' },
        { name: 'user_username', type: 'string' }
    ]
});

var FaresStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.Fares',
    proxy: {
        type: 'ajax',
        url: '/PromotionsDeductRate/promotionsDeductRatelist',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
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
FaresStore.on('beforeload', function () {
    Ext.apply(FaresStore.proxy.extraParams,
        {
            ddlSel: Ext.getCmp('ddlSel').getValue()
        });
});
function Query(x) {
    FaresStore.removeAll();
    Ext.getCmp("gdFgroup").store.loadPage(1, {
        params: {
            ddlSel: Ext.getCmp('ddlSel').getValue()
        }
    });
}
Ext.onReady(function () {
    var gdFgroup = Ext.create('Ext.grid.Panel', {
        id: 'gdFgroup',
        store: FaresStore,
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
            { header: ACTIVENAME, dataIndex: 'name', width: 200, align: 'center' },
            {
                header: VIPGROUP, dataIndex: 'group_name', width: 100, align: 'center',
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
            { header: ALLMONEY, dataIndex: 'amount', width: 120, align: 'center' },
            { header: POINTTYPE, dataIndex: 'bonus_type', width: 100, align: 'center', renderer: BounTypeShow },
            { header: 'condition_id', dataIndex: 'condition_id', hidden: true, width: 80, align: 'center' },
            { header: DOLLARPOINT, dataIndex: 'points', width: 100, align: 'center' },
            { header: POINTUP, dataIndex: 'rate', width: 100, align: 'center' },
            { header: BEGINTIME, dataIndex: 'startdate', width: 200, align: 'center' },
            { header: ENDTIME, dataIndex: 'end', width: 200, align: 'center' },
            { header: 'muser', dataIndex: 'muser', hidden: true },
            { header: QuanXianMuser, dataIndex: 'user_username', width: 80, align: 'center' },
             {
                 header: ISACTIVE,
                 dataIndex: 'active',
                 id: 'controlactive',
                 hidden: true,
                 align: 'center',
                 renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                     if (value) {

                         return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.id + "," + record.data.muser + ")'><img hidValue='0' id='img" + record.data.id + "' src='../../../Content/img/icons/accept.gif'/></a>";
                     } else {

                         return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.id + "," + record.data.muser + ")'><img hidValue='1' id='img" + record.data.id + "' src='../../../Content/img/icons/drop-no.gif'/></a>";
                     }
                 }
             }
        ],
        tbar: [

            { xtype: 'button', text: ADD, id: 'add', hidden: false, iconCls: 'icon-user-add', handler: onAddClick },
            { xtype: 'button', text: EDIT, id: 'edit', hidden: true, iconCls: 'icon-user-edit', disabled: true, handler: onEditClick },
            { xtype: 'button', text: DELETE, id: 'remove', hidden: true, iconCls: 'icon-user-remove', disabled: true, handler: onRemoveClick },
            '->',
            {
                xtype: 'combobox', editable: false, fieldLabel: OPTION, labelWidth: 60, id: 'ddlSel', store: DDLStore, displayField: 'txt', valueField: 'value', value: '1'

            }, {
                text: SEARCH,
                iconCls: 'icon-search',
                id: 'btnQuery',
                handler: Query
            }],

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
    // FaresStore.load({ params: { start: 0, limit: pageSize} });
});

/*************************************************************************************新增*************************************************************************************************/
onAddClick = function () {
    editFunction(null, FaresStore);
}

/*************************************************************************************編輯*************************************************************************************************/
onEditClick = function () {
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

/*************************************************************************************刪除*************************************************************************************************/
onRemoveClick = function () {
    var row = Ext.getCmp("gdFgroup").getSelectionModel().getSelection();
    if (row.length < 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else {
        Ext.Msg.confirm(CONFIRM, Ext.String.format(DELETE_INFO, row.length), function (btn) {
            if (btn == 'yes') {
                var rowIDs = '';
                for (var i = 0; i < row.length; i++) {
                    rowIDs += row[i].data.id + '|';
                }
                Ext.Ajax.request({
                    url: '/PromotionsDeductRate/delete',
                    method: 'post',
                    params: { rowID: rowIDs },
                    success: function (form, action) {
                        var result = Ext.decode(form.responseText);
                        Ext.Msg.alert(INFORMATION, SUCCESS);
                        if (result.success) {
                            FaresStore.load();
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

function UpdateActive(id, muser) {
    var activeValue = $("#img" + id).attr("hidValue");
    $.ajax({
        url: "/PromotionsDeductRate/UpdateActive",
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
                return;
            }
            FaresStore.removeAll();
            FaresStore.load();
            if (activeValue == 1) {
                $("#img" + id).attr("hidValue", 0);
                $("#img" + id).attr("src", "../../../Content/img/icons/accept.gif");
            } else {
                $("#img" + id).attr("hidValue", 1);
                $("#img" + id).attr("src", "../../../Content/img/icons/drop-no.gif");
            }
        },
        error: function (msg) {
            Ext.Msg.alert(INFORMATION, FAILURE);
        }
    });
}
