var pageSize = 15;//add by wwei0216w 2015/4/7 添加分頁+
///functionHistory的model
Ext.define('GIGADE.FUNCTIONHISTORY', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'Row_Id', type: "int" },
        { name: 'FunctionName', type: "string" },
        { name: 'Operate_Time', type: "string" },
        { name: 'User_Name', type: "string" },
        { name: 'Function_Id', type: "int" },
        { name: 'User_Id', type: "int" }
    ]
});

var historyStore = Ext.create('Ext.data.Store', {
    pageSize: pageSize,//add by wwei0216w 2015/4/7 添加分頁
    model: 'GIGADE.FUNCTIONHISTORY',
    proxy: {
        type: 'ajax',
        url: '/Function/QueryHistory',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'item',
            totalProperty: 'totalCount'//add by wwei0216w 2015/4/7 添加分頁
        }
    }
});

historyStore.on("beforeload", function () {
    historyStore.removeAll();
    Ext.apply(historyStore.proxy.extraParams, {
        function_id: Ext.getCmp("FunGrid").getSelectionModel().getSelection()[0].data.RowId,
        conditional: Ext.getCmp("query").value,
        startTime: Ext.getCmp("startTime").value,
        endTime: Ext.getCmp("endTime").value
    })
})

Ext.define('GIGADE.Function', {
    extend: 'Ext.data.Model',
    fields: [
                { name: "RowId", type: "int" },
                { name: "FunctionType", type: "string" },
                { name: "FunctionGroup", type: "string" },
                { name: "FunctionName", type: "string" },
                { name: "FunctionCode", type: "string" },
                { name: "IsEdit", type: "int" },
                { name: "IconCls", type: "string" },
                { name: "Remark", type: "string" },
                { name: "Count", type: "int" }
    ]
});
var FunStore = Ext.create('Ext.data.Store', {
    model: 'GIGADE.Function',
    groupField: 'FunctionGroup',
    proxy: {
        type: 'ajax',
        url: '/Function/GetFunction',
        actionMethods: 'post',
        reader: {
            type: 'json'
        }
    },
    sorters: [
        {
            property: 'FunctionGroup',
            direction: 'DESC'
        }
    ]
});
var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp("FunGrid").down('#remove').setDisabled(selections.length == 0);
            Ext.getCmp("FunGrid").down('#edit').setDisabled(selections.length == 0);
            Ext.getCmp("FunGrid").down('#authority').setDisabled(selections.length == 0);
            Ext.getCmp("FunGrid").down('#history').setDisabled(selections.length == 0);
        }
    }
});

//add by zhuoqin0830w  2015/07/06  添加 查看權限人員的 store
Ext.define('GIGADE.Privilege', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'CallId', type: "string" },
        { name: 'GroupName', type: "string" },
        { name: 'GroupId', type: "int" },
        { name: 'User_UserName', type: "string" }
    ]
});
var PrivilegeStore = Ext.create('Ext.data.Store', {
    model: 'GIGADE.Privilege',
    groupField: 'GroupName',
    proxy: {
        type: 'ajax',
        url: '/Function/GetPrivilege',
        actionMethods: 'post',
        reader: {
            type: 'json'
        }
    },
    sorters: [{
        property: 'GroupName',
        direction: 'DESC'
    }]
});

Ext.onReady(function () {
    var groupingFeature = Ext.create('Ext.grid.feature.Grouping', {
        groupHeaderTpl: GROUP + ':{name}'
    });
    var FunGrid = Ext.create('Ext.grid.Panel', {
        id: 'FunGrid',
        store: FunStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        features: [groupingFeature],
        columns: [
            { header: '', xtype: 'rownumberer', width: 40, align: 'center' },
            //添加查看欄位  add by zhuoqin0830w  2015/07/01
            {
                header: '', xtype: 'actioncolumn', width: 40, align: 'center',
                items: [{
                    icon: '/Content/img/icons/application_view_list.png',
                    iconCls: 'icon-cursor',
                    tooltip: GOBAL_BUYER,
                    handler: function (grid, rowIndex, colIndex) {
                        var rec = grid.getStore().getAt(rowIndex);
                        showPrivilege(rec, this);
                    }
                }]
            },
            { header: "function_id", dataIndex: 'RowId', width: 40, align: 'left', hidden: true },
            { header: NAME, dataIndex: 'FunctionName', width: 180, align: 'left' },
            { header: CODE, dataIndex: 'FunctionCode', width: 300, align: 'left' },
            { header: ICONCLS, dataIndex: 'IconCls', width: 80, align: 'left', renderer: function (val) { return '<div align="center"><div style="width:15px;height:15px" class="' + val + '"></div></div>' } },
            {
                header: CLICKS, dataIndex: 'Count', width: 80, align: 'center', renderer: function (value) {
                    return value + NEXT;
                }
            },
            { header: REMARK, dataIndex: 'Remark', align: 'left', flex: 1 }
        ],
        tbar: [
            { xtype: 'button', id: 'add', text: ADD, iconCls: 'ui-icon ui-icon-user-add', hidden: true, handler: onAddClick },
            { xtype: 'button', id: 'edit', text: EDIT, iconCls: 'ui-icon ui-icon-user-edit', hidden: true, disabled: true, handler: onEditClick },
            { xtype: 'button', id: 'remove', text: REMOVE, iconCls: 'ui-icon ui-icon-user-delete', hidden: true, disabled: true, handler: onRemoveClick },
            { xtype: 'button', id: 'authority', text: TOOL_AUTHORITY, iconCls: 'ui-icon ui-icon-key', hidden: true, disabled: true, handler: onToolClick },
        { xtype: 'button', id: 'history', text: FUNCTION_HISTORY, iconCls: 'ui-icon ui-icon-script', hidden: true, disabled: true, handler: QueryHistory },
            '->',
            { text: ' ' }
        ],
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

    Ext.create('Ext.Viewport', {
        layout: 'fit',
        items: [FunGrid],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                FunGrid.width = document.documentElement.clientWidth;
                this.doLayout();

            }
        }
    });
    ToolAuthority();
    FunStore.load();

})



onAddClick = function () {
    SaveWin();
}
onEditClick = function () {
    var row = Ext.getCmp("FunGrid").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        SaveWin(row[0]);
    }
}
onRemoveClick = function () {
    var row = Ext.getCmp("FunGrid").getSelectionModel().getSelection();
    if (row.length < 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else {
        Ext.Msg.confirm(CONFIRM, Ext.String.format(DELETE_INFO, row.length), function (btn) {
            if (btn == 'yes') {
                var rowIDs = '';
                for (var i = 0; i < row.length; i++) {
                    rowIDs += row[i].data.RowId + '|';
                }
                Ext.Ajax.request({
                    url: '/Function/DeleteFunction',
                    method: 'post',
                    params: { rowID: rowIDs },
                    success: function (form, action) {
                        var result = Ext.decode(form.responseText);
                        Ext.Msg.alert(INFORMATION, SUCCESS);
                        if (result.success) {
                            FunStore.load();
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
onToolClick = function () {
    var row = Ext.getCmp("FunGrid").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        ToolWin(row[0]);
    }
}

var cm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {}
});

function QueryHistory() {
    var row = Ext.getCmp("FunGrid").getSelectionModel().getSelection();
    if (row.length > 1) { Ext.Msg.alert(MESSAGE, ONLY_ONE); } else {
        var functionId = row[0].data.RowId;
        FunctionWin(functionId);
    }
}

function OnQueryHistory() {
    historyStore.loadPage(1);
}

function OnReset() {
    Ext.getCmp("query").setValue(""),
    Ext.getCmp("startTime").reset(),
    Ext.getCmp("endTime").reset()
}
///functionWin的信息窗口
var FunctionWin = function (function_id) {
    ///創建用於顯示功能歷史信息的grid
    var HistoryPanel = Ext.create("Ext.grid.Panel", {
        id: 'HistoryPanel',
        store: historyStore,
        border: false,
        columns: [{ header: NUMBER, dataIndex: 'Row_Id', width: 40, menuDisabled: true, sortable: false },
                  { header: 'functionId', dataIndex: 'Function_Id', menuDisabled: true, hidden: true, sortable: false },
                  { header: NAME, dataIndex: 'FunctionName', menuDisabled: true, sortable: false, flex: 1 },
                  {
                      header: USER_NAME, dataIndex: 'User_Name', menuDisabled: true, sortable: false,
                      renderer: function (value) {
                          if (value == "") {
                              return "--";
                          } else {
                              return value;
                          }
                      }
                  },
                  { header: DATELINE, sortable: false, menuDisabled: true, dataIndex: 'Operate_Time', align: 'center', width: 170 }],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: historyStore,
            pageSize: pageSize,
            displayInfo: true//add by wwei0216w 2015/4/7 添加分頁
        }),
        selModel: cm,
        tbar: [
            { xtype: 'textfield', id: 'query', fieldLabel: CONDITION, labelWidth: 35, width: 150, margin: '0 0 0 0' },
            { xtype: 'datefield', id: 'startTime', fieldLabel: TIME, labelWidth: 35, width: 135, format: 'Y-m-d H:i:s', disabledMin: true, disabledSec: true },
            { xtype: 'displayfield', value: '~' },
            { xtype: 'datefield', id: 'endTime', width: 100, format: 'Y-m-d H:i:s', disabledMin: true, disabledSec: true },
            { xtype: 'button', text: SEARCH, handler: OnQueryHistory },
            { xtype: 'button', id: 'reset', text: RESERT, handler: OnReset }
        ],
        listeners: {
            scrollershow: function (scroller) {
                if (scroller && scroller.scrollEl) {
                    scroller.clearManagedListeners();
                    scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                }
            }
        }
    });

    Ext.create('Ext.window.Window', {
        title: HISTORY_INFORMATION,
        items: HistoryPanel,
        width: 500,
        height: document.documentElement.clientHeight * 400 / 783,
        layout: 'fit',
        labelWidth: 100,
        closeAction: 'destroy',
        resizable: false,
        modal: 'true',
        listeners: {
            "show": function () {
                Ext.getCmp("HistoryPanel").store.loadPage(1, { //add by wwei0216w 2015/4/7 添加分頁
                    params: { function_id: function_id }
                });
            }
        }
    }).show();
}

//添加查看欄位  add by zhuoqin0830w  2015/07/06
function showPrivilege(rec, obj) {
    var functionId = rec.data.RowId;
    var functionName = rec.data.FunctionName;
    var groupingFeature = Ext.create('Ext.grid.feature.Grouping', {
        groupHeaderTpl: GROUP_NAME + ':{name}'
    });
    var PrivilegeGrid = Ext.create('Ext.grid.Panel', {
        id: 'PrivilegeGrid',
        store: PrivilegeStore,
        columnLines: true,
        features: [groupingFeature],
        columns: [
            { header: NUMBER, xtype: 'rownumberer', width: 50, align: 'center' },
            { header: PEOPLE_NAME, dataIndex: 'User_UserName', width: 90, align: 'center', sortable: false, menuDisabled: true },
            { header: PEOPLE_MAIL, dataIndex: 'CallId', width: 250, align: 'center', sortable: false, menuDisabled: true }],
        tbar: [{
            xtype: 'button', id: 'OutToExcel', text: REMIT_GOBAL_BUYER, iconCls: 'ui-icon ui-icon-user-add', handler: function () {
                OutToExcelClick(functionId, functionName)
            }
        }
        ],
    });

    var PrivilegeWin = new Ext.Window({
        title: GOBAL_BUYER + ' -- ' + functionName,
        id: 'PrivilegeWin',
        height: document.documentElement.clientHeight * 600 / 783,
        width: 450,
        closeAction: 'destroy',
        modal: true,
        layout: 'fit',
        items: [PrivilegeGrid],
        closable: true,
        listeners: {
            "show": function () {
                PrivilegeStore.load({
                    params: { functionId: functionId }
                });
            }
        }
    }).show();
}
//匯出權限人員  add by zhuoqin0830w  2015/07/06
function OutToExcelClick(functionId, functionName) {
    window.open("/Function/OutToExcel?RowId=" + functionId + "&FunctionName=" + functionName);
}