var CallidForm;
var pageSize = 22;
/**********************************************************************站臺管理主頁面**************************************************************************************/
//料位管理Model
Ext.define('gigade.Ilocs', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "loc_id", type: "string" },
        { name: "dc_id", type: "string" },
        { name: "whse_id", type: "string" },
        { name: "change_users", type: "string" },
        { name: "change_dtim", type: "string" },
        { name: "llts_id", type: "string" },
        { name: "ldes_id", type: "string" },
        { name: "sel_stk_pos", type: "string" },
        { name: "sel_pos_hgt", type: "string" },
        { name: "rsv_stk_pos", type: "string" },
        { name: "rsv_pos_hgt", type: "string" },
        { name: "stk_lmt", type: "string" },
        { name: "stk_pos_wid", type: "string" },
        { name: "lev", type: "string" },
        { name: "lhnd_id", type: "string" },
        { name: "row_id", type: "int" },
        { name: "ldsp_id", type: "string" },
        { name: "comingle_allow", type: "string" },
        { name: "lcat_id", type: "string" },
        { name: "lsta_id", type: "string" },
        { name: "stk_pos_dep", type: "string" },
         { name: "hash_loc_id", type: "string" }        
    ]
});

var IlocsStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.Ilocs',
    proxy: {
        type: 'ajax',
        url: '/WareHouse/GetIlocList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
    //    autoLoad: true
});
var SerchTypeStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
        { "txt": "可用", "value": "F" },
        { "txt": "已鎖定", "value": "H" },
        { "txt": "已指派", "value": "A" }
    ]
});
var Iloc_idTypeStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
        { "txt": "所有料位", "value": "0" },
        { "txt": "主料位", "value": "S" },
        { "txt": "副料位", "value": "R" }
    ]
});
IlocsStore.on('beforeload', function () {
    Ext.apply(IlocsStore.proxy.extraParams, {
        searchcontent: Ext.getCmp('searchcontent').getValue(),
        search_type: Ext.getCmp('search_type').getValue(),
        starttime: Ext.getCmp('start_time').getValue(),
        endtime: Ext.getCmp('end_time').getValue(),
        Ilocid_type: Ext.getCmp('Ilocid_type').getValue()
    });
});

var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp("gdIlocs").down('#edit').setDisabled(selections.length == 0);
            Ext.getCmp("gdIlocs").down('#delete').setDisabled(selections.length == 0);
        }
    }
});

function Query(x) {
    IlocsStore.removeAll();
    var start = Ext.getCmp("start_time");
    var end = Ext.getCmp("end_time");
    var search1 = Ext.getCmp('search_type');
    var search = Ext.getCmp("searchcontent");
    if (search.getValue().trim() == "") {
        if (search1.getValue() == null){
            if (start.getValue() == null || end.getValue() == null) {
                Ext.Msg.alert("提示", "請輸入查詢時間或查詢內容");
                return;
            }
        }      
    }

    Ext.getCmp("gdIlocs").store.loadPage(1, {
        params: {
            searchcontent: Ext.getCmp('searchcontent').getValue().trim(),
            search_type: Ext.getCmp('search_type').getValue(),
            starttime: Ext.getCmp('start_time').getValue(),
            endtime: Ext.getCmp('end_time').getValue(),
            Ilocid_type: Ext.getCmp('Ilocid_type').getValue()
        }
    });
}

Ext.onReady(function () {
    var gdIlocs = Ext.create('Ext.grid.Panel', {
        id: 'gdIlocs',
        store: IlocsStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
            { header: LOCID, dataIndex: 'loc_id', width: 150, align: 'center' },
             { header: "哈希料位", dataIndex: 'hash_loc_id', width: 150, align: 'center' },
            { header: DCID, dataIndex: 'dc_id', hidden: true, width: 150, align: 'center' },
            { header: WHSEID, dataIndex: 'whse_id', hidden: true, width: 150, align: 'center' },
            { header: CHANGEUSER, dataIndex: 'change_users', width: 100, align: 'center' },
            { header: CHANGEDTIM, dataIndex: 'change_dtim', width: 150, align: 'center' },
            { header: "料位類型", dataIndex: 'lcat_id', width: 150, align: 'center' },
            {
                header: "鎖定", dataIndex: 'lsta_id', width: 150, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value == "F") {//F表示可用
                        return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.row_id + ")'><img hidValue='F' id='img" + record.data.row_id + "' src='../../../Content/img/icons/hmenu-unlock.png'/></a>";
                    } else if (value == "H") {
                        return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.row_id + ")'><img hidValue='H' id='img" + record.data.row_id + "' src='../../../Content/img/icons/hmenu-lock.png'/></a>";
                    } else if (value = "A") {
                        return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.row_id + ")'><img hidValue='A' id='img" + record.data.row_id + "' src='../../../Content/img/icons/application_go.png'/></a>";
                    }
                }
            },
            { header: "comingle_allow", dataIndex: 'comingle_allow', hidden: true, width: 150, align: 'center' }
        ],
        tbar: [
            { xtype: 'button', text: ADD, id: 'add', hidden: false, iconCls: 'icon-user-add', handler: onAddClick },
            { xtype: 'button', text: EDIT, id: 'edit', hidden: true, iconCls: 'icon-user-edit', disabled: true, handler: onEditClick },
            { xtype: 'button', text: "刪除", id: 'delete', hidden: true, iconCls: 'icon-user-edit', disabled: true, handler: onDeleteClick },
            { xtype: 'button', text: "一鍵哈希", id: 'hash', hidden: true, handler: onHashAll },
            { xtype: 'button', text: "匯入Excel", id: 'ExportEnter', icon: '../../../Content/img/icons/excel.gif', handler: onExportEnter },
            { xtype: 'button', text: "匯出Excel", id: 'ExportOut', icon: '../../../Content/img/icons/excel.gif', handler: onExportOut },
            '->',
            { xtype: 'label', text: '創建時間:' },
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
                        var s_date = new Date(start.getValue());
                        if (end.getValue() < start.getValue()) {
                            end.setValue(new Date(s_date.setMonth(s_date.getMonth() + 1)));
                        }
                    },
                    specialkey: function (field, e) {
                        if (e.getKey() == Ext.EventObject.ENTER) {
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
                        if (end.getValue() < start.getValue()) {
                            Ext.Msg.alert("提示", "開始時間不能大於結束時間！");
                            end.setValue(new Date(s_date.setMonth(s_date.getMonth() + 1)));
                        }
                    },
                    specialkey: function (field, e) {
                        if (e.getKey() == Ext.EventObject.ENTER) {
                            Query();
                        }
                    }
                }
            },
            {
                xtype: 'combobox',
                name: 'Ilocid_type',
                id: 'Ilocid_type',
                editable: false,
                fieldLabel: "料位類型",
                labelWidth: 60,
                store: Iloc_idTypeStore,
                queryMode: 'local',
                submitValue: true,
                displayField: 'txt',
                valueField: 'value',
                typeAhead: true,
                forceSelection: false,
                emptyText: '請選擇料位類型',
                value: 0
            },
            {
                xtype: 'combobox',
                name: 'search_type',
                id: 'search_type',
                editable: false,
                fieldLabel: "狀態",
                labelWidth: 40,
                store: SerchTypeStore,
                queryMode: 'local',
                submitValue: true,
                displayField: 'txt',
                valueField: 'value',
                typeAhead: true,
                forceSelection: false,
                emptyText: '請選擇鎖定情況'
            },
            { xtype: 'textfield', allowBlank: true, fieldLabel: "料位編號", id: 'searchcontent', name: 'searchcontent', labelWidth: 60 },
            {
                xtype: 'button',
                text: SEARCH,
                iconCls: 'icon-search',
                id: 'btnQuery',
                handler: Query
            },
            {
                xtype: 'button',
                text: RESET,
                id: 'btn_reset',
                listeners: {
                    click: function () {
                        Ext.getCmp("searchcontent").setValue("");
                        Ext.getCmp('search_type').setValue(null);
                        Ext.getCmp('Ilocid_type').setValue(0);
                        Ext.getCmp('start_time').setValue(null);
                        Ext.getCmp('end_time').setValue(null);
                    }
                }
            }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: IlocsStore,
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
        items: [gdIlocs],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                gdIlocs.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    //IlocsStore.load({ params: { start: 0, limit: pageSize } });
});

/*************************************************************************************新增*************************************************************************************************/
onAddClick = function () {
    //addWin.show();
    editFunction(null, IlocsStore);
}

/*************************************************************************************編輯*************************************************************************************************/
onEditClick = function () {
    var row = Ext.getCmp("gdIlocs").getSelectionModel().getSelection();
    //alert(row[0]);
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        editFunction(row[0], IlocsStore);
    }
}

onDeleteClick = function () {
    var row = Ext.getCmp("gdIlocs").getSelectionModel().getSelection();
    if (row.length <= 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else {
        Ext.Msg.confirm(CONFIRM, Ext.String.format("是否確定要申請刪除此料位", row.length), function (btn) {
            if (btn == 'yes') {
                var sum = 0;
                for (var i = 0; i < row.length; i++) {
                    if (row[i].data.lsta_id == "A") {
                        sum = sum + 1;
                    }
                }
                if (sum > 0) {
                    Ext.Msg.alert(INFORMATION, "已指派的料位不能進行刪除操作!");
                } else {
                    var rowIDs = '';
                    for (var i = 0; i < row.length; i++) {
                        rowIDs += row[i].data.row_id + ',';
                    }
                    Ext.Ajax.request({
                        url: '/WareHouse/DeleteLocid',//執行方法
                        method: 'post',
                        params: { row_id: rowIDs },
                        success: function (form, action) {
                            var result = Ext.decode(form.responseText);
                            if (result.success) {
                                Ext.Msg.alert(INFORMATION, "已經刪除此料位!");
                                IlocsStore.loadPage(1);
                            }
                        },
                        failure: function () {
                            Ext.Msg.alert(INFORMATION, FAILURE);
                        }
                    });
                }
            }
        });
    }
}


function UpdateActive(id) {
    var activeValue = $("#img" + id).attr("hidValue");
    if (activeValue == "A") {
        Ext.Msg.alert("提示信息", "此料位已經指派!");
    }
    else {
        $.ajax({
            url: "/WareHouse/UpdateIlocActive",
            data: {
                "id": id,
                "active": activeValue
            },
            type: "POST",
            dataType: "json",
            success: function (msg) {
                Ext.getCmp("gdIlocs").store.load({
                    params: {
                        searchcontent: Ext.getCmp('searchcontent').getValue(),
                        search_type: Ext.getCmp('search_type').getValue(),
                        Ilocid_type: Ext.getCmp('Ilocid_type').getValue()
                    }
                });
            },
            error: function (msg) {
                Ext.Msg.alert("提示信息", "更改此料位失敗!");
            }
        });
    }

}

onExportEnter = function () {
    IlocExportFunction();
}

onExportOut = function () {
    window.open("/WareHouse/IlocExcelList?searchcontent=" + Ext.getCmp('searchcontent').getValue() + "&search_type=" + Ext.getCmp('search_type').getValue() + "&Ilocid_type=" + Ext.getCmp('Ilocid_type').getValue() + "&starttime=" + Ext.Date.format(new Date(Ext.getCmp('start_time').getValue()), 'Y-m-d H:i:s') + "&endtime=" + Ext.Date.format(new Date(Ext.getCmp('end_time').getValue()), 'Y-m-d H:i:s'));
}

onHashAll = function ()
{
    Ext.Ajax.request({
        url: '/WareHouse/HashAll',//執行方法
        method: 'post',
        success: function (form, action) {
            var result = Ext.decode(form.responseText);
            if (result.success) {
                Ext.Msg.alert(INFORMATION, "一鍵Hash成功!");
                IlocsStore.loadPage(1);
            }
        },
        failure: function () {
            Ext.Msg.alert(INFORMATION, FAILURE);
        }
    });
}
function Tomorrow() {
    var d;
    d = new Date();
    d.setDate(d.getDate() + 1);
    return d;
}