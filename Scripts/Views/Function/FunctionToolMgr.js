Ext.Loader.setConfig({
    enabled: true
});
//Ext.Loader.setPath('Ext.ux', '../../../Scripts/Ext4.0/ux');
Ext.Loader.setPath('Ext.ux', '../../../Scripts/Views/ProductParticulars');
Ext.require([
    'Ext.ux.CheckColumn'
]);

var ToolStore = Ext.create('Ext.data.Store', {
    model: 'GIGADE.Function',
    proxy: {
        type: 'ajax',
        url: '/Function/GetFunction',
        actionMethods: 'post',
        reader: {
            type: 'json'
        }
    }
});

var ToolWin = function (row) {

    var t_sm = Ext.create('Ext.selection.CheckboxModel', {
        listeners: {
            selectionchange: function (sm, selections) {
                Ext.getCmp("ToolGrid").down('#t_remove').setDisabled(selections.length == 0);
                Ext.getCmp("ToolGrid").down('#t_edit').setDisabled(selections.length == 0);
            }
        }
    });

    var ToolGrid = Ext.create('Ext.grid.Panel', {
        id: 'ToolGrid',
        store: ToolStore,
        columnLines: true,
        frame: true,
        columns: [
            { header: NUMBER, xtype: 'rownumberer', width: 50, align: 'center' },
            { header: TOOL_NAME, dataIndex: 'FunctionName', width: 150, align: 'center' },
            { header: TOOL_CODE, dataIndex: 'FunctionCode', width: 150, align: 'center' },
            { header: ISAUTHORIZED, xtype: 'checkcolumn', dataIndex: 'IsEdit', width: 60, align: 'center' }
        ],
        tbar: [
            { xtype: 'button', text: ADD, iconCls: 'icon-add', handler: function () { SaveToolWin(row); } },
            { xtype: 'button', id: 't_edit', text: EDIT, iconCls: 'icon-edit', disabled: true, handler: function () { onToolEditClick(row); } },
            { xtype: 'button', id: 't_remove', text: REMOVE, iconCls: 'icon-remove', disabled: true, handler: function () { onToolRemoveClick(row); } }
        ],
        listeners: {
            scrollershow: function (scroller) {
                if (scroller && scroller.scrollEl) {
                    scroller.clearManagedListeners();
                    scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                }
            }
        },
        selModel: t_sm
    });

    Ext.create('Ext.window.Window', {
        title: TOOL_MGR_TITLE,
        items: [ToolGrid],
        width: 500,
        height: document.documentElement.clientHeight * 400 / 783,
        layout: 'fit',
        labelWidth: 100,
        closeAction: 'destroy',
        resizable: false,
        modal: 'true',
        listeners: {
            "show": function () {
                ToolStore.load({
                    params: { TopValue: Ext.htmlEncode(row.data.RowId), Type: 2 }
                });
            }
        }
    }).show();
}

onToolEditClick = function (frow) {
    var row = Ext.getCmp("ToolGrid").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        SaveToolWin(frow, row[0]);
    }
}
onToolRemoveClick = function (frow) {
    var row = Ext.getCmp("ToolGrid").getSelectionModel().getSelection();
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
                            ToolStore.load({
                                params: { TopValue: Ext.htmlEncode(frow.data.RowId), Type: 2 }
                            });
                        }
                    },
                    failure: function () {

                    }
                });
            }
        });
    }
}