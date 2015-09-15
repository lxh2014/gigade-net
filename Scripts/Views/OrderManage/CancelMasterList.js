var CallidForm;
var pageSize = 25;
/***************************************取消單主頁面***********************************************/
//取消單Model
Ext.define('gigade.Fares', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "cancel_id", type: "int" },
        { name: "order_id", type: "string" },
        { name: "cancel_status", type: "string" },
        { name: "cancel_note", type: "string" },
        { name: "bank_note", type: "string" },
        { name: "cancel_createdate", type: "string" },
        { name: "cancel_updatedate", type: "string" },
        { name: "cancel_ipfrom", type: "string" }
    ]
});

var FaresStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.Fares',
    proxy: {
        type: 'ajax',
        //url:controller控制器/方法名
        url: '/OrderManage/GetOrderCancelMasterList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
    //    autoLoad: true
});

var sm = Ext.create('Ext.selection.CheckboxModel', {
    showHeaderCheckbox: false,
    renderer: function (value, metaData, record, rowIndex, colIndex, store, view) {
        if (record.data.cancel_status != "1") {
            metaData.tdCls = Ext.baseCSSPrefix + 'grid-cell-special';
            return '<div class="' + Ext.baseCSSPrefix + 'grid-row-checker">&#160;</div>';
        }
        else {
            return '';
        }
    },
    listeners: {
        selectionchange: function (sm, selections) {
            if (selections.length > 0) {
                if (selections[0].data.cancel_status == 0) {
            Ext.getCmp("gdFgroup").down('#edit').setDisabled(selections.length == 0);
        }
            } else {
                Ext.getCmp("gdFgroup").down('#edit').setDisabled(true);
            }
        }
    }
});
Ext.onReady(function () {
    var gdFgroup = Ext.create('Ext.grid.Panel', {
        id: 'gdFgroup',
        store: FaresStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        title: '取消單列表',
        frame: true,
        columns: [
            { header: "取消單號", dataIndex: 'cancel_id', width: 60, align: 'center' },
            {
                header: "付款單號", dataIndex: 'order_id', width: 150, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    return '<a href=javascript:TransToOrder(' + record.data.order_id + ') >' + record.data.order_id + '</a>';
                }
            },
            {
                header: "狀態", dataIndex: 'cancel_status', width: 100, align: 'center', renderer: function (value) {
                    if (value == "0") {
                        return Ext.String.format('<font color="#FF0000">{0}</font>', '待歸檔');
                    }
                    else if (value == "1") {
                        return Ext.String.format('歸檔');
                    }
                    else if (value == "2") {
                        return Ext.String.format('<font color="#FF0000">{0}</font>', '刪除');
                    }
                }
            },
            { header: "建立時間", dataIndex: 'cancel_createdate', width: 200, align: 'center' },
            { header: "備註", dataIndex: 'cancel_note', width: 300, align: 'center' }
        ],
        tbar: [
           //{ xtype: 'button', text: ADD, id: 'add', hidden: false, iconCls: 'icon-user-add', handler: onAddClick },
           { xtype: 'button', text: EDIT, id: 'edit', hidden: false, iconCls: 'icon-user-edit', disabled: true, handler: onEditClick }
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
    FaresStore.load({ params: { start: 0, limit: 25 } });
});
/******************************新增********************************************/
onAddClick = function () {
    editFunction(null, FaresStore);
}
/*****************************編輯*********************************************/
onEditClick = function () {
    var row = Ext.getCmp("gdFgroup").getSelectionModel().getSelection();
    //alert(row[0]);
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        editFunction(row[0], FaresStore);
    }
}
function TransToOrder(orderId) {
    var url = '/OrderManage/OrderDetialList?Order_Id=' + orderId;
    var panel = window.parent.parent.Ext.getCmp('ContentPanel');
    var copy = panel.down('#DeliverList');
    if (copy) {
        copy.close();
    }
    copy = panel.add({
        id: 'DeliverList',
        title: '訂單內容',
        html: window.top.rtnFrame(url),
        closable: true
    });
    panel.setActiveTab(copy);
    panel.doLayout();

}