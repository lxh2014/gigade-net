
var CallidForm;
var pageSize = 25;
/**********************************************************************站臺管理主頁面**************************************************************************************/
//站臺管理Model
Ext.define('gigade.NewDeliver', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "vendor_name_simple", type: "string" },
        { name: "deliver_id", type: "int" },
        { name: "type", type: "int" },
        { name: "delivery_store", type: "int" },
        { name: "delivery_code", type: "string" },
        { name: "delivery_freight_cost", type: "int" },
        { name: "delivery_status", type: "int" },
        { name: "estimated_arrival_period", type: "string" },
        { name: "delivery_date", type: "string" },
        { name: "created", type: "string" },
        { name: "states", type: "string" },
        { name: "types", type: "string" },
        { name: "stores", type: "string" },
        { name: "estimated_arrival_period_str", type: "string" },
          { name: "delivery_date_str", type: "string" },
        

    ]
});

var NewDeliverStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.NewDeliver',
    proxy: {
        type: 'ajax',
        url: '/OrderManage/GetNewDeliver',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
    //    autoLoad: true
});
NewDeliverStore.on('beforeload', function () {
    Ext.apply(NewDeliverStore.proxy.extraParams,
        {
            Order_Id: window.parent.GetOrderId()

        });
});
Ext.onReady(function () {
    var NewDeliverGrid = Ext.create('Ext.grid.Panel', {
        id: 'NewDeliverGrid',
        store: NewDeliverStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
            { header: "出貨商", dataIndex: 'vendor_name_simple', width: 200, align: 'center' },
            {
                header: "出貨單編號", dataIndex: 'deliver_id', width: 200, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    return Ext.String.format("<a  href=javascript:TranToDeliverDetial('/SendProduct/DeliverView','{0}') style='text-decoration:none'>{0}</a>", record.data.deliver_id);
                }
            },
            { header: "出貨類別", dataIndex: 'types', width: 100, align: 'center' },
            { header: "物流業者", dataIndex: 'stores', width: 100, align: 'center' },
            { header: "物流單號", dataIndex: 'delivery_code', width: 100, align: 'center' },
            { header: "物流費", dataIndex: 'delivery_freight_cost', width: 150, align: 'center' },
            { header: "狀態", dataIndex: 'states', width: 150, align: 'center' },
            { header: "預計到貨時段", dataIndex: 'estimated_arrival_period_str', width: 130, align: 'center' },
            { header: "寄送日期", dataIndex: 'delivery_date_str', width: 200, align: 'center' },
            { header: "建單日期", dataIndex: 'created', width: 200, align: 'center' }
        ],

        bbar: Ext.create('Ext.PagingToolbar', {
            store: NewDeliverStore,
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
        }

    });

    Ext.create('Ext.container.Viewport', {
        layout: 'fit',
        items: [NewDeliverGrid],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                NewDeliverGrid.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    QueryToolAuthorityByUrl('/OrderManage/NewDeliver');
    NewDeliverStore.load({ params: { start: 0, limit: 25 } });
});




//跳轉到出貨明細頁
function TranToDeliverDetial(url, deliver_id) {
    var urlTran = url + '?deliver_id=' + deliver_id;
    var panel = window.parent.parent.Ext.getCmp('ContentPanel');
    var copy = panel.down('#pnlDeliverView');
    if (copy) {
        copy.close();
    }
    copy = panel.add({
        id: 'pnlDeliverView',
        title: "出貨明細",
        html: window.top.rtnFrame(urlTran),
        closable: true
    });
    panel.setActiveTab(copy);
    panel.doLayout();
}
