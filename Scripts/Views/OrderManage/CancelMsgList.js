
var CallidForm;
var pageSize = 25;
/**********************************************************************取消訂單通知主頁面**************************************************************************************/
//取消訂單通知Model
Ext.define('gigade.CancelMsgs', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "cancel_id", type: "int" },
        { name: "order_id", type: "string" },
        { name: "scancel_type", type: "string" },
        { name: "cancel_status", type: "string" },
        { name: "cancel_content", type: "string" },
        { name: "cancel_createdate", type: "string" },
        { name: "cancel_ipfrom", type: "string" },
        { name: "order_name", type: "string" },
        { name: "sorder_payment", type: "string" },
        { name: "order_amount", type: "string" },
        { name: "order_mobile", type: "string" },
        { name: "user_email", type: "string" },
        { name: "sorder_status", type: "string" }
    ]
});

var CancelMsgStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.CancelMsgs',
    proxy: {
        type: 'ajax',
        url: '/OrderManage/GetOrderCancelMsgList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
    //    autoLoad: true
});

var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp("gdCancelMsg").down('#reply').setDisabled(selections.length == 0);
        }
    }
});

//var vipUserTpl = new Ext.XTemplate(
//    '<a href="/VipUserGroup3/VipUserGroupAddList?id={group_id}">{list}</a>'
//);
Ext.onReady(function () {
    var gdCancelMsg = Ext.create('Ext.grid.Panel', {
        id: 'gdCancelMsg',
        store: CancelMsgStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        title: CANCELTITLE,
        frame: true,
        columns: [
            { header: CANCELID, dataIndex: 'cancel_id', width: 60, align: 'center' },
            {
                header: ORDERID, dataIndex: 'order_id', width: 150, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    return '<a href=javascript:TransToOrder(' + record.data.order_id + ') >' + record.data.order_id + '</a>';
                }
            },
            { header: ORDERNAME, dataIndex: 'order_name', width: 200, align: 'center' },
            { header: ORDERAMOUNT, dataIndex: 'order_amount', width: 200, align: 'center' },
            { header: PAYTYPE, dataIndex: 'sorder_payment', width: 200, align: 'center' },
            { header: SLAVESTATUS, dataIndex: 'sorder_status', width: 100, align: 'center' },
            { header: CANCELTYPE, dataIndex: 'scancel_type', width: 200, align: 'center' },
            { header: CREATEDATE, dataIndex: 'cancel_createdate', width: 200, align: 'center' }
        ],
        tbar: [
           { xtype: 'button', text: REPLY, id: 'reply', hidden: false, iconCls: 'icon-user-edit', disabled: true, handler: onReplyClick }

        ],

        bbar: Ext.create('Ext.PagingToolbar', {
            store: CancelMsgStore,
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
        items: [gdCancelMsg],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                gdCancelMsg.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    CancelMsgStore.load({ params: { start: 0, limit: 25 } });
});
/*************************************************************************************回覆*/
onReplyClick = function () {
    var row = Ext.getCmp("gdCancelMsg").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        replyFunction(row[0], CancelMsgStore);
    }
}
function TransToOrder(orderId) {
    var url = '/OrderManage/OrderDetialList?Order_Id=' + orderId;
    var panel = window.parent.parent.Ext.getCmp('ContentPanel');
    var copy = panel.down('#DeliverList1');
    if (copy) {
        copy.close();
    }
    copy = panel.add({
        id: 'DeliverList1',
        title: '訂單內容',
        html: window.top.rtnFrame(url),
        closable: true
    });
    panel.setActiveTab(copy);
    panel.doLayout();
}


