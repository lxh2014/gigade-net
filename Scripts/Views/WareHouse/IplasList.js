var CallidForm;
var pageSize = 25;
//商品主料位管理Model
Ext.define('gigade.Iplas', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "loc_id", type: "string" },
        { name: "plas_id", type: "int" },
        { name: "item_id", type: "int" },
        { name: "loc_stor_cse_cap", type: "string" },
        { name: "create_users", type: "string" },
        { name: "product_name", type: "string" },
        { name:"upc_id",type:"string"},
        { name: "create_dtim", type: "string" }
    ]
});
var SearchStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
        { "txt": "料位", "value": "1" },
        { "txt": "條碼", "value": "2" },
        { "txt": "商品品號", "value": "3" }
    ]
});

var IplasStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.Iplas',
    proxy: {
        type: 'ajax',
        url: '/WareHouse/GetIPlasList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
    //    autoLoad: true
});
IplasStore.on('beforeload', function () {
    Ext.apply(IplasStore.proxy.extraParams,
        {
            starttime: Ext.getCmp('start_time').getValue(),
            endtime: Ext.getCmp('end_time').getValue(),
            search_type: Ext.getCmp('Search_type').getValue(),
            searchcontent: Ext.getCmp('searchcontent').getValue()
        });
});

var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp("gdIplas").down('#edit').setDisabled(selections.length == 0);
            Ext.getCmp("gdIplas").down('#delete').setDisabled(selections.length == 0);
        }
    }
});

function Query(x) {
    IplasStore.removeAll();

    var start = Ext.getCmp("start_time");
    var end = Ext.getCmp("end_time");
    var search = Ext.getCmp("searchcontent");
    if (search.getValue().trim() =="") {
        if (start.getValue() == null || end.getValue() == null) {
            Ext.Msg.alert("提示", "請輸入查詢時間或查詢內容");
            return;
        }

    }
    Ext.getCmp("gdIplas").store.loadPage(1, {
        params: {
            starttime: Ext.getCmp('start_time').getValue(),
            endtime: Ext.getCmp('end_time').getValue(),
            search_type: Ext.getCmp('Search_type').getValue(),
            searchcontent: Ext.getCmp('searchcontent').getValue().trim()
        }
    });
}

Ext.onReady(function () {
    var frm = Ext.create('Ext.form.Panel', {
        id: 'frm',
        layout: 'anchor',
        height: 80,
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
                            xtype: 'combobox',
                            name: 'Search_type',
                            id: 'Search_type',
                            editable: false,
                            fieldLabel: "查詢條件",
                            labelWidth: 60,
                            margin: '0 5 0 0',
                            store: SearchStore,
                            queryMode: 'local',
                            submitValue: true,
                            displayField: 'txt',
                            valueField: 'value',
                            typeAhead: true,
                            forceSelection: false,
                            value: 2
                        },
                        { xtype: 'textfield', allowBlank: true, fieldLabel: "查詢內容", id: 'searchcontent', name: 'searchcontent', labelWidth: 60 },
                         { xtype: 'label', margin: '2 0 0 10', text: '創建時間:' },
                        {
                            xtype: "datefield",
                            editable: false,
                            margin: '0 0 0 5',
                            id: 'start_time',
                            name: 'start_time',
                            format: 'Y/m/d',
                            listeners: {
                                select: function (a, b, c) {
                                    var start = Ext.getCmp("start_time");
                                    var end = Ext.getCmp("end_time");
                                    if (end.getValue() == null) {
                                        end.setValue(setNextMonth(start.getValue(), 1));
                                    } else if (start.getValue() > end.getValue()) {
                                        Ext.Msg.alert(INFORMATION, "開始時間不能大於結束時間");
                                        end.setValue(setNextMonth(start.getValue(), 1));
                                    }
                                    else if (end.getValue() > setNextMonth(start.getValue(), 1)) {
                                        // Ext.Msg.alert(INFORMATION, DATE_LIMIT);
                                        end.setValue(setNextMonth(start.getValue(), 1));
                                    }
                                },
                                specialkey: function (field, e) {
                                    if (e.getKey() == e.ENTER) {
                                        Query();
                                    }
                                }
                            }
                        },
                        { xtype: 'displayfield', value: '~ ' },
                        {
                            xtype: "datefield",
                            editable: false,
                            id: 'end_time',
                            name: 'end_time',
                            format: 'Y/m/d',
                            listeners: {
                                select: function (a, b, c) {
                                    var start = Ext.getCmp("start_time");
                                    var end = Ext.getCmp("end_time");
                                    var s_date = new Date(start.getValue());
                                    var now_date = new Date(end.getValue());
                                    if (start.getValue() != "" && start.getValue() != null) {
                                        if (end.getValue() < start.getValue()) {
                                            Ext.Msg.alert(INFORMATION, "結束時間不能小於開始時間");
                                            end.setValue(setNextMonth(start.getValue(), 1));
                                        } else if (end.getValue() > setNextMonth(start.getValue(), 1)) {
                                            //Ext.Msg.alert(INFORMATION, DATE_LIMIT);
                                            start.setValue(setNextMonth(end.getValue(), -1));
                                        }

                                    } else {
                                        start.setValue(setNextMonth(end.getValue(), -1));
                                    }
                                },
                                specialkey: function (field, e) {
                                    if (e.getKey() == e.ENTER) {
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
                        { xtype: 'button', text: SEARCH, iconCls: 'icon-search', id: 'btnQuery', handler: Query },
                        {
                            xtype: 'button',
                            text: RESET,                           
                            margin:'0 0 0 10',
                            id: 'btn_reset',
                            iconCls: 'ui-icon ui-icon-reset',
                            listeners: {
                                click: function () {
                                    Ext.getCmp("searchcontent").setValue("");
                                    Ext.getCmp('start_time').setValue("");
                                    Ext.getCmp('end_time').setValue("");
                                    Ext.getCmp("Search_type").setValue(2);
                                }
                            }
                        }
                 ]
             }
        ]
    });

    var gdIplas = Ext.create('Ext.grid.Panel', {
        id: 'gdIplas',
        store: IplasStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        flex: 9.4,
       
        columns: [
            { header: PRODID, dataIndex: 'item_id', width: 120, align: 'center' },
            { header: PRODNAME, dataIndex: 'product_name', width: 250, align: 'center' },
            { header: LOCID, dataIndex: 'loc_id', width: 150, align: 'center' },
            { header: '料位容量', dataIndex: 'loc_stor_cse_cap', width: 120, align: 'center' },
            { header: CREATEUSER, dataIndex: 'create_users', width: 100, align: 'center' },
            { header: CREATEDTIM, dataIndex: 'create_dtim', width: 150, align: 'center' }
        ],
        selType: 'cellmodel',
        plugins: [
            Ext.create('Ext.grid.plugin.CellEditing', {
                clicksToEdit: 1
            })
        ],
        tbar: [
            { xtype: 'button', text: ADD, id: 'add', hidden: false, iconCls: 'icon-user-add', handler: onAddClick },
            { xtype: 'button', text: EDIT, id: 'edit', hidden: true, iconCls: 'icon-user-edit', disabled: true, handler: onEditClick },
            { xtype: 'button', text: "摘除主料位", id: 'delete', hidden: true, iconCls: 'icon-user-remove', disabled: true, handler: onDeleteClick },
            { xtype: 'button', text: "匯入Excel", id: 'ExportEnter', icon: '../../../Content/img/icons/excel.gif', handler: onExportEnter },
            { xtype: 'button', text: "匯出Excel", id: 'ExportOut', icon: '../../../Content/img/icons/excel.gif', handler: onExportOut },
            { xtype: 'button', text: "料位轉移匯入", id: 'IplasExportEnter', icon: '../../../Content/img/icons/excel.gif', handler: IplasEnter },
        ],
       
        bbar: Ext.create('Ext.PagingToolbar', {
            store: IplasStore,
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
        items: [frm,gdIplas],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                gdIplas.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    //IplasStore.load({ params: { start: 0, limit: 25 } });
});

/*******************************新增*********************************************/
onAddClick = function () {
    //addWin.show();
    editFunction(null, IplasStore);
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
/************************編輯*********************************/
onEditClick = function () {
    var row = Ext.getCmp("gdIplas").getSelectionModel().getSelection();
    //alert(row[0]);
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        editFunction(row[0], IplasStore);
    }
}
/************************刪除**************************/
onDeleteClick = function () {
    var row = Ext.getCmp("gdIplas").getSelectionModel().getSelection();
    if (row.length <= 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else {
        Ext.Msg.confirm(CONFIRM, Ext.String.format("是否確定要刪除嗎?", row.length), function (btn) {
            if (btn == 'yes') {
                var rowIDs = '';
                for (var i = 0; i < row.length; i++) {
                    rowIDs += row[i].data.loc_id + ',';
                }
                Ext.Ajax.request({
                    url: '/WareHouse/DeleteIplasById',//執行方法
                    method: 'post',
                    params: { loc_id: rowIDs },
                    success: function (form, action) {
                        var result = Ext.decode(form.responseText);
                        if (result.success) {
                            if (result.sum != undefined && result.sum > 0)
                            {
                                Ext.Msg.alert(INFORMATION, "所選料位里存在商品，無法刪除!");
                            }
                            else if (result.sum == undefined && result.count > 0) {
                                Ext.Msg.alert(INFORMATION, "刪除成功!");
                            }
                            else {
                                Ext.Msg.alert(INFORMATION, "無法刪除!");
                            }
                            IplasStore.loadPage(1);
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
/************************匯入 匯出**************************/
onExport = function () {
    ExportFunction();
}
onExportExcel = function () {
    ExportFunction();
}


onExportEnter = function ()
{
    IplasExportFunction();
}
IplasEnter = function () {//匯入料位
    IplasEnterFunction();
}

onExportOut = function ()
{
    window.open("/WareHouse/IplasExcelList?searchcontent=" + Ext.getCmp("searchcontent").getValue() + "&search_type=" + Ext.getCmp('Search_type').getValue() + "&starttime=" + Ext.Date.format(new Date(Ext.getCmp('start_time').getValue()), 'Y-m-d H:i:s') + "&endtime=" + Ext.Date.format(new Date(Ext.getCmp('end_time').getValue()), 'Y-m-d H:i:s'));
}
function Tomorrow() {
    var d;
    d = new Date();
    d.setDate(d.getDate() + 1);
    return d;
}
