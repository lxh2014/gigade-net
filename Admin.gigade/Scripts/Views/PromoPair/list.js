﻿var CallidForm;
var pageSize = 10;
/**********************************************************************群組管理主頁面**************************************************************************************/
//群組管理Model
Ext.define('gigade.Fares', {
    extend: 'Ext.data.Model',
    fields: [ 
        { name: "id", type: "int" },
        { name: "event_name", type: "string" },
        { name: "event_desc", type: "string" },
        { name: "group_name", type: "string" },
        { name: "banner_image", type: "string" },
        { name: "condition_name", type: "string" },
        { name: "parameterName", type: "string" },
        { name: "payment_name", type: "string" },
        { name: "PN1", type: "string" },
        { name: "deliver_type", type: "string" },
        { name: "device", type: "string" },
        { name: "category_id", type: "string" },
        { name: "cate_red", type: "string" },
        { name: "cate_green", type: "string" },
        { name: "website", type: "string" },
        { name: "price", type: "string" },
        { name: "discount", type: "string" },
        { name: "starts", type: "string" },
        { name: "end", type: "string" },
        { name: "PTname", type: "string" },
        { name: "active", type: "bool" },
        { name: "condition_id", type: "string" },
        { name: "category_link_url", type: "string" },
        { name: 'muser', type: 'int' },
        { name: 'user_username', type: 'string' },
        { name: 'vendor_coverage',type:'string' }
    ]
});

var FaresStore = Ext.create('Ext.data.Store', {
    autoDestroy: true, //自動消除
    pageSize: pageSize,
    model: 'gigade.Fares',
    proxy: {
        type: 'ajax',
        url: '/PromoPair/Promolist',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'//總行數
        }
    }
});
//勾選框
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
    Ext.apply(FaresStore.proxy.extraParams, {
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
        store: FaresStore, //
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
            { header: HDID, dataIndex: 'condition_name', width: 70, align: 'center', align: 'center' },
            { header: ACTIVENAME, dataIndex: 'event_name', width: 120, align: 'center' },
            { header: ACTIVEDESC, dataIndex: 'event_desc', width: 120, align: 'center' },
            { header: VENDORCOVERAGE, dataIndex: 'vendor_coverage', width: 120, align: 'center' },
            {
                header: BANNERIMG,
                dataIndex: 'banner_image',
                width: 80,
                align: 'center',
                xtype: 'templatecolumn',
                tpl: '<a target="_blank" href="{category_link_url}" ><img width=50 name="tplImg"  height=50 src="{banner_image}" /></a>'
            },
            {
                header: GROUPNAME, dataIndex: 'group_name', width: 100, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (record.data.condition_id != 0) {
                        return VIPCONDITION;
                    }
                    else {
                        if (value == "") {
                            return BUFEN;
                        } else {
                            return value;
                        }
                    }
                } 
            },
        //{ header: VIPCONDITION, dataIndex: 'condition_name', width: 120, align: 'center', renderer: MultipleShow },
            { header: YSCLASS, dataIndex: 'deliver_type', width: 100, align: 'center', renderer: DeliverTypeShow },
            { header: PROSORT1, dataIndex: 'PTname', width: 100, align: 'center' },
        //{ header: PAYTYPE, dataIndex: 'payment_name', width: 100, align: 'center' },
        //{ header: BANK, dataIndex: 'PN1', width: 100, align: 'center' },
            { header: DEVICE, dataIndex: 'device', width: 100, align: 'center', renderer: DeviceShow },
            { header: BEGINTIME, dataIndex: 'starts', width: 130, align: 'center' },
            { header: ENDTIME, dataIndex: 'end', width: 130, align: 'center' },
            { header: 'muser', dataIndex: 'muser', hidden: true },
            { header: QuanXianMuser, dataIndex: 'user_username', width: 80, align: 'center' },
            {
                header: ISACTIVE,
                dataIndex: 'active',
                align: 'center',
                id: 'controlactive',
                hidden: true,
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
            { xtype: 'button', text: REMOVE, id: 'remove', hidden: true, iconCls: 'icon-user-remove', disabled: true, handler: onRemoveClick },
            '->',
            {
                xtype: 'combobox', editable: false, fieldLabel: OPTION, labelWidth: 60, id: 'ddlSel', store: DDLStore, displayField: 'txt', valueField: 'value', value: '1', listeners: {
                    "select": function (combo, record) {
                        FaresStore.removeAll();
                        Ext.getCmp("gdFgroup").store.loadPage(1, {
                            params: {
                                ddlSel: Ext.getCmp('ddlSel').getValue()
                            }
                        });
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
    // FaresStore.load({ params: { start: 0, limit: 10 } });
});
//選擇會員群組則顯示會員群組
function MultipleShow(val) {
    switch (val) {
        case "0":
            return DEVICE_1;
            break;
        default:
            return val;
            break;
    }
}
function DeliverTypeShow(val) {
    switch (val) {
        case "0":
            return "不分";
            break;
        case "1":
            return '常溫';
            break;
        case "2":
            return '冷藏';
            break;
        default:
            return val;
            break;
    }
}
function DeviceShow(val) {
    switch (val) {
        case "0":
            return "不分";
            break;
        case "1":
            return 'PC';
            break;
        case "4":
            return '手機/平板';
            break;
        default:
            return val;
            break;
    }
}
/********************************************新增*****************************************/
onAddClick = function () {
    //addWin.show();
    editFunction(null, FaresStore);
}

/*********************************************編輯***************************************/
onEditClick = function () {
    var row = Ext.getCmp("gdFgroup").getSelectionModel().getSelection();
    //alert(row[0].data.id);
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        editFunction(row[0], FaresStore);
    }
}
/*********************************************刪除***************************************/
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
                    url: '/PromoPair/PromoPairDelete',
                    method: 'post',
                    params: { rowID: rowIDs },
                    success: function (form, action) {
                        var result = Ext.decode(form.responseText);
                        Ext.Msg.alert(INFORMATION, SUCCESS);
                        if (result.success) {
                            FaresStore.load(1);
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




function UpdateActive(id,muser) {
    var activeValue = $("#img" + id).attr("hidValue");
    $.ajax({
        url: "/PromoPair/UpdateActive",
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
                FaresStore.removeAll();
                FaresStore.load();
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
            Ext.Msg.alert(INFORMATION, FAILURE);
        }
    });
}
